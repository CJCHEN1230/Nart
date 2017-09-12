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
using System.ComponentModel;
using HelixToolkit.Wpf.SharpDX;
using System.Diagnostics;

namespace Nart
{
    /// <summary>
    /// MainView主視窗介面
    /// </summary>
    public partial class MainView : Window , INotifyPropertyChanged
    {
        
        public static List<ModelData> AllModelData = new List<ModelData>(5);

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
        private MainViewModel _mainViewModel = null;
        public MainView()
        {            
            InitializeComponent();

            _mainViewModel = new MainViewModel(this);

            this.DataContext = _mainViewModel;

            AllocConsole();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {        
           // CamCtrl.CameraStart();            
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

                    //mainModelVisual.Children.Add(mData.ModelVisual);
                }

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
            //AllModelData[0].ModelVisual.Transform = new MatrixTransform3D(test);
        }
        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.AllModelData2[0].meshGeometry.PushMatrix(new SharpDX.Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -100, -100, 100, 1));

            Matrix3D  TEST= new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -100, -100, 100, 1);
            MainViewModel.AllModelData2[0].meshGeometry.Transform = new MatrixTransform3D(TEST);
        }

     

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            CameraControl.RegToggle = !CameraControl.RegToggle;             
        }
        private void TrackingButton_Click(object sender, RoutedEventArgs e)
        {
            CameraControl.TrackToggle = !CameraControl.TrackToggle;
        }
        private void load_Closed(object sender, EventArgs e)
        {            
            //CamCtrl.CameraClose();            
            System.Windows.Application.Current.Shutdown();            
        }
        public event PropertyChangedEventHandler PropertyChanged;//對PropertyChangedEventHandler的封裝
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        private void NewCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

    }
}
