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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
    using ProjectionCamera = HelixToolkit.Wpf.SharpDX.ProjectionCamera;

    public class MultiAngleViewModel : BaseViewModel
    {

     
        public Model3DGroup ModelGroup { get; private set; } = new Model3DGroup();
        public Transform3D ModelTransform { get; private set; } = new TranslateTransform3D(0, 0, 0);
        public ModelContainer3DX ModelContainer { get; private set; } = new ModelContainer3DX();
        public PhongMaterial ModelMaterial { get; set; }
        private Camera cam1;
        public Camera Camera1
        {
            get
            {
                return cam1;
            }

            private set
            {
                SetValue(ref cam1, value, "Camera1");
                CameraModel = value is PerspectiveCamera
                                       ? Perspective
                                       : value is OrthographicCamera ? Orthographic : null;
            }
        }
        private Camera cam2;
        public Camera Camera2
        {
            get
            {
                return cam2;
            }

            private set
            {
                SetValue(ref cam2, value, "Camera2");
                CameraModel = value is PerspectiveCamera
                                       ? Perspective
                                       : value is OrthographicCamera ? Orthographic : null;
            }
        }
        private Camera cam3;
        public Camera Camera3
        {
            get
            {
                return cam3;
            }

            private set
            {
                SetValue(ref cam3, value, "Camera3");
                CameraModel = value is PerspectiveCamera
                                       ? Perspective
                                       : value is OrthographicCamera ? Orthographic : null;
            }
        }
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D Model { get; private set; }
        private Vector3 light1Direction = new Vector3();
        public Vector3 Light1Direction
        {
            set
            {
                if (light1Direction != value)
                {
                    light1Direction = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return light1Direction;
            }
        }
        private Vector3 light2Direction = new Vector3();
        public Vector3 Light2Direction
        {
            set
            {
                if (light2Direction != value)
                {
                    light2Direction = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return light2Direction;
            }
        }
        private Vector3 light3Direction = new Vector3();
        public Vector3 Light3Direction
        {
            set
            {
                if (light3Direction != value)
                {
                    light3Direction = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return light3Direction;
            }
        }
        private Vector3D cam1LookDir = new Vector3D(-10, -10, -10);
        public Vector3D Cam1LookDir
        {
            set
            {
                if (cam1LookDir != value)
                {
                    cam1LookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value.ToVector3();
                }
            }
            get
            {
                return cam1LookDir;
            }
        }
        private Vector3D cam2LookDir = new Vector3D(-20, -20, -20);
        public Vector3D Cam2LookDir
        {
            set
            {
                if (cam2LookDir != value)
                {
                    cam2LookDir = value;
                    OnPropertyChanged();
                    Light2Direction = value.ToVector3();
                }
            }
            get
            {
                return cam2LookDir;
            }
        }
        private Vector3D cam3LookDir = new Vector3D(-30, -30, -30);
        public Vector3D Cam3LookDir
        {
            set
            {
                if (cam3LookDir != value)
                {
                    cam3LookDir = value;
                    OnPropertyChanged();
                    Light3Direction = value.ToVector3();
                }
            }
            get
            {
                return cam3LookDir;
            }
        }
        public Color4 LightColor { get; set; }
        public Color4 AmbientLightColor { get; set; }
        public bool IsRenderLight { get; set; }
        Rect3D  BoundingBox { get; set; }
        Point3D ModelCenter { get; set; }
        public HelixToolkit.Wpf.SharpDX.MeshBuilder b1 { get; set; } = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true, true);
        public MultiAngleViewModel(MultiAngleView multiview)
        {


            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            
           

            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla.stl"));
            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO.stl"));
            //multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO.stl"));



            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"));
            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl"));
            multiview.sharedContainer.Items.Add(LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"));



            //b1.AddSphere(new Vector3(0.25f, 0.25f, 0.25f), 0.75, 64, 64);
            //b1.AddBox(-new Vector3(0.25f, 0.25f, 0.25f), 1, 1, 1, HelixToolkit.Wpf.SharpDX.BoxFaces.All);
            //b1.AddBox(-new Vector3(5.0f, 0.0f, 0.0f), 1, 1, 1, HelixToolkit.Wpf.SharpDX.BoxFaces.All);
            //b1.AddSphere(new Vector3(5f, 0f, 0f), 0.75, 64, 64);
            //b1.AddCylinder(new Vector3(0f, -3f, -5f), new Vector3(0f, 3f, -5f), 1.2, 64);

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

             //PhongMaterial material = PhongMaterials.Chrome;

            //Material material=MaterialHelper.CreateMaterial( new SolidColorBrush(Color.FromRgb(40, 181, 187)),0.3, 50 ,100);

            //Color.White
            material.ReflectiveColor = Color.Black;
            material.AmbientColor = Color.Black;
            //material.AmbientColor = new Color(10, 10, 10, 255);
            material.DiffuseColor = new Color(40, 181, 187, 255);
            material.EmissiveColor = Color.Black;
            material.SpecularColor = new Color(50, 50, 50, 255);
            //material.SpecularColor = new Color(100, 100, 100, 255);
            // material.SpecularColor = new Color(90, 90, 90, 255);
            //material.SpecularColor = Color.White;
            material.SpecularShininess = 200;
            //material.ReflectiveColor = new Color(20, 20, 20, 255);
            

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
            AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);


            this.LightColor = (Color4)Color.White;
            this.IsRenderLight = true;


            //this.Light3Attenuation = new Vector3(100,-50, 60);
            // this.Light3Transform = new TranslateTransform3D(100, -50, 60);
            //CreateAnimatedTransform1(new Vector3D(0, 0, 4), new Vector3D(0, 1, 0), 5);

        }
        private void SetCamera()
        {


            this.Camera1 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
            this.Camera2 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
            this.Camera3 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
            SetupCameraBindings(this.Camera1, "Cam1LookDir");
            SetupCameraBindings(this.Camera2, "Cam2LookDir");
            SetupCameraBindings(this.Camera3, "Cam3LookDir");


            ResetCameraPosition();




            b1.AddSphere(new Vector3(Convert.ToSingle(ModelCenter.X + 100), Convert.ToSingle(ModelCenter.Y - 50), Convert.ToSingle(ModelCenter.Z - 60)), 5, 64, 64);
            b1.AddSphere(new Vector3(0, 0, 0), 10, 64, 64);

            this.Model = b1.ToMeshGeometry3D();
            this.ModelTransform = new System.Windows.Media.Media3D.TranslateTransform3D(0, 0, 0);
            this.ModelMaterial = PhongMaterials.Chrome;


        }

        internal void ResetCameraPosition()
        {
            BoundingBox = ModelGroup.Bounds;
            ModelCenter = new Point3D(BoundingBox.X + BoundingBox.SizeX / 2.0, BoundingBox.Y + BoundingBox.SizeY / 2.0, BoundingBox.Z + BoundingBox.SizeZ / 2.0);

            OrthographicCamera orthoCam1 = Camera1 as OrthographicCamera;
            orthoCam1.Position = new Point3D(ModelCenter.X, ModelCenter.Y - (BoundingBox.SizeY), ModelCenter.Z);
            orthoCam1.UpDirection = new Vector3D(0, 0, 1);
            orthoCam1.LookDirection = new Vector3D(0, BoundingBox.SizeY, 0);
            orthoCam1.NearPlaneDistance = -1000;
            orthoCam1.FarPlaneDistance = 1e15;
            orthoCam1.Width = BoundingBox.SizeX + 110;

            OrthographicCamera orthoCam2 = Camera2 as OrthographicCamera;
            //orthoCam2.Position = new Point3D(ModelCenter.X, ModelCenter.Y, ModelCenter.Z + (BoundingBox.SizeZ));
            //orthoCam2.UpDirection = new Vector3D(0, 1, 0);
            //orthoCam2.LookDirection = new Vector3D(0, 0, -BoundingBox.SizeZ);
            //orthoCam2.NearPlaneDistance = -1000;
            //orthoCam2.FarPlaneDistance = 1e15;
            //orthoCam2.Width = BoundingBox.SizeX + 110;
            orthoCam2.Position = new Point3D(ModelCenter.X, ModelCenter.Y - (BoundingBox.SizeY), ModelCenter.Z);
            orthoCam2.UpDirection = new Vector3D(0, 0, 1);
            orthoCam2.LookDirection = new Vector3D(0, BoundingBox.SizeY, 0);
            orthoCam2.NearPlaneDistance = -1000;
            orthoCam2.FarPlaneDistance = 1e15;
            orthoCam2.Width = BoundingBox.SizeX + 110;

            OrthographicCamera orthoCam3 = Camera3 as OrthographicCamera;
            orthoCam3.Position = new Point3D(ModelCenter.X - (BoundingBox.SizeX), ModelCenter.Y, ModelCenter.Z);
            orthoCam3.UpDirection = new Vector3D(0, 0, 1);
            orthoCam3.LookDirection = new Vector3D(BoundingBox.SizeX, 0, 0);
            orthoCam3.NearPlaneDistance = -1000;
            orthoCam3.FarPlaneDistance = 1e15;
            orthoCam3.Width = BoundingBox.SizeX + 110;

        }
        public void SetupCameraBindings(Camera camera, string camLookDir)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding(camLookDir, camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }

        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }

    }
}
