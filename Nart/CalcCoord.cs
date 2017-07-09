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

        private const double MatchError= 3;

        private List<List<Point3D>> WorldPoints = new List<List<Point3D>>(10);

        public static int PointsNumber = 0;

        private MainWindow _window = null;

        public CalcCoord(MainWindow window)
        {
            _window = window;
            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
            CalcLensCenter();
            CalcEpipolarGeometry();
            LoadNartReg("../../../data/reg.txt");

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
            Vector3D vec_x = new Vector3D(LensCenter[0].X - LensCenter[1].X, LensCenter[0].Y - LensCenter[1].Y, LensCenter[0].Z - LensCenter[1].Z);

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

        public void Rectificaion(List<BWMarker>[] OutputMarker)
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

                        
                        OutputMarker[i][j].CornerPoint[k].CameraPoint = new Point4D(xu, yu, _camParam[i].FocalLength, 1);

                        Point4D WorldPoint = _camParam[i].RotationInvert.Transform(OutputMarker[i][j].CornerPoint[k].CameraPoint);

                        Point4D rectify_Point = epipolarCoord.Transform(WorldPoint);

                        double Rectify_Y = _camParam[i].FocalLength * rectify_Point.Y / rectify_Point.Z / _camParam[i].dy;

                        OutputMarker[i][j].CornerPoint[k].RectifyY = Rectify_Y;
                        
                        accumX += imagePoint.X;
                        accumRecY += Rectify_Y;
                    }
                    OutputMarker[i][j].AvgRectifyY = accumRecY / 3.0;
                    OutputMarker[i][j].AvgX = accumX / 3.0;

                    OutputMarker[i][j].CornerPoint.Sort(delegate (NartPoint point1, NartPoint point2)
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

        public void MatchAndCalc3D(List<BWMarker>[] OutputMarker)
        {
            WorldPoints.Clear();
            for (int i = 0, count = 0; i < OutputMarker[0].Count; i++) //左相機Marker尋訪
            {
                for (int j = count; j < OutputMarker[1].Count; j++) //右相機Marker尋訪
                {
                    
                    if (Math.Abs(OutputMarker[0][i].AvgRectifyY - OutputMarker[1][j].AvgRectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[0].RectifyY - OutputMarker[1][j].CornerPoint[0].RectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[1].RectifyY - OutputMarker[1][j].CornerPoint[1].RectifyY) < MatchError
                       && Math.Abs(OutputMarker[0][i].CornerPoint[2].RectifyY - OutputMarker[1][j].CornerPoint[2].RectifyY) < MatchError)
                    {
                        List<Point3D> threePoints = new List<Point3D>(3);
                        for (int k = 0; k < OutputMarker[0][i].CornerPoint.Count; k++)
                        {                           
                            var point3D = CalcWorldPoint(OutputMarker[0][i].CornerPoint[k].CameraPoint, OutputMarker[1][j].CornerPoint[k].CameraPoint);
                            threePoints.Add(point3D);                           
                        }
                        WorldPoints.Add(threePoints);
                        count = j + 1;
                        break;                        
                    }
                    else
                        continue;
                }
            }

            SortedByLength(WorldPoints);

            PointsNumber = WorldPoints.Count * 3;
            _window.PointLabel.Content = PointsNumber.ToString() + "個點";

            for (int i = 0; i < WorldPoints.Count; i++) //左相機Marker尋訪
            {
                Console.WriteLine("\n\n第"+(i+1)+"組三邊長度");

                Vector3D a = new Vector3D(WorldPoints[i][0].X - WorldPoints[i][1].X, WorldPoints[i][0].Y - WorldPoints[i][1].Y, WorldPoints[i][0].Z - WorldPoints[i][1].Z);

                Vector3D b = new Vector3D(WorldPoints[i][2].X - WorldPoints[i][1].X, WorldPoints[i][2].Y - WorldPoints[i][1].Y, WorldPoints[i][2].Z - WorldPoints[i][1].Z);

                Vector3D c = new Vector3D(WorldPoints[i][0].X - WorldPoints[i][2].X, WorldPoints[i][0].Y - WorldPoints[i][2].Y, WorldPoints[i][0].Z - WorldPoints[i][2].Z);

                Console.WriteLine("\na:" + a.Length + "\nb:" + b.Length + "\nc:" + c.Length);

                Console.WriteLine("    ");
            }

        }

        /// <summary>
        /// 排序點大小
        /// </summary>
        private void SortedByLength(List<List<Point3D>> WorldPoints)
        {
            //第幾組點
            for (int i = 0; i < WorldPoints.Count; i++) 
            {
                double[] Length = new double[3];
                Length[0] = Math.Abs(WorldPoints[i][0].X - WorldPoints[i][1].X) + Math.Abs(WorldPoints[i][0].Y - WorldPoints[i][1].Y) + Math.Abs(WorldPoints[i][0].Z - WorldPoints[i][1].Z);

                Length[1] = Math.Abs(WorldPoints[i][1].X - WorldPoints[i][2].X) + Math.Abs(WorldPoints[i][1].Y - WorldPoints[i][2].Y) + Math.Abs(WorldPoints[i][1].Z - WorldPoints[i][2].Z);

                Length[2] = Math.Abs(WorldPoints[i][2].X - WorldPoints[i][0].X) + Math.Abs(WorldPoints[i][2].Y - WorldPoints[i][0].Y) + Math.Abs(WorldPoints[i][2].Z - WorldPoints[i][0].Z);

                //Vector3D VecA = new Vector3D(WorldPoints[i][0].X - WorldPoints[i][1].X, WorldPoints[i][0].Y - WorldPoints[i][1].Y, WorldPoints[i][2].Z - WorldPoints[i][2].Z);
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

                Point3D tempA = new Point3D(WorldPoints[i][Index[0]].X, WorldPoints[i][Index[0]].Y, WorldPoints[i][Index[0]].Z);
                Point3D tempB = new Point3D(WorldPoints[i][Index[1]].X, WorldPoints[i][Index[1]].Y, WorldPoints[i][Index[1]].Z);
                Point3D tempC = new Point3D(WorldPoints[i][Index[2]].X, WorldPoints[i][Index[2]].Y, WorldPoints[i][Index[2]].Z);

                WorldPoints[i][0] = tempA;
                WorldPoints[i][1] = tempB;
                WorldPoints[i][2] = tempC;
            }
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
            Point3D AvgA = new Point3D((A[0].X + A[1].X + A[2].X) / 3.0, (A[0].Y + A[1].Y + A[2].Y) / 3.0, (A[0].Z + A[1].Z + A[2].Z) / 3.0);
            
            Vector3D u1 = AvgA - A[0];

            Vector3D AC = AvgA - A[2];

            Vector3D v1 = Vector3D.CrossProduct(u1, AC);

            Vector3D w1 = Vector3D.CrossProduct(u1, v1);
            u1.Normalize();
            v1.Normalize();
            w1.Normalize();


            Point3D AvgB = new Point3D((B[0].X + B[1].X + B[2].X) / 3.0, (B[0].Y + B[1].Y + B[2].Y) / 3.0, (B[0].Z + B[1].Z + B[2].Z) / 3.0);

            Vector3D u2 = AvgB - B[0];

            Vector3D AC2 = AvgB - B[2];

            Vector3D v2 = Vector3D.CrossProduct(u2, AC2);

            Vector3D w2 = Vector3D.CrossProduct(u2, v2);
            u2.Normalize();
            v2.Normalize();
            w2.Normalize();

            Matrix3D transform1 =new Matrix3D(u1.X, v1.X, w1.X, 0,
                                              u1.Y, v1.Y, w1.Y, 0,
                                              u1.Z, v1.Z, w1.Z, 0,
                                             AvgA.X, AvgA.Y, AvgA.Z, 1);

            transform1.Invert();


            Matrix3D transform2 = new Matrix3D(u2.X, v2.X, w2.X, 0,
                                              u2.Y, v2.Y, w2.Y, 0,
                                              u2.Z, v2.Z, w2.Z, 0,
                                             AvgB.X, AvgB.Y, AvgB.Z, 1);

            Matrix3D finalTransform = transform1 * transform2;

            return finalTransform;
        }


        private void LoadNartReg(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);//"../../../data/CaliR_L.txt"
                string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                string[] data = new string[27];
                Array.Copy(contentArray, 0, data, 0, 27);

                double[] regData = Array.ConvertAll(data, double.Parse);

                Point3D[] CTball = new Point3D[3];
                CTball[0] = new Point3D(regData[0], regData[1], regData[2]);
                CTball[1] = new Point3D(regData[3], regData[4], regData[5]);
                CTball[2] = new Point3D(regData[6], regData[7], regData[8]);

                Point3D[] SplintBall = new Point3D[3];
                SplintBall[0] = new Point3D(regData[9], regData[10], regData[11]);
                SplintBall[1] = new Point3D(regData[12], regData[13], regData[14]);
                SplintBall[2] = new Point3D(regData[15], regData[16], regData[17]);

                Point3D[] SplintMarker = new Point3D[3];
                SplintMarker[0] = new Point3D(regData[18], regData[19], regData[20]);
                SplintMarker[1] = new Point3D(regData[21], regData[22], regData[23]);
                SplintMarker[2] = new Point3D(regData[24], regData[25], regData[26]);
                for (int i =0;i< CTball.Length ;i++)
                {
                    Console.WriteLine(CTball[i]);
                }

                for (int i = 0; i < SplintBall.Length; i++)
                {
                    Console.WriteLine(SplintBall[i]);
                }

                for (int i = 0; i < SplintMarker.Length; i++)
                {
                    Console.WriteLine(SplintMarker[i]);
                }
                Console.WriteLine("  ");
            }
            catch (Exception ex)
            {

                MessageBox.Show("註冊檔案錯誤");
            }
        }



    }

    
}
