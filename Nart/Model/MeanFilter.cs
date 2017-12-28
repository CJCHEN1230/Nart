using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    public class MeanFilter
    {
        public class PointStack
        {
            
            public Point3D[] OutputPoint = new Point3D[3];
            public string MarkerID;
            private Point3D[,] _pointsStack = new Point3D[5, 3];
            /// <summary>
            /// 
            /// </summary>
            private Point3D[] _totalValue = new Point3D[3];
            /// <summary>
            /// 
            /// </summary>
            private int _count = 0;
            /// <summary>
            /// 
            /// </summary>
            private int _currentIndex = 0;

            public Point3D[] AddItem(Point3D[] item)
            {
               

                //數量少於陣列總長度則往後加入
                if (_count < _pointsStack.GetLength(0))
                {

                    _count++;

                    ExtensionMethods.Point3DExtensions.AddPoint3D(ref _totalValue, ref item);

                    ExtensionMethods.Point3DExtensions.DividePoint3D(ref _totalValue, _count, ref OutputPoint);

                }
                else
                {
                    Point3D[] temp = new Point3D[3]
                        {_pointsStack[_currentIndex, 0], _pointsStack[_currentIndex, 1], _pointsStack[_currentIndex, 2]};
                    ExtensionMethods.Point3DExtensions.SubtractPoint3D(ref _totalValue, ref temp);

                    ExtensionMethods.Point3DExtensions.AddPoint3D(ref _totalValue, ref item);

                    ExtensionMethods.Point3DExtensions.DividePoint3D(ref _totalValue, _count, ref OutputPoint);
                }

                _pointsStack[_currentIndex, 0] = item[0];
                _pointsStack[_currentIndex, 1] = item[1];
                _pointsStack[_currentIndex, 2] = item[2];

                _currentIndex++;
                _currentIndex = _currentIndex % _pointsStack.GetLength(0);



                
                return OutputPoint;
            }
        }

        private List<PointStack> _allPointStack;        
       
        public MeanFilter(MarkerDatabase database)
        {
            _allPointStack = new List<PointStack>(database.MarkerInfo.Count);
            foreach (MarkerDatabase.MarkerData markerData in database.MarkerInfo)
            {
                PointStack pointStack = new PointStack {MarkerID = markerData.MarkerID};
                _allPointStack.Add(pointStack);
            }
        }

        public void CreatePointStack(MarkerDatabase database)
        {
            _allPointStack = new List<PointStack>(database.MarkerInfo.Count);
            foreach (MarkerDatabase.MarkerData markerData in database.MarkerInfo)
            {
                PointStack pointStack = new PointStack { MarkerID = markerData.MarkerID };
                _allPointStack.Add(pointStack);
            }
        }

        public void filter(ref List<Marker3D> worldPoints)
        {
            foreach (Marker3D marker in worldPoints) 
            {
                foreach (PointStack pointStack in _allPointStack)
                {
                    if (marker.MarkerId == pointStack.MarkerID)
                    {
                        Console.WriteLine("\n" + marker.MarkerId);
                        Console.WriteLine("input[0]:" + marker.ThreePoints[0] + "input[1]:" + marker.ThreePoints[1] + "input[2]:" + marker.ThreePoints[2]);
                        marker.ThreePoints = pointStack.AddItem(marker.ThreePoints);
                        Console.WriteLine("Output[0]:" + marker.ThreePoints[0] + "Output[1]:" + marker.ThreePoints[1] + "Output[2]:" + marker.ThreePoints[2]);

                    }
                }
            }


        }

    }
}
