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
        private readonly IList<BoneModel> HighlightItems = new List<BoneModel>(); //專門放點到的變色物件



        public MultiAngleViewModel(MultiAngleView _multiview)
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this._multiview = _multiview;           
            SetLight();
            SetCamera();
        }
        

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
        public static ObservableCollection<Element3D> TriangleModelCollection
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
                if (boneModel != null && boneModel.ModelContainer != null) 
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
        public void OnMouseDoubleClickHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var item in HighlightItems)
            {
                item.Highlight = false;
            }
            HighlightItems.Clear();

            Viewport3DX viewport = sender as Viewport3DX;
            if (viewport == null)
                return; 
            var point = e.GetPosition(viewport);
            var hitTests = viewport.FindHits(point);
            if (hitTests != null && hitTests.Count > 0)
            {
                var hit = hitTests[0];
                if (hit.ModelHit.DataContext == this)
                {
                   
                    if (hit.ModelHit is BoneModel)
                    {
                        BoneModel model = (BoneModel)hit.ModelHit;
                        //(hit.ModelHit as MeshGeometryModel3D).Material = PhongMaterials.Yellow;
                        model.Highlight = true;
                        HighlightItems.Add(model);
                    }
                    
                   
                }

            }

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
