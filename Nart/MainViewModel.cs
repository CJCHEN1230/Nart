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
using System.Windows.Input;

namespace Nart
{
    /// <summary>
    /// 此類別用來記錄各種MainView的屬性
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        public static ObservableCollection<ModelSettingItem> ModelSettingCollection;
        public static ObservableCollection<ModelData> ModelDataCollection;
        public CameraControl CamCtrl 
        {
            get;
            set;
        }
        public NartServer Server= new NartServer();
        private static string _pointNumber;
        private static int _tabIndex = 1; //預設tab頁面索引值        
        private ModelSettingView _modelSettingdlg;
        private MainView _mainWindow;
        
        public MainViewModel(MainView mainWindow)
        {
            _mainWindow = mainWindow;
            SetModelCommand = new RelayCommand(SetModel);

        }
        
        

        public static int TabIndex
        {
            get { return _tabIndex; }
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
        public ICommand SetModelCommand { private set; get; }


        public void InitCamCtrl()
        {

            CamCtrl = new CameraControl(new TIS.Imaging.ICImagingControl[2] { _mainWindow.CamHost1.icImagingControl, _mainWindow.CamHost2.icImagingControl });
            CamCtrl.CameraStart();
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

            _modelSettingdlg.ShowDialog();

            //Dialog 結束之後指派給_multiAngleViewModel中的值
            _mainWindow.multiAngleView._multiAngleViewModel.ModelDataCollection = ModelSettingViewModel.ModelDataCollection;
            MainViewModel.ModelDataCollection = ModelSettingViewModel.ModelDataCollection;
        }
    }
}
