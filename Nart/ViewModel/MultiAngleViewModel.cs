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
    using Nart.Model_Object;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
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
        private static string _craniofacialInfo;
        private static string _ballDistance;
        private static Camera _cam1 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private static Camera _cam2 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private static Camera _cam3 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
        private RenderTechnique _renderTechnique;
        private Vector3 _light1Direction = new Vector3();
        private Vector3 _light2Direction = new Vector3();
        private Vector3 _light3Direction = new Vector3();
        private Vector3D _cam1LookDir = new Vector3D();
        private Vector3D _cam2LookDir = new Vector3D();
        private Vector3D _cam3LookDir = new Vector3D();      
        private MultiAngleView _multiview;
        //private readonly IList<BoneModel> HighlightItems = new List<BoneModel>(); //專門放點到的變色物件



        public MultiAngleViewModel(MultiAngleView multiview)
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this._multiview = multiview;           
            SetLight();
            SetCamera();

            
            Binding binding = new Binding("BallCollection");
            binding.Source = MainViewModel.Data;
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(multiview.BallCollection, ItemsModel3D.ItemsSourceProperty, binding);
        }

        /// <summary>
        /// 五項指標資訊
        /// </summary>
        public static string CraniofacialInfo
        {
            get
            {
                return _craniofacialInfo;
            }
            set
            {
                SetStaticValue(ref _craniofacialInfo, value);
            }
        }
        /// <summary>
        /// 計算三球距離
        /// </summary>
        public static string BallDistance
        {
            get
            {
                return _ballDistance;
            }
            set
            {
                SetStaticValue(ref _ballDistance, value);
            }
        }
        public static Camera Camera1
        {
            get
            {
                return _cam1;
            }

            private set
            {
                SetStaticValue(ref _cam1, value);
            }
        }
        public static Camera Camera2
        {
            get
            {
                return _cam2;
            }

            private set
            {
                SetStaticValue(ref _cam2, value);
            }
        }
        public static Camera Camera3
        {
            get
            {
                return _cam3;
            }

            private set
            {
                SetStaticValue(ref _cam3, value);
            }
        }
        public Vector3 Light1Direction
        {
            get
            {
                return _light1Direction;
            }
            set
            {
                SetValue(ref _light1Direction, value);
            }
        }
        public Vector3 Light2Direction
        {
            get
            {
                return _light2Direction;
            }
            set
            {
                SetValue(ref _light2Direction, value);
            }
        }
        public Vector3 Light3Direction
        {
            get
            {
                return _light3Direction;
            }
            set
            {
                SetValue(ref _light3Direction, value);
            }
        }
        public Vector3D Cam1LookDir
        {
            set
            {
                if (_cam1LookDir != value)
                {
                    _cam1LookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value.ToVector3();
                }
            }
            get
            {
                return _cam1LookDir;
            }
        }
        public Vector3D Cam2LookDir
        {
            set
            {
                if (_cam2LookDir != value)
                {
                    _cam2LookDir = value;
                    OnPropertyChanged();
                    Light2Direction = value.ToVector3();
                }
            }
            get
            {
                return _cam2LookDir;
            }
        }
        public Vector3D Cam3LookDir
        {
            set
            {
                if (_cam3LookDir != value)
                {
                    _cam3LookDir = value;
                    OnPropertyChanged();
                    Light3Direction = value.ToVector3();
                }
            }
            get
            {
                return _cam3LookDir;
            }
        }
        public Color4 DirectionalLightColor { get; set; }
        public RenderTechnique RenderTechnique
        {
            get
            {
                return _renderTechnique;
            }
            set
            {
                SetValue(ref _renderTechnique, value);
            }
        }
        public IEffectsManager EffectsManager { get; protected set; }
        public IRenderTechniquesManager RenderTechniquesManager { get; protected set; }
        public static ObservableCollection<Element3D> BoneModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        public static ObservableCollection<Element3D> OspModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        public static ObservableCollection<Element3D> TriangleModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        public static ObservableCollection<Element3D> NavigationTargetCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        public static ObservableCollection<Element3D> NormalModelCollection
        {
            get;
            set;
        } = new ObservableCollection<Element3D>();
        /// <summary>
        /// 設定光源Ambientlight顏色、DirectionaLlight顏色        
        /// </summary>
        internal void SetLight()
        {         
            DirectionalLightColor = Color.White;
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

            Model3DGroup modelGroup = new Model3DGroup();

            //ModelGroup裡面有模型之後才調整相機位置
            if (BoneModelCollection != null && BoneModelCollection.Count != 0)
            {
                //重新調整模型中心                
                foreach (Element3D model in BoneModelCollection)
                {
                    BoneModel boneModel = model as BoneModel;
                    //如果選擇多模型但檔名是空或不存在則進不去
                    if (boneModel?.ModelContainer != null)
                    {
                        modelGroup.Children.Add(boneModel.ModelContainer);
                    }
                }
            }

            if (NavigationTargetCollection != null && NavigationTargetCollection.Count != 0)
            {
                for (int i = 0; i < NavigationTargetCollection.Count; i++)
                {
                    BoneModel boneModel = NavigationTargetCollection[i] as BoneModel;
                    //如果選擇多模型但檔名是空或不存在則進不去
                    if (boneModel?.ModelContainer != null)
                    {
                        modelGroup.Children.Add(boneModel.ModelContainer);
                    }
                }
            }

            //if (NormalModelCollection != null && NormalModelCollection.Count != 0)
            //{
            //    for (int i = 0; i < NormalModelCollection.Count; i++)
            //    {
            //        BoneModel normalModel = NormalModelCollection[i] as BoneModel;
            //        //如果選擇多模型但檔名是空或不存在則進不去
            //        if (normalModel != null && normalModel.ModelContainer != null)
            //        {
            //            modelGroup.Children.Add(normalModel.ModelContainer);
            //        }
            //    }
            //}

            if (modelGroup.Children.Count == 0)
                return;


            Rect3D boundingBox = modelGroup.Bounds;

            Point3D modelCenter = new Point3D(boundingBox.X + boundingBox.SizeX / 2.0, boundingBox.Y + boundingBox.SizeY / 2.0, boundingBox.Z + boundingBox.SizeZ / 2.0);
          
            OrthographicCamera orthoCam1 = Camera1 as OrthographicCamera;
            if (orthoCam1 != null)
            {
                orthoCam1.Position = new Point3D(modelCenter.X, modelCenter.Y - (boundingBox.SizeY), modelCenter.Z);
                orthoCam1.UpDirection = new Vector3D(0, 0, 1);
                orthoCam1.LookDirection = new Vector3D(0, boundingBox.SizeY, 0);
                orthoCam1.NearPlaneDistance = -1000;
                orthoCam1.FarPlaneDistance = 1e15;
                orthoCam1.Width = boundingBox.SizeX + 110;
            }

            OrthographicCamera orthoCam2 = Camera2 as OrthographicCamera;
            if (orthoCam2 != null)
            {
                orthoCam2.Position = new Point3D(modelCenter.X, modelCenter.Y, modelCenter.Z - (boundingBox.SizeZ));
                orthoCam2.UpDirection = new Vector3D(0, 1, 0);
                orthoCam2.LookDirection = new Vector3D(0, 0, boundingBox.SizeZ);
                orthoCam2.NearPlaneDistance = -1000;
                orthoCam2.FarPlaneDistance = 1e15;
                orthoCam2.Width = boundingBox.SizeX + 110;
            }

            OrthographicCamera orthoCam3 = Camera3 as OrthographicCamera;
            if (orthoCam3 != null)
            {
                orthoCam3.Position = new Point3D(modelCenter.X - (boundingBox.SizeX), modelCenter.Y, modelCenter.Z);
                orthoCam3.UpDirection = new Vector3D(0, 0, 1);
                orthoCam3.LookDirection = new Vector3D(boundingBox.SizeX, 0, 0);
                orthoCam3.NearPlaneDistance = -1000;
                orthoCam3.FarPlaneDistance = 1e15;
                orthoCam3.Width = boundingBox.SizeX + 110;
            }
        }
        /// <summary>
        /// 將自定義的camLookDir綁到相機實際的觀看方向
        /// </summary>
        public void SetupCameraBindings(string propertyName, Camera camera )
        {
            if (!(camera is ProjectionCamera))
                return;
            var binding = new Binding(propertyName);
            binding.Source = this;
            binding.Mode = BindingMode.OneWayToSource;
            BindingOperations.SetBinding(camera, ProjectionCamera.LookDirectionProperty, binding);
        }
     
        public void OnMouseDoubleClickHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (!MainViewModel.Data.CanSelectPoints)
                return;

            Viewport3DX viewport = sender as Viewport3DX;
            if (viewport == null)
                return; 
            var point = e.GetPosition(viewport);
            var hitTests = viewport.FindHits(point);

            if (hitTests != null && hitTests.Count > 0)
            {

                foreach (var hit in hitTests)
                {
                    if (hit.ModelHit is BoneModel)
                    {
                        Point3D center = hit.PointHit;


                        BallModel ball = new BallModel
                        {
                            BallName = "!!!!!",
                            BallInfo = "!!!!!"
                        };


                        var b1 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();

                        Vector3 center2 = new Vector3(Convert.ToSingle(center.X), Convert.ToSingle(center.Y), Convert.ToSingle(center.Z));
                        ball.Center = center2;
                        b1.AddSphere(center2, 2);
                        ball.Geometry = b1.ToMeshGeometry3D();


                        HelixToolkit.Wpf.SharpDX.PhongMaterial material = new PhongMaterial();

                        material.ReflectiveColor = SharpDX.Color.Black;
                        float ambient = 0.0f;
                        material.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                        material.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                        int specular = 90;
                        material.SpecularColor = new SharpDX.Color(specular, specular, specular, 255);
                        material.SpecularShininess = 60;
                        material.DiffuseColor = new Color4();






                        ball.Material = PhongMaterials.Silver;
                        ball.Transform = new System.Windows.Media.Media3D.MatrixTransform3D();

                        MainViewModel.Data.BallCollection.Add(ball);
                        break;
                    }

                }


            }

        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (!extension.Equals(".stl")) continue;
                    BoneModel model = new BoneModel
                    {
                        FilePath = files[i],
                        MarkerId = "",
                        DiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187),
                        Transform = new System.Windows.Media.Media3D.MatrixTransform3D()
                    };

                    model.LoadModel();

                    MultiAngleViewModel.BoneModelCollection.Add(model);
                }
                MultiAngleViewModel.ResetCameraPosition();
            }
        }
        public void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (extension.Equals(".stl"))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
        public void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (extension.Equals(".stl"))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }



        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string info = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(info));
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
