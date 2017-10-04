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
        public static ObservableCollection<ModelInfo> ModelInfoCollection;
        public static ObservableCollection<ModelData> ModelDataCollection;
        private MainView _window;
        public MainView MainWindow
        {
            get { return this._window; }
            set { this._window = value; }
        }
        public MainViewModel(MainView window)
        {
            MainWindow = window;
        }
        /// <summary>
        /// 顯示設置好的各項模型資訊，按下Set Model 之後並且按ok後會走到這
        /// </summary>       
        public void InitCamCtrl()
        {

            CamCtrl = new CameraControl(new TIS.Imaging.ICImagingControl[2] { _window.CamHost1.icImagingControl, _window.CamHost2.icImagingControl }, this);
            CamCtrl.CameraStart();
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
