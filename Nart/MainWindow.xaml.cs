using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIS.Imaging;
using UseCVLibrary;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Threading;
using NartControl;
using System.ComponentModel;

namespace Nart
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        internal ModelVisual3D LightModel = new ModelVisual3D();
        
        private Environment _envSetting;

        private CameraControl CamCtrl;
        
        public static List<ModelData> AllModelData =new List<ModelData>(5);

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

        public MainWindow()
        {
            
            InitializeComponent();
            this.DataContext = this;

            AllocConsole();

            displaytest();

            _envSetting = new Environment(this);
           
            CamCtrl = new CameraControl(873, 815, this);

            CamHost1.Child = CamCtrl.icImagingControl[0];
            CamHost2.Child = CamCtrl.icImagingControl[1];
        }

        

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {        
            CamCtrl.CameraStart();            
        }
        
        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Title = "New Project";
            dlg.DefaultExt = ".stl";
            dlg.Multiselect = true;
            dlg.Filter = "STL File (.stl)|*.stl";


            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (String filename in dlg.FileNames)
                {
                    ModelData mData = new ModelData(filename);

                    mainModelVisual.Children.Add(mData.ModelVisual);
                }
                _envSetting.SetCamera();
            }
        }

       
        private void Rotate_Click(object sender, RoutedEventArgs e)
        {


            //var axis = new Vector3D(0, 0, 1);
            //var angle = 10;

            //var matrix = MV2.Transform.Value;
            //matrix.Rotate(new Quaternion(axis, angle));

            //MV2.Transform = new MatrixTransform3D(matrix);

            Matrix3D test = new Matrix3D();
            test.SetIdentity();
            AllModelData[0].ModelVisual.Transform = new MatrixTransform3D(test);
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            string path = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla.stl";
            StLReader stlRed = new HelixToolkit.Wpf.StLReader();
            var model =stlRed.Read(path);

            //foreach (var models in model)
            //{
            //    //var sharpModel = new MeshGeometryModel3D()
            //    //{
            //    //    Geometry = model.Geometry,
            //    //    Material = model.Material,
            //    //    Name = model.Name,
            //    //    Transform = new MatrixTransform3D(model.Transform.ToMatrix3D()),
            //    //};
            //    //loadedModels.Children.Add(sharpModel);
            //}
        }

        private void allRotate_Click(object sender, RoutedEventArgs e)
        {

            //Console.WriteLine("allRotate_Click: " + CamCtrl.count.CurrentCount);
            var axis = new Vector3D(0, 0, 1);
            var angle = 30;

            var matrix = mainModelVisual.Transform.Value;
            
            matrix.Rotate(new Quaternion(axis, angle));

            mainModelVisual.Transform = new MatrixTransform3D(matrix);
        }

        private ModelVisual3D MV1 = new ModelVisual3D();
        private ModelVisual3D MV2 = new ModelVisual3D();
        private ModelData mData1 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl");
        private ModelData mData2 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible digital segment BVRO_0.4.stl");
        private ModelData mData3 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull wo maxilla w ramus BVRO_4.stl");
        private ModelData mData4 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\下顎球1.stl");

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void displaytest()
        {

            mData1.DatabaseIndex = 0;
            mData2.DatabaseIndex = 1;
            mData2.DatabaseIndex = 2;

            AllModelData.Add(mData1);
            AllModelData.Add(mData2);
            AllModelData.Add(mData3);

            mainModelVisual.Children.Add(mData1.ModelVisual);
            mainModelVisual.Children.Add(mData2.ModelVisual);
            mainModelVisual.Children.Add(mData3.ModelVisual);

        }



        private void NewCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


        private void RegButton_Click(object sender, RoutedEventArgs e)
        {

            CameraControl.RegToggle = !CameraControl.RegToggle; 
            Console.WriteLine("Width:" + CamHost1.ActualWidth + "   Height:" + CamHost1.ActualHeight);
               
           
        }

        private void load_Closed(object sender, EventArgs e)
        {
            //Console.WriteLine("load_Closed Thread ID:" + Thread.CurrentThread.ManagedThreadId);
            
            CamCtrl.CameraClose();
            
            System.Windows.Application.Current.Shutdown();
            //System.Environment.Exit(System.Environment.ExitCode);

        }

        private void TrackingButton_Click(object sender, RoutedEventArgs e)
        {
            CameraControl.TrackToggle = !CameraControl.TrackToggle;
        }
    }
}
