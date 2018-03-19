using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Nart.ExtensionMethods;
using Nart.Model_Object;
using System.Windows.Media.Animation;
using SharpDX;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.AccessControl;

namespace Nart
{
    /// <summary>
    /// MainView主視窗介面
    /// </summary>
    public partial class MainView : Window
    {

        public MainViewModel MainViewModel = null;
        
        public MainView()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel(this);
            this.DataContext = MainViewModel;
            AllocConsole();
        }

       
        private void CamHost1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CamHost1.IsActivated)
            {
                CamHost1.InitializeCamSetting(CamHost1.ActualWidth, CamHost1.ActualHeight);
            }
        }
        private void CamHost2_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CamHost2.IsActivated)
            {
                CamHost2.InitializeCamSetting(CamHost2.ActualWidth, CamHost2.ActualHeight);
                MainViewModel.InitCamCtrl();
                CamHost1.IsActivated = true;
                CamHost2.IsActivated = true;
            }
        }

        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("Kernel32")]
        public static extern void FreeConsole();



        
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
           

        }
        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            //ball的模型
            Model3DGroup ballGroup = new Model3DGroup();
            foreach (BallModel model in MainViewModel.ProjData.BallCollection)
            {
                System.Windows.Media.Media3D.MeshGeometry3D ballMesh = new System.Windows.Media.Media3D.MeshGeometry3D
                {
                    Positions = new Point3DCollection(),
                    Normals = new Vector3DCollection(),
                    TriangleIndices = new Int32Collection()
                };
                HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = model.Geometry as HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

                if (geometry == null)
                    return;

                foreach (Vector3 position in geometry.Positions)
                {
                    ballMesh.Positions.Add(new Point3D(position.X, position.Y, position.Z));
                }
                foreach (Vector3 normal in geometry.Normals)
                {
                    ballMesh.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));
                }
                foreach (int triangleindice in geometry.TriangleIndices)
                {
                    ballMesh.TriangleIndices.Add(triangleindice);
                }
                System.Windows.Media.Media3D.GeometryModel3D ballModel = new System.Windows.Media.Media3D.GeometryModel3D
                {
                    Geometry = ballMesh,
                };

                ballGroup.Children.Add(ballModel);
            }
        
            StlExporter export1 = new StlExporter();
            string name1 = "ball.stl";
            using (var fileStream = File.Create("D:\\Desktop\\test\\" + name1))
            {
                export1.Export(ballGroup, fileStream);
            }


            
            //存球資料
            FileStream fs = new FileStream("D:\\Desktop\\test\\balldata.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (BallModel model in MainViewModel.ProjData.BallCollection)
            {               
                
               sw.Write(model.BallCenter.X+" "+model.BallCenter.Y+" "+ model.BallCenter.Z+"\r\n");
            }
            //清空緩衝區
             sw.Flush();
              //關閉流
              sw.Close();
              fs.Close();
             


}

    }
}
