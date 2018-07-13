using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Nart.ExtensionMethods;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace Nart.Model_Object
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using Color = System.Windows.Media.Color;
    using GeometryModel3D = System.Windows.Media.Media3D.GeometryModel3D;
    using MeshGeometry3D = System.Windows.Media.Media3D.MeshGeometry3D;
    using Model3D = System.Windows.Media.Media3D.Model3D;
    public class OspModel : MeshGeometryModel3D
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
        /// <summary>
        /// 模型有Load進去則為true
        /// </summary>
        public new bool IsLoaded = false;
        /// <summary>
        /// OSP原始的Normal
        /// </summary>
        public Vector3D OspOriNormal;
        /// <summary>
        /// OSP才有的平面點
        /// </summary>
        public Point3D OspPlanePoint;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        public string MarkerId;
        /// <summary>
        /// 模型顏色
        /// </summary>
        public Color DiffuseColor;
        private string _filePath;
        private string _safeFileName;
        /// <summary>
        /// 模型路徑
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (value != _filePath)
                    _filePath = value;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    SafeFileName = Path.GetFileName(_filePath);
                }
            }
        }
        public string SafeFileName
        {
            get
            {
                return _safeFileName;
            }
            set
            {
                if (value != _safeFileName)
                    _safeFileName = value;
            }
        }
        private bool _highlight = false;

        public OspModel()
        {
        }
        public OspModel(SerializationInfo info, StreamingContext context)
        {
            IsRendering = (bool)info.GetValue("IsRendering", typeof(bool));
            MarkerId = (string)info.GetValue("MarkerId", typeof(string));        
            Matrix3D matrix = new Matrix3D();
            matrix.M11 = (double)info.GetValue("M11", typeof(double)); matrix.M12 = (double)info.GetValue("M12", typeof(double)); matrix.M13 = (double)info.GetValue("M13", typeof(double)); matrix.M14 = (double)info.GetValue("M14", typeof(double));
            matrix.M21 = (double)info.GetValue("M21", typeof(double)); matrix.M22 = (double)info.GetValue("M22", typeof(double)); matrix.M23 = (double)info.GetValue("M23", typeof(double)); matrix.M24 = (double)info.GetValue("M24", typeof(double));
            matrix.M31 = (double)info.GetValue("M31", typeof(double)); matrix.M32 = (double)info.GetValue("M32", typeof(double)); matrix.M33 = (double)info.GetValue("M33", typeof(double)); matrix.M34 = (double)info.GetValue("M34", typeof(double));
            matrix.OffsetX = (double)info.GetValue("M41", typeof(double)); matrix.OffsetY = (double)info.GetValue("M42", typeof(double)); matrix.OffsetZ = (double)info.GetValue("M43", typeof(double)); matrix.M44 = (double)info.GetValue("M44", typeof(double));
            Transform = new MatrixTransform3D(matrix);
            Color ospColor = new Color();
            ospColor.A = (byte)info.GetValue("ospColor_A", typeof(byte));
            ospColor.R = (byte)info.GetValue("ospColor_R", typeof(byte));
            ospColor.G = (byte)info.GetValue("ospColor_G", typeof(byte));
            ospColor.B = (byte)info.GetValue("ospColor_B", typeof(byte));
            DiffuseColor = ospColor;
        }
        public bool Highlight
        {
            set
            {
                if (_highlight == value) { return; }
                _highlight = value;
                if (_highlight)
                {                   
                    this.Material = new PhongMaterial { EmissiveColor = SharpDX.Color.Yellow };
                }
                else
                {
                    this.Material = new PhongMaterial { EmissiveColor = SharpDX.Color.Transparent };
                }
            }
            get
            {
                return _highlight;
            }
        }                
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsRendering", IsRendering);
            info.AddValue("MarkerId", MarkerId);
            info.AddValue("M11", Transform.Value.M11); info.AddValue("M12", Transform.Value.M12); info.AddValue("M13", Transform.Value.M13); info.AddValue("M14", Transform.Value.M14);
            info.AddValue("M21", Transform.Value.M21); info.AddValue("M22", Transform.Value.M22); info.AddValue("M23", Transform.Value.M23); info.AddValue("M24", Transform.Value.M24);
            info.AddValue("M31", Transform.Value.M31); info.AddValue("M32", Transform.Value.M32); info.AddValue("M33", Transform.Value.M33); info.AddValue("M34", Transform.Value.M34);
            info.AddValue("M41", Transform.Value.OffsetX); info.AddValue("M42", Transform.Value.OffsetY); info.AddValue("M43", Transform.Value.OffsetZ); info.AddValue("M44", Transform.Value.M44);
            info.AddValue("ospColor_A", DiffuseColor.A);
            info.AddValue("ospColor_R", DiffuseColor.R);
            info.AddValue("ospColor_G", DiffuseColor.G);
            info.AddValue("ospColor_B", DiffuseColor.B);

        }
        public void SetOspMaterial()
        {

            if (DiffuseColor != null)
            {
                this.Material = new PhongMaterial();
               
                Color4Collection colors = new Color4Collection();

                for (int i = 0; i < this.Geometry.Positions.Count; i++)
                {
                    colors.Add(DiffuseColor.ToColor4());
                }

                this.Geometry.Colors = colors;
            }   
        }
        public void LoadOsp()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (this.IsLoaded || !System.IO.File.Exists(FilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            this.ModelContainer = reader.Read(FilePath);

            var geometryModel = this.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //MIP產生的OSP一定是六個點
            if (mesh.Positions.Count != 6)
                return;

            this.OspOriNormal = new Vector3D(mesh.Normals[0].X, mesh.Normals[0].Y, mesh.Normals[0].Z);
            this.OspOriNormal.Normalize();//將上述向量正規化
            this.OspPlanePoint = mesh.Positions[0];

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


            modelGeometry.Colors = new Color4Collection();
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                modelGeometry.Colors.Add(new Color4(0.1f, 0.1f, 0.8f, 0.2f));
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

            this.Geometry = modelGeometry;

            SetOspMaterial();

            this.Transform = new MatrixTransform3D();

            this.IsLoaded = true;
        }
        /// <summary>
        /// 儲存模型檔案，第一個參數指定路徑，第二個參數指定是否要儲存轉移後的模型
        /// </summary>
        public void SaveModel(string filepath, bool isSavedTransform)
        {

            HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = Geometry as HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

            if (geometry == null)
                return;


            System.Windows.Media.Media3D.MeshGeometry3D mesh = new System.Windows.Media.Media3D.MeshGeometry3D
            {
                Positions = new Point3DCollection(),
                Normals = new Vector3DCollection(),
                TriangleIndices = new Int32Collection()
            };

            foreach (Vector3 position in geometry.Positions)
            {
                mesh.Positions.Add(new Point3D(position.X, position.Y, position.Z));
            }
            foreach (Vector3 normal in geometry.Normals)
            {
                mesh.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));
            }
            foreach (int triangleindice in geometry.TriangleIndices)
            {
                mesh.TriangleIndices.Add(triangleindice);
            }

            GeometryModel3D model = new GeometryModel3D
            {
                Geometry = mesh,

            };
            if (isSavedTransform)
            {
                model.Transform = Transform;
            }


            StlExporter export = new StlExporter();

            using (var fileStream = File.Create(System.IO.Path.Combine(filepath, SafeFileName)))
            {
                export.Export(model, fileStream);
            }



        }
        public Vector3D GetCurrentNormal()
        {
            Vector3D currentNormal = Transform.Transform(OspOriNormal);
            currentNormal.Normalize();
            return currentNormal;
        }
        /// <summary>
        /// 重設此類別的Shader
        /// </summary>
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }
    }
}
