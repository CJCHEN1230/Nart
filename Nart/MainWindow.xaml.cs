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
           // CamCtrl.CameraStart();
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
                ModelData mData3 = new ModelData(dlg.FileNames[0]);


                mainModelVisual.Children.Add(mData3.ModelVisual);

                _envSetting.SetCamera();
            }
        }

       
        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            var axis = new Vector3D(0, 0, 1);
            var angle = 10;

            var matrix = MV2.Transform.Value;
            matrix.Rotate(new Quaternion(axis, angle));

            MV2.Transform = new MatrixTransform3D(matrix);
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
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

            //Rect3D rect3d = mainModelVisual.FindBounds(mainModelVisual.Transform);
            //var dev1 = new Vector3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);
            //dev1 = dev1 * -1;

            //var matrix1 = mainModelVisual.Transform.Value;
            
            //matrix1.Translate(dev1);

            //mainModelVisual.Transform = new MatrixTransform3D(matrix1);



            //Model3DGroup model1 = Display3d("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\Ramus L-fine.stl");


            //Model3DGroup model2 = Display3d("D:\\Desktop\\研究資料\\蔡慧君完整頭顱模型\\Ramus R-fine.stl");


            //Rect3D rect3d1 = model1.Bounds;
            //Rect3D rect3d2 = model2.Bounds;

            //GeometryModel3D Geomodel = model1.Children[0] as GeometryModel3D;

            //DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(40, 181, 187)));
            //Geomodel.Material = material;



            //var dev1 = new Vector3D(rect3d1.X + rect3d1.SizeX / 2.0, rect3d1.Y + rect3d1.SizeY / 2.0, rect3d1.Z + rect3d1.SizeZ / 2.0);
            //dev1 = dev1 * -1;


            //var dev2 = new Vector3D(rect3d2.X + rect3d2.SizeX / 2.0, rect3d2.Y + rect3d2.SizeY / 2.0, rect3d2.Z + rect3d2.SizeZ / 2.0);
            //dev2 = dev2 * -1;


            //var matrix1 = MV1.Transform.Value;
            //var matrix2 = MV2.Transform.Value;

            //matrix1.Translate(dev1);
            //matrix2.Translate(dev2);


            //MV1.Content = model1;
            //MV1.Transform = new MatrixTransform3D(matrix1);

            //MV2.Content = model2;
            //MV2.Transform = new MatrixTransform3D(matrix2);


            //mainModelVisual.Children.Add(MV1);
            //mainModelVisual.Children.Add(MV2);
            //helixViewport.Children.Add(MV1);
            //helixViewport.Children.Add(MV2);

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
