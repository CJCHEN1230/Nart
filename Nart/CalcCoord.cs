using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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

        public void Rectificaion(List<List<PointF>>[] OutputCorPt)
        {

            //OutputCorPt = gcnew List < List<PointF> ^> ();
            //eachEllipse = gcnew List<PointF>(3);

            //Parallel.For(0, 2, i =>
            //{
            //    icImagingControl[i].DisplayImageBuffer(_displayBuffer[i]);
            //});

            //for (int i=0; i< OutputCorPt.Length; i++)
            
            List<PointF>[] RectifyY= new List<PointF>[2];
            

            RectifyY[0] = new List<PointF>(OutputCorPt[0].Count);
            RectifyY[1] = new List<PointF>(OutputCorPt[1].Count);
          
            
            Parallel.For(0, 2, i =>
            {
                Console.WriteLine("\n第" + (i + 1) + "張圖片");
                for (int j = 0; j < OutputCorPt[i].Count; j++)
                {
                    double accumX = 0;
                    double accumRecY = 0;
                    
                    Console.WriteLine("\n第" + (j + 1) + "個Marker");
                    for (int k = 0; k < 3; k++) //一個標記裡面三個點
                    {
                        //Console.WriteLine("\n(" + OutputCorPt[i][j][k].X + "\t" + OutputCorPt[i][j][k].Y + ")");

                        double xd = _camParam[i].dpx * (OutputCorPt[i][j][k].X - _camParam[i].Cx) / _camParam[i].Sx;
                        double yd = _camParam[i].dy * (/*1200 - */OutputCorPt[i][j][k].Y - _camParam[i].Cy);

                        double r = Math.Sqrt(xd * xd + yd * yd);
                      
                        double xu = xd * (1 + _camParam[i].Kappa1 * r * r);
                        double yu = yd * (1 + _camParam[i].Kappa1 * r * r);

                        Point4D PlainCenter = new Point4D(xu, yu, _camParam[i].FocalLength, 1);

                        //_camParam[i].Rotation.Invert();

                        PlainCenter = _camParam[i].RotationInvert.Transform(PlainCenter);

                        Point4D rectify_Point = epipolarCoord.Transform(PlainCenter);

                        double Rectify_Y = _camParam[i].FocalLength * rectify_Point.Y / rectify_Point.Z / _camParam[i].dy;

                        Console.WriteLine("\n(" + OutputCorPt[i][j][k].X + "\t" + Rectify_Y + ")");

                        accumX += OutputCorPt[i][j][k].X;
                        accumRecY += Rectify_Y;
                    }

                    PointF temp = new PointF(Convert.ToSingle(accumX / 3.0), Convert.ToSingle(accumRecY / 3.0));

                    RectifyY[i].Add(temp);
                
                   
                }
            });

            //for (int i=0;i<RectifyY.Length ;i++)
            //{
            //    Console.WriteLine("\n\n第" + (i + 1) + "張圖片");
            //    for (int j=0;j< RectifyY[i].Count; j++)
            //    {
            //        Console.WriteLine("\nX:" + RectifyY[i][j].X + "   Y:" + RectifyY[i][j].Y);
            //    }
            //}


            //Console.Read();
            
        }
    }

    
}
