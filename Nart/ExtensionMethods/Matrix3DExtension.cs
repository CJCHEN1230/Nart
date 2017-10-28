﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart.ExtensionMethods
{
    public static class Matrix3DExtension
    {
        public static void AddMatrix3D(ref Matrix3D A, ref Matrix3D B)
        {
            A.M11 = A.M11 + B.M11; A.M12 = A.M12 + B.M12; A.M13 = A.M13 + B.M13; A.M14 = A.M14 + B.M14;
            A.M21 = A.M21 + B.M21; A.M22 = A.M22 + B.M22; A.M23 = A.M23 + B.M23; A.M24 = A.M24 + B.M24;
            A.M31 = A.M31 + B.M31; A.M32 = A.M32 + B.M32; A.M33 = A.M33 + B.M33; A.M34 = A.M34 + B.M34;
            A.OffsetX = A.OffsetX + B.OffsetX; A.OffsetY = A.OffsetY + B.OffsetY; A.OffsetZ = A.OffsetZ + B.OffsetZ; A.M44 = A.M44 + B.M44;
        }
        public static void SubtractMatrix3D(ref Matrix3D A, ref Matrix3D B)
        {
            A.M11 = A.M11 - B.M11; A.M12 = A.M12 - B.M12; A.M13 = A.M13 - B.M13; A.M14 = A.M14 - B.M14;
            A.M21 = A.M21 - B.M21; A.M22 = A.M22 - B.M22; A.M23 = A.M23 - B.M23; A.M24 = A.M24 - B.M24;
            A.M31 = A.M31 - B.M31; A.M32 = A.M32 - B.M32; A.M33 = A.M33 - B.M33; A.M34 = A.M34 - B.M34;
            A.OffsetX = A.OffsetX - B.OffsetX; A.OffsetY = A.OffsetY - B.OffsetY; A.OffsetZ = A.OffsetZ - B.OffsetZ; A.M44 = A.M44 - B.M44;
        }

        public static void DivideMatrix3D(ref Matrix3D A, int divisor, ref Matrix3D result)
        {
            double divisorDouble = Convert.ToDouble(divisor);

            result.M11 = A.M11 / divisorDouble; result.M12 = A.M12 / divisorDouble; result.M13 = A.M13 / divisorDouble; result.M14 = A.M14 / divisorDouble;
            result.M21 = A.M21 / divisorDouble; result.M22 = A.M22 / divisorDouble; result.M23 = A.M23 / divisorDouble; result.M24 = A.M24 / divisorDouble;
            result.M31 = A.M31 / divisorDouble; result.M32 = A.M32 / divisorDouble; result.M33 = A.M33 / divisorDouble; result.M34 = A.M34 / divisorDouble;
            result.OffsetX = A.OffsetX / divisorDouble; result.OffsetY = A.OffsetY / divisorDouble; result.OffsetZ = A.OffsetZ / divisorDouble; result.M44 = A.M44 / divisorDouble;

        }
    }
}