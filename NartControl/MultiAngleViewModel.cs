using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;


namespace NartControl
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;

    public class MultiAngleViewModel : INotifyPropertyChanged
    {

        public Element3DCollection ModelGeometry { get; private set; } = new Element3DCollection();
       
        public Transform3D ModelTransform { get; private set; } = new TranslateTransform3D(0, 0, 0);
        public RenderTechnique RenderTechnique { get; private set; }
        public DefaultEffectsManager EffectsManager { get; private set; }
        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }
        public ModelContainer3DX ModelContainer { get; private set; } = new ModelContainer3DX();
        public Camera Camera1 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera2 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera3 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(5, 12, -5), UpDirection = new Vector3D(0, 1, 0) };        
        public MultiAngleViewModel(MultiAngleView multiview)
        {
           
            
           
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];

            //EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            //multiview.sharedContainer2.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO.stl"));
            //multiview.sharedContainer2.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla.stl"));
            //multiview.sharedContainer2.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO.stl"));
            
            //ModelContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"));
            //ModelContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO_0.4.stl"));
            //ModelContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO_4.stl"));

            //LoadSTL()
        }


        public MeshGeometryModel3D LoadSTL(string path)
        {
            ////using Point3D = System.Windows.Media.Media3D.Point3D;
            ////using Vector3D = System.Windows.Media.Media3D.Vector3D;
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();
            Model3DGroup model3dgroup = reader.Read(path);

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            PhongMaterial material = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
            material.AmbientColor = new Color4(Convert.ToSingle(0.349), Convert.ToSingle(0.349), Convert.ToSingle(0.349), 1);
            material.DiffuseColor = new Color4(Convert.ToSingle(0), Convert.ToSingle(0.5019), Convert.ToSingle(0), 1);
            material.SpecularColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);
            material.SpecularShininess = 12.8F;
            material.ReflectiveColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);


            //設定模型幾何形狀
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            geometry.Normals = new Vector3Collection();
            geometry.Positions = new Vector3Collection();
            geometry.Indices = new IntCollection();

            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                geometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                geometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                geometry.Indices.Add(triangleindice);
            }

            MeshGeometryModel3D meshGeometry = new MeshGeometryModel3D();

            //將建立好的指派進helix.sharp格式
            meshGeometry = new MeshGeometryModel3D();
            meshGeometry.Material = material;
            meshGeometry.Geometry = geometry;
            meshGeometry.Transform = new TranslateTransform3D(0, 140, 180);


            return meshGeometry;




            //meshgeometry.Attach(modelView.RenderHost); //viewport3dx

        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
