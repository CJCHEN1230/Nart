using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Nart
{
    public class ModelData
    {
        /// <summary>
        /// 儲存模型資料
        /// </summary>        
        private Model3DGroup _modleGroup;
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D ModelTransform = new Matrix3D();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D TotalModelTransform = new Matrix3D();
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，7是累積總數量
        /// </summary>
        private Matrix3D[] ModelTransformSet = new Matrix3D[10];
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;
        /// <summary>
        /// 此模型對應資料庫中的索引
        /// </summary>
        public int DatabaseIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 累加矩陣的數目超過10個就不加了
        /// </summary>       
        public int Count
        {
            get;
            set;
        }
        
        public String Filename
        {
            get;            
            set;           
        }
        public ModelData(String filename)
        {
            
            Filename = filename;

            Count = 0;

            DatabaseIndex = -1;

            TotalModelTransform = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            ModelTransform.SetIdentity();

            Display3d(Filename);

            Rect3D rect3d = _modleGroup.Bounds;

            BoundingCenter = new Point3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);

            GeometryModel3D Geomodel = _modleGroup.Children[0] as GeometryModel3D;
            
            Material material=MaterialHelper.CreateMaterial( new SolidColorBrush(Color.FromRgb(40, 181, 187)),0.3, 50 ,100);
            //Material material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(40, 181, 187)), 200, 100,255,true);
            //Material material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(60, 231, 123)), 40, 50);
            //Material material = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(200, 120, 90)), 40, 50);

            Geomodel.Material = material;

            Geomodel.BackMaterial = material;

            
            //MainView._model3dgroup.Children.Add(_modleGroup);


        }

        public Point3D BoundingCenter
        {
            get;
            set;
        }

        public void AddItem(Matrix3D item)
        {
            

            if (Count < ModelTransformSet.Length)
            {
                Count++;

                TotalModelTransform = AddMatrix3D(TotalModelTransform, item);

                ModelTransform  = DivideMatrix3D(TotalModelTransform, Count);
            }
            else
            {
                TotalModelTransform = SubtractMatrix3D(TotalModelTransform, ModelTransformSet[CurrenIndex]);

                TotalModelTransform = AddMatrix3D(TotalModelTransform, item);

                ModelTransform = DivideMatrix3D(TotalModelTransform, ModelTransformSet.Length);

            }
            
            ModelTransformSet[CurrenIndex] = item;
            
            CurrenIndex++;
            CurrenIndex = CurrenIndex % ModelTransformSet.Length;
            
        }
        /// <summary>
        /// 每個ModelData當中都會儲存最終轉換的轉移矩陣ModelTransform
        /// </summary>
        public void SetTransformMatrix()
        {
            _modleGroup.Transform = new MatrixTransform3D(ModelTransform);
        }       
        /// <summary>
        /// 輸入路徑位置，將load好的模型存進_modleGroup
        /// </summary>
        private void Display3d(string model)
        {            
            try
            {                                
                ModelImporter import = new ModelImporter();
            
                _modleGroup = import.Load(model);

            }
            catch (Exception e)
            {               
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            
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
