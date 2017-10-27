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
using NartControl;


namespace Nart
{
    using Model;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
    using ProjectionCamera = HelixToolkit.Wpf.SharpDX.ProjectionCamera;

    public class MultiAngleViewModel : ObservableObject
    {          
        private static string DevAngle; //DeviationAngle
        private static string DevDist;     //DeviationDistance
        private static string FrontalDevAngle;//FrontalDeviationAngle
        private static string HorizontalDevAngle;//HorizontalDeviationAngle
        private static string PosteriorDevDist;      //PosteriorDeviationDistance
        private static Camera cam1 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private static Camera cam2 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private static Camera cam3 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private RenderTechnique renderTechnique;
        private Vector3 light1Direction = new Vector3();
        private Vector3 light2Direction = new Vector3();
        private Vector3 light3Direction = new Vector3();
        private Vector3D cam1LookDir = new Vector3D();
        private Vector3D cam2LookDir = new Vector3D();
        private Vector3D cam3LookDir = new Vector3D();      
        private MultiAngleView _multiview;
        private readonly IList<ModelData> HighlightItems = new List<ModelData>(); //專門放點到的變色物件



        public MultiAngleViewModel(MultiAngleView _multiview)
        {

            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this._multiview = _multiview;
            CreateDefaultModels();
            SetLight();
            SetCamera();
        }




        private static readonly Random rnd = new Random();
        private void CreateDefaultModels()
        {
            
            //var b2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, true, true);
            //b2.AddSphere(new Vector3(15f, 0f, 0f), 4, 64, 64);
            //b2.AddSphere(new Vector3(25f, 0f, 0f), 2, 32, 32);
            //b2.AddTube(new Vector3[] { new Vector3(10f, 5f, 0f), new Vector3(10f, 7f, 0f) }, 2, 12, false, true, true);


            

            //ModelData Skull = new Nart.ModelData();
       
       
            ////利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            //StLReader reader = new HelixToolkit.Wpf.StLReader();

            //Skull.ModelContainer = reader.Read("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl");

         
            //var geometryModel = Skull.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            //var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //var color = rnd.NextColor();

            

            ////設定模型幾何形狀
            //HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            //modelGeometry.Normals = new Vector3Collection();
            //modelGeometry.Positions = new Vector3Collection();
            //modelGeometry.Indices = new IntCollection();

            ////將從stlreader讀到的資料轉入
            //foreach (Point3D position in mesh.Positions)
            //{
            //    modelGeometry.Positions.Add(new Vector3(
            //          Convert.ToSingle(position.X)
            //        , Convert.ToSingle(position.Y)
            //        , Convert.ToSingle(position.Z)));
            //}

            //foreach (Vector3D normal in mesh.Normals)
            //{
            //    modelGeometry.Normals.Add(new Vector3(
            //          Convert.ToSingle(normal.X)
            //        , Convert.ToSingle(normal.Y)
            //        , Convert.ToSingle(normal.Z)));
            //}

            //foreach (Int32 triangleindice in mesh.TriangleIndices)
            //{
            //    modelGeometry.Indices.Add(triangleindice);
            //}

            //Skull.ModelGeometry = modelGeometry;

            ////設定模型材質
            //Skull.ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
            //Skull.ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
            //float ambient = 0.0f;
            //Skull.ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
            //Skull.ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            //int Specular = 90;
            //Skull.ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
            //Skull.ModelMaterial.SpecularShininess = 60;
            //Skull.ModelMaterial.DiffuseColor = rnd.NextColor().ToColor4();

















            ////StLReader reader2 = new HelixToolkit.Wpf.StLReader();

            ////Model3DGroup model3dgroup = reader2.Read("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl");


            ////var geometryModel2 = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            ////var mesh2 = geometryModel2.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;



            //////設定模型幾何形狀
            ////HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry2 = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            ////modelGeometry2.Normals = new Vector3Collection();
            ////modelGeometry2.Positions = new Vector3Collection();
            ////modelGeometry2.Indices = new IntCollection();
            ////modelGeometry2.Colors = new Color4Collection();

            ////for (int i = 0; i < mesh.Positions.Count; i++)
            ////{
            ////    modelGeometry2.Colors.Add(new Color4(0.1f, 0.1f, 0.8f, 0.2f));
            ////}

            //////將從stlreader讀到的資料轉入
            ////foreach (Point3D position in mesh2.Positions)
            ////{
            ////    modelGeometry2.Positions.Add(new Vector3(
            ////          Convert.ToSingle(position.X)
            ////        , Convert.ToSingle(position.Y)
            ////        , Convert.ToSingle(position.Z)));
            ////}

            ////foreach (Vector3D normal in mesh2.Normals)
            ////{
            ////    modelGeometry2.Normals.Add(new Vector3(
            ////          Convert.ToSingle(normal.X)
            ////        , Convert.ToSingle(normal.Y)
            ////        , Convert.ToSingle(normal.Z)));
            ////}

            ////foreach (Int32 triangleindice in mesh2.TriangleIndices)
            ////{
            ////    modelGeometry2.Indices.Add(triangleindice);
            ////}


            ////_multiview.osp.Geometry = modelGeometry2;



            //StLReader reader2 = new HelixToolkit.Wpf.StLReader();

            //Model3DGroup model3dgroup = reader2.Read("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl");

            //var geometryModel2 = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            //var mesh2 = geometryModel2.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

      

            ////定義整個對稱面八個點
            //Point3D[] ospPoint = new Point3D[8];
            //int[] repeatIndex = new int[2] { -1, -1 };
            //int[] repeatIndex2 = new int[2] { -1, -1 };

            ////前半網格點
            //for (int i = 0, k = 0; i < mesh2.Positions.Count / 2; i++)
            //{ //後半網格點
            //    for (int j = 3; j < mesh2.Positions.Count; j++)
            //    {   //尋找前半跟後半有哪些重複存下引數在repeatIndex  repeatIndex2
            //        double error = Math.Abs(Math.Pow(mesh2.Positions[i].X - mesh2.Positions[j].X, 2)) + Math.Abs(Math.Pow(mesh2.Positions[i].Y - mesh2.Positions[j].Y, 2)) + Math.Abs(Math.Pow(mesh2.Positions[i].Z - mesh2.Positions[j].Z, 2));
            //        if (error < 10E-008)
            //        {
            //            repeatIndex[k] = i;
            //            repeatIndex2[k] = j;
            //            k++;
            //        }
            //    }
            //}
            ////找出沒有重複的那個引數字
            //int thirdIndex = 3 - repeatIndex[0] - repeatIndex[1];
            //int thirdIndex2 = 12 - repeatIndex2[0] - repeatIndex2[1];


            //if (thirdIndex == 0)
            //{
            //    ospPoint[0] = mesh2.Positions[0];
            //    ospPoint[1] = mesh2.Positions[1];
            //    ospPoint[2] = mesh2.Positions[thirdIndex2];
            //    ospPoint[3] = mesh2.Positions[2];
            //}
            //else if (thirdIndex == 1)
            //{
            //    ospPoint[0] = mesh2.Positions[1];
            //    ospPoint[1] = mesh2.Positions[2];
            //    ospPoint[2] = mesh2.Positions[thirdIndex2];
            //    ospPoint[3] = mesh2.Positions[0];
            //}
            //else
            //{
            //    ospPoint[0] = mesh2.Positions[2];
            //    ospPoint[1] = mesh2.Positions[0];
            //    ospPoint[2] = mesh2.Positions[thirdIndex2];
            //    ospPoint[3] = mesh2.Positions[1];
            //}


            //for (int i = 4; i < ospPoint.Length; i++)
            //{
            //    ospPoint[i] = new Point3D(ospPoint[i - 4].X - 0.1 * mesh2.Normals[0].X,
            //                                                      ospPoint[i - 4].Y - 0.1 * mesh2.Normals[0].Y,
            //                                                      ospPoint[i - 4].Z - 0.1 * mesh2.Normals[0].Z);
            //}

            //for (int i = 0; i < ospPoint.Length / 2; i++)
            //{
            //    ospPoint[i] = new Point3D(ospPoint[i].X + 0.1 * mesh2.Normals[0].X,
            //                                                      ospPoint[i].Y + 0.1 * mesh2.Normals[0].Y,
            //                                                      ospPoint[i].Z + 0.1 * mesh2.Normals[0].Z);
            //}



            //HelixToolkit.Wpf.SharpDX.MeshGeometry3D ospGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            //ospGeometry.Normals = new Vector3Collection();
            //ospGeometry.Positions = new Vector3Collection();
            //ospGeometry.Indices = new IntCollection();
            
            //// 0 1 2 3
            //CreatePoint(0, 1, 2, 3, ospGeometry.Positions, ospPoint);

            //CreateNormal(0, 1, 2, ospGeometry.Normals, ospPoint);


            //////5 4 7 6
            //CreatePoint(5, 4, 7, 6, ospGeometry.Positions, ospPoint);
            //CreateNormal(5, 4, 7, ospGeometry.Normals, ospPoint);

            //for (int i = 0; i < ospGeometry.Positions.Count; i++)
            //{
            //    ospGeometry.Indices.Add(i);
            //}
            //ospGeometry.Colors = new Color4Collection();

            //for (int i = 0; i < ospGeometry.Positions.Count; i++)
            //{
            //    ospGeometry.Colors.Add(new Color4(0.1f, 0.1f, 0.8f, 0.2f));
            //}




            //PhongMaterial OSPMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
            ////OSPMaterial.ReflectiveColor = SharpDX.Color.Black;
            ////OSPMaterial.AmbientColor = new SharpDX.Color(0.0f, 0.0f, 0.0f, 1.0f);
            ////OSPMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            ////OSPMaterial.SpecularColor = new SharpDX.Color(90, 90, 90, 255);
            ////OSPMaterial.SpecularShininess = 60;
            ////OSPMaterial.DiffuseColor = new Color4(0.1f, 0.1f, 0.8f, 0.2f);


            //osp.Material = OSPMaterial;
            //osp.Geometry = ospGeometry;


            //_multiview.sharedContainer.Items.Add(osp);


           




            //ModelDataCollection.Add(Skull);

       
        }

        
        //////////////////////////////////////

        public static string DeviationAngle
        {
            get { return DevAngle; }
            set
            {
                SetStaticValue(ref DevAngle, value);
            }
        }
        public static string DeviationDistance
        {
            get { return DevDist; }
            set
            {
                SetStaticValue(ref DevDist, value);
            }
        }
        public static string FrontalDeviationAngle
        {
            get { return FrontalDevAngle; }
            set
            {
                SetStaticValue(ref FrontalDevAngle, value);
            }
        }
        public static string HorizontalDeviationAngle
        {
            get { return HorizontalDevAngle; }
            set
            {
                SetStaticValue(ref HorizontalDevAngle, value);
            }
        }
        public static string PosteriorDeviationDistance
        {
            get { return PosteriorDevDist; }
            set
            {
                SetStaticValue(ref PosteriorDevDist, value);
            }
        }    
        public static Camera Camera1
        {
            get
            {
                return cam1;
            }

            private set
            {
                SetStaticValue(ref cam1, value);
            }
        }
        public static Camera Camera2
        {
            get
            {
                return cam2;
            }

            private set
            {
                SetStaticValue(ref cam2, value);
            }
        }
        public static Camera Camera3
        {
            get
            {
                return cam3;
            }

            private set
            {
                SetStaticValue(ref cam3, value);
            }
        }
        public Vector3 Light1Direction
        {
            get
            {
                return light1Direction;
            }
            set
            {
                SetValue(ref light1Direction, value);
            }
        }
        public Vector3 Light2Direction
        {
            get
            {
                return light2Direction;
            }
            set
            {
                SetValue(ref light2Direction, value);
            }
        }
        public Vector3 Light3Direction
        {
            get
            {
                return light3Direction;
            }
            set
            {
                SetValue(ref light3Direction, value);
            }
        }
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
        public Color4 DirectionalLightColor { get; set; }
        public RenderTechnique RenderTechnique
        {
            get
            {
                return renderTechnique;
            }
            set
            {
                SetValue(ref renderTechnique, value);
            }
        }
        public IEffectsManager EffectsManager { get; protected set; }
        public IRenderTechniquesManager RenderTechniquesManager { get; protected set; }
        public static ObservableCollection<Element3D> BoneModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        public static ObservableCollection<Element3D> OSPModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();

        /// <summary>
        /// 設定光源Ambientlight顏色、DirectionaLlight顏色        
        /// </summary>
        internal void SetLight()
        {         
            this.DirectionalLightColor = (Color4)Color.White;
        }
        /// <summary>
        /// 初始化相機參數，並綁定相機觀看方向
        /// </summary>
        private void SetCamera()
        {
            SetupCameraBindings("Cam1LookDir", Camera1);
            SetupCameraBindings("Cam2LookDir", Camera2);
            SetupCameraBindings("Cam3LookDir", Camera3);

            ResetCameraPosition();
        }
        /// <summary>
        /// 重設相機觀向位置
        /// </summary>
        internal static void ResetCameraPosition()
        {

            //ModelGroup裡面有模型之後才調整相機位置
            if (BoneModelCollection == null || BoneModelCollection.Count == 0) 
                return;

            //重新調整模型中心
            Model3DGroup modelGroup = new Model3DGroup();
            for (int i = 0; i < BoneModelCollection.Count; i++) 
            {
                BoneModel boneModel = BoneModelCollection[i] as BoneModel;
                //如果選擇多模型但檔名是空或不存在則進不去
                if (boneModel.ModelContainer != null)
                {                   
                    modelGroup.Children.Add(boneModel.ModelContainer);
                }
            }


            Rect3D boundingBox = modelGroup.Bounds;

            Point3D modelCenter = new Point3D(boundingBox.X + boundingBox.SizeX / 2.0, boundingBox.Y + boundingBox.SizeY / 2.0, boundingBox.Z + boundingBox.SizeZ / 2.0);
          

            OrthographicCamera orthoCam1 = Camera1 as OrthographicCamera;
            orthoCam1.Position = new Point3D(modelCenter.X, modelCenter.Y - (boundingBox.SizeY), modelCenter.Z);
            orthoCam1.UpDirection = new Vector3D(0, 0, 1);
            orthoCam1.LookDirection = new Vector3D(0, boundingBox.SizeY, 0);
            orthoCam1.NearPlaneDistance = -1000;
            orthoCam1.FarPlaneDistance = 1e15;
            orthoCam1.Width = boundingBox.SizeX + 110;

            OrthographicCamera orthoCam2 = Camera2 as OrthographicCamera;
            orthoCam2.Position = new Point3D(modelCenter.X, modelCenter.Y, modelCenter.Z - (boundingBox.SizeZ));
            orthoCam2.UpDirection = new Vector3D(0, 1, 0);
            orthoCam2.LookDirection = new Vector3D(0, 0, boundingBox.SizeZ);
            orthoCam2.NearPlaneDistance = -1000;
            orthoCam2.FarPlaneDistance = 1e15;
            orthoCam2.Width = boundingBox.SizeX + 110;

            OrthographicCamera orthoCam3 = Camera3 as OrthographicCamera;
            orthoCam3.Position = new Point3D(modelCenter.X - (boundingBox.SizeX), modelCenter.Y, modelCenter.Z);
            orthoCam3.UpDirection = new Vector3D(0, 0, 1);
            orthoCam3.LookDirection = new Vector3D(boundingBox.SizeX, 0, 0);
            orthoCam3.NearPlaneDistance = -1000;
            orthoCam3.FarPlaneDistance = 1e15;
            orthoCam3.Width = boundingBox.SizeX + 110;

        }
        /// <summary>
        /// 將自定義的cam1LookDir綁到相機實際的觀看方向
        /// </summary>
        public void SetupCameraBindings(string propertyName, Camera camera )//Camera1, "Cam1LookDir"
        {

            if (camera is ProjectionCamera)
            {
                SetBinding(propertyName, camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }
        /// <summary>
        /// path:目標屬性名稱
        /// dobj:來源屬性
        /// viewModel
        /// mode:綁定方向
        /// </summary>
        private void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }
        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var item in HighlightItems)
            {
                item.Highlight = false;
            }
            HighlightItems.Clear();
            //Material = PhongMaterials.White;
            var viewport = sender as Viewport3DX;
            if (viewport == null) { return; }
            var point = e.GetPosition(viewport);
            Console.WriteLine("\npoint:  " + point);
            var hitTests = viewport.FindHits(point);
            Console.WriteLine("hit count:  " + hitTests.Count);
            if (hitTests != null && hitTests.Count > 0)
            {
                var hit = hitTests[0];
                if (hit.ModelHit.DataContext is ModelData)
                {
                    System.Windows.Media.Media3D.Point3D TEST = hit.PointHit;

                    var model = hit.ModelHit.DataContext as ModelData;
                    model.Highlight = true;
                    HighlightItems.Add(model);
                }
              
            }
        }

        private void CreatePoint(int index0, int index1, int index2, int index3, Vector3Collection positions, Point3D[] ospPoint)
        {
            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index0].X)
                    , Convert.ToSingle(ospPoint[index0].Y)
                    , Convert.ToSingle(ospPoint[index0].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index1].X)
                    , Convert.ToSingle(ospPoint[index1].Y)
                    , Convert.ToSingle(ospPoint[index1].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index3].X)
                    , Convert.ToSingle(ospPoint[index3].Y)
                    , Convert.ToSingle(ospPoint[index3].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index1].X)
                    , Convert.ToSingle(ospPoint[index1].Y)
                    , Convert.ToSingle(ospPoint[index1].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index2].X)
                    , Convert.ToSingle(ospPoint[index2].Y)
                    , Convert.ToSingle(ospPoint[index2].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index3].X)
                    , Convert.ToSingle(ospPoint[index3].Y)
                    , Convert.ToSingle(ospPoint[index3].Z)));
        }
        private void CreateNormal(int index0, int index1, int index2, Vector3Collection normals, Point3D[] ospPoint)
        {
            Vector3 normal = Vector3.Cross(new Vector3(Convert.ToSingle(ospPoint[index1].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index1].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index1].Z - ospPoint[index0].Z))
                , new Vector3(Convert.ToSingle(ospPoint[index2].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index2].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index2].Z - ospPoint[index0].Z)));


            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }







        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string info = "")
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(null, new PropertyChangedEventArgs(info));
            }
        }
        protected static bool SetStaticValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")//CallerMemberName主要是.net4.5後定義好的caller訊息，能將訊息傳給後者的變數，目的在使用時不用特地傳入"Property"名稱
        {
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            OnStaticPropertyChanged(propertyName);
            return true;
        }

    }
}
