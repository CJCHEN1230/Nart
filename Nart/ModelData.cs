﻿using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using NartControl;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    using Color = System.Windows.Media.Color;
    public class ModelData : ObservableObject 
    {
        /// <summary>
        /// 模型幾何形狀
        /// </summary>
        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry;
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D ModelGeometry
        {
            get
            {
                return modelGeometry;
            }
            set
            {
                SetValue(ref modelGeometry, value);
            }
        }
        /// <summary>
        /// 模型材質設定
        /// </summary>
        private PhongMaterial modelMaterial;
        public PhongMaterial ModelMaterial
        {
            get
            {
                return modelMaterial;
            }
            set
            {
                SetValue(ref modelMaterial, value);
            }
        }
        /// <summary>
        /// 模型矩陣設定
        /// </summary>     
        private MatrixTransform3D modelTransform;
        public MatrixTransform3D ModelTransform
        {
            get
            {
                return modelTransform;
            }
            set
            {
                SetValue(ref modelTransform, value);
            }
        }
        private CullMode cMode = CullMode.Back;
        public CullMode CMode
        {
            get
            {
                return cMode;
            }
            set
            {
                SetValue(ref cMode, value);
            }
        }
        /// <summary>
        /// 反轉Normal方向
        /// </summary>
        private bool ivtNormal = false;
        public bool IvtNormal
        {
            get
            {
                return ivtNormal;
            }
            set
            {
                SetValue(ref ivtNormal, value);
            }
        }
        /// <summary>
        /// Helixtoolkit裡面是右手定則
        /// </summary>
        private bool frontCounterClockwise = true;
        public bool FrontCounterClockwise
        {
            get
            {
                return frontCounterClockwise;
            }
            set
            {
                SetValue(ref frontCounterClockwise, value);
            }
        }
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        private String markerID;
        public String MarkerID
        {
            get
            {
                return markerID;
            }
            set
            {
                SetValue(ref markerID, value);
            }
        }
        /// <summary>
        /// 讀檔時特別存下Model3DGroup的，方便計算BoundingBox中心
        /// </summary>
        public Model3DGroup ModelContainer;
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D FinalModelTransform = new Matrix3D();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D TotalModelTransform = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private Matrix3D[] ModelTransformSet = new Matrix3D[10];
        public int Count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;
        public bool IsOSP = false;
        public bool IsLoaded = false;
        public bool IsRemoved = false;
        public bool IsAdded = true;
        public ModelData()
        {
            
        }
        public ModelData(ModelSettingItem oldModelInfo)
        {
            //ModelGeometry = oldModelInfo.ModelGeometry;
            //ModelMaterial = oldModelInfo.ModelMaterial;
            //ModelTransform = oldModelInfo.ModelTransform;
            //ModelFilePath = oldModelInfo.ModelFilePath;
            //ModelDiffuseColor = oldModelInfo.ModelDiffuseColor;            
        }
        public void AddItem(Matrix3D item)
        {
            if (Count < ModelTransformSet.Length)
            {
                Count++;

                TotalModelTransform = AddMatrix3D(TotalModelTransform, item);

                FinalModelTransform = DivideMatrix3D(TotalModelTransform, Count);

            }
            else
            {
                TotalModelTransform = SubtractMatrix3D(TotalModelTransform, ModelTransformSet[CurrenIndex]);

                TotalModelTransform = AddMatrix3D(TotalModelTransform, item);

                FinalModelTransform = DivideMatrix3D(TotalModelTransform, ModelTransformSet.Length);
            }

            ModelTransformSet[CurrenIndex] = item;

            CurrenIndex++;
            CurrenIndex = CurrenIndex % ModelTransformSet.Length;

        }
        public void SetTransformMatrix()
        {
            ModelTransform = new MatrixTransform3D(FinalModelTransform);
        }
        private Matrix3D AddMatrix3D(Matrix3D A, Matrix3D B)
        {
            return new Matrix3D(A.M11 + B.M11, A.M12 + B.M12, A.M13 + B.M13, A.M14 + B.M14,
                                A.M21 + B.M21, A.M22 + B.M22, A.M23 + B.M23, A.M24 + B.M24,
                                A.M31 + B.M31, A.M32 + B.M32, A.M33 + B.M33, A.M34 + B.M34,
                                A.OffsetX + B.OffsetX, A.OffsetY + B.OffsetY, A.OffsetZ + B.OffsetZ, A.M44 + B.M44);
        }
        private Matrix3D SubtractMatrix3D(Matrix3D A, Matrix3D B)
        {
            return new Matrix3D(A.M11 - B.M11, A.M12 - B.M12, A.M13 - B.M13, A.M14 - B.M14,
                                A.M21 - B.M21, A.M22 - B.M22, A.M23 - B.M23, A.M24 - B.M24,
                                A.M31 - B.M31, A.M32 - B.M32, A.M33 - B.M33, A.M34 - B.M34,
                                A.OffsetX - B.OffsetX, A.OffsetY - B.OffsetY, A.OffsetZ - B.OffsetZ, A.M44 - B.M44);
        }
        private Matrix3D DivideMatrix3D(Matrix3D A, double Divisor)
        {
            return new Matrix3D(A.M11 / Divisor, A.M12 / Divisor, A.M13 / Divisor, A.M14 / Divisor,
                                A.M21 / Divisor, A.M22 / Divisor, A.M23 / Divisor, A.M24 / Divisor,
                                A.M31 / Divisor, A.M32 / Divisor, A.M33 / Divisor, A.M34 / Divisor,
                                A.OffsetX / Divisor, A.OffsetY / Divisor, A.OffsetZ / Divisor, A.M44 / Divisor);
        }
    }
}
