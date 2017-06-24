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

namespace Nart
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ModelVisual3D LightModel = new ModelVisual3D();
        
        private Environment _envSetting;

        private CameraControl CamCtrl;
        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();

            displaytest();

            _envSetting = new Environment(this);


            CamCtrl = new CameraControl((int)CamHost2.Width, (int)CamHost2.Height);

            CamHost1.Child = CamCtrl.icImagingControl[0];
            CamHost2.Child = CamCtrl.icImagingControl[1];


            

        }

        

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           CamCtrl.CameraStart();
        }
        
        private Model3DGroup Display3d(string model)
        {
            Model3DGroup device = null;
            try
            {
                //Adding a gesture here
                //helixViewport.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //Load the 3D model file
                device = import.Load(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
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
            CamCtrl.mre.Set();
            
            var axis = new Vector3D(0, 0, 1);
            var angle = 10;

            var matrix = MV2.Transform.Value;
            matrix.Rotate(new Quaternion(axis, angle));

            MV2.Transform = new MatrixTransform3D(matrix);

            //Console.WriteLine("執行緒:  " + Thread.CurrentThread.ManagedThreadId);
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            //CamCtrl.count.Signal();
            //Console.WriteLine("TRANSLATE: "+CamCtrl.count.CurrentCount);
            var dev = new Vector3D(0, 10, 0);

            var matrix = MV1.Transform.Value;
            matrix.Translate(dev);
            //matrix.Rotate(new Quaternion(axis, angle));

            MV1.Transform = new MatrixTransform3D(matrix);

            //Model3DGroup triangle = new Model3DGroup();
            //Point3D p0 = new Point3D(0, 0, 0);
            //Point3D p1 = new Point3D(5, 0, 0);
            //Point3D p2 = new Point3D(5, 0, 5);
            //Point3D p3 = new Point3D(0, 0, 5);
            //Point3D p4 = new Point3D(0, 5, 0);
            //Point3D p5 = new Point3D(5, 5, 0);
            //Point3D p6 = new Point3D(5, 5, 5);

            //triangle.Children.Add(CreateTriangleModel(p1, p4, p3));
            //triangle.Children.Add(CreateTriangleModel(p1, p4, p6));
            //triangle.Children.Add(CreateTriangleModel(p3, p1, p6));
            //ModelVisual3D Model = new ModelVisual3D();
            //Model.Content = triangle;
            //this.helixViewport.Children.Add(Model);


           
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
        private ModelData mData1 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\Ramus L-fine.stl");
        private ModelData mData2 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\Ramus R-fine.stl");
        private ModelData mData3 = new ModelData("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\maxilla cut-teeth4-fine-combine.stl");

        private void displaytest() {



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

        
    }
}
