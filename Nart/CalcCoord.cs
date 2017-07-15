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
using UseCVLibrary;

namespace Nart
{
    public class CalcCoord
    {
        /// <summary>
        ///兩台相機參數
        /// </summary>
        private CamParam[] _camParam = new CamParam[2];
        /// <summary>
        ///雙相機鏡心在世界座標
        /// </summary>
        public Point4D[] LensCenter = new Point4D[2];
        /// <summary>
        /// 基線座標系的旋轉矩陣
        /// </summary>
        public Matrix3D epipolarCoord;

        private const double MatchError = 3;

        private List<Marker3D> WorldPoints = new List<Marker3D>(10);

        //public static int PointsNumber = 0;

        private MainWindow _window = null;

        public List<double[]> MarkerDB = new List<double[]>(5);

        private Point3D[] CTBall; //CT珠子中心座標

        private Point3D[] MSBall; //MS點的珠子中心座標

        private Point3D[] MSMarker; //MS點的Marker中心座標

        private List<Marker3D> OriWorldPoints = new List<Marker3D>(10);

        private List<Marker3D> MSWorldPoints = new List<Marker3D>(10);

        private Matrix3D CTtoMS;

        private Matrix3D MStoCT;

        private Matrix3D OriWorldtoMS;

        public int CurrentHeadIndex = -1;

        public int RegHeadIndex = -1;
        ///<summary>
        ///頭在Database中引數
        /// </summary>
        public int RegSplintIndex = -1;
        /// <summary>
        ///頭在Database中引數
        /// </summary>
        private readonly int HeadInDatabase;
        /// <summary>
        ///咬板在Database中引數
        /// </summary>
        private readonly int SplintInDatabase;
        /// <summary>
        ///上顎在Database中引數
        /// </summary>
        private readonly int MaxillaInDatabase;
        /// <summary>
        ///下顎在Database中引數
        /// </summary>
        private readonly int MandibleInDatabase;


        private List<int> DatabaseIndex = new List<int>();

        public CalcCoord(MainWindow window)
        {
            _window = window;
            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
            CalcLensCenter();
            CalcEpipolarGeometry();
            LoadNartReg("../../../data/reg20170713.txt");
            CreateDatabase(out HeadInDatabase,out SplintInDatabase, out MaxillaInDatabase, out MandibleInDatabase);
            

        }
        /// <summary>
        /// 計算鏡心在世界座標
        /// </summary>
        private void CalcLensCenter()
        {
            LensCenter[0] = new Point4D(0, 0, 0, 1);
            LensCenter[1] = new Point4D(0, 0, 0, 1);

            //Console.WriteLine("Rotation1:\n" +
            //    _camParam[0].extParam.M11 + "\t" + _camParam[0].extParam.M12 + "\t" + _camParam[0].extParam.M13 + "\t" + _camParam[0].extParam.M14 + "\n" +
            //    _camParam[0].extParam.M21 + "\t" + _camParam[0].extParam.M22 + "\t" + _camParam[0].extParam.M23 + "\t" + _camParam[0].extParam.M24 + "\n" +
            //    _camParam[0].extParam.M31 + "\t" + _camParam[0].extParam.M32 + "\t" + _camParam[0].extParam.M33 + "\t" + _camParam[0].extParam.M34 + "\n" +
            //    _camParam[0].extParam.OffsetX + "\t" + _camParam[0].extParam.OffsetY + "\t" + _camParam[0].extParam.OffsetZ + "\t" + _camParam[0].extParam.M44 + "\n");

            //_camParam[0].extParam.Invert();
            //_camParam[1].extParam.Invert();

            //Console.WriteLine("Rotation2:\n" +
            //   _camParam[0].extParam.M11 + "\t" + _camParam[0].extParam.M12 + "\t" + _camParam[0].extParam.M13 + "\t" + _camParam[0].extParam.M14 + "\n" +
            //   _camParam[0].extParam.M21 + "\t" + _camParam[0].extParam.M22 + "\t" + _camParam[0].extParam.M23 + "\t" + _camParam[0].extParam.M24 + "\n" +
            //   _camParam[0].extParam.M31 + "\t" + _camParam[0].extParam.M32 + "\t" + _camParam[0].extParam.M33 + "\t" + _camParam[0].extParam.M34 + "\n" +
            //   _camParam[0].extParam.OffsetX + "\t" + _camParam[0].extParam.OffsetY + "\t" + _camParam[0].extParam.OffsetZ + "\t" + _camParam[0].extParam.M44 + "\n");



            //Console.WriteLine("\n\ninvinvExtParam\n" + _camParam[0].invExtParam);

            LensCenter[0] = _camParam[0].invExtParam.Transform(LensCenter[0]);
            LensCenter[1] = _camParam[1].invExtParam.Transform(LensCenter[1]);

            

        }
        /// <summary>
        /// 計算基線座標系
        /// </summary>
        private void CalcEpipolarGeometry()
        {
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
          
            epipolarCoord = new Matrix3D(vec_x.X, vec_y.X, vec_z.X, 0,
                                         vec_x.Y, vec_y.Y, vec_z.Y, 0,
                                         vec_x.Z, vec_y.Z, vec_z.Z, 0,
                                               0,       0,       0, 1);

            
        }
        /// <summary>
        /// 扭正左右兩邊拍攝到的像素座標點
        /// </summary>
        public void Rectify(List<BWMarker>[] OutputMarker)
        {

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

                        Point4D rectify_Point = epipolarCoord.Transform(WorldPoint);

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
            return new Point3D((LeftPoint.X + RightPoint.X) / 2.0, (LeftPoint.Y + RightPoint.Y) / 2.0, (LeftPoint.Z + RightPoint.Z) / 2.0);


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
                        Marker3D threeWorldPoints = new Marker3D(); 
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



            _window.PointLabel.Content = (WorldPoints.Count * 3).ToString() + "個點";

            

        }
        /// <summary>
        /// 將計算出的3D座標點與資料庫比對並存下引數
        /// </summary>
        public void MatchRealMarker()
        {
            for (int i = 0; i < WorldPoints.Count; i++)
            {               
                WorldPoints[i].CompareDatabase(MarkerDB);
            }


            //for (int i = 0; i < WorldPoints.Count; i++) //左相機Marker尋訪
            //{
            //    Console.WriteLine("\n\n排序後第" + (i + 1) + "組三邊長度");

            //    Vector3D a = new Vector3D(WorldPoints[i].ThreePoints[0].X - WorldPoints[i].ThreePoints[1].X, WorldPoints[i].ThreePoints[0].Y - WorldPoints[i].ThreePoints[1].Y, WorldPoints[i].ThreePoints[0].Z - WorldPoints[i].ThreePoints[1].Z);

            //    Vector3D b = new Vector3D(WorldPoints[i].ThreePoints[2].X - WorldPoints[i].ThreePoints[1].X, WorldPoints[i].ThreePoints[2].Y - WorldPoints[i].ThreePoints[1].Y, WorldPoints[i].ThreePoints[2].Z - WorldPoints[i].ThreePoints[1].Z);

            //    Vector3D c = new Vector3D(WorldPoints[i].ThreePoints[0].X - WorldPoints[i].ThreePoints[2].X, WorldPoints[i].ThreePoints[0].Y - WorldPoints[i].ThreePoints[2].Y, WorldPoints[i].ThreePoints[0].Z - WorldPoints[i].ThreePoints[2].Z);

            //    Console.WriteLine("\na:" + a.Length + "\nb:" + b.Length + "\nc:" + c.Length);

            //    Console.WriteLine("\nIndex:" + WorldPoints[i].DatabaseIndex);

            //    //Console.WriteLine("\nIndex:" + WorldPoints[i]. );

            //    Console.WriteLine("    ");
            //}
        }
        /// <summary>
        /// 以長度排序點大小
        /// </summary>
        private void SortedByLength(Point3D[] WorldPoints)
        {
            //第幾組點
           
                double[] Length = new double[3];

                Vector3D a = new Vector3D(WorldPoints[0].X - WorldPoints[1].X, WorldPoints[0].Y - WorldPoints[1].Y, WorldPoints[0].Z - WorldPoints[1].Z);

                Vector3D b = new Vector3D(WorldPoints[1].X - WorldPoints[2].X, WorldPoints[1].Y - WorldPoints[2].Y, WorldPoints[1].Z - WorldPoints[2].Z);

                Vector3D c = new Vector3D(WorldPoints[2].X - WorldPoints[0].X, WorldPoints[2].Y - WorldPoints[0].Y, WorldPoints[2].Z - WorldPoints[0].Z);

                Length[0] = a.Length;

                Length[1] = b.Length;

                Length[2] = c.Length;

                int[] Index = new int[3];

            if (Length[0] > Length[1] && Length[1] > Length[2])
            {
                Index[0] = 0;
                Index[1] = 1;
                Index[2] = 2;
            }

            else if (Length[0] > Length[2] && Length[2] > Length[1])
            {
                Index[0] = 1;
                Index[1] = 0;
                Index[2] = 2;
            }

            else if (Length[1] > Length[0] && Length[0] > Length[2])
            {
                Index[0] = 2;
                Index[1] = 1;
                Index[2] = 0;
            }

            else if (Length[1] > Length[2] && Length[2] > Length[0])
            {
                Index[0] = 1;
                Index[1] = 2;
                Index[2] = 0;
            }

            else if (Length[2] > Length[1] && Length[1] > Length[0])
            {
                Index[0] = 0;
                Index[1] = 2;
                Index[2] = 1;

            }

            else if (Length[2] > Length[0] && Length[0] > Length[1])
            {
                Index[0] = 2;
                Index[1] = 0;
                Index[2] = 1;
            }

            Point3D tempA = new Point3D(WorldPoints[Index[0]].X, WorldPoints[Index[0]].Y, WorldPoints[Index[0]].Z);
            Point3D tempB = new Point3D(WorldPoints[Index[1]].X, WorldPoints[Index[1]].Y, WorldPoints[Index[1]].Z);
            Point3D tempC = new Point3D(WorldPoints[Index[2]].X, WorldPoints[Index[2]].Y, WorldPoints[Index[2]].Z);

            WorldPoints[0] = tempA;
            WorldPoints[1] = tempB;
            WorldPoints[2] = tempC;
            
        }
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
        private void LoadNartReg(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);//"../../../data/CaliR_L.txt"
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

                MSMarker = new Point3D[3] { new Point3D(regData[18], regData[19], regData[20]),
                                            new Point3D(regData[21], regData[22], regData[23]),
                                            new Point3D(regData[24], regData[25], regData[26])};
                //SortedByLength(CTBall);
                //SortedByLength(MSBall);
                //SortedByLength(MSMarker);

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
                for (int i = 0; i < MSMarker.Length; i++)
                {
                    Console.WriteLine(MSMarker[i]);
                }
                Console.WriteLine("  ");
            }
            catch (Exception ex)
            {

                MessageBox.Show("註冊檔案錯誤");
            }
        }
        /// <summary>
        /// 輸入Marker的data
        /// </summary>
        private void CreateDatabase(out int headInDatabase , out int splintInDatabase, out int maxillaInDatabase, out int mandibleInDatabase)
        {
            double[] data1 = new double[3] { 22.82, 19.59, 11.88 }; //上顎           
            //double[] data2 = new double[3] { 24.64, 20.58, 13.79 }; 
            double[] data3 = new double[3] { 24.6, 22.597, 9.96 }; //下顎
            double[] data4 = new double[3] { 23.06, 17.753, 14.886 }; //頭
            double[] data5 = new double[3] { 21.36, 15.64, 11.8 }; //咬片

            MarkerDB.Add(data1);
            //MarkerDB.Add(data2);
            MarkerDB.Add(data3);
            MarkerDB.Add(data4);
            MarkerDB.Add(data5);

            maxillaInDatabase = 0;
            mandibleInDatabase = 1;
            headInDatabase = 2;
            splintInDatabase = 3;


            DatabaseIndex.Add(maxillaInDatabase);
            DatabaseIndex.Add(mandibleInDatabase);            
            DatabaseIndex.Add(headInDatabase);
            DatabaseIndex.Add(splintInDatabase);
        }
        /// <summary>
        /// 輸入點群資料跟指定的Marker(上、下顎)索引，回傳資料庫的索引位置
        /// </summary>
        private int GetSpecIndex(List<Marker3D> Markerdata,int specIndex)
        {
            for (int i = 0; i < Markerdata.Count; i++)
            {
                if (Markerdata[i].DatabaseIndex == specIndex)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 註冊當前狀態的值
        /// </summary>
        public void Registraion()
        {
            OriWorldPoints.Clear();
            MSWorldPoints.Clear();
 

            RegSplintIndex = GetSpecIndex(WorldPoints, SplintInDatabase);
           

            Console.WriteLine("\n\n第" + (RegSplintIndex + 1) + "組點是咬板");


            RegHeadIndex = GetSpecIndex(WorldPoints, HeadInDatabase);
            
            Console.WriteLine("\n\n第" + (RegHeadIndex + 1) + "組點是頭部");
         

            if (RegSplintIndex != -1 && RegHeadIndex != -1)
            {
                Console.WriteLine("\n\n第" + (RegSplintIndex + 1) + "組點是咬板");
                OriWorldtoMS = TransformCoordinate(WorldPoints[RegSplintIndex].ThreePoints, MSMarker);

                {
                    Console.WriteLine("\n\nWorldPoints[RegSplintIndex].ThreePoints");

                    Vector3D a = new Vector3D(WorldPoints[RegSplintIndex].ThreePoints[0].X - WorldPoints[RegSplintIndex].ThreePoints[1].X, WorldPoints[RegSplintIndex].ThreePoints[0].Y - WorldPoints[RegSplintIndex].ThreePoints[1].Y, WorldPoints[RegSplintIndex].ThreePoints[0].Z - WorldPoints[RegSplintIndex].ThreePoints[1].Z);

                    Vector3D b = new Vector3D(WorldPoints[RegSplintIndex].ThreePoints[2].X - WorldPoints[RegSplintIndex].ThreePoints[1].X, WorldPoints[RegSplintIndex].ThreePoints[2].Y - WorldPoints[RegSplintIndex].ThreePoints[1].Y, WorldPoints[RegSplintIndex].ThreePoints[2].Z - WorldPoints[RegSplintIndex].ThreePoints[1].Z);

                    Vector3D c = new Vector3D(WorldPoints[RegSplintIndex].ThreePoints[0].X - WorldPoints[RegSplintIndex].ThreePoints[2].X, WorldPoints[RegSplintIndex].ThreePoints[0].Y - WorldPoints[RegSplintIndex].ThreePoints[2].Y, WorldPoints[RegSplintIndex].ThreePoints[0].Z - WorldPoints[RegSplintIndex].ThreePoints[2].Z);

                    Console.WriteLine("\na:" + a.Length + "\nb:" + b.Length + "\nc:" + c.Length);



                    Console.WriteLine("\n\nMSMarker");

                    Vector3D d = new Vector3D(MSMarker[0].X - MSMarker[1].X, MSMarker[0].Y - MSMarker[1].Y, MSMarker[0].Z - MSMarker[1].Z);

                    Vector3D e = new Vector3D(MSMarker[2].X - MSMarker[1].X, MSMarker[2].Y - MSMarker[1].Y, MSMarker[2].Z - MSMarker[1].Z);

                    Vector3D f = new Vector3D(MSMarker[0].X - MSMarker[2].X, MSMarker[0].Y - MSMarker[2].Y, MSMarker[0].Z - MSMarker[2].Z);

                    Console.WriteLine("\nd:" + d.Length + "\ne:" + e.Length + "\nf:" + f.Length);

                    Console.WriteLine("    ");
                }






                for (int i = 0; i < WorldPoints.Count; i++)
                {
                    //創建註冊時的世界座標點
                    Marker3D OriWorldPoint = new Marker3D();
                    OriWorldPoint.DatabaseIndex = WorldPoints[i].DatabaseIndex;
                    WorldPoints[i].ThreePoints.CopyTo(OriWorldPoint.ThreePoints, 0); //將當前世界座標點存進OriWorldPoint當作註冊時的狀態
                   
                    //創建轉成MS座標系的世界座標點
                    Marker3D MSWorldPoint = new Marker3D();
                    MSWorldPoint.DatabaseIndex = WorldPoints[i].DatabaseIndex;
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
                    Console.WriteLine(OriWorldPoints[i].DatabaseIndex);
                }
                Console.WriteLine("\n\n\n");
                for (int i = 0; i < MSWorldPoints.Count; i++)
                {
                    Console.WriteLine("\n第" + (i + 1) + "組 MS座標");
                    Console.WriteLine(MSWorldPoints[i].ThreePoints[0]);
                    Console.WriteLine(MSWorldPoints[i].ThreePoints[1]);
                    Console.WriteLine(MSWorldPoints[i].ThreePoints[2]);
                    Console.WriteLine("引數:"+MSWorldPoints[i].DatabaseIndex);
                }

            }
            else
            {
                MessageBox.Show("找不到咬板或頭部Marker");
            }
            CameraControl.RegToggle = false;
        }

        public void CalcModelTransform()
        {

            CurrentHeadIndex = GetSpecIndex(WorldPoints, HeadInDatabase);

            //沒有找到頭的Marker
            if (CurrentHeadIndex != -1) 
            {
                Parallel.For(0, DatabaseIndex.Count-2, i =>
                {
                    int CurrentIndex = GetSpecIndex(WorldPoints, DatabaseIndex[i]); //當前處理的可動部位(上、下顎)索引
                    
                    int MSandOriIndex = GetSpecIndex(MSWorldPoints, DatabaseIndex[i]); //當前處理的上下顎對應到的MS跟Original World索引值
                    //Console.WriteLine("\n\n\n\n\ni:" + i + "\nMSandOriIndex: " + MSandOriIndex);
                    if (CurrentIndex != -1) 
                    {

                        //Matrix3D level1 = CTtoMS; //CT轉MS
                        Matrix3D level1 = TransformCoordinate(CTBall, MSBall);

                        Matrix3D level2 = TransformCoordinate(MSWorldPoints[MSandOriIndex].ThreePoints, WorldPoints[CurrentIndex].ThreePoints);//"註冊檔紀錄的可動部分的marker座標轉到MS座標的結果 MS Marker" to "追蹤LED(現在位置)"

                        Matrix3D level3 = TransformCoordinate(WorldPoints[CurrentHeadIndex].ThreePoints, OriWorldPoints[RegHeadIndex].ThreePoints);

                        Matrix3D level4 = TransformCoordinate(OriWorldPoints[RegSplintIndex].ThreePoints, MSMarker);
                  
                        Matrix3D level5 = TransformCoordinate(MSBall, CTBall);

                        Matrix3D Final = level1 * level2 * level3 * level4 * level5;

                        MainWindow.AllModelData[i].AddItem(Final);
                        //MainWindow.AllModelData[i].ModelTransform = Final;
                    }
                });                
            }
        }
    }

    
}
