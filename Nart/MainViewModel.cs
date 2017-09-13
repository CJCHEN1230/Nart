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
    public class MainViewModel : INotifyPropertyChanged
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

        public static ObservableCollection<ModelInfo> allModelInfo;

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

            //ModelInfo mdata1 = new ModelInfo();
            //mdata1.LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl");
            //MainWindow.multiAngleView.PutModellnView(mdata1.MeshGeometryData);


            //ModelData2 mdata2 = new ModelData2("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl");
            //MainWindow.multiAngleView.AddModel(mdata2.meshGeometry);
            //AllModelData2.Add(mdata2);

            //ModelData2 mdata3 = new ModelData2("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl");
            //MainWindow.multiAngleView.AddModel(mdata3.meshGeometry);
            //AllModelData2.Add(mdata3);


        }
        public void LoadSettingModel()
        {
            //for (int i=0; i< allModelInfo.Count; i++)
            //{
            //    allModelInfo[i].LoadSTL(allModelInfo[i].ModelFilePath);
            //    MainWindow.multiAngleView.PutModellnView(allModelInfo[i].MeshGeometryData);
            //}



            ModelInfo mdata1 = new ModelInfo();
            mdata1.LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl");
            MainWindow.multiAngleView.PutModellnView(mdata1.MeshGeometryData);



            //ModelInfo mdata1 = new ModelInfo("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl");


            //MainWindow.multiAngleView.AddModel(mdata1.meshGeometry);
            //AllModelData2.Add(mdata1);

            //ModelData2 mdata2 = new ModelData2("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl");
            //MainWindow.multiAngleView.AddModel(mdata2.meshGeometry);
            //AllModelData2.Add(mdata2);

            //ModelData2 mdata3 = new ModelData2("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl");
            //MainWindow.multiAngleView.AddModel(mdata3.meshGeometry);
            //AllModelData2.Add(mdata3);


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
