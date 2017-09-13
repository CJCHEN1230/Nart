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
            }
        }
        /// <summary>
        /// BSP名稱
        /// </summary>
        private String bspFilePath;
        public String BSPFilePath
        {
            get
            {
                return bspFilePath;
            }
            set
            {
                SetValue(ref bspFilePath, value);
            }
        }
        /// <summary>
        /// Model 顏色
        /// </summary>
        private System.Windows.Media.Color modelDiffuseColor;
        public System.Windows.Media.Color ModelDiffuseColor
        {
            get
            {
                return modelDiffuseColor;
            }
            set
            {
                SetValue(ref modelDiffuseColor, value);
            }

        }
        /// <summary>
        /// BSP 顏色
        /// </summary>
        private System.Windows.Media.Color bspDiffuseColor;
        public System.Windows.Media.Color BSPDiffuseColor
        {
            get
            {
                return bspDiffuseColor;
            }
            set
            {
                SetValue(ref bspDiffuseColor, value);
            }
        }
        /// <summary>
        /// 儲存模型資料，包括矩陣轉移、材質、網格
        /// </summary>        
        public MeshGeometryModel3D MeshGeometryData { get; private set; }
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private SharpDX.Matrix ModelTransformMatrix = new SharpDX.Matrix();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private SharpDX.Matrix TotalModelTransform = new SharpDX.Matrix();
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private SharpDX.Matrix[] ModelTransformSet = new SharpDX.Matrix[10];
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;
        public int DatabaseIndex { get; set; }
        public int Count { get; set; }
        public ModelInfo()
        {
   
            Count = 0;

            DatabaseIndex = -1;

            TotalModelTransform = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ModelTransformMatrix = new Matrix(1, 0, 0, 0
                                                                     , 0, 1, 0, 0
                                                                     , 0, 0, 1, 0
                                                                     , 0, 0, 0, 1); //單位矩陣
       
        }
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        public void LoadSTL()
        {
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            //阻擋檔案不存在的情況
            if (!System.IO.File.Exists(ModelFilePath))
            {
                return;
            }
           
            Model3DGroup model3dgroup = reader.Read(ModelFilePath);

            MultiAngleViewModel.AllModelGroup.Children.Add(model3dgroup);

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();

            ModelMaterial.ReflectiveColor = Color.Black;
            float ambient = 0.0f;
            ModelMaterial.AmbientColor = new Color(ambient, ambient, ambient, 1.0f);
            ModelMaterial.DiffuseColor = new Color(40, 181, 187, 255);
            ModelMaterial.DiffuseColor = ModelDiffuseColor.ToColor4();

            ModelMaterial.EmissiveColor = Color.Black; //這是自己發光的顏色
            float Specular = 0.5f;
            ModelMaterial.SpecularColor = new Color(Specular, Specular, Specular, 255);
            ModelMaterial.SpecularShininess = 80;


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

            MeshGeometryData = new MeshGeometryModel3D();
            //將建立好的指派進helix.sharp格式的MeshGeometryModel3D
            MeshGeometryData.Material = this.ModelMaterial;
            MeshGeometryData.Geometry = ModelGeometry;
            MeshGeometryData.Transform = new TranslateTransform3D(0, 0, 0);


        }
    }
}
