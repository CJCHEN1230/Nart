using HelixToolkit.Wpf;
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
        /// 讀檔時存模型的容器，方便之後算BoundingBox
        /// </summary>
        public Model3DGroup ModelContainer;
        /// <summary>
        /// 已從ModelDataCollection中移除則會變成true，在model setting 的頁面中按"-"時會改變成true
        /// </summary>
        public bool IsRemoved = false;
        /// <summary>
        /// 有被加進去模型清單才為True
        /// </summary>
        public bool IsAdded = false;
        public bool IsLoaded = false;
        /// <summary>
        /// 模型幾何形狀
        /// </summary>
        private HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry;        
        /// <summary>
        /// 模型材質設定
        /// </summary>
        private PhongMaterial modelMaterial;        
        /// <summary>
        /// 模型矩陣設定
        /// </summary>     
        private MatrixTransform3D modelTransform;
        /// <summary>
        /// 剔除面
        /// </summary>
        private CullMode cMode = CullMode.Back;
        /// <summary>
        /// 反轉Normal方向
        /// </summary>
        private bool ivtNormal = false;
        /// <summary>
        /// Helixtoolkit裡面是右手定則，跟Opengl一樣
        /// </summary>
        private bool frontCounterClockwise = true;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        private String markerID;
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D _finalModelTransform = new Matrix3D();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D _totalModelTransform = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private Matrix3D[] _modelTransformSet = new Matrix3D[10];
        /// <summary>
        /// Count是在ModelTransformSet中的累積數量
        /// </summary>
        private int Count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;

        public ModelData()
        {
            
        }        
        public void AddItem(Matrix3D item)
        {
            //數量少於陣列總長度則往後加入
            if (Count < _modelTransformSet.Length)
            {
                Count++;

                AddMatrix3D(ref _totalModelTransform, ref item);

                DivideMatrix3D(ref _totalModelTransform,  Count, ref _finalModelTransform);

            }
            else
            {
                SubtractMatrix3D(ref _totalModelTransform, ref _modelTransformSet[CurrenIndex]);

                AddMatrix3D(ref _totalModelTransform, ref item);

                DivideMatrix3D(ref _totalModelTransform, Count, ref _finalModelTransform);
            }

            _modelTransformSet[CurrenIndex] = item;

            CurrenIndex++;
            CurrenIndex = CurrenIndex % _modelTransformSet.Length;

        }
        public void SetTransformMatrix()
        {
            ModelTransform = new MatrixTransform3D(_finalModelTransform);
        }       
        private void AddMatrix3D(ref Matrix3D A, ref Matrix3D B)
        {
            A.M11 = A.M11 + B.M11; A.M12 = A.M12 + B.M12; A.M13 = A.M13 + B.M13; A.M14 = A.M14 + B.M14;
            A.M21 = A.M21 + B.M21; A.M22 = A.M22 + B.M22; A.M23 = A.M23 + B.M23; A.M24 = A.M24 + B.M24;
            A.M31 = A.M31 + B.M31; A.M32 = A.M32 + B.M32; A.M33 = A.M33 + B.M33; A.M34 = A.M34 + B.M34;
            A.OffsetX = A.OffsetX + B.OffsetX; A.OffsetY = A.OffsetY + B.OffsetY; A.OffsetZ = A.OffsetZ + B.OffsetZ; A.M44 = A.M44 + B.M44;
        }
        private void  SubtractMatrix3D(ref Matrix3D A, ref Matrix3D B)
        {
            A.M11 = A.M11 - B.M11; A.M12 = A.M12 - B.M12; A.M13 = A.M13 - B.M13; A.M14 = A.M14 - B.M14;
            A.M21 = A.M21 - B.M21; A.M22 = A.M22 - B.M22; A.M23 = A.M23 - B.M23; A.M24 = A.M24 - B.M24;
            A.M31 = A.M31 - B.M31; A.M32 = A.M32 - B.M32; A.M33 = A.M33 - B.M33; A.M34 = A.M34 - B.M34;
            A.OffsetX = A.OffsetX - B.OffsetX; A.OffsetY = A.OffsetY - B.OffsetY; A.OffsetZ = A.OffsetZ - B.OffsetZ; A.M44 = A.M44 - B.M44;
        }
        private void DivideMatrix3D(ref Matrix3D A, int divisor ,ref Matrix3D result)
        {
            double divisorDouble = Convert.ToDouble(divisor);

            result.M11 = A.M11 / divisorDouble; result.M12 = A.M12 / divisorDouble; result.M13 = A.M13 / divisorDouble; result.M14 = A.M14 / divisorDouble;
            result.M21 = A.M21 / divisorDouble; result.M22 = A.M22 / divisorDouble; result.M23 = A.M23 / divisorDouble; result.M24 = A.M24 / divisorDouble;
            result.M31 = A.M31 / divisorDouble; result.M32 = A.M32 / divisorDouble; result.M33 = A.M33 / divisorDouble; result.M34 = A.M34 / divisorDouble;
            result.OffsetX = A.OffsetX / divisorDouble; result.OffsetY = A.OffsetY / divisorDouble; result.OffsetZ = A.OffsetZ / divisorDouble; result.M44 = A.M44 / divisorDouble;

        }

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
    }
}
