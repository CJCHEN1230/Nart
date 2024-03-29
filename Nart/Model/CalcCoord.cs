﻿using Nart.Model_Object;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using HelixToolkit.Wpf.SharpDX;
using MathNet.Numerics.LinearAlgebra.Single;
using UseCVLibrary;
using Matrix = SharpDX.Matrix;
using Matrix3DExtensions = Nart.ExtensionMethods.Matrix3DExtensions;
using MathNet.Numerics.LinearAlgebra;
using Nart.Experiment;

namespace Nart
{
    public class CalcCoord
    {

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
        ///在註冊時存取下來算平均值用的頭部世界座標資料，可容納數量也是用來計算平均值所取的數量
        /// </summary>
        private readonly List<Point3D[]> _headMarkerStack = new List<Point3D[]>(10);
        /// <summary>
        ///在註冊時存取下來算平均值用的咬板世界座標資料，可容納數量也是用來計算平均值所取的數量
        /// </summary>
        private readonly List<Point3D[]> _splintMarkerStack = new List<Point3D[]>(10);
        /// <summary>
        ///顱顎面資訊Go Po Or Me...
        /// </summary>
        private CraniofacialInfo _craniofacialInfo;
        /// <summary>
        ///判斷是不是相同點的pixel高度容忍值
        /// </summary>
        private const double MatchError = 8;
        /// <summary>
        ///CT珠子中心座標
        /// </summary>
        private Point3D[] _CTBall;
        /// <summary>
        ///MS點的珠子中心座標
        /// </summary>
        private Point3D[] _MSBall;
        /// <summary>
        ///MS點的Marker中心座標
        /// </summary>
        private Point3D[] _MSSplintMarker;
        /// <summary>
        ///咬板Marker在CT中的座標
        /// </summary>
        private Point3D[] _oriSplintInCT = new Point3D[3];
        private Point3D[] _interSplintInCT = new Point3D[3];
        private Point3D[] _finalSplintInCT = new Point3D[3];
        private Point3D[] _headMarkerInCT;




        /// <summary>
        ///咬板Marker從CT中轉到世界座標
        /// </summary>
        private Matrix3D _cTtoWorld;
        /// <summary>
        ///咬板Marker從世界座標中轉到CT
        /// </summary>
        private Matrix3D _worldtoCT;

        private Matrix3D _CTtoMS;
        private Matrix3D _MStoCT;
        private Matrix3D _oriWorldtoMS;


        /// <summary>
        /// 當前點世界座標
        /// </summary>
        private List<Marker3D> _curWorldPoints = new List<Marker3D>(10);
        /// <summary>
        /// 註冊時的點世界座標
        /// </summary>
        private List<Marker3D> _oriWorldPoints = new List<Marker3D>(10);
        /// <summary>
        /// 世界座標轉換到MS的座標
        /// </summary>
        private List<Marker3D> _msWorldPoints = new List<Marker3D>(10);

        public static List<Marker3D> ForExperiment = new List<Marker3D>(10);
        //private MeanFilter _meanFilter;


        /// <summary>
        /// 每計算過幾次顯示一次
        /// </summary>
        public int ShowPeriod;
        public int ShowPeriod2 = 0;

        public CalcCoord()
        {

            _craniofacialInfo = new CraniofacialInfo("./data/ceph.csv");


            //_meanFilter = new MeanFilter(Database);



            CalcLensCenter();
            CalcEpipolarGeometry();
        }
        /// <summary>
        /// 計算鏡心在世界座標
        /// </summary>
        private void CalcLensCenter()
        {
            LensCenter[0] = new Point4D(0, 0, 0, 1);
            LensCenter[1] = new Point4D(0, 0, 0, 1);

           
            LensCenter[0] = SystemData.CameraParam[0].InvExtParam.Transform(LensCenter[0]);
            LensCenter[1] = SystemData.CameraParam[1].InvExtParam.Transform(LensCenter[1]);
        }
        /// <summary>
        /// 計算基線座標系
        /// </summary>
        private void CalcEpipolarGeometry()
        {
            //虛擬成像平面的中心
            Point4D planeCenter = new Point4D(0, 0, SystemData.CameraParam[0].FocalLength, 1);                       

            planeCenter = SystemData.CameraParam[0].InvExtParam.Transform(planeCenter);

            //Base line            
            Vector3D vecX = new Vector3D(LensCenter[1].X - LensCenter[0].X, LensCenter[1].Y - LensCenter[0].Y, LensCenter[1].Z - LensCenter[0].Z);

            vecX.Normalize();
          
            Vector3D vecTemp =new Vector3D(planeCenter.X - LensCenter[0].X, planeCenter.Y - LensCenter[0].Y, planeCenter.Z - LensCenter[0].Z);
                  
            Vector3D vecY = Vector3D.CrossProduct(vecTemp, vecX);
            vecY.Normalize();
        
            Vector3D vecZ = Vector3D.CrossProduct(vecX, vecY);
          
            EpipolarCoord = new Matrix3D(vecX.X, vecY.X, vecZ.X, 0,
                                         vecX.Y, vecY.Y, vecZ.Y, 0,
                                         vecX.Z, vecY.Z, vecZ.Z, 0,
                                               0,       0,       0, 1);

            
        }
        /// <summary>
        /// 輸入兩張照片的Marker資料，扭正左右兩邊拍攝到的像素座標點並存進RectifyY，並儲存相機坐標系的點進CameraPoint，並算出AvgRectifyY、AvgX
        /// </summary>
        public void Rectify(List<BWMarker>[] outputMarker)
        {
            //計算出平均x跟平均y
            Parallel.For(0, 2, i =>
            { 

            for (int j = 0; j < outputMarker[i].Count; j++)
                {
                    double accumX = 0;
                    double accumRecY = 0;
                    
                    for (int k = 0; k < 3; k++) //一個標記裡面三個點
                    {

                        PointF imagePoint=(PointF)(outputMarker[i][j].CornerPoint[k].ImagePoint);

                        double xd = SystemData.CameraParam[i].Dpx * (imagePoint.X - SystemData.CameraParam[i].Cx) / SystemData.CameraParam[i].Sx;
                        double yd = SystemData.CameraParam[i].Dy * (imagePoint.Y - SystemData.CameraParam[i].Cy);


                        double r = Math.Sqrt(xd * xd + yd * yd);

                        double xu = xd * (1 + SystemData.CameraParam[i].Kappa * r * r);
                        double yu = yd * (1 + SystemData.CameraParam[i].Kappa * r * r);

                        //儲存計算出來的相機座標系的點
                        outputMarker[i][j].CornerPoint[k].CameraPoint = new Point4D(xu, yu, SystemData.CameraParam[i].FocalLength, 1);

                        Point4D worldPoint = SystemData.CameraParam[i].RotationInvert.Transform(outputMarker[i][j].CornerPoint[k].CameraPoint);

                        Point4D rectifyPoint = EpipolarCoord.Transform(worldPoint);

                        double rectifyY = SystemData.CameraParam[i].FocalLength * rectifyPoint.Y / rectifyPoint.Z / SystemData.CameraParam[i].Dy;
                        //儲存此點扭正後的值
                        outputMarker[i][j].CornerPoint[k].RectifyY = rectifyY;
                        
                        accumX += imagePoint.X;
                        accumRecY += rectifyY;
                    }
                    outputMarker[i][j].AvgRectifyY = accumRecY / 3.0;
                    outputMarker[i][j].AvgX = accumX / 3.0;

                    outputMarker[i][j].CornerPoint.Sort(
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
                
                outputMarker[i].Sort();
                
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

            Point4D worldPoint1 = SystemData.CameraParam[0].InvExtParam.Transform(a);
            Point4D worldPoint2 = SystemData.CameraParam[1].InvExtParam.Transform(b);


            Vector3D d1 = new Vector3D(LensCenter[0].X - worldPoint1.X, LensCenter[0].Y - worldPoint1.Y, LensCenter[0].Z - worldPoint1.Z);
            Vector3D d2 = new Vector3D(LensCenter[1].X - worldPoint2.X, LensCenter[1].Y - worldPoint2.Y, LensCenter[1].Z - worldPoint2.Z);
            Point3D a1 = new Point3D(LensCenter[0].X, LensCenter[0].Y, LensCenter[0].Z);
            Point3D a2 = new Point3D(LensCenter[1].X, LensCenter[1].Y, LensCenter[1].Z);

            double delta = d1.LengthSquared * d2.LengthSquared - Vector3D.DotProduct(d2, d1) * Vector3D.DotProduct(d2, d1);

            double deltaX = Vector3D.DotProduct(a2 - a1, d1) * d2.LengthSquared - (Vector3D.DotProduct(a2 - a1, d2)) * Vector3D.DotProduct(d2, d1);

            double deltaY = d1.LengthSquared * Vector3D.DotProduct(a1 - a2, d2) - Vector3D.DotProduct(d2, d1) * Vector3D.DotProduct(a1 - a2, d1);

            double t1 = deltaX / delta;

            double t2 = deltaY / delta;


            Point3D leftPoint = new Point3D(a1.X + t1 * d1.X, a1.Y + t1 * d1.Y, a1.Z + t1 * d1.Z);
            Point3D rightPoint = new Point3D(a2.X + t2 * d2.X, a2.Y + t2 * d2.Y, a2.Z + t2 * d2.Z);

            //Console.WriteLine("左相機真實解:" + LeftPoint);
            //Console.WriteLine("右相機真實解:" + RightPoint);
            //Console.WriteLine("真實解:" + new Point3D((LeftPoint.X + RightPoint.X) / 2.0, (LeftPoint.Y + RightPoint.Y) / 2.0, (LeftPoint.Z + RightPoint.Z) / 2.0));
            return new Point3D((leftPoint.X + rightPoint.X) / 2.0, (leftPoint.Y + rightPoint.Y) / 2.0, -(leftPoint.Z + rightPoint.Z) / 2.0);


        }
        /// <summary>
        /// 匹配兩張圖的扭正點並反算出世界座標
        /// </summary>
        public void MatchAndCalc3D(List<BWMarker>[] outputMarker)
        {
            MarkerDatabase Database = SystemData.MarkerData;
            _curWorldPoints.Clear();
            int markerNumber = 0;
            string markerName = "";
            for (int i = 0, count = 0; i < outputMarker[0].Count; i++) //左相機Marker尋訪
            {
                for (int j = count; j < outputMarker[1].Count; j++) //右相機Marker尋訪
                {
                    //比對包括中間及四個點的Marker扭正Y位置
                    if (Math.Abs(outputMarker[0][i].AvgRectifyY - outputMarker[1][j].AvgRectifyY) < MatchError
                       && Math.Abs(outputMarker[0][i].CornerPoint[0].RectifyY - outputMarker[1][j].CornerPoint[0].RectifyY) < MatchError
                       && Math.Abs(outputMarker[0][i].CornerPoint[1].RectifyY - outputMarker[1][j].CornerPoint[1].RectifyY) < MatchError
                       && Math.Abs(outputMarker[0][i].CornerPoint[2].RectifyY - outputMarker[1][j].CornerPoint[2].RectifyY) < MatchError)
                    {
                        Marker3D threeWorldPoints = new Marker3D(); //Marker上面的三點世界座標

                        Parallel.For(0, 3, k =>
                        {
                            Point3D point3D = CalcWorldPoint(outputMarker[0][i].CornerPoint[k].CameraPoint, outputMarker[1][j].CornerPoint[k].CameraPoint);
                            threeWorldPoints.ThreePoints[k] = point3D;
                        });
                        
                        threeWorldPoints.SortedByLength(); //對計算到的3D點排序
                        
                        threeWorldPoints.CompareDatabase(ref Database.MarkerInfo);//比對當前世界座標與資料庫並存下引數與Marker的ID

                        _curWorldPoints.Add(threeWorldPoints);

                        count = j + 1;
                        

                        if (threeWorldPoints.MarkerId != "")
                        {
                            markerNumber++;

                            markerName += threeWorldPoints.MarkerId + "  ";
                        }

                        break;
                    }             
                }
            }

            ForExperiment.Clear();
            for (int i = 0; i < _curWorldPoints.Count; i++)
            {
                ForExperiment.Add(_curWorldPoints[i]);
            }
            ///_meanFilter.filter(ref _curWorldPoints);


            MainViewModel.PointNumber = (_curWorldPoints.Count * 3).ToString() + "Points";
            MainViewModel.MarkerNumber = markerNumber.ToString() + "Markers:   " + markerName;

        }       
        /// <summary>
        /// 目前尚未使用這種排序
        /// </summary>
        private void SortMarker(List<BWMarker>[] outputMarker)
        {
            Parallel.For(0, 2, imageIndex =>
            {
                for (int i = 0; i < outputMarker[imageIndex].Count; i++)
                { //第一張圖每個點輪流
                    int closePoints = 0;

                    for (int j = i + 1; j < outputMarker[imageIndex].Count;)
                    { //同樣是第一張圖，但是對自己的下一個點算y距離


                        if (Math.Abs(outputMarker[imageIndex][i].AvgRectifyY - outputMarker[imageIndex][j].AvgRectifyY) < 3)
                        { //y距離小於3pixel則計算累積個數在count，並直接算下一個點
                            closePoints++;
                            i++;
                            j = i + 1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (closePoints != 0)
                    { //count不等於0，意謂該高度附進有超過一個點，對此附近的點再進行x方向的排序
                        for (int k = i - closePoints, test = 0; k < i; k++, test++)
                        {
                            BWMarker temp;
                            for (int l = i - closePoints; l < i - test; l++)
                            {
                                if (outputMarker[imageIndex][l].AvgX > outputMarker[imageIndex][l + 1].AvgX)
                                {
                                    temp = outputMarker[imageIndex][l];
                                    outputMarker[imageIndex][l] = outputMarker[imageIndex][l + 1];
                                    outputMarker[imageIndex][l + 1] = temp;
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

            //尋訪資料庫中的Marker
            for (int i = 0, j = 0; i < SystemData.MarkerData.MarkerInfo.Count; i++)
            {   //尋訪世界座標中的Marker
                for (j = 0; j < _curWorldPoints.Count; j++)
                {
                    //如果資料庫中的MarkerID存在當前資料庫就檢查下一個
                    if (SystemData.MarkerData.MarkerInfo[i].MarkerID.Equals(_curWorldPoints[j].MarkerId))
                    {
                        break; //換下一個資料庫中的Marker
                    }
                }
                //資料庫中的Marker在WorldPoint中都找不到時
                if (j == _curWorldPoints.Count)
                {
                    SystemData.MarkerData.MarkerInfo.RemoveAt(i);
                    i--;
                }
            }
            SystemData.MarkerData.ResetIndex();
            //Database重建之後，將meanFilter裡面的stack也重建
           // _meanFilter.CreatePointStack(Database);
        }
        /// <summary>
        /// 傳入兩組三個點，任意對準座標軸，回傳轉移矩陣
        /// </summary>
        private Matrix3D OriginalTransformCoordinate(ref Point3D[] a, ref Point3D[] b)
        {
            List<Point3D[]> twoPoints = new List<Point3D[]>(2) { a, b };
            Point3D[] avg = new Point3D[2];
            Vector3D[] u = new Vector3D[2];
            Vector3D[] v = new Vector3D[2];
            Vector3D[] w = new Vector3D[2];

            Parallel.For(0, twoPoints.Count, i =>
           {
               avg[i] = new Point3D((twoPoints[i][0].X + twoPoints[i][1].X + twoPoints[i][2].X) / 3.0, (twoPoints[i][0].Y + twoPoints[i][1].Y + twoPoints[i][2].Y) / 3.0, (twoPoints[i][0].Z + twoPoints[i][1].Z + twoPoints[i][2].Z) / 3.0);

               u[i] = twoPoints[i][0] - avg[i];

               Vector3D temp = twoPoints[i][2] - avg[i];

               v[i] = Vector3D.CrossProduct(u[i], temp);

               w[i] = Vector3D.CrossProduct(u[i], v[i]);

               u[i].Normalize();
               v[i].Normalize();
               w[i].Normalize();

           });

            Matrix3D translate1 = new Matrix3D(1, 0, 0, 0,
                                              0, 1, 0, 0,
                                              0, 0, 1, 0,
                                             -avg[0].X, -avg[0].Y, -avg[0].Z, 1);

            Matrix3D rotate1 = new Matrix3D(u[0].X, v[0].X, w[0].X, 0,
                                              u[0].Y, v[0].Y, w[0].Y, 0,
                                              u[0].Z, v[0].Z, w[0].Z, 0,
                                             0, 0, 0, 1);

            Matrix3D transform1 = translate1 * rotate1;



            Matrix3D transform2 = new Matrix3D(u[1].X, u[1].Y, u[1].Z, 0,
                                               v[1].X, v[1].Y, v[1].Z, 0,
                                               w[1].X, w[1].Y, w[1].Z, 0,
                                             avg[1].X, avg[1].Y, avg[1].Z, 1);

            Matrix3D finalTransform = transform1 * transform2;


            return finalTransform;
        }
        /// <summary>
        /// 使用Kabsch algorithm做三點對三點座標轉換
        /// </summary>
        private Matrix3D TransformCoordinate(ref Point3D[] a,ref Point3D[] b)
        {
            if (a.Length != b.Length)
                return new Matrix3D();

            Point3D aavg = new Point3D(0, 0, 0);
            Point3D bavg = new Point3D(0, 0, 0);

            int Length = a.Length;

            //先算兩批點的點中心
            for (int i = 0; i < Length; i++) 
            {
                aavg.X += a[i].X;  aavg.Y += a[i].Y; aavg.Z += a[i].Z;
                bavg.X += b[i].X; bavg.Y += b[i].Y; bavg.Z += b[i].Z;
                if (i == Length-1)
                {
                    aavg.X /= Length; aavg.Y /= Length; aavg.Z /= Length;
                    bavg.X /= Length; bavg.Y /= Length; bavg.Z /= Length;
                }
            }
       
            float[,] PTArray = new float[Length, 3];
            float[,] QArray = new float[Length, 3];

            for (int i = 0; i < Length; i++)
            {
                PTArray[i, 0] = Convert.ToSingle(a[i].X - aavg.X); PTArray[i, 1] = Convert.ToSingle(a[i].Y - aavg.Y); PTArray[i, 2] = Convert.ToSingle(a[i].Z - aavg.Z);
                QArray[i, 0] = Convert.ToSingle(b[i].X - bavg.X); QArray[i, 1] = Convert.ToSingle(b[i].Y - bavg.Y); QArray[i, 2] = Convert.ToSingle(b[i].Z - bavg.Z);
            }

            var PT = DenseMatrix.OfArray(PTArray).Transpose();
            var Q = DenseMatrix.OfArray(QArray);
            

            var A = PT * Q;
            var svd1 = A.Svd(true);
            Matrix<float> U = svd1.U;

            Matrix<float> VT = svd1.VT;

            var UVT = U * VT;
            float det = UVT.Determinant();
            var dia = DenseMatrix.OfArray(new float[,]
            {
                {1.0f, 0.0f, 0.0f},
                {0.0f, 1.0f, 0.0f},
                {0.0f, 0.0f, det}
            });

            var R = U * dia * VT;
            Matrix3D T = new Matrix3D(1, 0, 0, 0
                                                                , 0, 1, 0, 0
                                                                , 0, 0, 1, 0
                                                                , -aavg.X, -aavg.Y, -aavg.Z, 1);
            Matrix3D R1 = new Matrix3D(R[0, 0], R[0, 1], R[0, 2], 0
                                                                  , R[1, 0], R[1, 1], R[1, 2], 0
                                                                  , R[2, 0], R[2, 1], R[2, 2], 0,
                                                                  bavg.X, bavg.Y, bavg.Z, 1);



            Matrix3D finalTransform = T * R1;


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

                
                _CTBall = new Point3D[3] { new Point3D(regData[0], regData[1], regData[2]),
                                          new Point3D(regData[3], regData[4], regData[5]),
                                          new Point3D(regData[6], regData[7], regData[8])};
                
                _MSBall = new Point3D[3] { new Point3D(regData[9], regData[10], regData[11]),
                                          new Point3D(regData[12], regData[13], regData[14]),
                                          new Point3D(regData[15], regData[16], regData[17])};

                _MSSplintMarker = new Point3D[3] { new Point3D(regData[18], regData[19], regData[20]),
                                            new Point3D(regData[21], regData[22], regData[23]),
                                            new Point3D(regData[24], regData[25], regData[26])};
              

                _CTtoMS = TransformCoordinate(ref _CTBall,ref  _MSBall);


                _MStoCT = TransformCoordinate(ref _MSBall,ref _CTBall);


                Console.WriteLine("\nCTBall");
                for (int i =0;i< _CTBall.Length ;i++)
                {
                    Console.WriteLine(_CTBall[i]);
                }

                Console.WriteLine("\nSplintBall");
                for (int i = 0; i < _MSBall.Length; i++)
                {
                    Console.WriteLine(_MSBall[i]);
                }

                Console.WriteLine("\nSplintMarker");
                for (int i = 0; i < _MSSplintMarker.Length; i++)
                {
                    Console.WriteLine(_MSSplintMarker[i]);
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
        /// <summary>
        /// 匯入咬板標記片上的點在CT當中的座標
        /// </summary>
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

                _oriSplintInCT = new Point3D[3] { new Point3D(matrixInfo[0], matrixInfo[1], matrixInfo[2]),
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


        private void LoadSplintPoint2()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                                             
                {
                    doc.Load("./data/蔡慧君/原咬.xml");
                    XmlNodeList pointList = doc.SelectNodes("SplintData/MSFiducial/Point");
                    Point3D[] fiducialPointMS = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointMS[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointMS[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointMS[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/CT/Point");
                    Point3D[] fiducialPointCT = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointCT[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointCT[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointCT[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/MSMarker/Point");
                    Point3D[] MSMarkerPoint = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        MSMarkerPoint[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        MSMarkerPoint[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        MSMarkerPoint[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }

                    Matrix3D transformMat = TransformCoordinate(ref fiducialPointMS, ref fiducialPointCT);
                    transformMat.Transform(MSMarkerPoint);
                    _oriSplintInCT = MSMarkerPoint;

                    string path = "D:\\Desktop\\OriSplintInCT.txt";
                    try
                    {
                        string fileContent = File.ReadAllText(path);
                        string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                        float[] pointInfo = Array.ConvertAll(contentArray, float.Parse);
                        _oriSplintInCT[0] = new Point3D(pointInfo[0], pointInfo[1], pointInfo[2]);
                        _oriSplintInCT[1] = new Point3D(pointInfo[3], pointInfo[4], pointInfo[5]);
                        _oriSplintInCT[2] = new Point3D(pointInfo[6], pointInfo[7], pointInfo[8]);

                        //////////////////////////
                        Matrix3D level2 = TransformCoordinate(ref _oriSplintInCT, ref MSMarkerPoint);

                        level2.Transform(fiducialPointCT);

                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("點的讀取錯誤");
                    }
                }
                {
                    doc.Load("./data/蔡慧君/疊上inter.xml");
                    XmlNodeList pointList = doc.SelectNodes("SplintData/MSFiducial/Point");
                    Point3D[] fiducialPointMS = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointMS[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointMS[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointMS[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/CT/Point");
                    Point3D[] fiducialPointCT = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointCT[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointCT[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointCT[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/MSMarker/Point");
                    Point3D[] MSMarkerPoint = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        MSMarkerPoint[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        MSMarkerPoint[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        MSMarkerPoint[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }

                    Matrix3D transformMat = TransformCoordinate(ref fiducialPointMS, ref fiducialPointCT);
                    transformMat.Transform(MSMarkerPoint);
                    _interSplintInCT = MSMarkerPoint;

                    //string path = "D:\\Desktop\\interSplintInCT.txt";
                    //try
                    //{
                    //    string fileContent = File.ReadAllText(path);
                    //    string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                    //    float[] pointInfo = Array.ConvertAll(contentArray, float.Parse);
                    //    _interSplintInCT[0] = new Point3D(pointInfo[0], pointInfo[1], pointInfo[2]);
                    //    _interSplintInCT[1] = new Point3D(pointInfo[3], pointInfo[4], pointInfo[5]);
                    //    _interSplintInCT[2] = new Point3D(pointInfo[6], pointInfo[7], pointInfo[8]);

                    //    //////////////////////////
                    //    Matrix3D level2 = TransformCoordinate(ref _interSplintInCT, ref MSMarkerPoint);

                    //    level2.Transform(fiducialPointCT);
                    //}
                    //catch
                    //{
                    //    System.Windows.MessageBox.Show("點的讀取錯誤");

                    //}

                }
                {
                    doc.Load("./data/蔡慧君/疊下final.xml");
                    XmlNodeList pointList = doc.SelectNodes("SplintData/MSFiducial/Point");
                    Point3D[] fiducialPointMS = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointMS[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointMS[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointMS[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/CT/Point");
                    Point3D[] fiducialPointCT = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        fiducialPointCT[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        fiducialPointCT[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        fiducialPointCT[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }
                    pointList = doc.SelectNodes("SplintData/MSMarker/Point");
                    Point3D[] MSMarkerPoint = new Point3D[pointList.Count];
                    for (int i = 0; i < pointList.Count; i++)
                    {
                        XmlNodeList fiducialPoint = pointList[i].ChildNodes;

                        MSMarkerPoint[i].X = Convert.ToDouble(fiducialPoint[0].InnerText);
                        MSMarkerPoint[i].Y = Convert.ToDouble(fiducialPoint[1].InnerText);
                        MSMarkerPoint[i].Z = Convert.ToDouble(fiducialPoint[2].InnerText);
                    }

                    Matrix3D transformMat = TransformCoordinate(ref fiducialPointMS, ref fiducialPointCT);
                    transformMat.Transform(MSMarkerPoint);
                    _finalSplintInCT = MSMarkerPoint;

                    //string path = "D:\\Desktop\\finalSplintInCT.txt";
                    //try
                    //{
                    //    string fileContent = File.ReadAllText(path);
                    //    string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                    //    float[] pointInfo = Array.ConvertAll(contentArray, float.Parse);
                    //    _finalSplintInCT[0] = new Point3D(pointInfo[0], pointInfo[1], pointInfo[2]);
                    //    _finalSplintInCT[1] = new Point3D(pointInfo[3], pointInfo[4], pointInfo[5]);
                    //    _finalSplintInCT[2] = new Point3D(pointInfo[6], pointInfo[7], pointInfo[8]);

                    //    Matrix3D level2 = TransformCoordinate(ref _finalSplintInCT, ref MSMarkerPoint);

                    //    level2.Transform(fiducialPointCT);
                    //}
                    //catch
                    //{
                    //    System.Windows.MessageBox.Show("點的讀取錯誤");

                    //}
                }
            }
            catch
            {
                MessageBox.Show("咬板檔案錯誤");
            }


            












        }






        /// <summary>
        /// 輸入點群資料跟指定的Marker(上、下顎、頭)ID，回傳資料庫的索引位置
        /// </summary>
        private int GetSpecIndex(List<Marker3D> markerdata,String id)
        {
            for (int i = 0; i < markerdata.Count; i++) 
            {
                if (markerdata[i].MarkerId.Equals(id)) 
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void InitializeRegistration()
        {
            _oriWorldPoints.Clear();
            _msWorldPoints.Clear();
           
            //取得咬板Marker在WorldPoints中的索引值
            SystemData.RegSplintIndex = GetSpecIndex(_curWorldPoints, "Splint");             
            //取得頭Marker在WorldPoints中的索引值
            SystemData.RegHeadIndex = GetSpecIndex(_curWorldPoints, "Head");

            //註冊需要咬板與頭部標記片，找不到就返回
            if (SystemData.RegSplintIndex == -1 || SystemData.RegHeadIndex== -1)
            {
                //只缺少咬板標記片
                if (SystemData.RegSplintIndex == -1 && SystemData.RegHeadIndex != -1)
                {
                    MessageBox.Show("找不到咬板標記片");
                }
                //只缺少頭部標記片
                else if (SystemData.RegSplintIndex != -1 && SystemData.RegHeadIndex == -1)
                {
                    MessageBox.Show("找不到頭部標記片");
                }
                //兩個都沒有
                else
                {
                    MessageBox.Show("找不到咬板以及頭部標記片");
                }
                SystemData.RegToggle = false;
                return;
            }


            //讀進_splintInCT檔案
            if (!LoadSplintPoint("./data/蔡慧君測試用.txt"))
            {
                SystemData.RegToggle = false;
                return;
            }

            LoadSplintPoint2();


            //簡化資料庫的Marker
            SimplifyDatabase();
            //
            CalcFHCoord();
            
        
            _cTtoWorld = TransformCoordinate(ref _oriSplintInCT,ref  _curWorldPoints[SystemData.RegSplintIndex].ThreePoints);
            _worldtoCT = TransformCoordinate(ref _curWorldPoints[SystemData.RegSplintIndex].ThreePoints,ref _oriSplintInCT);


            MainViewModel.ProjData.IsRegInitialized = true;
            
        }
        /// <summary>
        /// 原Nart的註冊當前狀態的值
        /// </summary>
        public void Registraion()
        {
            _oriWorldPoints.Clear();
            _msWorldPoints.Clear();

            if (!LoadNartReg(ModelSettingViewModel.RegPath))
            {
                return;
            }

            //先判斷當前標記片數量是否過少
            if (_curWorldPoints.Count >= 2)
            {
                //取得咬板Marker在WorldPoints中的索引值
                SystemData.RegSplintIndex = GetSpecIndex(_curWorldPoints, "Splint");
                Console.WriteLine("\n\n第" + (SystemData.RegSplintIndex + 1) + "組點是咬板");
                //取得頭Marker在WorldPoints中的索引值
                SystemData.RegHeadIndex = GetSpecIndex(_curWorldPoints, "Head");
                Console.WriteLine("\n\n第" + (SystemData.RegHeadIndex + 1) + "組點是頭部");

                //註冊需要咬板與頭部標記片
                if (SystemData.RegSplintIndex != -1 && SystemData.RegHeadIndex != -1)
                {

                    SimplifyDatabase();

                    CalcFHCoord();

                    for (int i = 0; i < SystemData.MarkerData.MarkerInfo.Count; i++)
                    {
                        Console.WriteLine("\n\n" + SystemData.MarkerData.MarkerInfo[i].ThreeLength[0] + " " + SystemData.MarkerData.MarkerInfo[i].ThreeLength[1] + " " + SystemData.MarkerData.MarkerInfo[i].ThreeLength[2]);
                    }
                    Console.WriteLine("splint:" + SystemData.MarkerData.SplintIndex);
                    Console.WriteLine("Head:" + SystemData.MarkerData.HeadIndex);
                    

                    for (int i = 0; i < _curWorldPoints.Count; i++)
                    {
                        Console.WriteLine("MarkerID" + i + ":" + _curWorldPoints[i].MarkerId);
                    }


                    _oriWorldtoMS = TransformCoordinate(ref _curWorldPoints[SystemData.RegSplintIndex].ThreePoints,ref _MSSplintMarker);

                    foreach (Marker3D marker3D in _curWorldPoints)
                    {
                        //創建註冊時的世界座標點
                        Marker3D oriWorldPoint = new Marker3D { MarkerId = marker3D.MarkerId };
                        marker3D.ThreePoints.CopyTo(oriWorldPoint.ThreePoints, 0); //將當前世界座標點存進OriWorldPoint當作註冊時的狀態

                        //將所有世界座標點乘上OriWorldtoMS 轉成MS座標
                        Marker3D msWorldPoint = new Marker3D { MarkerId = marker3D.MarkerId };
                        _oriWorldtoMS.Transform(marker3D.ThreePoints);
                        marker3D.ThreePoints.CopyTo(msWorldPoint.ThreePoints, 0);

                        _oriWorldPoints.Add(oriWorldPoint);
                        _msWorldPoints.Add(msWorldPoint);
                    }

                    MessageBox.Show("註冊了" + _curWorldPoints.Count + "組Marker");

                    for (int i = 0; i < _oriWorldPoints.Count; i++)
                    {
                        Console.WriteLine("\n第" + (i + 1) + "組 世界座標");
                        Console.WriteLine(_oriWorldPoints[i].ThreePoints[0]);
                        Console.WriteLine(_oriWorldPoints[i].ThreePoints[1]);
                        Console.WriteLine(_oriWorldPoints[i].ThreePoints[2]);
                        Console.WriteLine(_oriWorldPoints[i].MarkerId);
                    }
                    Console.WriteLine("\n\n\n");
                    for (int i = 0; i < _msWorldPoints.Count; i++)
                    {
                        Console.WriteLine("\n第" + (i + 1) + "組 MS座標");
                        Console.WriteLine(_msWorldPoints[i].ThreePoints[0]);
                        Console.WriteLine(_msWorldPoints[i].ThreePoints[1]);
                        Console.WriteLine(_msWorldPoints[i].ThreePoints[2]);
                        Console.WriteLine("引數:" + _msWorldPoints[i].MarkerId);
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
            SystemData.RegToggle = false;
        }
        /// <summary>
        /// 導航的註冊
        /// </summary>
        public void Registraion2()
        {
            _oriWorldPoints.Clear();
            _msWorldPoints.Clear();

            //註冊前初始化
            if (!MainViewModel.ProjData.IsRegInitialized)
            {
                InitializeRegistration();
            }

            //取得咬板Marker在WorldPoints中的索引值
            SystemData.RegSplintIndex = GetSpecIndex(_curWorldPoints, "Splint");
            //取得頭Marker在WorldPoints中的索引值
            SystemData.RegHeadIndex = GetSpecIndex(_curWorldPoints, "Head");


            //註冊需要咬板與頭部標記片
            if (SystemData.RegSplintIndex != -1 && SystemData.RegHeadIndex != -1)
            {

                _splintMarkerStack.Add(_curWorldPoints[SystemData.RegSplintIndex].ThreePoints);
                _headMarkerStack.Add(_curWorldPoints[SystemData.RegHeadIndex].ThreePoints);


                //各收集到100組後進來
                if (_headMarkerStack.Count == _headMarkerStack.Capacity && _splintMarkerStack.Count == _splintMarkerStack.Capacity)
                {

                    Point3D[] headMarkerPoint = new Point3D[3];
                    foreach (Point3D[] points in _headMarkerStack)
                    {
                        headMarkerPoint[0].X += points[0].X;
                        headMarkerPoint[0].Y += points[0].Y;
                        headMarkerPoint[0].Z += points[0].Z;

                        headMarkerPoint[1].X += points[1].X;
                        headMarkerPoint[1].Y += points[1].Y;
                        headMarkerPoint[1].Z += points[1].Z;

                        headMarkerPoint[2].X += points[2].X;
                        headMarkerPoint[2].Y += points[2].Y;
                        headMarkerPoint[2].Z += points[2].Z;
                    }

                    headMarkerPoint[0].X /= _headMarkerStack.Count;
                    headMarkerPoint[0].Y /= _headMarkerStack.Count;
                    headMarkerPoint[0].Z /= _headMarkerStack.Count;

                    headMarkerPoint[1].X /= _headMarkerStack.Count; ;
                    headMarkerPoint[1].Y /= _headMarkerStack.Count; ;
                    headMarkerPoint[1].Z /= _headMarkerStack.Count; ;

                    headMarkerPoint[2].X /= _headMarkerStack.Count; ;
                    headMarkerPoint[2].Y /= _headMarkerStack.Count; ;
                    headMarkerPoint[2].Z /= _headMarkerStack.Count; ;




                    Point3D[] splintMarkerPoint = new Point3D[3];
                    foreach (Point3D[] points in _splintMarkerStack)
                    {
                        splintMarkerPoint[0].X += points[0].X;
                        splintMarkerPoint[0].Y += points[0].Y;
                        splintMarkerPoint[0].Z += points[0].Z;

                        splintMarkerPoint[1].X += points[1].X;
                        splintMarkerPoint[1].Y += points[1].Y;
                        splintMarkerPoint[1].Z += points[1].Z;

                        splintMarkerPoint[2].X += points[2].X;
                        splintMarkerPoint[2].Y += points[2].Y;
                        splintMarkerPoint[2].Z += points[2].Z;
                    }

                    splintMarkerPoint[0].X /= _splintMarkerStack.Count;
                    splintMarkerPoint[0].Y /= _splintMarkerStack.Count;
                    splintMarkerPoint[0].Z /= _splintMarkerStack.Count;

                    splintMarkerPoint[1].X /= _splintMarkerStack.Count;
                    splintMarkerPoint[1].Y /= _splintMarkerStack.Count;
                    splintMarkerPoint[1].Z /= _splintMarkerStack.Count;

                    splintMarkerPoint[2].X /= _splintMarkerStack.Count;
                    splintMarkerPoint[2].Y /= _splintMarkerStack.Count;
                    splintMarkerPoint[2].Z /= _splintMarkerStack.Count;


                    _curWorldPoints[SystemData.RegSplintIndex].ThreePoints = splintMarkerPoint;
                    _curWorldPoints[SystemData.RegHeadIndex].ThreePoints = headMarkerPoint;



                    _cTtoWorld = TransformCoordinate(ref _oriSplintInCT,ref  _curWorldPoints[SystemData.RegSplintIndex].ThreePoints);
                    _worldtoCT = TransformCoordinate(ref _curWorldPoints[SystemData.RegSplintIndex].ThreePoints,ref  _oriSplintInCT);

                    _headMarkerInCT = new Point3D[3];

                    _curWorldPoints[SystemData.RegHeadIndex].ThreePoints.CopyTo(_headMarkerInCT, 0);

                      _worldtoCT.Transform(_headMarkerInCT);

                    







                    Marker3D oriWorldPoint1 = new Marker3D
                    {
                        ThreePoints = splintMarkerPoint,
                        MarkerId = "Splint"
                    };
                    oriWorldPoint1.SortedByLength();
                    _oriWorldPoints.Add(oriWorldPoint1);

                    Marker3D oriWorldPoint2 = new Marker3D
                    {
                        ThreePoints = headMarkerPoint,
                        MarkerId = "Head"
                    };
                    oriWorldPoint2.SortedByLength();
                    _oriWorldPoints.Add(oriWorldPoint2);

                    SystemData.RegSplintIndex = 0;                    
                    SystemData.RegHeadIndex = 1;


                    _headMarkerStack.Clear();
                    _splintMarkerStack.Clear();
                    MessageBox.Show("註冊了" + _curWorldPoints.Count + "組Marker");
                    //註冊結束之後，將註冊狀態關閉
                    SystemData.RegToggle = false;
                    MainViewModel.ProjData.IsRegistered = true;
                }
            }
        }        
        /// <summary>
        /// 計算出每個Marker的轉移矩陣
        /// </summary>
        public void CalcModelTransform()
        {
            int currentHeadIndex = GetSpecIndex(_curWorldPoints, "Head");

            //沒有找到頭的Marker
            if (currentHeadIndex != -1) 
            {
                
                Parallel.For(0, _curWorldPoints.Count, i =>
               {
                   //當前Marker找不到或找到的是頭部Marker則跳過
                   if (!_curWorldPoints[i].MarkerId.Equals("")/*&& !WorldPoints[i].MarkerID.Equals("Splint") */&& !_curWorldPoints[i].MarkerId.Equals("Head"))
                    {
                        int mSandOriIndex = GetSpecIndex(_msWorldPoints, _curWorldPoints[i].MarkerId);//取得當前世界座標在註冊時的座標索引值是多少

                        //Matrix3D level1 = TransformCoordinate(CTBall, MSBall);

                        Matrix3D level2 = TransformCoordinate(ref _msWorldPoints[mSandOriIndex].ThreePoints,ref _curWorldPoints[i].ThreePoints);//"註冊檔紀錄的可動部分的marker座標轉到MS座標的結果 MS Marker" to "追蹤LED(現在位置)"

                        Matrix3D level3 = TransformCoordinate(ref _curWorldPoints[currentHeadIndex].ThreePoints,ref _msWorldPoints[SystemData.RegHeadIndex].ThreePoints);

                        //Matrix3D level4 = TransformCoordinate(MSBall, CTBall);
                        
                        Matrix3D final = _CTtoMS * level2 * level3 * _MStoCT;

                        var boneCollection = MainViewModel.ProjData.BoneCollection;
                       foreach (BoneModel boneModel in boneCollection)
                       {                          
                           if (boneModel != null && boneModel.MarkerId == _curWorldPoints[i].MarkerId)
                           {
                               boneModel.AddItem(final);                       
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
            int currentHeadIndex = GetSpecIndex(_curWorldPoints, "Head");

            //沒有找到頭的Marker
            if (currentHeadIndex != -1)
            {

                //Parallel.For(0, WorldPoints.Count, i =>
                for (int i = 0; i < _curWorldPoints.Count; i++) 
                {
                    //當前Marker找不到或找到的是頭部Marker則跳過
                    if (_curWorldPoints[i].MarkerId.Equals("Splint")) 
                    {
                  
                        int splintMarkerIndex = GetSpecIndex(_curWorldPoints, "Splint");//取得當前世界座標在註冊時的座標索引值是多少
                        Matrix3D level1;
                                     
                            
                        if (SystemData.IsFirstStage)           
                            level1 = TransformCoordinate(ref _interSplintInCT, ref _curWorldPoints[i].ThreePoints);
                        else if(SystemData.IsSecondStage)
                            level1 = TransformCoordinate(ref _finalSplintInCT, ref _curWorldPoints[i].ThreePoints);
                        else
                            level1 = TransformCoordinate(ref _oriSplintInCT, ref _curWorldPoints[i].ThreePoints);

                        Matrix3D level2 = TransformCoordinate(ref _curWorldPoints[currentHeadIndex].ThreePoints,ref  _headMarkerInCT);


                        Matrix3D final =  level1 * level2;

                        var boneCollection = MainViewModel.ProjData.BoneCollection;
                        foreach (BoneModel boneModel in boneCollection)
                        {                           
                            if (boneModel.MarkerId == _curWorldPoints[i].MarkerId&& boneModel.IsRendering)
                            {
                               boneModel.AddItem(final);
                            }
                        }
                    }
                    //});
                }

            }
        }
        /// <summary>
        /// 計算出FH平面所形成的坐標系，以及Go點連線與下顎對稱面交點
        /// </summary>
        private void CalcFHCoord()
        {
            int mandibleOspIndex = -1;
            int headOspIndex = -1;
            
            //先找出模型引數
            for (int i = 0; i < MultiAngleViewModel.OspModelCollection.Count; i++)
            {                
                OspModel ospModel = MultiAngleViewModel.OspModelCollection[i] as OspModel;

                if (ospModel != null && ospModel.IsLoaded)
                {
                    if (ospModel.MarkerId == "Head")
                    {
                        headOspIndex = i;
                    }
                    if (ospModel.MarkerId == "C")
                    {
                        mandibleOspIndex = i;
                    }
                }
            }
            //如果下顎跟頭顱的引數都沒找到 則根本無法計算五項指標
            if (headOspIndex == -1 || mandibleOspIndex == -1)
            {
                MessageBox.Show("找不到下顎跟頭顱的OSP");
                return;
            }
            //取得頭顱跟下顎的模型
            OspModel headOsp = MultiAngleViewModel.OspModelCollection[headOspIndex] as OspModel;
            OspModel mandibleOsp = MultiAngleViewModel.OspModelCollection[mandibleOspIndex] as OspModel;


            //頭顱模型的索引值
            //headNormal = headOSP.OSPOriNormal;
            //下顎模型的索引值
            //mandibleNormal = mandibleOSP.OSPOriNormal;
            Vector3D headNormal = headOsp.GetCurrentNormal();
            Vector3D mandibleNormal = mandibleOsp.GetCurrentNormal();


            //Po Or四個點
            Point3D[] poOr = new Point3D[] { _craniofacialInfo.PoL, _craniofacialInfo.PoR, _craniofacialInfo.OrL, _craniofacialInfo.OrR };            
            Point3D normalPlanePoint = headOsp.OspPlanePoint;
            Point3D[] PoOrProj = new Point3D[4];

            //計算Po Or四點投影到頭顱對稱面的點座標存進PoOrProj
            for (int i = 0; i < poOr.Length; i++)
            {
                double v1 = normalPlanePoint.X - poOr[i].X;
                double v2 = normalPlanePoint.Y - poOr[i].Y;
                double v3 = normalPlanePoint.Z - poOr[i].Z;

                double t1 = (headNormal.X * v1 + headNormal.Y * v2 + headNormal.Z * v3) / (headNormal.LengthSquared);

                double x1 = poOr[i].X + headNormal.X * t1;
                double y1 = poOr[i].Y + headNormal.Y * t1;
                double z1 = poOr[i].Z + headNormal.Z * t1;
                PoOrProj[i] = new Point3D(x1, y1, z1);
            }

            //投影到頭顱對稱面的Po兩點平均
            Point3D avgPo = new Point3D((PoOrProj[0].X + PoOrProj[1].X) / 2.0, (PoOrProj[0].Y + PoOrProj[1].Y) / 2.0, (PoOrProj[0].Z + PoOrProj[1].Z) / 2.0);
            //投影到頭顱對稱面的Or兩點平均
            Point3D avgOr = new Point3D((PoOrProj[2].X + PoOrProj[3].X) / 2.0, (PoOrProj[2].Y + PoOrProj[3].Y) / 2.0, (PoOrProj[2].Z + PoOrProj[3].Z) / 2.0);
            //定義FH平面座標系
            Vector3D fhy = avgPo - avgOr;
            Vector3D fhz = Vector3D.CrossProduct(fhy, headNormal);
            Vector3D fhx = Vector3D.CrossProduct(fhy, fhz);

            fhx.Normalize();
            fhy.Normalize();
            fhz.Normalize();


            FHCoord = new Matrix3D(fhx.X, fhy.X, fhz.X, 0,
                                                             fhx.Y, fhy.Y, fhz.Y, 0,
                                                             fhx.Z, fhy.Z, fhz.Z, 0,
                                                                      0,         0,          0, 1);


            //計算Go點連線與下顎對稱面交點 存進GoIntersection
            Point3D manPlanePoint = mandibleOsp.OspPlanePoint;
            Vector3D goVector = _craniofacialInfo.GoL - _craniofacialInfo.GoR;

            double t = (mandibleNormal.X * (manPlanePoint.X - _craniofacialInfo.GoL.X) + mandibleNormal.Y * (manPlanePoint.Y - _craniofacialInfo.GoL.Y) + mandibleNormal.Z * (manPlanePoint.Z - _craniofacialInfo.GoL.Z))
                /(mandibleNormal.X * goVector.X + mandibleNormal.Y * goVector.Y + mandibleNormal.Z * goVector.Z);
            double x = _craniofacialInfo.GoL.X + goVector.X * t;
            double y = _craniofacialInfo.GoL.Y + goVector.Y * t;
            double z = _craniofacialInfo.GoL.Z + goVector.Z * t;
            _craniofacialInfo.GoIntersection = new Point3D(x, y, z);

        }        
        /// <summary>
        /// 計算出顱面資訊
        /// </summary>
        public void CalcCraniofacialInfo()
        {
            int mandibleOspIndex = -1;
            int headOspIndex = -1;
            if (ShowPeriod == 5)
            {
                //先找出模型引數
                for (int i = 0; i < MultiAngleViewModel.OspModelCollection.Count; i++)
                {
                    OspModel ospModel = MultiAngleViewModel.OspModelCollection[i] as OspModel;

                    if (ospModel!=null && ospModel.IsLoaded)
                    {
                        if (ospModel.MarkerId == "Head")
                        {
                            headOspIndex = i;
                        }
                        if (ospModel.MarkerId == "C")
                        {
                            mandibleOspIndex = i;
                        }
                    }
                }

                if (headOspIndex == -1 || mandibleOspIndex == -1)
                {
                    return;
                }

                OspModel headOsp = MultiAngleViewModel.OspModelCollection[headOspIndex] as OspModel;
                OspModel mandibleOsp = MultiAngleViewModel.OspModelCollection[mandibleOspIndex] as OspModel;
                
                Vector3D headNormal;
                Vector3D mandibleNormal;
                headNormal = headOsp.GetCurrentNormal();
                mandibleNormal = mandibleOsp.GetCurrentNormal();

                
                //先算DA
                double da = Vector3D.AngleBetween(headNormal, mandibleNormal);

                Vector3D fhMandibleNormal = FHCoord.Transform(mandibleNormal);
                double x = fhMandibleNormal.X;
                double y = fhMandibleNormal.Y;
                double z = fhMandibleNormal.Z;
                
                double fda = Math.Acos(Math.Abs(x) / Math.Sqrt(x * x + z * z)) / Math.PI * 180.0;

                double hda = Math.Acos(Math.Abs(x) / Math.Sqrt(x * x + y * y)) / Math.PI * 180.0;

                Point3D me = mandibleOsp.Transform.Transform(_craniofacialInfo.Me);
                Point3D headNormalPoint = headOsp.OspPlanePoint;//法向量平面上的點

                double dd = Vector3D.DotProduct(headNormal, me - headNormalPoint);


                Point3D intersection = mandibleOsp.Transform.Transform(_craniofacialInfo.GoIntersection);
                double pdd = Vector3D.DotProduct(headNormal, intersection - headNormalPoint);

                

                string info = "DA:    "+ Math.Round(da, 4)
                        + "\n\nDD:    "+ Math.Round(dd, 3)
                        + "\n\nFDA:  "+ Math.Round(fda, 2)
                        + "\n\nHDA: "+ Math.Round(hda, 2)
                        + "\n\nPDD:  "+ Math.Round(pdd, 3);

                MainViewModel.ProjData.DA = Convert.ToSingle(da);
                MainViewModel.ProjData.FDA = Convert.ToSingle(fda);
                MainViewModel.ProjData.HDA = Convert.ToSingle(hda);
                MainViewModel.ProjData.DD = Convert.ToSingle(dd);
                MainViewModel.ProjData.PDD = Convert.ToSingle(pdd);




                MultiAngleViewModel.CraniofacialInfo = info;


                ShowPeriod = 0;
            }
            ShowPeriod++;
        }
        /// <summary>
        /// 計算紅綠藍三球、以及其他手點球的距離
        /// </summary>
        public void CalcBallDistance()
        {
            //累計次數到五次才進來
            if (ShowPeriod2 == 5)
            {
                //如果沒有三角形導引物則返回
                if (MultiAngleViewModel.TriangleModelCollection == null || MultiAngleViewModel.TriangleModelCollection.Count == 0)
                    return;
                if (SystemData.IsFinished == true)
                    return;

                DraggableTriangle targetTriangle;//導引目標
                DraggableTriangle movedTriangle;//移動物件
                //在第二階段
                if (SystemData.IsSecondStage)
                {                    
                    //第二階段但第一階段導的是上顎
                    if (MainViewModel.ProjData.FirstNavigation.Equals("Maxilla"))
                    {
                        targetTriangle = MultiAngleViewModel.TriangleModelCollection[1] as DraggableTriangle;
                        movedTriangle = MultiAngleViewModel.TriangleModelCollection[3] as DraggableTriangle;                        
                    }
                    else
                    {
                        targetTriangle = MultiAngleViewModel.TriangleModelCollection[0] as DraggableTriangle;
                        movedTriangle = MultiAngleViewModel.TriangleModelCollection[2] as DraggableTriangle;
                    }
                }
                //在第一階段
                else if (SystemData.IsFirstStage)
                {
                    if (MainViewModel.ProjData.FirstNavigation.Equals("Maxilla"))
                    {
                        targetTriangle = MultiAngleViewModel.TriangleModelCollection[0] as DraggableTriangle;
                        movedTriangle = MultiAngleViewModel.TriangleModelCollection[2] as DraggableTriangle;
                    }
                    else
                    {
                        targetTriangle = MultiAngleViewModel.TriangleModelCollection[1] as DraggableTriangle;
                        movedTriangle = MultiAngleViewModel.TriangleModelCollection[3] as DraggableTriangle;
                    }
                }
                else
                {
                    ShowPeriod2 = 0;
                    return;
                }

                if (targetTriangle == null || movedTriangle == null)
                    return;

                Vector3 red;
                Vector3 green;
                Vector3 blue;
                Matrix mat = Matrix3DExtensions.ToMatrix(movedTriangle.Transform.Value);
                //計算三顆球乘上轉移矩陣過後
                Vector3.TransformCoordinate(ref movedTriangle.Positions[0], ref mat, out red);
                Vector3.TransformCoordinate(ref movedTriangle.Positions[1], ref mat, out green);
                Vector3.TransformCoordinate(ref movedTriangle.Positions[2], ref mat, out blue);



                Vector3 red2;
                Vector3 green2;
                Vector3 blue2;
                Matrix mat2 = Matrix3DExtensions.ToMatrix(targetTriangle.Transform.Value);
                Vector3.TransformCoordinate(ref targetTriangle.Positions[0], ref mat2, out red2);
                Vector3.TransformCoordinate(ref targetTriangle.Positions[1], ref mat2, out green2);
                Vector3.TransformCoordinate(ref targetTriangle.Positions[2], ref mat2, out blue2);


                Vector3 redVector;
                Vector3 greenVector;
                Vector3 blueVector;
                Vector3.Subtract(ref red2, ref red, out redVector);
                Vector3.Subtract(ref green2, ref green, out greenVector);
                Vector3.Subtract(ref blue2, ref blue, out blueVector);


                float redLength = redVector.Length();
                float greenLength = greenVector.Length();
                float blueLength = blueVector.Length();

                if (SystemData.IsFirstStage)
                {
                    MainViewModel.ProjData.Stage1Red = redLength;
                    MainViewModel.ProjData.Stage1Green = greenLength;
                    MainViewModel.ProjData.Stage1Blue = blueLength;
                }

                if (SystemData.IsSecondStage)
                {
                    MainViewModel.ProjData.Stage2Red = redLength;
                    MainViewModel.ProjData.Stage2Green = greenLength;
                    MainViewModel.ProjData.Stage2Blue = blueLength;
                }

                string navInfo = "Red:      " + Math.Round(redLength, 2)
                    + "\n\n" + "Green:  " + Math.Round(greenLength, 2)
                    + "\n\n" + "Blue:     " + Math.Round(blueLength, 2);

                MultiAngleViewModel.NavBallDistance = navInfo;



                //string ballDistanceInfo = "              dx              dy              dz\n";
                string ballDistanceInfo ="".PadLeft(15)+ "dx".PadLeft(10) + "dy".PadLeft(10) + "dz".PadLeft(10);

                //以下這段計算導航小球的距離 
                ObservableCollection <BallModel> ballCollection = MainViewModel.ProjData.BallCollection;
                ProjectData data = MainViewModel.ProjData;

                //當選擇的是先導航上顎且在第一個階段  或  選擇先導航下顎但已經在第二階段
                if ((data.FirstNavigation.Equals("Maxilla")&& SystemData.IsFirstStage) ||
                    (data.FirstNavigation.Equals("Mandible") && SystemData.IsSecondStage))
                {
                    foreach (BallModel model in ballCollection)
                    {
                        if (model.ModelType == ModelType.MovedMaxilla)
                        {
                            model.IsRendering = true;

                            Vector3 outputPoint;
                            Vector3 outputDistance;
                            Matrix mat3 = Matrix3DExtensions.ToMatrix(movedTriangle.Transform.Value);

                            Vector3 ballCenter = model.BallCenter;
                            Vector3.TransformCoordinate(ref ballCenter, ref mat3, out outputPoint);
                            Vector3.Subtract(ref ballCenter, ref outputPoint, out outputDistance);
                            model.BallDistance = outputDistance;


                            ballDistanceInfo += "\n"+model.BallName.PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.X, 2))).PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.Y, 2))).PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.Z, 2))).PadLeft(10);
                            
                        }
                        else
                        {
                            model.IsRendering = false;
                        }
                    }
                }
                else if ((data.FirstNavigation.Equals("Mandible") && SystemData.IsFirstStage) ||
                         (data.FirstNavigation.Equals("Maxilla") && SystemData.IsSecondStage))
                {
                    foreach (BallModel model in ballCollection)
                    {
                        if (model.ModelType == ModelType.MovedMandible)
                        {
                            model.IsRendering = true;

                            Vector3 outputPoint;
                            Vector3 outputDistance;
                            Matrix mat3 = Matrix3DExtensions.ToMatrix(movedTriangle.Transform.Value);

                            Vector3 ballCenter = model.BallCenter;
                            Vector3.TransformCoordinate(ref ballCenter, ref mat3, out outputPoint);
                            Vector3.Subtract(ref ballCenter, ref outputPoint, out outputDistance);
                            model.BallDistance = outputDistance;

                            ballDistanceInfo += "\n" + model.BallName.PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.X, 2))).PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.Y, 2))).PadLeft(10) +
                                                ("" + Math.Abs(Math.Round(outputDistance.Z, 2))).PadLeft(10);
                        }
                        else
                        {
                            model.IsRendering = false;
                        }
                    }
                }

               
                MultiAngleViewModel.BallDistance = ballDistanceInfo;
                

                ShowPeriod2 = 0;
                }
                
            
            

            ShowPeriod2++;
           
        }

    }
}
