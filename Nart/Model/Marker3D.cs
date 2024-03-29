﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static Nart.MarkerDatabase;

namespace Nart
{
    //
    // 摘要:紀錄辨識好的世界座標系資料
    //     
    public class Marker3D
    {
        /// <summary>
        /// 儲存該Marker的三個世界座標點
        /// </summary>
        public Point3D[] ThreePoints = new Point3D[3];
        /// <summary>
        /// 儲存該Marker的三個長度
        /// </summary>
        public double[] ThreeLength = new double[3];
        /// <summary>
        /// 記錄在資料庫中對應的MarkerID
        /// </summary>
        public string MarkerId = "";
        private Point3D[,] _pointsStack = new Point3D[10, 3];
        private Point3D[] _totalValue = new Point3D[3];

        private int _count = 0;
        private int _currentIndex = 0;
        public void AddItem(Point3D[] item)
        {
            //數量少於陣列總長度則往後加入
            if (_count < _pointsStack.GetLength(1))
            {

                _count++;

                ExtensionMethods.Point3DExtensions.AddPoint3D(ref _totalValue, ref item);

                ExtensionMethods.Point3DExtensions.DividePoint3D(ref _totalValue, _count, ref ThreePoints);

            }
            else
            {
                Point3D[] temp = new Point3D[3]
                    {_pointsStack[_currentIndex, 0], _pointsStack[_currentIndex, 1], _pointsStack[_currentIndex, 2]};
                ExtensionMethods.Point3DExtensions.SubtractPoint3D(ref _totalValue, ref temp);

                ExtensionMethods.Point3DExtensions.AddPoint3D(ref _totalValue, ref item);

                ExtensionMethods.Point3DExtensions.DividePoint3D(ref _totalValue, _count, ref ThreePoints);
            }

            _pointsStack[_currentIndex, 0] = item[0];
            _pointsStack[_currentIndex, 1] = item[1];            
            _pointsStack[_currentIndex, 2] = item[2];

            _currentIndex++;
            _currentIndex = _currentIndex % _pointsStack.GetLength(1);

        }
        /// <summary>
        /// 依照邊長排序點
        /// </summary>
        public void SortedByLength()
        {
            //第幾組點

            double[] length = new double[3];
            Vector3D a = new Vector3D(ThreePoints[0].X - ThreePoints[1].X, ThreePoints[0].Y - ThreePoints[1].Y, ThreePoints[0].Z - ThreePoints[1].Z);

            Vector3D b = new Vector3D(ThreePoints[1].X - ThreePoints[2].X, ThreePoints[1].Y - ThreePoints[2].Y, ThreePoints[1].Z - ThreePoints[2].Z);

            Vector3D c = new Vector3D(ThreePoints[2].X - ThreePoints[0].X, ThreePoints[2].Y - ThreePoints[0].Y, ThreePoints[2].Z - ThreePoints[0].Z);


            length[0] = a.Length;

            length[1] = b.Length;

            length[2] = c.Length;


            int[] index = new int[3];

            if (length[0] > length[1] && length[1] > length[2])
            {
                index[0] = 0;
                index[1] = 1;
                index[2] = 2;
                ThreeLength[0] = length[0];
                ThreeLength[1] = length[1];
                ThreeLength[2] = length[2];
            }

            else if (length[0] > length[2] && length[2] > length[1])
            {
                index[0] = 1;
                index[1] = 0;
                index[2] = 2;
                ThreeLength[0] = length[0];
                ThreeLength[1] = length[2];
                ThreeLength[2] = length[1];
            }

            else if (length[1] > length[0] && length[0] > length[2])
            {
                index[0] = 2;
                index[1] = 1;
                index[2] = 0;
                ThreeLength[0] = length[1];
                ThreeLength[1] = length[0];
                ThreeLength[2] = length[2];
            }

            else if (length[1] > length[2] && length[2] > length[0])
            {
                index[0] = 1;
                index[1] = 2;
                index[2] = 0;
                ThreeLength[0] = length[1];
                ThreeLength[1] = length[2];
                ThreeLength[2] = length[0];
            }

            else if (length[2] > length[1] && length[1] > length[0])
            {
                index[0] = 0;
                index[1] = 2;
                index[2] = 1;
                ThreeLength[0] = length[2];
                ThreeLength[1] = length[1];
                ThreeLength[2] = length[0];
            }

            else if (length[2] > length[0] && length[0] > length[1])
            {
                index[0] = 2;
                index[1] = 0;
                index[2] = 1;
                ThreeLength[0] = length[2];
                ThreeLength[1] = length[0];
                ThreeLength[2] = length[1];
            }

            Point3D tempA = new Point3D(ThreePoints[index[0]].X, ThreePoints[index[0]].Y, ThreePoints[index[0]].Z);
            Point3D tempB = new Point3D(ThreePoints[index[1]].X, ThreePoints[index[1]].Y, ThreePoints[index[1]].Z);
            Point3D tempC = new Point3D(ThreePoints[index[2]].X, ThreePoints[index[2]].Y, ThreePoints[index[2]].Z);

            ThreePoints[0] = tempA;
            ThreePoints[1] = tempB;
            ThreePoints[2] = tempC;

        }
        /// <summary>
        /// 輸入資料庫，並存下對應的索引值到DatabaseIndex
        /// </summary>
        public void CompareDatabase(ref List<MarkerData> markerDb)
        {
           
            for (int i = 0; i < markerDb.Count; i++)
            {
                
                double diff1 = ThreeLength[0] - markerDb[i].ThreeLength[0];
                double diff2 = ThreeLength[1] - markerDb[i].ThreeLength[1];
                double diff3 = ThreeLength[2] - markerDb[i].ThreeLength[2];

                
                if (Math.Abs(diff1) < 1 && Math.Abs(diff2) < 1 && Math.Abs(diff3) < 1) 
                {                   
                    this.MarkerId = markerDb[i].MarkerID;
                    return;
                }

            }
                     
        }

    }
}
