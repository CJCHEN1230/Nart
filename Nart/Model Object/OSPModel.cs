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
    using Color = System.Windows.Media.Color;
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
        public String MarkerId;
        /// <summary>
        /// 模型顏色
        /// </summary>
        public Color DiffuseColor;
        /// <summary>
        /// 模型路徑
        /// </summary>
        public String FilePath;
        private bool _highlight = false;

        public OspModel()
        {
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
