using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Nart.Model_Object;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Soap;
using Nart.Experiment;
using Microsoft.Win32;

namespace Nart
{
    /// <summary>
    /// 此類別用來記錄各種MainView的屬性
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 專案資料
        /// </summary>
        public static ProjectData ProjData/* = new ProjectData()*/;
        /// <summary>
        /// 主要相機顯示流程及計算
        /// </summary>
        public CameraControl CamCtrl;
        /// <summary>
        /// 原始MainView
        /// </summary>
        public  MainView _mainWindow;
        /// <summary>
        /// 顯示點的數量，顯示在StatusBar
        /// </summary>
        private static string _pointNumber;
        /// <summary>
        /// tab頁面索引值    
        /// </summary>
        private static int _tabIndex = 1; 
        /// <summary>
        /// 原始Nart的設置頁面
        /// </summary>
        private ModelSettingView _modelSettingdlg;
        /// <summary>
        /// 術中導航的設置頁面
        /// </summary>
        private NavigateView _navigatedlg;
        /// <summary>
        /// 控制旋轉平台及XYZ平台的頁面
        /// </summary>
        private CtrlRotPlatform _ctrlRotPlatform;
        /// <summary>
        /// 起始右側三視角畫面的真實寬度
        /// </summary>
        private double ModelViewWidth;
        private NartServer Server/*= new NartServer()*/;
        static MainViewModel()
        {
            ProjData = new ProjectData();
            
        }
        public MainViewModel(MainView mainWindow)
        {
            _mainWindow = mainWindow;

            _mainWindow.RegBtn.IsEnabled = false;
            _mainWindow.TrackBtn.IsEnabled = false;
            _mainWindow.Stage1Btn.Visibility = Visibility.Hidden;
            _mainWindow.Stage2Btn.Visibility = Visibility.Hidden;
            _mainWindow.FinishBtn.Visibility = Visibility.Hidden;

            LoadMarkerDatabaseCommand = new RelayCommand(LoadMarkerData);
            SetModelCommand = new RelayCommand(SetModel);
            RegisterCommand = new RelayCommand(Register);
            SetNavigationCommand = new RelayCommand(SetNavigation);
            CtrlRotPlatformCommand = new RelayCommand(OpenRotPlatform);
            TrackCommand = new RelayCommand(Track);
            CloseWindowCommand = new RelayCommand(OnClosed, null);
            DeleteBallCommad = new RelayCommand(DeleteBallItem);
            DeleteBoneCommad = new RelayCommand(DeleteBoneItem);
            SaveProjectCommand = new RelayCommand(SaveProject);
            LoadProjectCommand = new RelayCommand(LoadProject);
            FlyInSettingCommand = new RelayCommand(FlyInSettingView);
            FlyOutSettingCommand = new RelayCommand(FlyOutSettingView);
            Stage1Command = new RelayCommand(Stage1);
            Stage2Command = new RelayCommand(Stage2);
            FinishCommand = new RelayCommand(Finish);
            ResetCommand = new RelayCommand(Reset);
            ShowCoordinateCommand = new RelayCommand(ShowCoordinate);
            ShowCubeCommand = new RelayCommand(ShowCube);
            ResetInterfaceCommand = new RelayCommand(ResetInterface);
            GridLoadedCommand = new RelayCommand(GridLoaded);

            BindPatientData();
            BindBallData();
            BindBoneData();
            BindProgramState();
        }                
        public static int TabIndex
        {
            get
            {
                return _tabIndex;
            }
            set
            {
                SetStaticValue(ref _tabIndex, value);
            }
        }
        public static string PointNumber
        {
            get { return _pointNumber; }
            set
            {
                SetStaticValue(ref _pointNumber, value);
            }
        }
        public ICommand SaveProjectCommand { private set; get; }
        public ICommand LoadProjectCommand { private set; get; }
        public ICommand SetModelCommand { private set; get; }
        public ICommand LoadMarkerDatabaseCommand { private set; get; }
        public ICommand SetNavigationCommand { private set; get; }
        /// <summary>
        /// 註冊按鈕
        /// </summary>
        public ICommand RegisterCommand { private set; get; }
        /// <summary>
        /// 追蹤按鈕
        /// </summary>
        public ICommand TrackCommand { private set; get; }
        /// <summary>
        /// 關閉程式
        /// </summary>
        public ICommand CloseWindowCommand { private set; get; }
        /// <summary>
        /// 刪除球Item的Command        
        /// </summary>
        public ICommand DeleteBallCommad { private set; get; }
        /// <summary>
        /// 刪除骨骼模型的Command        
        /// </summary>
        public ICommand DeleteBoneCommad { private set; get; }
        /// <summary>
        /// 飛入設定頁面        
        /// </summary>
        public ICommand FlyInSettingCommand { private set; get; }
        /// <summary>
        /// 飛出設定頁面        
        /// </summary>
        public ICommand FlyOutSettingCommand { private set; get; }
        /// <summary>
        /// 開啟控制旋轉平台的Command        
        /// </summary>
        public ICommand CtrlRotPlatformCommand { private set; get; }
        /// <summary>
        /// 這個階段主要要顯示出所設定的第一階段的上or下顎，且顯示三角形模型
        /// </summary>
        public ICommand Stage1Command { private set; get; }
        /// <summary>
        /// 這個階段主要要顯示出所設定的第二階段的上or下顎，且顯示三角形模型
        /// </summary>
        public ICommand Stage2Command { private set; get; }
        public ICommand FinishCommand { private set; get; }
        /// <summary>
        /// 重設相機視角
        /// </summary>
        public ICommand ResetCommand { private set; get; }
        public ICommand ShowCoordinateCommand { private set; get; }
        public ICommand ShowCubeCommand { private set; get; }
        public ICommand ResetInterfaceCommand { private set; get; }
        public ICommand GridLoadedCommand { private set; get; }
        public void InitCamCtrl()
        {

            CamCtrl = new CameraControl(new TIS.Imaging.ICImagingControl[2] { _mainWindow.CamHost1.IcImagingControl, _mainWindow.CamHost2.IcImagingControl });
            CamCtrl.CameraStart();
        }
        /// <summary>
        /// 開啟儲存的專案        
        /// </summary>
        public void ImportProject(string filename)
        {
            string fullFilePath = filename;

            switch (System.IO.Path.GetExtension(fullFilePath).ToLower())
            {

                case ".nart":
                    {

                        string projectName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);//檔名不包含副檔名
                        string filePath = System.IO.Path.GetDirectoryName(fullFilePath);//路徑
                        string tempDirectory = System.IO.Path.Combine(filePath, projectName);//路徑+檔名(不包含副檔名)

                        Directory.CreateDirectory(tempDirectory);
                        ZipFile.ExtractToDirectory(fullFilePath, tempDirectory);

                        string xmlFilePath = System.IO.Path.Combine(tempDirectory, projectName + ".xml");
                        if (!File.Exists(xmlFilePath))
                        {
                            return;
                        }

                        ProjectData projectData;
                        using (FileStream myFileStream = new FileStream(xmlFilePath, FileMode.Open))
                        {
                            SoapFormatter soapFormatter = new SoapFormatter();
                            projectData = (ProjectData)soapFormatter.Deserialize(myFileStream);
                            myFileStream.Close();
                        }


                        foreach (BoneModel boneModel in projectData.BoneCollection)
                        {
                            //BoneModel boneModel = projectData.BoneCollection[i];
                            boneModel.FilePath = System.IO.Path.Combine(tempDirectory, boneModel.SafeFileName);
                            boneModel.LoadModel();
                            if (boneModel.ModelType == ModelType.MovedMaxilla)
                            {
                                foreach (BoneModel targetModel in projectData.TargetCollection)
                                {
                                    if (targetModel.ModelType == ModelType.TargetMaxilla)
                                    {
                                        targetModel.FilePath = boneModel.FilePath;
                                        targetModel.LoadModel();
                                    }
                                }
                            }
                            else if (boneModel.ModelType == ModelType.MovedMandible)
                            {
                                foreach (BoneModel targetModel in projectData.TargetCollection)
                                {
                                    if (targetModel.ModelType == ModelType.TargetMandible)
                                    {
                                        targetModel.FilePath = boneModel.FilePath;
                                        targetModel.LoadModel();
                                    }
                                }
                            }
                        }



                        MainViewModel.ProjData.UpdateData(projectData);
                        MultiAngleViewModel.ResetCameraPosition();
                        System.IO.Directory.Delete(tempDirectory, true);

                        if (MainViewModel.ProjData.IsNavDone)
                        {
                            ShowFinalInfo();
                        }

                        break;
                    }

            }
        }
        private void LoadMarkerData(object o)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Marker Database" ,
                DefaultExt = ".xml",
                Multiselect = false,
                Filter = "XML File (.xml)|*.xml"
         
            };
            
            if (true == dlg.ShowDialog())
            {
                //MarkerDatabase database = MainViewModel.Data.database;
                ////將路徑存到ProjectData 裡面的 MarkerDatabase當中
                //database.Filepath =dlg.FileName;
                //database.LoadDatabase();
            }
            RestoreGridLength();
        }
        /// <summary>
        /// 顯示設置好的各項模型資訊，按下Set Model 之後並且按ok後會走到這
        /// </summary>       
        private void SetModel(object o)
        {
            if (_modelSettingdlg == null)
            {
                _modelSettingdlg = new ModelSettingView();
            }
            _modelSettingdlg.Owner = _mainWindow;

            _modelSettingdlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            _modelSettingdlg.ShowDialog();

            RestoreGridLength();
        }
        private void SetNavigation(object o)
        {
            if (_navigatedlg == null)
            {
                _navigatedlg = new NavigateView();
            }
            _navigatedlg.Owner = _mainWindow;

            _navigatedlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            _navigatedlg.ShowDialog();

            RestoreGridLength();
        }
        private void OpenRotPlatform(object o)
        {
            if (_ctrlRotPlatform == null)
            {
                _ctrlRotPlatform = new CtrlRotPlatform();
            }
            _ctrlRotPlatform.Owner = _mainWindow;

            _ctrlRotPlatform.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            _ctrlRotPlatform.Show();

            RestoreGridLength();
        }
        private void Register(object o)
        {
            SystemData.RegToggle = !SystemData.RegToggle;
            RestoreGridLength();
        }
        private void Track(object o)
        {
            SystemData.TrackToggle = !SystemData.TrackToggle;

            _mainWindow.Stage1Btn.Visibility = Visibility.Visible;


            AmplifyModelView();


        }
        private void OnClosed(object o)
        {
            if (CamCtrl!=null)
            {
                CamCtrl.CameraClose();
            }
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// DeleteBallCommad 的實作內容，刪除球項目
        /// </summary>
        private void DeleteBallItem(object o)
        {
            ListView ballListView =  _mainWindow.BallListView;

            if (ballListView.SelectedItem != null)
            {
                //選擇的BallModel
                BallModel selectedModelItem = (BallModel)ballListView.SelectedItem;

                int temp = ballListView.SelectedIndex;

                ObservableCollection<BallModel> ballCollection = MainViewModel.ProjData.BallCollection;

                ballCollection.Remove(selectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ballCollection.Count == temp)
                {
                    ballListView.SelectedIndex = ballCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    ballListView.SelectedIndex = temp;
                }

                ListViewItem item = ballListView.ItemContainerGenerator.ContainerFromIndex(ballListView.SelectedIndex) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }

            }
        }
        /// <summary>
        /// DeleteBoneCommad 的實作內容，刪除Bone模型項目
        /// </summary>
        private void DeleteBoneItem(object o)
        {
            ListView boneListView = _mainWindow.BoneListView;

            if (boneListView.SelectedItem != null)
            {
                //選擇的BoneModel
                BoneModel selectedModelItem = (BoneModel)boneListView.SelectedItem;

                int temp = boneListView.SelectedIndex;

                var ballCollection = MainViewModel.ProjData.BoneCollection;

                ballCollection.Remove(selectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ballCollection.Count == temp)
                {
                    boneListView.SelectedIndex = ballCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    boneListView.SelectedIndex = temp;
                }
                ListViewItem item = boneListView.ItemContainerGenerator.ContainerFromIndex(boneListView.SelectedIndex) as ListViewItem;
                item?.Focus();
            }
        }
        private void SaveProject(object o)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Nart_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".nart",
                DefaultExt = ".nart",
                Filter = "Nart Project Files (.nart)|*.nart"
            };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                // 建立暫存目錄
                string fullFilePath = dlg.FileName;//完整路徑+副檔名
                if (File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                string projectName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);//檔名不包含副檔名
                string filePath = System.IO.Path.GetDirectoryName(fullFilePath);//路徑
                string tempDirectory = System.IO.Path.Combine(filePath, projectName);//路徑+檔名(不包含副檔名)

                if (File.Exists(filePath))
                {
                    System.IO.Directory.Delete(filePath);
                }

                //先創建一個資料夾
                if (System.IO.Directory.Exists(tempDirectory) == true)
                {
                    System.IO.Directory.Delete(tempDirectory);
                }
                else
                {
                    System.IO.Directory.CreateDirectory(tempDirectory);
                }

                // 專案檔輸出                             
                string xmlFilePah = System.IO.Path.Combine(tempDirectory, projectName) + ".xml";

                using (FileStream myFileStream = new FileStream(xmlFilePah, FileMode.Create))
                {
                    SoapFormatter soapFormatter = new SoapFormatter();
                    soapFormatter.Serialize(myFileStream, MainViewModel.ProjData);
                    myFileStream.Close();
                }

                foreach (BoneModel boneModel in MainViewModel.ProjData.BoneCollection)
                {
                    boneModel.SaveModel(tempDirectory, false);
                }

                


                ZipFile.CreateFromDirectory(tempDirectory, fullFilePath);
                System.IO.Directory.Delete(tempDirectory, true);
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }
        }
        private void LoadProject(object o)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".nart",
                Multiselect = false,
                Filter = "Nart Project Files (.nart)|*.nart"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                if (System.IO.File.Exists(dlg.FileName) == false)
                    return;

                if (System.IO.Path.GetExtension(dlg.FileName).ToLower() != ".nart")
                    return;

                ImportProject(dlg.FileName);
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
        private void FlyInSettingView(object o)
        {
            ColumnDefinition SettingCol = _mainWindow.SettingCol;
            ColumnDefinition InfoCol = _mainWindow.InfoCol;
            ColumnDefinition ModelViewCol = _mainWindow.ModelViewCol;
            var storyboard = new Storyboard();
            //先把ModelViewCol設定成固定值
            ModelViewCol.Width = new GridLength(ModelViewCol.ActualWidth, GridUnitType.Pixel);

            //設定InfoCol從當前Pixel長度到0Pixel
            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(InfoCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };
            Storyboard.SetTargetProperty(gla2, new PropertyPath(ColumnDefinition.WidthProperty));
            Storyboard.SetTarget(gla2, InfoCol);


            //設定SettingCol從當前Pixel長度到InfoCol動畫前的Pixel長度
            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(SettingCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(InfoCol.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 200),
                    FillBehavior = FillBehavior.HoldEnd,
                };
            Storyboard.SetTargetProperty(gla, new PropertyPath(ColumnDefinition.WidthProperty));
            Storyboard.SetTarget(gla, SettingCol);
            storyboard.Children.Add(gla);
            storyboard.Children.Add(gla2);
            _mainWindow.BeginStoryboard(storyboard);            

        }
        private void FlyOutSettingView(object o)
        {
            ColumnDefinition SettingCol = _mainWindow.SettingCol;
            ColumnDefinition InfoCol = _mainWindow.InfoCol;
            ColumnDefinition ModelViewCol = _mainWindow.ModelViewCol;

            ModelViewCol.Width = new GridLength(ModelViewCol.ActualWidth, GridUnitType.Pixel);

            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(SettingCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            //myanim.Completed += (s, e) =>
            //{
            //    //your completed action here
            //};

            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(InfoCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(SettingCol.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };


            SettingCol.BeginAnimation(ColumnDefinition.WidthProperty, gla);
            InfoCol.BeginAnimation(ColumnDefinition.WidthProperty, gla2);
        }        
        private void Stage1(object o)
        {
            //確定已經設定導航資訊，且已經有按Tracking的情形
            if (!MainViewModel.ProjData.IsNavSet || !SystemData.TrackToggle)
                return;

            string firstNavigation = MainViewModel.ProjData.FirstNavigation;
            foreach (Element3D model in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle dragModel = model as DraggableTriangle;
                if (dragModel == null)
                    return;

                switch (firstNavigation)
                {
                    case "Maxilla":
                        if (dragModel.ModelType == ModelType.TargetMaxillaTriangle ||
                            dragModel.ModelType == ModelType.MovedMaxillaTriangle)
                        {
                            dragModel.IsRendering = true;
                        }
                        else
                        {
                            dragModel.IsRendering = false;
                        }
                        break;
                    case "Mandible":
                        if (dragModel.ModelType == ModelType.TargetMandibleTriangle ||
                            dragModel.ModelType == ModelType.MovedMandibleTriangle)
                        {
                            dragModel.IsRendering = true;
                        }
                        else
                        {
                            dragModel.IsRendering = false;
                        }
                        break;
                }
            }

            var boneCollection = MainViewModel.ProjData.BoneCollection;
            foreach (BoneModel model in boneCollection)
            {
                switch (firstNavigation)
                {
                    //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話不讓他改變位置
                    case "Maxilla":
                        if (model.ModelType == ModelType.MovedMandible)
                        {
                            model.Transform = Transform3D.Identity;
                            model.IsTransformApplied = false;
                        }
                        break;
                    case "Mandible":
                        if (model.ModelType == ModelType.MovedMaxilla)
                        {
                            model.Transform = Transform3D.Identity;
                            model.IsTransformApplied = false;
                        }
                        break;
                }
            }

            var targetCollection = MainViewModel.ProjData.TargetCollection;
            //顯示出所選擇的目標模型
            foreach (BoneModel targetModel in targetCollection)
            {

                if (targetModel == null)
                    return;
                //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話則不顯示
                switch (firstNavigation)
                {
                    case "Maxilla":
                        if (targetModel.ModelType == ModelType.TargetMaxilla)
                        {
                            targetModel.IsRendering = true;
                        }
                        break;
                    case "Mandible":
                        if (targetModel.ModelType == ModelType.TargetMandible)
                        {
                            targetModel.IsRendering = true;
                        }
                        break;
                }
            }
            //第一階段按下
            SystemData.IsFirstStage = true;

            _mainWindow.Stage1Btn.Visibility = Visibility.Hidden;
            _mainWindow.Stage2Btn.Visibility = Visibility.Visible;
        }        
        private void Stage2(object o)
        {
            //確定已經註冊的情況
            if (!MainViewModel.ProjData.IsNavSet || !SystemData.IsFirstStage || !SystemData.TrackToggle)
                return;
            string firstNavigation = MainViewModel.ProjData.FirstNavigation;

            //因為是第二階段，所以三角導引物是否顯示相反就好
            foreach (Element3D dragModel in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle model = dragModel as DraggableTriangle;

                if (model != null)
                    model.IsRendering = !model.IsRendering;
            }
            var boneCollection = MainViewModel.ProjData.BoneCollection;

            //因為是第二階段，所以模型是否可以更新位置相反就好
            foreach (BoneModel model in boneCollection)
            {
                model.IsTransformApplied = !model.IsTransformApplied;
            }



            foreach (BoneModel targetModel in MainViewModel.ProjData.TargetCollection)
            {
                if (targetModel != null &&
                    (targetModel.ModelType.Equals(ModelType.TargetMandible) || targetModel.ModelType.Equals(ModelType.TargetMaxilla)))
                {
                    targetModel.IsRendering = !targetModel.IsRendering;
                }
            }
            SystemData.IsFirstStage = false;
            SystemData.IsSecondStage = true;

            
            _mainWindow.Stage2Btn.Visibility = Visibility.Hidden;
            _mainWindow.FinishBtn.Visibility = Visibility.Visible;
        }
        private void Finish(object o)
        {
            SystemData.IsFinished = true;

            foreach (BoneModel targetModel in MainViewModel.ProjData.TargetCollection)
            {
                if (targetModel != null &&
                    (targetModel.ModelType.Equals(ModelType.TargetMandible) || targetModel.ModelType.Equals(ModelType.TargetMaxilla)))
                {
                    targetModel.IsRendering = false;
                }
            }

            foreach (Element3D dragModel in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle model = dragModel as DraggableTriangle;

                if (model != null)
                    model.IsRendering = false;
            }


            //如果已在導航結束的階段則直接顯示所有球資訊
            if (SystemData.IsFinished)
            {
                ShowFinalInfo();
            }

            SystemData.RegToggle = false;
            SystemData.TrackToggle = false;
            SystemData.IsFirstStage = false;
            SystemData.IsSecondStage = false;
            ProjData.IsNavDone = true;
            _mainWindow.TrackBtn.IsEnabled = false;

            _mainWindow.FinishBtn.Visibility = Visibility.Hidden;


            ResetLayout();



        }        
        private void Reset(object o)
        {
            MultiAngleViewModel.ResetCameraPosition();
        }
        private void ShowCoordinate(object o)
        {
            MultiAngleViewModel.ShowCoordinate = !MultiAngleViewModel.ShowCoordinate;
        }
        private void ShowCube(object o)
        {
            MultiAngleViewModel.ShowCube = !MultiAngleViewModel.ShowCube;            
        }
        private void ResetInterface(object o)
        {
            ResetLayout();
        }
        private void GridLoaded(object o)
        {
            //介面為了方便將 Setting的View的Margin左邊設定成-500，這邊只是讓他回到原位
            Thickness margin = _mainWindow.buttonList.Margin;
            margin.Left = 0;
            margin.Right = 0;
            _mainWindow.buttonList.Margin = margin;


            ColumnDefinition InfoCol = _mainWindow.InfoCol;
            ColumnDefinition ModelViewCol = _mainWindow.ModelViewCol;
            Grid MainGrid = _mainWindow.MainGrid;

            //將模型頁面的寬度，設置在整個Grid的0.5~1倍
            ModelViewCol.MaxWidth = MainGrid.ActualWidth;
            ModelViewCol.MinWidth = MainGrid.ActualWidth * 1.0 / 2.0;
            //一定要將SettingCol InfoCol的Width都是Star，且ModelViewCol的Width是absolute，GridSplitter才能正常運作         
            InfoCol.Width = new GridLength(InfoCol.ActualWidth, GridUnitType.Star);
            ModelViewCol.Width = new GridLength(ModelViewCol.ActualWidth, GridUnitType.Pixel);
            ModelViewWidth = ModelViewCol.ActualWidth;
        }
        /// <summary>
        /// 將Grid回復到原始狀態 
        /// </summary>
        private void RestoreGridLength()
        {
            

            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.SettingCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.InfoCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(_mainWindow.SettingCol.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            _mainWindow.SettingCol.BeginAnimation(ColumnDefinition.WidthProperty, gla);
            _mainWindow.InfoCol.BeginAnimation(ColumnDefinition.WidthProperty, gla2);          
        }
        /// <summary>
        /// 重新設定整個Layout
        /// </summary>
        private void ResetLayout()
        {
            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.SettingCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.InfoCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(_mainWindow.MainGrid.ActualWidth - ModelViewWidth, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };
            GridLengthAnimation gla3 =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.ModelViewCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(ModelViewWidth, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.Stop,
                };
            gla3.Completed += (s, e) =>
            {
                _mainWindow.ModelViewCol.Width = new GridLength(ModelViewWidth, GridUnitType.Pixel);
            };


            _mainWindow.SettingCol.BeginAnimation(ColumnDefinition.WidthProperty, gla);
            _mainWindow.InfoCol.BeginAnimation(ColumnDefinition.WidthProperty, gla2);
            _mainWindow.ModelViewCol.BeginAnimation(ColumnDefinition.WidthProperty, gla3);

        }
        /// <summary>
        /// 將觀看Model的View放到最大，tracking的時候用
        /// </summary>
        private void AmplifyModelView()
        {


            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.SettingCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.InfoCol.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(1, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            GridLengthAnimation gla3 =
                new GridLengthAnimation
                {
                    From = new GridLength(_mainWindow.ModelViewCol.ActualWidth, GridUnitType.Pixel),
                    //To = new GridLength(_mainWindow.MainGrid.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(5, GridUnitType.Star),
                    Duration = new TimeSpan(0, 0, 0, 0, 200),
                    FillBehavior = FillBehavior.Stop,
                };
            gla3.Completed += (s, e) =>
            {
                _mainWindow.ModelViewCol.Width = new GridLength(_mainWindow.MainGrid.ActualWidth, GridUnitType.Pixel);
            };


            _mainWindow.SettingCol.BeginAnimation(ColumnDefinition.WidthProperty, gla);
            _mainWindow.InfoCol.BeginAnimation(ColumnDefinition.WidthProperty, gla2);
            _mainWindow.ModelViewCol.BeginAnimation(ColumnDefinition.WidthProperty, gla3);

        }        
        /// <summary>
        /// Bind Patient Information expander裡面的textbox 
        /// </summary>
        private void BindPatientData()
        {
            Binding binding1 = new Binding("Name");
            binding1.Source = ProjData;
            binding1.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.NameTB, TextBlock.TextProperty, binding1);

            Binding binding2 = new Binding("ID");
            binding2.Source = ProjData;
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.IDTB, TextBlock.TextProperty, binding2);

            Binding binding3 = new Binding("Institution");
            binding3.Source = ProjData;
            binding3.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.InstitutionTB, TextBlock.TextProperty, binding3);
        }
        /// <summary>
        /// Bind Navigation balls expander裡面的listview跟switch toggle 
        /// </summary>
        private void BindBallData()
        {
            Binding binding = new Binding("BallCollection");
            binding.Source = ProjData;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.BallListView, ItemsControl.ItemsSourceProperty, binding);

            Binding binding2 = new Binding("CanSelectPoints");
            binding2.Source = ProjData;
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.SelectTB, ToggleButton.IsCheckedProperty, binding2);

            Binding binding3 = new Binding("SelectPointState");
            binding3.Source = ProjData;
            binding3.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.stateTB, TextBlock.TextProperty, binding3);

            Binding binding4 = new Binding("BallCollection");
            binding4.Source = ProjData;
            binding4.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(_mainWindow.multiAngleView.BallCollection, ItemsModel3D.ItemsSourceProperty, binding4);


        }
        /// <summary>
        /// Bind bone Model expander裡面的listview 
        /// </summary>
        private void BindBoneData()
        {
            //將data中的BoneCollection綁到此控制項的item上面   
            Binding binding = new Binding("BoneCollection");
            binding.Source = ProjData;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.BoneListView, ItemsControl.ItemsSourceProperty, binding);

            Binding binding2 = new Binding("BoneCollection");
            binding2.Source = ProjData;
            binding2.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(_mainWindow.multiAngleView.BoneCollection, ItemsModel3D.ItemsSourceProperty, binding2);

            Binding binding3 = new Binding("TargetCollection");
            binding3.Source = ProjData;
            binding3.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.TargetModelListView, ItemsControl.ItemsSourceProperty, binding3);

            Binding binding4 = new Binding("TargetCollection");
            binding4.Source = ProjData;
            binding4.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(_mainWindow.multiAngleView.Targetollection, ItemsModel3D.ItemsSourceProperty, binding4);
        }
        /// <summary>
        /// 綁定狀態資訊到Button的IsEnabled狀態
        /// </summary>
        private void BindProgramState()
        {
            //綁定IsNavSet到Registration Button上面，如果設置完成則RegBtn可以點選
            Binding binding = new Binding("IsNavSet");
            binding.Source = ProjData;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.RegBtn, Button.IsEnabledProperty, binding);
            //綁定IsRegistered到Registration Button上面，如果設置完成則RegBtn可以點選
            Binding binding2 = new Binding("IsRegistered");
            binding2.Source = ProjData;
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(_mainWindow.TrackBtn, Button.IsEnabledProperty, binding2);
        }
        private void ShowFinalInfo()
        {
            MultiAngleViewModel.NavBallDistance = "Stage1"
                    + "\nRed:      " + Math.Round(MainViewModel.ProjData.Stage1Red, 2)
                    + "\n" + "Green:  " + Math.Round(MainViewModel.ProjData.Stage1Green, 2)
                    + "\n" + "Blue:     " + Math.Round(MainViewModel.ProjData.Stage1Blue, 2)
                    + "\n\nStage2"
                    + "\nRed:      " + Math.Round(MainViewModel.ProjData.Stage2Red, 2)
                    + "\n" + "Green:  " + Math.Round(MainViewModel.ProjData.Stage2Green, 2)
                    + "\n" + "Blue:     " + Math.Round(MainViewModel.ProjData.Stage2Blue, 2);

            ObservableCollection<BallModel> ballCollection = MainViewModel.ProjData.BallCollection;
            string maxillaBallInfo = "Maxilla";
            string mandibleBallInfo = "\nMandible";
            foreach (BallModel model in ballCollection)
            {
                if (model.ModelType == ModelType.MovedMaxilla)
                {
                    maxillaBallInfo += "\n" + model.BallName.PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.X, 2))).PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.Y, 2))).PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.Z, 2))).PadLeft(10);
                }
                else if (model.ModelType == ModelType.MovedMandible)
                {
                    mandibleBallInfo += "\n" + model.BallName.PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.X, 2))).PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.Y, 2))).PadLeft(10) +
                                        ("" + Math.Abs(Math.Round(model.BallDistance.Z, 2))).PadLeft(10);
                }
            }

            MultiAngleViewModel.BallDistance = maxillaBallInfo + mandibleBallInfo;
            string info = "DA:    " + Math.Round(MainViewModel.ProjData.DA, 4)
                   + "\n\nDD:    " + Math.Round(MainViewModel.ProjData.DD, 3)
                   + "\n\nFDA:  " + Math.Round(MainViewModel.ProjData.FDA, 2)
                   + "\n\nHDA: " + Math.Round(MainViewModel.ProjData.HDA, 2)
                   + "\n\nPDD:  " + Math.Round(MainViewModel.ProjData.PDD, 3);
            MultiAngleViewModel.CraniofacialInfo = info;
        }
        public void CamHost1_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CamHost1");
        }
        public void CamHost2_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CamHost2");
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
