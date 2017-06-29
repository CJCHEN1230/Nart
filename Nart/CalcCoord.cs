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

        
        public CalcCoord()
        {
            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
            CalcLensCenter();
            CalcEpipolarGeometry();
            //Rectificaion();
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

            _camParam[0].extParam.Invert();
            _camParam[1].extParam.Invert();

            //Console.WriteLine("Rotation2:\n" +
            //   _camParam[0].extParam.M11 + "\t" + _camParam[0].extParam.M12 + "\t" + _camParam[0].extParam.M13 + "\t" + _camParam[0].extParam.M14 + "\n" +
            //   _camParam[0].extParam.M21 + "\t" + _camParam[0].extParam.M22 + "\t" + _camParam[0].extParam.M23 + "\t" + _camParam[0].extParam.M24 + "\n" +
            //   _camParam[0].extParam.M31 + "\t" + _camParam[0].extParam.M32 + "\t" + _camParam[0].extParam.M33 + "\t" + _camParam[0].extParam.M34 + "\n" +
            //   _camParam[0].extParam.OffsetX + "\t" + _camParam[0].extParam.OffsetY + "\t" + _camParam[0].extParam.OffsetZ + "\t" + _camParam[0].extParam.M44 + "\n");


            LensCenter[0] = _camParam[0].extParam.Transform(LensCenter[0]);
            LensCenter[1] = _camParam[1].extParam.Transform(LensCenter[1]);

            
       
        }
        /// <summary>
        /// 計算基線座標系
        /// </summary>
        private void CalcEpipolarGeometry()
        {
            Point4D PlainCenter = new Point4D(0, 0, _camParam[0].FocalLength, 1);
            
            //PlainCenter[1] = new Point4D(0, 0, _camParam[1].FocalLength, 1);
            //LensCenter[1] = new Point4D(0, 0, 0, 1);

            PlainCenter = _camParam[0].extParam.Transform(PlainCenter);

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
                //Console.WriteLine("\n第" + (i + 1) + "張圖片");
                for (int j = 0; j < OutputMarker[i].Count; j++)
                {
                    double accumX = 0;
                    double accumRecY = 0;
                    
                    //Console.WriteLine("\n第" + (j + 1) + "個Marker");
                    for (int k = 0; k < 3; k++) //一個標記裡面三個點
                    {
                        //Console.WriteLine("\n(" + OutputCorPt[i][j][k].X + "\t" + OutputCorPt[i][j][k].Y + ")");

                        double xd = _camParam[i].dpx * (OutputMarker[i][j].CornerPoint[k].X - _camParam[i].Cx) / _camParam[i].Sx;
                        double yd = _camParam[i].dy * (/*1200 - */OutputMarker[i][j].CornerPoint[k].Y - _camParam[i].Cy);

                        double r = Math.Sqrt(xd * xd + yd * yd);
                      
                        double xu = xd * (1 + _camParam[i].Kappa1 * r * r);
                        double yu = yd * (1 + _camParam[i].Kappa1 * r * r);

                        Point4D PlainCenter = new Point4D(xu, yu, _camParam[i].FocalLength, 1);

                        //_camParam[i].Rotation.Invert();

                        PlainCenter = _camParam[i].RotationInvert.Transform(PlainCenter);

                        Point4D rectify_Point = epipolarCoord.Transform(PlainCenter);

                        double Rectify_Y = _camParam[i].FocalLength * rectify_Point.Y / rectify_Point.Z / _camParam[i].dy;

                      
                        OutputMarker[i][j].CornerPoint[k] = new Point3D(OutputMarker[i][j].CornerPoint[k].X, OutputMarker[i][j].CornerPoint[k].Y, Rectify_Y);                                                

                        accumX += OutputMarker[i][j].CornerPoint[k].X;
                        accumRecY += Rectify_Y;
                    }
                    OutputMarker[i][j].AvgRectifyY = accumRecY / 3.0;
                    OutputMarker[i][j].AvgX = accumX / 3.0;

                    OutputMarker[i][j].CornerPoint.Sort(delegate (Point3D point1, Point3D point2)
                    {
                        double diff = point1.Z - point2.Z;
                        if (diff > 2)
                            return 1;
                        else if (diff < -2)
                            return -1;
                        else
                        {
                            double diff2 = point1.X - point2.X;
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
            //            Console.WriteLine("\n :(" + OutputMarker[i][j].CornerPoint[k].X + "," + OutputMarker[i][j].CornerPoint[k].Y + "," + OutputMarker[i][j].CornerPoint[k].Z + ")");                        
            //        }
            //        Console.WriteLine("\nAvergeX :" + OutputMarker[i][j].AvgX);
            //        Console.WriteLine("\nRectifyY :" + OutputMarker[i][j].AvgRectifyY);
            //    }
            //}


            //Console.Read();

        }
    }

    
}
