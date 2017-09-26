using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using NartControl;
using System.Collections.ObjectModel;

namespace Nart
{
    /// <summary>
    /// 此類別用來記錄各種MainView的屬性
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        public CameraControl CamCtrl
        {
            get;
            set;
        }
        public NartServer nartserver
        {
            get;
            set;
        } = new NartServer();
        public static List<ModelInfo> AllModelData2 = new List<ModelInfo>(5);
        public static List<MeshGeometryModel3D> AllModelData3 = new List<MeshGeometryModel3D>(5);

        public static ObservableCollection<ModelInfo> ModelInfoCollection = new ObservableCollection<ModelInfo>();

        private MainView _window;
        public MainView MainWindow
        {
            get { return this._window; }
            set { this._window = value; }
        }
        public MainViewModel(MainView window)
        {
            MainWindow = window;



            CamCtrl = new CameraControl(873, 815, this);
            MainWindow.CamHost1.Child = CamCtrl.icImagingControl[0];
            MainWindow.CamHost2.Child = CamCtrl.icImagingControl[1];
            CamCtrl.CameraStart();

           






            //ObservableCollection < ModelInfo > temp = new ObservableCollection<ModelInfo>();
            //ModelInfo TEST = new ModelInfo
            //{
            //    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
            //                                                                    ,
            //    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
            //                                                                    ,
            //    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
            //                                                                    ,
            //    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
            //};
            //TEST.LoadSTL();
            //temp.Add(TEST);
            //MainWindow.multiAngleView._multiAngleViewModel.ModelInfoCollection = temp;





        }


        /// <summary>
        /// 顯示設置好的各項模型資訊，按下Set Model 之後並且按ok後會走到這
        /// </summary>
        public void LoadSettingModel()
        {
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < MainViewModel.ModelInfoCollection.Count; i++) 
            {
                //檢查模型有無load，有換過檔名就變成沒有Load
                if (!MainViewModel.ModelInfoCollection[i].IsLoaded)
                {
                    //不知道為什麼整個ModelInfo一定要重建，理論上應該只要LoatSTL有走過就好
                    MainViewModel.ModelInfoCollection[i] = new ModelInfo
                    {
                        CMode = MainViewModel.ModelInfoCollection[i].CMode,
                        ModelFilePath = MainViewModel.ModelInfoCollection[i].ModelFilePath,
                        ModelDiffuseColor = MainViewModel.ModelInfoCollection[i].ModelDiffuseColor,
                        OSPFilePath = MainViewModel.ModelInfoCollection[i].OSPFilePath,
                        OSPDiffuseColor = MainViewModel.ModelInfoCollection[i].OSPDiffuseColor
                    };
                    MainViewModel.ModelInfoCollection[i].LoadSTL();
                }
            }
            MainWindow.multiAngleView._multiAngleViewModel.ModelInfoCollection = MainViewModel.ModelInfoCollection;

        }


        private int _tabIndex = 1; //預設tab頁面索引值
        public int TabIndex
        {
            get { return this._tabIndex; }
            set
            {
                SetValue(ref _tabIndex, value);
            }
        }




        private string _pointNumber;
        public string PointNumber
        {
            get { return this._pointNumber; }
            set
            {
                SetValue(ref _pointNumber, value);
            }
        }







    }
}
