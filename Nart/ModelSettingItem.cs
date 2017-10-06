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
    /// <summary>
    /// 模型設置清單中Listview中的項目
    /// </summary>
    public class ModelSettingItem : ObservableObject
    {
        /// <summary>
        /// 設置列當中的模型物件
        /// </summary>
        public ModelData Model = new ModelData();
        /// <summary>
        /// 設置列當中的OSP物件
        /// </summary>
        public ModelData OSP = new ModelData();
        /// <summary>
        /// 模型名稱
        /// </summary>
        private String modelFilePath;
        /// <summary>
        /// OSP名稱
        /// </summary>
        private String ospFilePath;
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color modelDiffuseColor;
        /// <summary>
        /// OSP 顏色
        /// </summary>
        private Color ospDiffuseColor;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        private String markerID;
        /// <summary>
        /// combobox選項的內容
        /// </summary>
        private List<String> comboboxList = MarkerDatabase.MarkerIDList;

                               
        public ModelSettingItem()
        {                       
            
        }
        /// <summary>
        /// 設定模型材質
        /// </summary>        
        private void SetModelMaterial()
        {
            if (Model.ModelMaterial == null)
            {
                Model.ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
                Model.ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
                float ambient = 0.0f;
                Model.ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                Model.ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                int Specular = 90;
                Model.ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
                Model.ModelMaterial.SpecularShininess = 60;
            }

            Model.ModelMaterial.DiffuseColor = ModelDiffuseColor.ToColor4();
        }
        /// <summary>
        /// 設定OSP材質
        /// </summary>        
        private void SetOSPMaterial()
        {
            if (OSP.ModelMaterial == null)
            {
                OSP.ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
                OSP.ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
                float ambient = 0.0f;
                OSP.ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                OSP.ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                int Specular = 90;
                OSP.ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
                OSP.ModelMaterial.SpecularShininess = 60;
            }

            OSP.ModelMaterial.DiffuseColor = OSPDiffuseColor.ToColor4();
        }
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        public void Load()
        {            
            LoadModel();            
            LoadOSP();
        }
        /// <summary>
        /// load進模型檔案
        /// </summary>
        public void LoadModel()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (Model.IsLoaded || !System.IO.File.Exists(ModelFilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            Model.ModelContainer = reader.Read(ModelFilePath);

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = Model.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            //SetMaterial(ref Model.ModelMaterial, modelDiffuseColor);
            SetModelMaterial();

            //設定模型幾何形狀
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            modelGeometry.Normals = new Vector3Collection();
            modelGeometry.Positions = new Vector3Collection();
            modelGeometry.Indices = new IntCollection();

            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                modelGeometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                modelGeometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                modelGeometry.Indices.Add(triangleindice);
            }
            Model.MarkerID = this.MarkerID;

            Model.ModelGeometry = modelGeometry;

            Model.ModelTransform = new MatrixTransform3D();

            Model.IsLoaded = true;                        
        }
        /// <summary>
        /// load進OSP檔案
        /// </summary>
        public void LoadOSP()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (OSP.IsLoaded || !System.IO.File.Exists(OSPFilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            OSP.ModelContainer = reader.Read(OSPFilePath);

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = OSP.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            SetOSPMaterial();
            //設定模型幾何形狀
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D ospGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            ospGeometry.Normals = new Vector3Collection();
            ospGeometry.Positions = new Vector3Collection();
            ospGeometry.Indices = new IntCollection();

            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                ospGeometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                ospGeometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                ospGeometry.Indices.Add(triangleindice);
            }
            OSP.MarkerID = this.MarkerID;

            OSP.ModelGeometry = ospGeometry;

            OSP.ModelTransform = new MatrixTransform3D();

            OSP.IsLoaded = true;
        }


        public String ModelFilePath
        {
            get
            {
                return modelFilePath;
            }
            set
            {
                SetValue(ref modelFilePath, value);
                Model.IsLoaded = false;                
            }
        }
        public String OSPFilePath
        {
            get
            {
                return ospFilePath;
            }
            set
            {
                SetValue(ref ospFilePath, value);
                OSP.IsLoaded = false;
            }
        }
        public Color ModelDiffuseColor
        {
            get
            {
                return modelDiffuseColor;
            }
            set
            {
                SetValue(ref modelDiffuseColor, value);
                if (Model.IsLoaded == true)
                {
                    SetModelMaterial();
                }
            }
        }
        public Color OSPDiffuseColor
        {
            get
            {
                return ospDiffuseColor;
            }
            set
            {
                SetValue(ref ospDiffuseColor, value);
                if (OSP.IsLoaded == true)
                {
                    SetOSPMaterial();
                }
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
    }
}
