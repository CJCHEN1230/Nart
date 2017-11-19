using Nart.ExtensionMethods;
using Nart.Model_Object;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using UseCVLibrary;

namespace Nart
{
    public class CalcCoord
    {
        ///<summary>
        ///頭在註冊資料中引數
        /// </summary>
        public int RegHeadIndex = -1;
        ///<summary>
        ///咬板在註冊資料中引數
        /// </summary>
        public int RegSplintIndex = -1;
        /// <summary>
        ///Marker的database
        /// </summary>
        public MarkerDatabase database = new MarkerDatabase();       
        /// <summary>
        ///雙相機鏡心在世界座標
        /// </summary>
        public Point4D[] LensCenter = new Point4D[2];
        /// <summary>
        /// 基線座標系的旋轉矩陣
        /// </summary>
        public Matrix3D EpipolarCoord;
        /// <summary>
        /// FH平面座標系的旋轉矩陣
        /// </summary>
        public Matrix3D FHCoord;
        /// <summary>
        ///兩台相機參數
        /// </summary>
        private CamParam[] _camParam = new CamParam[2];
        /// <summary>
        ///顱顎面資訊Go Po Or Me...
        /// </summary>
        private CraniofacialInfo _craniofacialInfo;
        /// <summary>
        ///判斷是不是相同點的pixel高度容忍值
        /// </summary>
        private const double MatchError = 5;
        /// <summary>
        ///CT珠子中心座標
        /// </summary>
        private Point3D[] CTBall;
        /// <summary>
        ///MS點的珠子中心座標
        /// </summary>
        private Point3D[] MSBall;
        /// <summary>
        ///MS點的Marker中心座標
        /// </summary>
        private Point3D[] MSSplintMarker;
        /// <summary>
        ///咬板Marker在CT中的座標
        /// </summary>
        private Point3D[] SplintInCT;
        /// <summary>
        ///咬板Marker從CT中轉到世界座標
        /// </summary>
        private Matrix3D CTtoWorld;
        /// <summary>
        ///咬板Marker從CT中轉到世界座標
        /// </summary>
        private Matrix3D WorldtoCT;

        private Matrix3D CTtoMS;
        private Matrix3D MStoCT;
        private Matrix3D OriWorldtoMS;
        


        /// <summary>
        /// 當前點世界座標
        /// </summary>
        private List<Marker3D> WorldPoints = new List<Marker3D>(10);
        /// <summary>
        /// 註冊時的點世界座標
        /// </summary>
        private List<Marker3D> OriWorldPoints = new List<Marker3D>(10);
        /// <summary>
        /// 世界座標轉換到MS的座標
        /// </summary>
        private List<Marker3D> MSWorldPoints = new List<Marker3D>(10);
        /// <summary>
        /// 每計算過幾次顯示一次
        /// </summary>
        public int ShowPeriod;
        public int ShowPeriod2 = 0;

        public CalcCoord()
        {

            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
            _craniofacialInfo = new CraniofacialInfo("../../../data/ceph.csv");
            
            CalcLensCenter();
            CalcEpipolarGeometry();
            
            //CreateDatabase();
        }
        /// <summary>
        /// 計算鏡心在世界座標
        /// </summary>
        private void CalcLensCenter()
        {
            LensCenter[0] = new Point4D(0, 0, 0, 1);
            LensCenter[1] = new Point4D(0, 0, 0, 1);

           
            LensCenter[0] = _camParam[0].invExtParam.Transform(LensCenter[0]);
            LensCenter[1] = _camParam[1].invExtParam.Transform(LensCenter[1]);
        }
        /// <summary>
        /// 計算基線座標系
        /// </summary>
        private void CalcEpipolarGeometry()
        {
            //虛擬成像平面的中心
            Point4D PlainCenter = new Point4D(0, 0, _camParam[0].FocalLength, 1);                       

            PlainCenter = _camParam[0].invExtParam.Transform(PlainCenter);

            //Base line
            //Vector3D vec_x = new Vector3D(LensCenter[0].X - LensCenter[1].X, LensCenter[0].Y - LensCenter[1].Y, LensCenter[0].Z - LensCenter[1].Z);
            Vector3D vec_x = new Vector3D(LensCenter[1].X - LensCenter[0].X, LensCenter[1].Y - LensCenter[0].Y, LensCenter[1].Z - LensCenter[0].Z);

            vec_x.Normalize();
          
            Vector3D vec_temp =new Vector3D(PlainCenter.X - LensCenter[0].X, PlainCenter.Y - LensCenter[0].Y, PlainCenter.Z - LensCenter[0].Z);
                  
            Vector3D vec_y = Vector3D.CrossProduct(vec_temp, vec_x);
            vec_y.Normalize();
        
            Vector3D vec_z = Vector3D.CrossProduct(vec_x, vec_y);
          
            EpipolarCoord = new Matrix3D(vec_x.X, vec_y.X, vec_z.X, 0,
                                         vec_x.Y, vec_y.Y, vec_z.Y, 0,
                                         vec_x.Z, vec_y.Z, vec_z.Z, 0,
                                               0,       0,       0, 1);

            
        }
        /// <summary>
        /// 輸入兩張照片的Marker資料，扭正左右兩邊拍攝到的像素座標點並存進RectifyY，並儲存相機坐標系的點進CameraPoint，並算出AvgRectifyY、AvgX
        /// </summary>
        public void Rectify(List<BWMarker>[] OutputMarker)
        {
            //計算出平均x跟平均y
            Parallel.For(0, 2, i =>
            { 

            for (int j = 0; j < OutputMarker[i].Count; j++)
                {
                    double accumX = 0;
                    double accumRecY = 0;
                    
                    for (int k = 0; k < 3; k++) //一個標記裡面三個點
                    {

                        PointF imagePoint=(PointF)(OutputMarker[i][j].CornerPoint[k].ImagePoint);

                        double xd = _camParam[i].dpx * (imagePoint.X - _camParam[i].Cx) / _camParam[i].Sx;
                        double yd = _camParam[i].dy * (imagePoint.Y - _camParam[i].Cy);


                        double r = Math.Sqrt(xd * xd + yd * yd);

                        double xu = xd * (1 + _camParam[i].Kappa1 * r * r);
                        double yu = yd * (1 + _camParam[i].Kappa1 * r * r);

                        //儲存計算出來的相機座標系的點
                        OutputMarker[i][j].CornerPoint[k].CameraPoint = new Point4D(xu, yu, _camParam[i].FocalLength, 1);

                        Point4D WorldPoint = _camParam[i].RotationInvert.Transform(OutputMarker[i][j].CornerPoint[k].CameraPoint);

                        Point4D rectify_Point = EpipolarCoord.Transform(WorldPoint);

                        double Rectify_Y = _camParam[i].FocalLength * rectify_Point.Y / rectify_Point.Z / _camParam[i].dy;
                        //儲存此點扭正後的值
                        OutputMarker[i][j].CornerPoint[k].RectifyY = Rectify_Y;
                        
                        accumX += imagePoint.X;
                        accumRecY += Rectify_Y;
                    }
                    OutputMarker[i][j].AvgRectifyY = accumRecY / 3.0;
                    OutputMarker[i][j].AvgX = accumX / 3.0;

                    OutputMarker[i][j].CornerPoint.Sort(
                    delegate (NartPoint point1, NartPoint point2)
                    {
                        double diff = point1.RectifyY - point2.RectifyY;
                        if (diff > 2)
                            return 1;
                        else if (diff < -2)
                            return -1;
                        else
                        {
                            double diff2 = ((PointF)point1.ImagePoint).X - ((PointF)point2.ImagePoint).X;
                            if (diff2 > 0)
                                return 1;
                            else
                                return -1;
                        }
                    });

                }
                
                OutputMarker[i].Sort();
                
            });

            //for (int i = 0; i < OutputMarker.Length; i++)
            //{
            //    Console.WriteLine("\n\n\n第" + (i + 1) + "張");

            //    for (int j = 0; j < OutputMarker[i].Count; j++)
            //    {
            //        Console.WriteLine("\n\n第" + (j + 1) + "組");
            //        for (int k = 0; k < OutputMarker[i][j].CornerPoint.Count; k++)
            //        {
            //            PointF imagePoint2 = (PointF)(OutputMarker[i][j].CornerPoint[k].ImagePoint);
            //            Console.WriteLine("\n :(" + imagePoint2.X + "," + imagePoint2.Y + "," + OutputMarker[i][j].CornerPoint[k].RectifyY + ")");
            //        }
            //        Console.WriteLine("\nAvergeX :" + OutputMarker[i][j].AvgX);
            //        Console.WriteLine("\nRectifyY :" + OutputMarker[i][j].AvgRectifyY);
            //    }
            //}
        }
        /// <summary>
        /// 輸入兩個相機座標點並反算出世界座標
        /// </summary>
        public Point3D CalcWorldPoint(Point4D a, Point4D b)
        {

            Point4D WorldPoint1 = _camParam[0].invExtParam.Transform(a);
            Point4D WorldPoint2 = _camParam[1].invExtParam.Transform(b);


            Vector3D d1 = new Vector3D(LensCenter[0].X - WorldPoint1.X, LensCenter[0].Y - WorldPoint1.Y, LensCenter[0].Z - WorldPoint1.Z);
            Vector3D d2 = new Vector3D(LensCenter[1].X - WorldPoint2.X, LensCenter[1].Y - WorldPoint2.Y, LensCenter[1].Z - WorldPoint2.Z);
            Point3D A1 = new Point3D(LensCenter[0].X, LensCenter[0].Y, LensCenter[0].Z);
            Point3D A2 = new Point3D(LensCenter[1].X, LensCenter[1].Y, LensCenter[1].Z);

            double delta = d1.LengthSquared * d2.LengthSquared - Vector3D.DotProduct(d2, d1) * Vector3D.DotProduct(d2, d1);

            double deltaX = Vector3D.DotProduct(A2 - A1, d1) * d2.LengthSquared - (Vector3D.DotProduct(A2 - A1, d2)) * Vector3D.DotProduct(d2, d1);

            double deltaY = d1.LengthSquared * Vector3D.DotProduct(A1 - A2, d2) - Vector3D.DotProduct(d2, d1) * Vector3D.DotProduct(A1 - A2, d1);

            double t1 = deltaX / delta;

            double t2 = deltaY / delta;


            Point3D LeftPoint = new Point3D(A1.X + t1 * d1.X, A1.Y + t1 * d1.Y, A1.Z + t1 * d1.Z);
            Point3D RightPoint = new Point3D(A2.X + t2 * d2.X, A2.Y + t2 * d2.Y, A2.Z + t2 * d2.Z);

            //Console.WriteLine("左相機真實解:" + LeftPoint);
            //Console.WriteLine("右相機真實解:" + RightPoint);
            //Console.WriteLine("真實解:" + new Point3D((LeftPoint.X + RightPoint.X) / 2.0, (LeftPoint.Y + RightPoint.Y) / 2.0, (LeftPoint.Z + RightPoint.Z) / 2.0));
            return new Point3D((LeftPoint.X + RightPoint.X) / 2.0, (LeftPoint.Y + RightPoint.Y) / 2.0, -(LeftPoint.Z + RightPoint.Z) / 2.0);


        }
        /// <summary>
        /// 匹配兩張圖的扭正點並反算出世界座標
        /// </summary>
        public void MatchAndCalc3D(List<BWMarker>[] OutputMarker)
        {
            WorldPoints.Clear();
            for (int i = 0, count = 0; i < OutputMarker[0].Count; i++) //左相機Marker尋訪
            {
                for (int j = count; j < OutputMarker[1].Count; j++) //右相機Marker尋訪
                {
                    //比對包括中間及四個點的Marker扭正Y位置
                    if (Math.Abs(OutputMarker[0][i].AvgRectifyY - OutputMarker[1][j].AvgRectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[0].RectifyY - OutputMarker[1][j].CornerPoint[0].RectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[1].RectifyY - OutputMarker[1][j].CornerPoint[1].RectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[2].RectifyY - OutputMarker[1][j].CornerPoint[2].RectifyY) < MatchError)
                    {
                        Marker3D threeWorldPoints = new Marker3D(); //Marker上面的三點世界座標
                        for (int k = 0; k < OutputMarker[0][i].CornerPoint.Count; k++)
                        {                           
                            Point3D point3D = CalcWorldPoint(OutputMarker[0][i].CornerPoint[k].CameraPoint, OutputMarker[1][j].CornerPoint[k].CameraPoint);
                            threeWorldPoints.ThreePoints[k] = point3D;
                        }
                        
                        WorldPoints.Add(threeWorldPoints);
                        count = j + 1;
                        break;                        
                    }
                    else
                        continue;
                }
            }

            //對計算到的3D點排序
            for (int i =0; i< WorldPoints.Count; i++)
            {
                WorldPoints[i].SortedByLength();
            }

            MainViewModel.PointNumber = (WorldPoints.Count * 3).ToString() + "個點";      
        }
        /// <summary>
        /// 將計算出的3D座標點與資料庫比對並存下引數
        /// </summary>
        public void MatchRealMarker()
        {
            for (int i = 0; i < WorldPoints.Count; i++)
            {               
                WorldPoints[i].CompareDatabase(database.MarkerInfo);
            }
        }
        /// <summary>
        /// 目前尚未使用這種排序
        /// </summary>
        private void SortMarker(List<BWMarker>[] OutputMarker)
        {
            Parallel.For(0, 2, Image_Index =>
            {
                for (int i = 0; i < OutputMarker[Image_Index].Count; i++)
                { //第一張圖每個點輪流
                    int ClosePoints = 0;

                    for (int j = i + 1; j < OutputMarker[Image_Index].Count;)
                    { //同樣是第一張圖，但是對自己的下一個點算y距離


                        if (Math.Abs(OutputMarker[Image_Index][i].AvgRectifyY - OutputMarker[Image_Index][j].AvgRectifyY) < 3)
                        { //y距離小於3pixel則計算累積個數在count，並直接算下一個點
                            ClosePoints++;
                            i++;
                            j = i + 1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (ClosePoints != 0)
                    { //count不等於0，意謂該高度附進有超過一個點，對此附近的點再進行x方向的排序
                        for (int k = i - ClosePoints, test = 0; k < i; k++, test++)
                        {
                            BWMarker temp;
                            for (int l = i - ClosePoints; l < i - test; l++)
                            {
                                if (OutputMarker[Image_Index][l].AvgX > OutputMarker[Image_Index][l + 1].AvgX)
                                {
                                    temp = OutputMarker[Image_Index][l];
                                    OutputMarker[Image_Index][l] = OutputMarker[Image_Index][l + 1];
                                    OutputMarker[Image_Index][l + 1] = temp;
                                }
                            }
                        }
                    }
                }
            });


        }
        /// <summary>
        /// 在註冊時使用，比對當前WorldPoints並清除MarkerDB中不需要的資料
        /// </summary>
        private void SimplifyDatabase()
        {
            //Reset 資料庫中的Marker
            //database = new MarkerDatabase();
            //尋訪資料庫中的Marker
            for (int i = 0, j = 0; i < database.MarkerInfo.Count; i++)
            {   //尋訪世界座標中的Marker
                for (j = 0; j < WorldPoints.Count; j++)
                {
                    //如果資料庫中的MarkerID存在當前資料庫就檢查下一個
                    if (database.MarkerInfo[i].ID.Equals(WorldPoints[j].MarkerID))
                    {
                        break; //換下一個資料庫中的Marker
                    }
                }
                //資料庫中的Marker在WorldPoint中都找不到時
                if (j == WorldPoints.Count)
                {
                    database.MarkerInfo.RemoveAt(i);
                    i--;
                }
            }
            database.ResetIndex();
        }        
        /// <summary>
        /// 傳入兩組三個點所組成的座標系，回傳轉換矩陣
        /// </summary>
        private Matrix3D TransformCoordinate(Point3D[] A,Point3D[] B)
        {          
            List<Point3D[]> twoPoints = new List<Point3D[]>(2) { A, B };
            Point3D[] Avg = new Point3D[2];
            Vector3D[] u = new Vector3D[2];
            Vector3D[] v = new Vector3D[2];
            Vector3D[] w = new Vector3D[2];

            Parallel.For(0, twoPoints.Count , i =>
            {
                Avg[i] = new Point3D((twoPoints[i][0].X + twoPoints[i][1].X + twoPoints[i][2].X) / 3.0, (twoPoints[i][0].Y + twoPoints[i][1].Y + twoPoints[i][2].Y) / 3.0, (twoPoints[i][0].Z + twoPoints[i][1].Z + twoPoints[i][2].Z) / 3.0);

                u[i] = twoPoints[i][0] - Avg[i];

                Vector3D temp = twoPoints[i][2] - Avg[i];

                v[i] = Vector3D.CrossProduct(u[i], temp);

                w[i] = Vector3D.CrossProduct(u[i], v[i]);

                u[i].Normalize();
                v[i].Normalize();
                w[i].Normalize();

            });

            Matrix3D translate1 = new Matrix3D(1, 0, 0, 0,
                                              0, 1, 0, 0,
                                              0, 0, 1, 0,
                                             -Avg[0].X, -Avg[0].Y, -Avg[0].Z, 1);

            Matrix3D rotate1 = new Matrix3D(u[0].X, v[0].X, w[0].X, 0,
                                              u[0].Y, v[0].Y, w[0].Y, 0,
                                              u[0].Z, v[0].Z, w[0].Z, 0,
                                             0, 0, 0, 1);

            Matrix3D transform1= translate1 * rotate1;


         
            Matrix3D transform2 = new Matrix3D(u[1].X, u[1].Y, u[1].Z, 0,
                                               v[1].X, v[1].Y, v[1].Z, 0,
                                               w[1].X, w[1].Y, w[1].Z, 0,
                                             Avg[1].X, Avg[1].Y, Avg[1].Z, 1);

            Matrix3D finalTransform = transform1 * transform2;


            return finalTransform;
        }
        /// <summary>
        /// 匯入原N-Art的註冊檔
        /// </summary>
        private bool LoadNartReg(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);
                string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                string[] data = new string[27];
                Array.Copy(contentArray, 0, data, 0, 27);

                double[] regData = Array.ConvertAll(data, double.Parse);

                
                CTBall = new Point3D[3] { new Point3D(regData[0], regData[1], regData[2]),
                                          new Point3D(regData[3], regData[4], regData[5]),
                                          new Point3D(regData[6], regData[7], regData[8])};
                
                MSBall = new Point3D[3] { new Point3D(regData[9], regData[10], regData[11]),
                                          new Point3D(regData[12], regData[13], regData[14]),
                                          new Point3D(regData[15], regData[16], regData[17])};

                MSSplintMarker = new Point3D[3] { new Point3D(regData[18], regData[19], regData[20]),
                                            new Point3D(regData[21], regData[22], regData[23]),
                                            new Point3D(regData[24], regData[25], regData[26])};
              

                CTtoMS = TransformCoordinate(CTBall, MSBall);


                MStoCT = TransformCoordinate(MSBall, CTBall);


                Console.WriteLine("\nCTBall");
                for (int i =0;i< CTBall.Length ;i++)
                {
                    Console.WriteLine(CTBall[i]);
                }

                Console.WriteLine("\nSplintBall");
                for (int i = 0; i < MSBall.Length; i++)
                {
                    Console.WriteLine(MSBall[i]);
                }

                Console.WriteLine("\nSplintMarker");
                for (int i = 0; i < MSSplintMarker.Length; i++)
                {
                    Console.WriteLine(MSSplintMarker[i]);
                }
                Console.WriteLine("  ");
                return true;
            }
            catch
            {
                MessageBox.Show("註冊檔案錯誤");

                return false;
            }
        }
        private bool LoadSplintPoint(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);
                string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                double[] matrixInfo = Array.ConvertAll(contentArray, double.Parse);

                if (matrixInfo.Length != 9)
                {
                    throw new Exception();
                }

                SplintInCT = new Point3D[3] { new Point3D(matrixInfo[0], matrixInfo[1], matrixInfo[2]),
                                            new Point3D(matrixInfo[3], matrixInfo[4], matrixInfo[5]),
                                            new Point3D(matrixInfo[6], matrixInfo[7], matrixInfo[8])};

                return true;
            }
            catch
            {
                MessageBox.Show("找不到咬板於CT座標檔案");
                return false;
            }
        }
        
        
        
        
        /// <summary>
        /// 輸入點群資料跟指定的Marker(上、下顎、頭)ID，回傳資料庫的索引位置
        /// </summary>
        private int GetSpecIndex(List<Marker3D> Markerdata,String ID)
        {
            for (int i = 0; i < Markerdata.Count; i++) 
            {
                if (Markerdata[i].MarkerID.Equals(ID)) 
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 原Nart的註冊當前狀態的值
        /// </summary>
        public void Registraion()
        {
            OriWorldPoints.Clear();
            MSWorldPoints.Clear();

            if (!LoadNartReg(ModelSettingViewModel.RegPath))
            {                
                return;
            }

            //先判斷當前標記片數量是否過少
            if (WorldPoints.Count >=2)
            {
                //取得咬板Marker在WorldPoints中的索引值
                RegSplintIndex = GetSpecIndex(WorldPoints, "Splint");
                Console.WriteLine("\n\n第" + (RegSplintIndex + 1) + "組點是咬板");
                //取得頭Marker在WorldPoints中的索引值
                RegHeadIndex = GetSpecIndex(WorldPoints, "Head");
                Console.WriteLine("\n\n第" + (RegHeadIndex + 1) + "組點是頭部");

                //註冊需要咬板與頭部標記片
                if (RegSplintIndex != -1 && RegHeadIndex != -1)
                {
                    
                    SimplifyDatabase();

                    CalcFHCoord();

                    for (int i = 0; i < database.MarkerInfo.Count; i++)
                    {
                        Console.WriteLine("\n\n" + database.MarkerInfo[i].ThreeLength[0] + " " + database.MarkerInfo[i].ThreeLength[1] + " " + database.MarkerInfo[i].ThreeLength[2]);
                    }
                    Console.WriteLine("splint:" + database.SplintIndex);
                    Console.WriteLine("Head:" + database.HeadIndex);

                    MatchRealMarker();//比對當前世界座標與資料庫並存下引數

                    for (int i = 0; i < WorldPoints.Count; i++) 
                    {
                        Console.WriteLine("MarkerID"+i +":"+WorldPoints[i].MarkerID);
                    }


                    OriWorldtoMS = TransformCoordinate(WorldPoints[RegSplintIndex].ThreePoints, MSSplintMarker);

                    for (int i = 0; i < WorldPoints.Count; i++)
                    {
                        //創建註冊時的世界座標點
                        Marker3D OriWorldPoint = new Marker3D();
                        OriWorldPoint.MarkerID = WorldPoints[i].MarkerID;
                        WorldPoints[i].ThreePoints.CopyTo(OriWorldPoint.ThreePoints, 0); //將當前世界座標點存進OriWorldPoint當作註冊時的狀態

                        //將所有世界座標點乘上OriWorldtoMS 轉成MS座標
                        Marker3D MSWorldPoint = new Marker3D();
                        MSWorldPoint.MarkerID = WorldPoints[i].MarkerID;
                        OriWorldtoMS.Transform(WorldPoints[i].ThreePoints);
                        WorldPoints[i].ThreePoints.CopyTo(MSWorldPoint.ThreePoints, 0);

                        OriWorldPoints.Add(OriWorldPoint);
                        MSWorldPoints.Add(MSWorldPoint);
                    }

                    MessageBox.Show("註冊了" + WorldPoints.Count + "組Marker");

                    for (int i = 0; i < OriWorldPoints.Count; i++)
                    {
                        Console.WriteLine("\n第" + (i + 1) + "組 世界座標");
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[0]);
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[1]);
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[2]);
                        Console.WriteLine(OriWorldPoints[i].MarkerID);
                    }
                    Console.WriteLine("\n\n\n");
                    for (int i = 0; i < MSWorldPoints.Count; i++)
                    {
                        Console.WriteLine("\n第" + (i + 1) + "組 MS座標");
                        Console.WriteLine(MSWorldPoints[i].ThreePoints[0]);
                        Console.WriteLine(MSWorldPoints[i].ThreePoints[1]);
                        Console.WriteLine(MSWorldPoints[i].ThreePoints[2]);
                        Console.WriteLine("引數:" + MSWorldPoints[i].MarkerID);
                    }

                }
                else
                {
                    MessageBox.Show("找不到咬板或頭部Marker");
                }
            }
            else
            {
                MessageBox.Show("包括咬板與頭部還需要其他標記片");
            }
            CameraControl.RegToggle = false;
        }
        /// <summary>
        /// 導航的註冊
        /// </summary>
        public void Registraion2()
        {
            OriWorldPoints.Clear();
            MSWorldPoints.Clear();

            if (!LoadSplintPoint("../../../data/蔡慧君測試用.txt"))
            {
                return;
            }

            //先判斷當前標記片數量是否過少
            if (WorldPoints.Count >=2)
            {
                //取得咬板Marker在WorldPoints中的索引值
                RegSplintIndex = GetSpecIndex(WorldPoints, "Splint");
                Console.WriteLine("\n\n第" + (RegSplintIndex + 1) + "組點是咬板");
                //取得頭Marker在WorldPoints中的索引值
                RegHeadIndex = GetSpecIndex(WorldPoints, "Head");
                Console.WriteLine("\n\n第" + (RegHeadIndex + 1) + "組點是頭部");

                //註冊需要咬板與頭部標記片
                if (RegSplintIndex != -1 && RegHeadIndex != -1)
                {

                    SimplifyDatabase();

                    CalcFHCoord();

                    for (int i = 0; i < database.MarkerInfo.Count; i++)
                    {
                        Console.WriteLine("\n\n" + database.MarkerInfo[i].ThreeLength[0] + " " + database.MarkerInfo[i].ThreeLength[1] + " " + database.MarkerInfo[i].ThreeLength[2]);
                    }
                    Console.WriteLine("splint:" + database.SplintIndex);
                    Console.WriteLine("Head:" + database.HeadIndex);

                    MatchRealMarker();//將當前世界座標與資料庫比較並存下於資料庫中的引數

                    for (int i = 0; i < WorldPoints.Count; i++)
                    {
                        Console.WriteLine("MarkerID" + i + ":" + WorldPoints[i].MarkerID);
                    }

                    CTtoWorld = TransformCoordinate(SplintInCT, WorldPoints[RegSplintIndex].ThreePoints);
                    WorldtoCT = TransformCoordinate(WorldPoints[RegSplintIndex].ThreePoints, SplintInCT);


                    for (int i = 0; i < WorldPoints.Count; i++)
                    {
                        //創建註冊時的世界座標點
                        Marker3D OriWorldPoint = new Marker3D();
                        OriWorldPoint.MarkerID = WorldPoints[i].MarkerID;
                        WorldPoints[i].ThreePoints.CopyTo(OriWorldPoint.ThreePoints, 0); //將當前世界座標點存進OriWorldPoint當作註冊時的狀態

                        OriWorldPoints.Add(OriWorldPoint);
                       
                    }

                    MessageBox.Show("註冊了" + WorldPoints.Count + "組Marker");

                    for (int i = 0; i < OriWorldPoints.Count; i++)
                    {
                        Console.WriteLine("\n第" + (i + 1) + "組 世界座標");
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[0]);
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[1]);
                        Console.WriteLine(OriWorldPoints[i].ThreePoints[2]);
                        Console.WriteLine(OriWorldPoints[i].MarkerID);
                    }
                    Console.WriteLine("\n\n\n");
                   
                }
                else
                {
                    MessageBox.Show("找不到咬板或頭部Marker");
                }
            }
            else
            {
                MessageBox.Show("包括咬板與頭部還需要其他標記片");
            }
            CameraControl.RegToggle = false;
        }
        /// <summary>
        /// 計算出每個Marker的轉移矩陣
        /// </summary>
        public void CalcModelTransform()
        {
            int currentHeadIndex = GetSpecIndex(WorldPoints, "Head");

            //沒有找到頭的Marker
            if (currentHeadIndex != -1) 
            {
                
                Parallel.For(0, WorldPoints.Count, i =>
               {
                   //當前Marker找不到或找到的是頭部Marker則跳過
                   if (!WorldPoints[i].MarkerID.Equals("")/*&& !WorldPoints[i].MarkerID.Equals("Splint") */&& !WorldPoints[i].MarkerID.Equals("Head"))
                    {
                        int MSandOriIndex = GetSpecIndex(MSWorldPoints, WorldPoints[i].MarkerID);//取得當前世界座標在註冊時的座標索引值是多少

                        //Matrix3D level1 = TransformCoordinate(CTBall, MSBall);

                        Matrix3D level2 = TransformCoordinate(MSWorldPoints[MSandOriIndex].ThreePoints, WorldPoints[i].ThreePoints);//"註冊檔紀錄的可動部分的marker座標轉到MS座標的結果 MS Marker" to "追蹤LED(現在位置)"

                        Matrix3D level3 = TransformCoordinate(WorldPoints[currentHeadIndex].ThreePoints, MSWorldPoints[RegHeadIndex].ThreePoints);

                        //Matrix3D level4 = TransformCoordinate(MSBall, CTBall);
                        
                        Matrix3D Final = CTtoMS * level2 * level3 * MStoCT;

                      
                       for (int j = 0; j < MultiAngleViewModel.BoneModelCollection.Count; j++)
                       {
                           BoneModel boneModel = MultiAngleViewModel.BoneModelCollection[j] as BoneModel;
                           if (boneModel.MarkerID == WorldPoints[i].MarkerID)
                           {
                               boneModel.AddItem(Final);
                           }
                       }
                   }                                                        
               });

            }
        }
        /// <summary>
        /// 計算出每個Marker的轉移矩陣
        /// </summary>
        public void CalcModelTransform2()
        {
            int currentHeadIndex = GetSpecIndex(WorldPoints, "Head");

            //沒有找到頭的Marker
            if (currentHeadIndex != -1)
            {

                //Parallel.For(0, WorldPoints.Count, i =>
                for (int i = 0; i < WorldPoints.Count; i++) 
                {
                    //當前Marker找不到或找到的是頭部Marker則跳過
                    if (WorldPoints[i].MarkerID.Equals("Splint")) 
                    {
                  
                        int SplintMarkerIndex = GetSpecIndex(WorldPoints, "Splint");//取得當前世界座標在註冊時的座標索引值是多少
                                                
                        Matrix3D level1 = TransformCoordinate(SplintInCT, WorldPoints[i].ThreePoints);

                        Matrix3D level2 = TransformCoordinate(WorldPoints[currentHeadIndex].ThreePoints, OriWorldPoints[RegHeadIndex].ThreePoints);

                        //Matrix3D level3 = WorldtoCT;

                        Matrix3D Final =  level1 * level2 * WorldtoCT;


                        for (int j = 0; j < MultiAngleViewModel.BoneModelCollection.Count; j++)
                        {
                            BoneModel boneModel = MultiAngleViewModel.BoneModelCollection[j] as BoneModel;
                            if (boneModel.MarkerID == WorldPoints[i].MarkerID&& boneModel.IsRendering)
                            {
                                boneModel.AddItem(Final);
                            }
                        }
                    }
                    //});
                }

            }
        }
        /// <summary>
        /// 計算出FH平片所形成的坐標系
        /// </summary>
        private void CalcFHCoord()
        {
            int mandibleOSPIndex = -1;
            int headOSPIndex = -1;
            Vector3D headNormal;
            Vector3D mandibleNormal;

            //先找出模型引數
            for (int i = 0; i < MultiAngleViewModel.OSPModelCollection.Count; i++)
            {                
                OSPModel ospModel = MultiAngleViewModel.OSPModelCollection[i] as OSPModel;

                if (ospModel != null && ospModel.IsLoaded)
                {
                    if (ospModel.MarkerID == "Head")
                    {
                        headOSPIndex = i;
                    }
                    if (ospModel.MarkerID == "C")
                    {
                        mandibleOSPIndex = i;
                    }
                }
            }
            //如果下顎跟頭顱的引數都沒找到 則根本無法計算五項指標
            if (headOSPIndex == -1 || mandibleOSPIndex == -1)
            {
                return;
            }
            //取得頭顱跟下顎的模型
            OSPModel headOSP = MultiAngleViewModel.OSPModelCollection[headOSPIndex] as OSPModel;
            OSPModel mandibleOSP = MultiAngleViewModel.OSPModelCollection[mandibleOSPIndex] as OSPModel;


            //頭顱模型的索引值
            //headNormal = headOSP.OSPOriNormal;
            //下顎模型的索引值
            //mandibleNormal = mandibleOSP.OSPOriNormal;
            headNormal = headOSP.GetCurrentNormal();
            mandibleNormal = mandibleOSP.GetCurrentNormal();


            //Po Or四個點
            Point3D[] PoOr = new Point3D[] { _craniofacialInfo.PoL, _craniofacialInfo.PoR, _craniofacialInfo.OrL, _craniofacialInfo.OrR };            
            Point3D NormalPlanePoint = headOSP.OSPPlanePoint;
            Point3D[] PoOrProj = new Point3D[4];

            //計算Po Or四點投影到頭顱對稱面的點座標存進PoOrProj
            for (int i = 0; i < PoOr.Length; i++)
            {
                double v1 = NormalPlanePoint.X - PoOr[i].X;
                double v2 = NormalPlanePoint.Y - PoOr[i].Y;
                double v3 = NormalPlanePoint.Z - PoOr[i].Z;

                double t1 = (headNormal.X * v1 + headNormal.Y * v2 + headNormal.Z * v3) / (headNormal.LengthSquared);

                double x = PoOr[i].X + headNormal.X * t1;
                double y = PoOr[i].Y + headNormal.Y * t1;
                double z = PoOr[i].Z + headNormal.Z * t1;
                PoOrProj[i] = new Point3D(x, y, z);
            }

            //投影到頭顱對稱面的Po兩點平均
            Point3D avgPo = new Point3D((PoOrProj[0].X + PoOrProj[1].X) / 2.0, (PoOrProj[0].Y + PoOrProj[1].Y) / 2.0, (PoOrProj[0].Z + PoOrProj[1].Z) / 2.0);
            //投影到頭顱對稱面的Or兩點平均
            Point3D avgOr = new Point3D((PoOrProj[2].X + PoOrProj[3].X) / 2.0, (PoOrProj[2].Y + PoOrProj[3].Y) / 2.0, (PoOrProj[2].Z + PoOrProj[3].Z) / 2.0);
            //定義FH平面座標系
            Vector3D FHY = avgPo - avgOr;
            Vector3D FHZ = Vector3D.CrossProduct(FHY, headNormal);
            Vector3D FHX = Vector3D.CrossProduct(FHY, FHZ);

            FHX.Normalize();
            FHY.Normalize();
            FHZ.Normalize();


            FHCoord = new Matrix3D(FHX.X, FHY.X, FHZ.X, 0,
                                                             FHX.Y, FHY.Y, FHZ.Y, 0,
                                                             FHX.Z, FHY.Z, FHZ.Z, 0,
                                                                      0,         0,          0, 1);


            //計算Go點連線與下顎對稱面交點 存進GoIntersection
            Point3D manPlanePoint = mandibleOSP.OSPPlanePoint;
            Vector3D GoVector = _craniofacialInfo.GoL - _craniofacialInfo.GoR;

            double t = (mandibleNormal.X * (manPlanePoint.X - _craniofacialInfo.GoL.X) + mandibleNormal.Y * (manPlanePoint.Y - _craniofacialInfo.GoL.Y) + mandibleNormal.Z * (manPlanePoint.Z - _craniofacialInfo.GoL.Z))
                /(mandibleNormal.X * GoVector.X + mandibleNormal.Y * GoVector.Y + mandibleNormal.Z * GoVector.Z);
            double X = _craniofacialInfo.GoL.X + GoVector.X * t;
            double Y = _craniofacialInfo.GoL.Y + GoVector.Y * t;
            double Z = _craniofacialInfo.GoL.Z + GoVector.Z * t;
            _craniofacialInfo.GoIntersection = new Point3D(X, Y, Z);

        }
        
        /// <summary>
        /// 計算出顱面資訊
        /// </summary>
        public void CalcCraniofacialInfo()
        {
            int mandibleOSPIndex = -1;
            int headOSPIndex = -1;
            if (ShowPeriod == 5)
            {
                //先找出模型引數
                for (int i = 0; i < MultiAngleViewModel.OSPModelCollection.Count; i++)
                {
                    OSPModel ospModel = MultiAngleViewModel.OSPModelCollection[i] as OSPModel;

                    if (ospModel!=null && ospModel.IsLoaded)
                    {
                        if (ospModel.MarkerID == "Head")
                        {
                            headOSPIndex = i;
                        }
                        if (ospModel.MarkerID == "C")
                        {
                            mandibleOSPIndex = i;
                        }
                    }
                }

                if (headOSPIndex == -1 || mandibleOSPIndex == -1)
                {
                    return;
                }

                OSPModel headOSP = MultiAngleViewModel.OSPModelCollection[headOSPIndex] as OSPModel;
                OSPModel mandibleOSP = MultiAngleViewModel.OSPModelCollection[mandibleOSPIndex] as OSPModel;
                
                Vector3D headNormal;
                Vector3D mandibleNormal;
                headNormal = headOSP.GetCurrentNormal();
                mandibleNormal = mandibleOSP.GetCurrentNormal();

                
                //先算DA
                double DA = Vector3D.AngleBetween(headNormal, mandibleNormal);

                Vector3D FHMandibleNormal = FHCoord.Transform(mandibleNormal);
                double x = FHMandibleNormal.X;
                double y = FHMandibleNormal.Y;
                double z = FHMandibleNormal.Z;
                
                double FDA = Math.Acos(Math.Abs(x) / Math.Sqrt(x * x + z * z)) / Math.PI * 180.0;

                double HDA = Math.Acos(Math.Abs(x) / Math.Sqrt(x * x + y * y)) / Math.PI * 180.0;

                Point3D Me = mandibleOSP.Transform.Transform(_craniofacialInfo.Me);
                Point3D headNormalPoint = headOSP.OSPPlanePoint;//法向量平面上的點

                double DD = Vector3D.DotProduct(headNormal, Me - headNormalPoint);


                Point3D Intersection = mandibleOSP.Transform.Transform(_craniofacialInfo.GoIntersection);
                double PDD = Vector3D.DotProduct(headNormal, Intersection - headNormalPoint);

                

                string info = "DA:    "+ Math.Round(DA, 4).ToString()
                        + "\n\nDD:    "+ Math.Round(DD, 3).ToString()
                        + "\n\nFDA:  "+ Math.Round(FDA, 2).ToString()
                        + "\n\nHDA: "+ Math.Round(HDA, 2).ToString()
                        + "\n\nPDD:  "+ Math.Round(PDD, 3).ToString();

                MultiAngleViewModel.CraniofacialInfo = info;


                ShowPeriod = 0;
            }
            ShowPeriod++;
        }

        public void CalcBallDistance()
        {
            if (ShowPeriod2 == 5)
            {
                if (MultiAngleViewModel.TriangleModelCollection == null || MultiAngleViewModel.TriangleModelCollection.Count == 0)
                    return;


                if (!NavigateViewModel.firstStageDone)
                {
                    DraggableTriangle targetTriangle = MultiAngleViewModel.TriangleModelCollection[0] as DraggableTriangle;
                    DraggableTriangle movedTriangle = MultiAngleViewModel.TriangleModelCollection[2] as DraggableTriangle;
                    if (movedTriangle.MarkerID.Equals("Maxilla"))
                    {

                        Vector3 red = targetTriangle.positions[0];
                        Vector3 green = targetTriangle.positions[1];
                        Vector3 blue = targetTriangle.positions[2];

                        Matrix mat = movedTriangle.Transform.Value.ToMatrix();


                        Vector3 red2 = new Vector3();
                        Vector3 green2 = new Vector3();
                        Vector3 blue2 = new Vector3();
                        Vector3.TransformCoordinate(ref targetTriangle.positions[0], ref mat, out red2);
                        Vector3.TransformCoordinate(ref targetTriangle.positions[1], ref mat, out green2);
                        Vector3.TransformCoordinate(ref targetTriangle.positions[2], ref mat, out blue2);


                        var redVector = new Vector3();
                        var greenVector = new Vector3();
                        var blueVector = new Vector3();

                        Vector3.Subtract(ref red2, ref red, out redVector);
                        Vector3.Subtract(ref green2, ref green, out greenVector);
                        Vector3.Subtract(ref blue2, ref blue, out blueVector);


                        float redLength = redVector.Length();
                        float greenLength = greenVector.Length();
                        float blueLength = blueVector.Length();


                        string info = "Red:      " + Math.Round(redLength, 3).ToString()
                            + "\n\n" + "Green:  " + Math.Round(greenLength, 3).ToString()
                            + "\n\n" + "Blue:     " + Math.Round(blueLength, 3).ToString();

                        MultiAngleViewModel.BallDistance = info;

                        ShowPeriod2 = 0;

                    }

                }
                else
                {
                    DraggableTriangle targetTriangle = MultiAngleViewModel.TriangleModelCollection[1] as DraggableTriangle;
                    DraggableTriangle movedTriangle = MultiAngleViewModel.TriangleModelCollection[3] as DraggableTriangle;
                    if (movedTriangle.MarkerID.Equals("Mandible"))
                    {

                        Vector3 red = targetTriangle.positions[0];
                        Vector3 green = targetTriangle.positions[1];
                        Vector3 blue = targetTriangle.positions[2];

                        Matrix mat = movedTriangle.Transform.Value.ToMatrix();


                        Vector3 red2 = new Vector3();
                        Vector3 green2 = new Vector3();
                        Vector3 blue2 = new Vector3();
                        Vector3.TransformCoordinate(ref targetTriangle.positions[0], ref mat, out red2);
                        Vector3.TransformCoordinate(ref targetTriangle.positions[1], ref mat, out green2);
                        Vector3.TransformCoordinate(ref targetTriangle.positions[2], ref mat, out blue2);


                        var redVector = new Vector3();
                        var greenVector = new Vector3();
                        var blueVector = new Vector3();

                        Vector3.Subtract(ref red2, ref red, out redVector);
                        Vector3.Subtract(ref green2, ref green, out greenVector);
                        Vector3.Subtract(ref blue2, ref blue, out blueVector);


                        float redLength = redVector.Length();
                        float greenLength = greenVector.Length();
                        float blueLength = blueVector.Length();


                        string info = "Red:      " + Math.Round(redLength, 3).ToString()
                            + "\n\n" + "Green:  " + Math.Round(greenLength, 3).ToString()
                            + "\n\n" + "Blue:     " + Math.Round(blueLength, 3).ToString();

                        MultiAngleViewModel.BallDistance = info;

                        ShowPeriod2 = 0;

                    }
                }
            }

            ShowPeriod2++;
           
        }

    }
}
