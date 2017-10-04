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
    public class ModelData : ObservableObject //, IEquatable<ModelData>
    {
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
        public Model3DGroup SingleModel;

        public bool IsOSP = false;
        public bool IsLoaded = false;
        public bool IsRemoved = false;
        public bool IsAdded = false;
        public ModelData()
        {
            
        }
        public ModelData(ModelInfo oldModelInfo)
        {
            ModelGeometry = oldModelInfo.ModelGeometry;
            ModelMaterial = oldModelInfo.ModelMaterial;
            ModelTransform = oldModelInfo.ModelTransform;
            ModelFilePath = oldModelInfo.ModelFilePath;
            ModelDiffuseColor = oldModelInfo.ModelDiffuseColor;            
        }
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        private void SetModelMaterial()
        {
            ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
            ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
            float ambient = 0.0f;
            ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
            ModelMaterial.DiffuseColor = ModelDiffuseColor.ToColor4();
            ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            int Specular = 90;
            ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
            ModelMaterial.SpecularShininess = 60;
        }
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

            IsLoaded = true;
        }

        //public bool Equals(ModelData Other)
        //{
        //    if (Other.ModelFilePath == null || this.ModelFilePath == null)
        //        return false;
        //    return Other.ModelFilePath.Equals(this.ModelFilePath);
        //}
    }
}
