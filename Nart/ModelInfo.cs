using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using NartControl;
using NartControl.Control;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;


namespace Nart
{
    using SharpDX.Direct3D11;
    using Color = System.Windows.Media.Color;
    public class ModelInfo : ObservableObject
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
        /// <summary>
        /// 模型名稱
        /// </summary>
        private String modelFilePath;
        public String ModelFilePath
        {
            get
            {
                return modelFilePath;
            }
            set
            {
                SetValue(ref modelFilePath, value);
                IsLoaded = false;
            }
        }
        /// <summary>
        /// OSP名稱
        /// </summary>
        private String ospFilePath;
        public String OSPFilePath
        {
            get
            {
                return ospFilePath;
            }
            set
            {
                SetValue(ref ospFilePath, value);
            }
        }
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color modelDiffuseColor;
        public Color ModelDiffuseColor
        {
            get
            {
                return modelDiffuseColor;
            }
            set
            {
                SetValue(ref modelDiffuseColor, value);
                SetModelMaterial();
            }
        }
        /// <summary>
        /// OSP 顏色
        /// </summary>
        private Color ospDiffuseColor;
        public Color OSPDiffuseColor
        {
            get
            {
                return ospDiffuseColor;
            }
            set
            {
                SetValue(ref ospDiffuseColor, value);
            }
        }    
        private CullMode cMode;
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
        /// combobox選項的內容
        /// </summary>
        private List<String> comboboxList = MarkerDatabase.MarkerIDList;
        public List<String> ComboBoxList
        {
            get
            {
                return comboboxList;
            }
            set
            {
                SetValue(ref comboboxList, value);
            }
        }    
        /// <summary>
        /// 讀檔時特別存下Model3DGroup的
        /// </summary>
        public Model3DGroup SingleModel
        {
            get;
            set;
        }
        /// <summary>
        /// 儲存模型資料，包括矩陣轉移、材質、網格
        /// </summary>        
        public MeshGeometryModel3D MeshGeometryData { get; private set; } = new MeshGeometryModel3D();




        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D FinalModelTransform = new Matrix3D();
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D ModelTransformMatrix = new Matrix3D();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D TotalModelTransform = new Matrix3D();
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private Matrix3D[] ModelTransformSet = new Matrix3D[10];
        /// <summary>
        /// 設定模型材質
        /// </summary>
        private void SetModelMaterial()
        {
            ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
            ModelMaterial.ReflectiveColor =SharpDX.Color.Black;
            float ambient = 0.0f;
            ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
            ModelMaterial.DiffuseColor = ModelDiffuseColor.ToColor4();
            ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            float Specular = 0.5f;
            ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
            ModelMaterial.SpecularShininess = 80;
        }
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;
        public bool IsLoaded = false;
        public int DatabaseIndex { get; set; }
        public int Count { get; set; }
        public ModelInfo()
        {
   
            Count = 0;

            DatabaseIndex = -1;

            TotalModelTransform = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            
            ModelTransformMatrix.SetIdentity();


        }
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        public void LoadSTL()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (IsLoaded || !System.IO.File.Exists(ModelFilePath)) 
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            SingleModel = reader.Read(ModelFilePath);            

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = SingleModel.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            SetModelMaterial();
            //設定模型幾何形狀
            ModelGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            
            ModelGeometry.Normals = new Vector3Collection();
            ModelGeometry.Positions = new Vector3Collection();
            ModelGeometry.Indices = new IntCollection();

            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                ModelGeometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                ModelGeometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                ModelGeometry.Indices.Add(triangleindice);
            }
            OnPropertyChanged("ModelGeometry");

            //MeshGeometryData = new MeshGeometryModel3D();
            //將建立好的指派進helix.sharp格式的MeshGeometryModel3D
            MeshGeometryData.Material = this.ModelMaterial;
            MeshGeometryData.Geometry = this.ModelGeometry;
            MeshGeometryData.Transform = new TranslateTransform3D(0, 0, 0);
            IsLoaded = true;
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
        /// <summary>
        /// 每個ModelData當中都會儲存最終轉換的轉移矩陣ModelTransform
        /// </summary>
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
