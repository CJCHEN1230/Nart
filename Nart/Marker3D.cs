using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    //
    // 摘要:紀錄辨識好的世界座標系資料
    //     
    class Marker3D
    {
        /// <summary>
        /// 儲存該Marker的三個世界座標點
        /// </summary>
        public Point3D[] ThreePoints;
        /// <summary>
        /// 儲存該Marker的三個長度
        /// </summary>
        public double[] ThreeLength;

        /// <summary>
        /// 記錄在資料庫中對應的Marker索引
        /// </summary>
        public int DatabaseIndex = -1;


        
        public Marker3D()
        {
            ThreePoints = new Point3D[3];

            ThreeLength = new double[3];
        }

        /// <summary>
        /// 依照邊長排序點
        /// </summary>
        public void SortedByLength()
        {
            //第幾組點

            double[] Length = new double[3];
            Vector3D a = new Vector3D(ThreePoints[0].X - ThreePoints[1].X, ThreePoints[0].Y - ThreePoints[1].Y, ThreePoints[0].Z - ThreePoints[1].Z);

            Vector3D b = new Vector3D(ThreePoints[1].X - ThreePoints[2].X, ThreePoints[1].Y - ThreePoints[2].Y, ThreePoints[1].Z - ThreePoints[2].Z);

            Vector3D c = new Vector3D(ThreePoints[2].X - ThreePoints[0].X, ThreePoints[2].Y - ThreePoints[0].Y, ThreePoints[2].Z - ThreePoints[0].Z);


            Length[0] = a.Length;

            Length[1] = b.Length;

            Length[2] = c.Length;


            int[] Index = new int[3];

            if (Length[0] > Length[1] && Length[1] > Length[2])
            {
                Index[0] = 0;
                Index[1] = 1;
                Index[2] = 2;
                ThreeLength[0] = Length[0];
                ThreeLength[1] = Length[1];
                ThreeLength[2] = Length[2];
            }

            else if (Length[0] > Length[2] && Length[2] > Length[1])
            {
                Index[0] = 1;
                Index[1] = 0;
                Index[2] = 2;
                ThreeLength[0] = Length[0];
                ThreeLength[1] = Length[2];
                ThreeLength[2] = Length[1];
            }

            else if (Length[1] > Length[0] && Length[0] > Length[2])
            {
                Index[0] = 2;
                Index[1] = 1;
                Index[2] = 0;
                ThreeLength[0] = Length[1];
                ThreeLength[1] = Length[0];
                ThreeLength[2] = Length[2];
            }

            else if (Length[1] > Length[2] && Length[2] > Length[0])
            {
                Index[0] = 1;
                Index[1] = 2;
                Index[2] = 0;
                ThreeLength[0] = Length[1];
                ThreeLength[1] = Length[2];
                ThreeLength[2] = Length[0];
            }

            else if (Length[2] > Length[1] && Length[1] > Length[0])
            {
                Index[0] = 0;
                Index[1] = 2;
                Index[2] = 1;
                ThreeLength[0] = Length[2];
                ThreeLength[1] = Length[1];
                ThreeLength[2] = Length[0];
            }

            else if (Length[2] > Length[0] && Length[0] > Length[1])
            {
                Index[0] = 2;
                Index[1] = 0;
                Index[2] = 1;
                ThreeLength[0] = Length[2];
                ThreeLength[1] = Length[0];
                ThreeLength[2] = Length[1];
            }

            Point3D tempA = new Point3D(ThreePoints[Index[0]].X, ThreePoints[Index[0]].Y, ThreePoints[Index[0]].Z);
            Point3D tempB = new Point3D(ThreePoints[Index[1]].X, ThreePoints[Index[1]].Y, ThreePoints[Index[1]].Z);
            Point3D tempC = new Point3D(ThreePoints[Index[2]].X, ThreePoints[Index[2]].Y, ThreePoints[Index[2]].Z);

            ThreePoints[0] = tempA;
            ThreePoints[1] = tempB;
            ThreePoints[2] = tempC;

        }

        /// <summary>
        /// 輸入資料庫，並存下對應的索引值到DatabaseIndex
        /// </summary>
        public void CompareDatabase(List<double[]> MarkerDB)
        {
           
            for (int i = 0; i < MarkerDB.Count; i++)
            {
                double diff1 = ThreeLength[0] - MarkerDB[i][0];
                double diff2 = ThreeLength[1] - MarkerDB[i][1];
                double diff3 = ThreeLength[2] - MarkerDB[i][2];

                
                if (Math.Abs(diff1) < 1 && Math.Abs(diff2) < 1 && Math.Abs(diff3) < 1) 
                {
                   
                    DatabaseIndex = i;
                    return;
                }

            }
                     
        }

    }
}
