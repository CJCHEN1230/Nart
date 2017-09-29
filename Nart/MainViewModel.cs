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
using NartControl.Control;

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

        //CameraWrapper temp = new CameraWrapper();

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



            //CamCtrl = new CameraControl(878, 764, this);
            //MainWindow.CamHost1.Child = CamCtrl.icImagingControl[0];
            //MainWindow.CamHost2.Child = CamCtrl.icImagingControl[1];
            //CamCtrl.CameraStart();
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
                        ComboBoxList = MainViewModel.ModelInfoCollection[i].ComboBoxList,
                        MarkerID = MainViewModel.ModelInfoCollection[i].MarkerID,                        
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

        public void InitCamCtrl()
        {

            CamCtrl = new CameraControl(new TIS.Imaging.ICImagingControl[2] { _window.CamHost1.icImagingControl, _window.CamHost2.icImagingControl }, this);
            CamCtrl.CameraStart();
        }
        private int _tabIndex = 0; //預設tab頁面索引值
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
