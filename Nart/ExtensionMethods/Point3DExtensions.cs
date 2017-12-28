using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart.ExtensionMethods
{
    public static class Point3DExtensions
    {
        public static void AddPoint3D(ref Point3D[] a, ref Point3D[] b)
        {
            AddPoint3D(ref a[0], ref b[0]);
            AddPoint3D(ref a[1], ref b[1]);
            AddPoint3D(ref a[2], ref b[2]);
        }
        public static void AddPoint3D(ref Point3D a, ref Point3D b)
        {
            a.X = a.X + b.X;
            a.Y = a.Y + b.Y;
            a.Z = a.Z + b.Z;            
        }

        public static void SubtractPoint3D(ref Point3D[] a, ref Point3D[] b)
        {
            SubtractPoint3D(ref a[0], ref b[0]);
            SubtractPoint3D(ref a[1], ref b[1]);
            SubtractPoint3D(ref a[2], ref b[2]);
        }
        public static void SubtractPoint3D(ref Point3D a, ref Point3D b)
        {
            a.X = a.X - b.X;
            a.Y = a.Y - b.Y;
            a.Z = a.Z - b.Z;
        }

        public static void DividePoint3D(ref Point3D[] a, int divisor, ref Point3D[] result)
        {
            //double divisorDouble = Convert.ToDouble(divisor);

            DividePoint3D(ref a[0], divisor, ref result[0]);
            DividePoint3D(ref a[1], divisor, ref result[1]);
            DividePoint3D(ref a[2], divisor, ref result[2]);
        }
        public static void DividePoint3D(ref Point3D a, int divisor, ref Point3D result)
        {
            double divisorDouble = Convert.ToDouble(divisor);

            result.X = a.X / divisorDouble;
            result.Y = a.Y / divisorDouble;
            result.Z = a.Z / divisorDouble;             
        }

    }
}
