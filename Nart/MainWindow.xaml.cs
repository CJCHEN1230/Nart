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
      

        Model3DGroup model3dgroup = new Model3DGroup();

        internal OrthographicCamera OrthographicCam = new OrthographicCamera();

        internal ModelVisual3D LightModel = new ModelVisual3D();

        private Environment _envSetting;

        private CameraControl CamCtrl;
        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();


            _envSetting = new Environment(this);
            //modelVisual = new ModelVisual3D();
            //modelVisual.Content = Display3d(MODEL_PATH);

            // this.helixViewport.Background = new SolidColorBrush(Colors.Black);


            //helixViewport.Children.Add(modelVisual);

            CamCtrl = new CameraControl((int)CamHost2.Width, (int)CamHost2.Height);

            CamHost1.Child = CamCtrl.icImagingControl[0];
            CamHost2.Child = CamCtrl.icImagingControl[1];

        }

        

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // CamCtrl.CameraStart();
        }

        internal void SetCamera()
        {
            //myOCamera = helixViewport.Camera as OrthographicCamera;

            //myOCamera.LookAt(environmentSetting.BoundingBox_center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 1.0);

            //double dx = environmentSetting.BoundingBox_max.X - environmentSetting.BoundingBox_min.X;
            //double dy = environmentSetting.BoundingBox_max.Y - environmentSetting.BoundingBox_min.Y;
            //double dz = environmentSetting.BoundingBox_max.Z - environmentSetting.BoundingBox_min.Z;

            //myOCamera.Width = Math.Max(dx, Math.Max(dy, dz)) * 1.5;
            //myOCamera.NearPlaneDistance = environmentSetting.BoundingBox_min.Y - dy * 15.0;
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
                Model3DGroup model = Display3d(dlg.FileNames[0]);

                Rect3D rect3d = model.Bounds;

                GeometryModel3D Geomodel = model.Children[0] as GeometryModel3D;

                DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Colors.White));
                Geomodel.Material = material;

                var dev = new Vector3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);
                dev = dev * -1;

                var matrix = modelVisual.Transform.Value;

                matrix.Translate(dev);

                var moreModelVisual3D = new ModelVisual3D();

                

                moreModelVisual3D.Content = model;
                moreModelVisual3D.Transform = new MatrixTransform3D(matrix);

                // model3dgroup.Children.Add(model);
                //model3dgroup.Children.Add(model);
                //modelVisual.Content = model3dgroup; ;

                //modelVisual.Content = model;                               
                helixViewport.Children.Add(moreModelVisual3D);
            }
        }

        private void NewCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var axis = new Vector3D(0, 0, 1);
            var angle = 10;

            var matrix = modelVisual.Transform.Value;
            matrix.Rotate(new Quaternion(axis, angle));

            modelVisual.Transform = new MatrixTransform3D(matrix);
        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            var dev = new Vector3D(0, 100, 0);

            var matrix = modelVisual.Transform.Value;
            matrix.Translate(dev);
            //matrix.Rotate(new Quaternion(axis, angle));

            modelVisual.Transform = new MatrixTransform3D(matrix);

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

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

    }
}
