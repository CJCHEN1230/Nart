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
    
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
    using ProjectionCamera = HelixToolkit.Wpf.SharpDX.ProjectionCamera;

    public class MultiAngleViewModel : ObservableObject
    {
        /// <summary>
        /// 將load好的Model3DGroup加進去方便計算BoundingBox，因為setCamera會使用到，但在其他類別Load進模型檔所以設定成static
        /// </summary>

        private RenderTechnique renderTechnique;
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
        private Camera cam1;
        public Camera Camera1
        {
            get
            {
                return cam1;
            }

            private set
            {
                SetValue(ref cam1, value);
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
                SetValue(ref cam2, value);
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
                SetValue(ref cam3, value);
            }
        }
        private Vector3 light1Direction = new Vector3();
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
        private Vector3 light2Direction = new Vector3();
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
        private Vector3 light3Direction = new Vector3();
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
        private Vector3D cam1LookDir = new Vector3D();
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
        private Vector3D cam2LookDir = new Vector3D();
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
        private Vector3D cam3LookDir = new Vector3D();
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
        private Color4 ambientLightColor;
        public Color4 AmbientLightColor
        {
            get
            {
                return ambientLightColor;
            }
            set
            {
                SetValue(ref ambientLightColor, value);
            }
        }
        public bool IsRenderLight { get; set; }
        private ObservableCollection<Nart.ModelInfo> modelInfoCollection;
        public ObservableCollection<Nart.ModelInfo> ModelInfoCollection
        {
            get
            {
                return modelInfoCollection;
            }
            set
            {
                SetValue(ref modelInfoCollection, value);
                ResetCameraPosition();
            }
        }       
        private MultiAngleView _multiview;
        public MultiAngleViewModel(MultiAngleView _multiview)
        {

            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            this._multiview = _multiview;

            SetLight();
            SetCamera();
        }
        /// <summary>
        /// 設定光源Ambientlight顏色、DirectionaLlight顏色        
        /// </summary>
        internal void SetLight()
        {
            AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            AmbientLightColor = Color.White;

            this.LightColor = (Color4)Color.White;
            this.IsRenderLight = true;
        }
        /// <summary>
        /// 初始化相機參數，並綁定相機觀看方向
        /// </summary>
        private void SetCamera()
        {
            Camera1 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
            Camera2 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();
            Camera3 = new HelixToolkit.Wpf.SharpDX.OrthographicCamera();


            SetupCameraBindings(Camera1, "Cam1LookDir");
            SetupCameraBindings(Camera2, "Cam2LookDir");
            SetupCameraBindings(Camera3, "Cam3LookDir");


            ResetCameraPosition();
        }
        /// <summary>
        /// 重設相機觀向位置
        /// </summary>
        internal void ResetCameraPosition()
        {

            //ModelGroup裡面有模型之後才調整相機位置
            if (ModelInfoCollection == null || ModelInfoCollection.Count == 0) 
                return;

            //重新調整模型中心
            Model3DGroup ModelGroup = new Model3DGroup();
            for (int i = 0; i < ModelInfoCollection.Count; i++) 
            {   //如果選擇多模型但檔名是空或不存在則進不去
                if (ModelInfoCollection[i].SingleModel != null)
                {
                    ModelGroup.Children.Add(ModelInfoCollection[i].SingleModel);
                }
            }


            Rect3D BoundingBox = ModelGroup.Bounds;

            Point3D ModelCenter = new Point3D(BoundingBox.X + BoundingBox.SizeX / 2.0, BoundingBox.Y + BoundingBox.SizeY / 2.0, BoundingBox.Z + BoundingBox.SizeZ / 2.0);

            
            OrthographicCamera orthoCam1 = Camera1 as OrthographicCamera;
            orthoCam1.Position = new Point3D(ModelCenter.X, ModelCenter.Y - (BoundingBox.SizeY), ModelCenter.Z);
            orthoCam1.UpDirection = new Vector3D(0, 0, 1);
            orthoCam1.LookDirection = new Vector3D(0, BoundingBox.SizeY, 0);
            orthoCam1.NearPlaneDistance = -1000;
            orthoCam1.FarPlaneDistance = 1e15;
            orthoCam1.Width = BoundingBox.SizeX + 110;

            OrthographicCamera orthoCam2 = Camera2 as OrthographicCamera;
            orthoCam2.Position = new Point3D(ModelCenter.X, ModelCenter.Y, ModelCenter.Z - (BoundingBox.SizeZ));
            orthoCam2.UpDirection = new Vector3D(0, 1, 0);
            orthoCam2.LookDirection = new Vector3D(0, 0, BoundingBox.SizeZ);
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
        /// <summary>
        /// 將自訂義的cam1LookDir綁到相機實際的觀看方向
        /// </summary>
        public void SetupCameraBindings(Camera camera, string propertyName)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding(propertyName, camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }
        /// <summary>
        /// path:目標屬性
        /// dobj:來源屬性
        /// viewModel
        /// mode:綁定方向
        /// </summary>
        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }

    }
}
