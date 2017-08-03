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
using DemoCore;

namespace NartControl
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

    public class MultiAngleViewModel : BaseViewModel
    {

     
        public Model3DGroup ModelGroup { get; private set; } = new Model3DGroup();
        public Transform3D ModelTransform { get; private set; } = new TranslateTransform3D(0, 0, 0);
        public ModelContainer3DX ModelContainer { get; private set; } = new ModelContainer3DX();
        public PhongMaterial ModelMaterial { get; set; }
        public Camera Camera1 { get; private set; } = new OrthographicCamera { Position = new Point3D(0, 0, 0), LookDirection = new Vector3D(0, 0, 0), UpDirection = new Vector3D(0, 1, 0) };
        public Camera Camera2 { get; private set; } = new OrthographicCamera { Position = new Point3D(0, 0, 0), LookDirection = new Vector3D(0, 0, 0), UpDirection = new Vector3D(0, 1, 0) };
        public Camera Camera3 { get; private set; } = new OrthographicCamera { Position = new Point3D(0, 0, 0), LookDirection = new Vector3D(0, 0, 0), UpDirection = new Vector3D(0, 1, 0) };
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D Model { get; private set; }
        public Transform3D Light1Transform { get; private set; }
        public Vector3 Light1Direction { get; set; }
        public Vector3 Light2Direction { get; set; }
        public Vector3 Light3Direction { get; set; }
        public Color4 Light1Color { get; set; }
        public Color4 AmbientLightColor { get; set; }
        public bool IsRenderLight { get; set; }
        Rect3D  BoundingBox { get; set; }

        public HelixToolkit.Wpf.SharpDX.MeshBuilder b1 { get; set; } = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true, true);
        public MultiAngleViewModel(MultiAngleView multiview)
        {
           
            
           
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);



            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla.stl"));
            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO.stl"));
            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO.stl"));

            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"));
            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO_0.4.stl"));
            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO_4.stl"));



            b1.AddSphere(new Vector3(0.25f, 0.25f, 0.25f), 0.75, 64, 64);
            b1.AddBox(-new Vector3(0.25f, 0.25f, 0.25f), 1, 1, 1, HelixToolkit.Wpf.SharpDX.BoxFaces.All);
            b1.AddBox(-new Vector3(5.0f, 0.0f, 0.0f), 1, 1, 1, HelixToolkit.Wpf.SharpDX.BoxFaces.All);
            b1.AddSphere(new Vector3(5f, 0f, 0f), 0.75, 64, 64);
            b1.AddCylinder(new Vector3(0f, -3f, -5f), new Vector3(0f, 3f, -5f), 1.2, 64);

            this.Model = b1.ToMeshGeometry3D();
            this.ModelTransform = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);
            this.ModelMaterial = PhongMaterials.Chrome;

            SetLight();
            SetCamera();
        }
        public MeshGeometryModel3D LoadSTL(string path)
        {
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();
            Model3DGroup model3dgroup = reader.Read(path);

            ModelGroup.Children.Add(model3dgroup);

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
            meshGeometry.Transform = new TranslateTransform3D(0, 0, 0);


            return meshGeometry;



            



        }

        internal void SetLight()
        {
            AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            this.Light1Color = (Color4)Color.White;
            this.IsRenderLight = true;
            this.Light1Direction = new Vector3(-10, 5, 6);
            this.Light2Direction = new Vector3(10, -5, 6);
            this.Light3Direction = new Vector3(-5, -5, -6);
        }
        internal void SetCamera()
        {

            BoundingBox = ModelGroup.Bounds;
            Point3D Center = new Point3D(BoundingBox.X + BoundingBox.SizeX / 2.0, BoundingBox.Y + BoundingBox.SizeY / 2.0, BoundingBox.Z + BoundingBox.SizeZ / 2.0);

            OrthographicCamera orthoCam1 = Camera1 as OrthographicCamera;
            orthoCam1.Position = new Point3D(Center.X, Center.Y - (BoundingBox.SizeY), Center.Z);
            orthoCam1.UpDirection = new Vector3D(0, 0, 1);
            orthoCam1.LookDirection = new Vector3D(0, BoundingBox.SizeY, 0);
            orthoCam1.NearPlaneDistance = -1;
            orthoCam1.FarPlaneDistance = 1e15;
            orthoCam1.Width = BoundingBox.SizeX + 110;
            
            OrthographicCamera orthoCam2 = Camera2 as OrthographicCamera;
            orthoCam2.Position = new Point3D(Center.X, Center.Y, Center.Z + (BoundingBox.SizeZ));
            orthoCam2.UpDirection = new Vector3D(0, 1, 0);
            orthoCam2.LookDirection = new Vector3D(0, 0, -BoundingBox.SizeZ);
            orthoCam2.NearPlaneDistance = -1;
            orthoCam2.FarPlaneDistance = 1e15;
            orthoCam2.Width = BoundingBox.SizeX + 110;

            OrthographicCamera orthoCam3 = Camera3 as OrthographicCamera;
            orthoCam3.Position = new Point3D(Center.X - (BoundingBox.SizeX), Center.Y, Center.Z);
            orthoCam3.UpDirection = new Vector3D(0, 0, 1);
            orthoCam3.LookDirection = new Vector3D(BoundingBox.SizeX, 0, 0);
            orthoCam3.NearPlaneDistance = -1;
            orthoCam3.FarPlaneDistance = 1e15;
            orthoCam3.Width = BoundingBox.SizeX + 110;

            //b1.AddSphere(new Vector3(Convert.ToSingle( rect3d.X), Convert.ToSingle(rect3d.Y), Convert.ToSingle(rect3d.Z)), 5, 64, 64);
            //b1.AddSphere(new Vector3(Convert.ToSingle(rect3d.X + rect3d.SizeX / 2.0), Convert.ToSingle(rect3d.Y + rect3d.SizeY / 2.0), Convert.ToSingle(rect3d.Z + rect3d.SizeZ / 2.0)), 10, 64, 64);
            //b1.AddSphere(new Vector3(0, 0, 0), 10, 64, 64);

            //this.Model = b1.ToMeshGeometry3D();
            //this.ModelTransform = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);
            //this.ModelMaterial = PhongMaterials.Chrome;


        }
    }
}
