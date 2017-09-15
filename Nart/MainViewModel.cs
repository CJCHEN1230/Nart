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


            CamCtrl = new CameraControl(873, 815, MainWindow);
            MainWindow.CamHost1.Child = CamCtrl.icImagingControl[0];
            MainWindow.CamHost2.Child = CamCtrl.icImagingControl[1];
            CamCtrl.CameraStart();            
        }


        /// <summary>
        /// 顯示設置好的各項模型資訊
        /// </summary>
        public void LoadSettingModel()
        {
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < MainViewModel.ModelInfoCollection.Count; i++) 
            {
                //檢查模型有無load，有換過檔名就變成沒有Load
                if (!MainViewModel.ModelInfoCollection[i].IsLoaded)
                {
                    MainViewModel.ModelInfoCollection[i] = new ModelInfo {
                        ModelFilePath = MainViewModel.ModelInfoCollection[i].ModelFilePath,
                        ModelDiffuseColor = MainViewModel.ModelInfoCollection[i].ModelDiffuseColor,
                        BSPFilePath = MainViewModel.ModelInfoCollection[i].BSPFilePath,
                        BSPDiffuseColor = MainViewModel.ModelInfoCollection[i].BSPDiffuseColor  };
                    MainViewModel.ModelInfoCollection[i].LoadSTL();
                }
            }
            MainWindow.multiAngleView._multiAngleViewModel.ModelInfoCollection = MainViewModel.ModelInfoCollection;            
        }



        private string _pointNumber;
        public string PointNumber
        {
            get { return this._pointNumber; }
            set
            {
                if (this._pointNumber != value)
                {
                    this._pointNumber = value;
                    this.NotifyPropertyChanged("PointNumber");
                }
            }
        }

        //public static Model3DGroup _model3dgroup = new Model3DGroup();

        //public static List<ModelData> AllModelData = new List<ModelData>(5);
        //public Element3DCollection ModelGeometry { get; private set; }

        //public GroupModel3D GroupModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
