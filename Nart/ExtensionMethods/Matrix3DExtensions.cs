using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart.ExtensionMethods
{
    public static class Matrix3DExtensions
    {
        public static void AddMatrix3D(ref Matrix3D a, ref Matrix3D b)
        {
            a.M11 = a.M11 + b.M11; a.M12 = a.M12 + b.M12; a.M13 = a.M13 + b.M13; a.M14 = a.M14 + b.M14;
            a.M21 = a.M21 + b.M21; a.M22 = a.M22 + b.M22; a.M23 = a.M23 + b.M23; a.M24 = a.M24 + b.M24;
            a.M31 = a.M31 + b.M31; a.M32 = a.M32 + b.M32; a.M33 = a.M33 + b.M33; a.M34 = a.M34 + b.M34;
            a.OffsetX = a.OffsetX + b.OffsetX; a.OffsetY = a.OffsetY + b.OffsetY; a.OffsetZ = a.OffsetZ + b.OffsetZ; a.M44 = a.M44 + b.M44;
        }
        public static void SubtractMatrix3D(ref Matrix3D a, ref Matrix3D b)
        {
            a.M11 = a.M11 - b.M11; a.M12 = a.M12 - b.M12; a.M13 = a.M13 - b.M13; a.M14 = a.M14 - b.M14;
            a.M21 = a.M21 - b.M21; a.M22 = a.M22 - b.M22; a.M23 = a.M23 - b.M23; a.M24 = a.M24 - b.M24;
            a.M31 = a.M31 - b.M31; a.M32 = a.M32 - b.M32; a.M33 = a.M33 - b.M33; a.M34 = a.M34 - b.M34;
            a.OffsetX = a.OffsetX - b.OffsetX; a.OffsetY = a.OffsetY - b.OffsetY; a.OffsetZ = a.OffsetZ - b.OffsetZ; a.M44 = a.M44 - b.M44;
        }
        public static void DivideMatrix3D(ref Matrix3D a, int divisor, ref Matrix3D result)
        {
            double divisorDouble = Convert.ToDouble(divisor);

            result.M11 = a.M11 / divisorDouble; result.M12 = a.M12 / divisorDouble; result.M13 = a.M13 / divisorDouble; result.M14 = a.M14 / divisorDouble;
            result.M21 = a.M21 / divisorDouble; result.M22 = a.M22 / divisorDouble; result.M23 = a.M23 / divisorDouble; result.M24 = a.M24 / divisorDouble;
            result.M31 = a.M31 / divisorDouble; result.M32 = a.M32 / divisorDouble; result.M33 = a.M33 / divisorDouble; result.M34 = a.M34 / divisorDouble;
            result.OffsetX = a.OffsetX / divisorDouble; result.OffsetY = a.OffsetY / divisorDouble; result.OffsetZ = a.OffsetZ / divisorDouble; result.M44 = a.M44 / divisorDouble;

        }
        public static Matrix3D Translate3D(global::SharpDX.Vector3 v)
        {
            var m = Matrix3D.Identity;
            m.OffsetX = v.X;
            m.OffsetY = v.Y;
            m.OffsetZ = v.Z;
            return m;
        }

        public static Matrix ToMatrix(this Matrix3D m)
        {
            return new Matrix(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.OffsetX,
                (float)m.OffsetY,
                (float)m.OffsetZ,
                (float)m.M44);
        }


        public static Matrix3D ToMatrix3D(this Matrix m)
        {
            return new Matrix3D(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.M41,
                (float)m.M42,
                (float)m.M43,
                (float)m.M44);
        }
    }
}
