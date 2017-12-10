﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;


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
    using ProjectionCamera = HelixToolkit.Wpf.SharpDX.ProjectionCamera;

    public class MultiAngleViewModel : ObservableObject
    {                  
        private static string _craniofacialInfo;
        private static string _ballDistance;
        private RenderTechnique _renderTechnique;
        private Vector3 _light1Direction;
        private Vector3 _light2Direction;
        private Vector3 _light3Direction;
        private Vector3D _cam1LookDir;
        private Vector3D _cam2LookDir;
        private Vector3D _cam3LookDir;
        private MultiAngleView _multiview;
        //private readonly IList<BoneModel> HighlightItems = new List<BoneModel>(); //專門放點到的變色物件



        public MultiAngleViewModel(MultiAngleView multiview)
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
            _multiview = multiview;           
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
        /// 顯示三球距離的string
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
            get;
        } = new OrthographicCamera();
        public static Camera Camera2
        {
            get;
        } = new OrthographicCamera();
        public static Camera Camera3
        {
            get;
        } = new OrthographicCamera();
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
                if (_cam1LookDir == value)
                    return;
                _cam1LookDir = value;
                OnPropertyChanged();
                Light1Direction = value.ToVector3();
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
                if (_cam2LookDir == value)
                    return;
                _cam2LookDir = value;
                OnPropertyChanged();
                Light2Direction = value.ToVector3();
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
                if (_cam3LookDir == value)
                    return;
                _cam3LookDir = value;
                OnPropertyChanged();
                Light3Direction = value.ToVector3();
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
        public static void ResetCameraPosition()
        {

            Model3DGroup modelGroup = new Model3DGroup();

            //一般的BoneModel放置的位置，目前暫定放原始位置
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

            //NavigationTargetCollection 放置上下顎的目標位置
            if (NavigationTargetCollection != null && NavigationTargetCollection.Count != 0)
            {
                foreach (Element3D model in NavigationTargetCollection)
                {
                    BoneModel boneModel = model as BoneModel;
                    //如果選擇多模型但檔名是空或不存在則進不去
                    if (boneModel?.ModelContainer != null)
                    {
                        modelGroup.Children.Add(boneModel.ModelContainer);
                    }
                }
            }
            //NormalModelCollection 放置拖拉進來的模型位置
            if (NormalModelCollection != null && NormalModelCollection.Count != 0)
            {
                foreach (Element3D model in NormalModelCollection)
                {
                    BoneModel normalModel = model as BoneModel;
                    //如果選擇多模型但檔名是空或不存在則進不去
                    //if (normalModel != null && normalModel.ModelContainer != null)
                    if (normalModel?.ModelContainer != null)
                    {
                        modelGroup.Children.Add(normalModel.ModelContainer);
                    }
                }
            }

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

            //null或是點到的東西為少於零個
            if (hitTests == null || hitTests.Count <= 0)
                return;

            foreach (var hit in hitTests)
            {
                //不是BoneModel的話換下一個模型
                if (!(hit.ModelHit is BoneModel))
                    continue;

                Point3D center = hit.PointHit;
                Vector3D normal = hit.NormalAtHit;
                normal.Normalize();


                BallModel ball = new BallModel
                {
                    BallName = "!!!!!",
                    BallInfo = "!!!!!"
                };


                var b1 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();

                Vector3 center2 = new Vector3(Convert.ToSingle(center.X), Convert.ToSingle(center.Y), Convert.ToSingle(center.Z));
                ball.Center = center2;
                b1.AddSphere(center2, 1.5);
                //b1.AddPipe(,,2,5,4);


                ball.Geometry = b1.ToMeshGeometry3D();


                      


                ball.Material = PhongMaterials.Silver;
                ball.Transform = new MatrixTransform3D();

                MainViewModel.Data.BallCollection.Add(ball);
                break;
            }
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file)?.ToLower();
                    //判斷當前的檔案副檔名是否為stl，不是的話則跳過
                    if (extension != null && !extension.Equals(".stl"))
                        continue;
                    BoneModel model = new BoneModel
                    {
                        FilePath = file,
                        MarkerId = "",
                        DiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187),
                        Transform = new MatrixTransform3D()
                    };
                    model.LoadModel();
                    MultiAngleViewModel.BoneModelCollection.Add(model);
                }
            }
            MultiAngleViewModel.ResetCameraPosition();
        }
        public void OnDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file)?.ToLower();
                    //判斷當前的檔案副檔名是否為stl，不是的話則跳過
                    if (extension != null && !extension.Equals(".stl"))
                        continue;
                    //當有一個是時候直接允許拖"放"
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                    return;
                }
            }
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        public void OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
            {
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file)?.ToLower();
                    if (extension == null || !extension.Equals(".stl"))
                        continue;
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                    return;
                }
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }



        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string info = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(info));
        }
        protected static bool SetStaticValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")//CallerMemberName主要是.net4.5後定義好的caller訊息，能將訊息傳給後者的變數，目的在使用時不用特地傳入"Property"名稱
        {
            if (Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            OnStaticPropertyChanged(propertyName);
            return true;
        }

    }
}
