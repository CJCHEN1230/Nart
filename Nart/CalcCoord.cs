using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public CalcCoord()
        {
            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
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

            PointsNumber = WorldPoints.Count * 3;

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

        public Point3D  CalcWorldPoint(Point4D a,Point4D b)
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
    }

    
}
