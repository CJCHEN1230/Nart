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

            Thickness margin = buttonList.Margin;
            margin.Left = 0;
            margin.Right = 0;
            buttonList.Margin = margin;

            this.DataContext = MainViewModel;

            Col0.Width = new GridLength(0, GridUnitType.Star);
            Col1.Width = new GridLength(1.2, GridUnitType.Star);
            Col2.Width = new GridLength(5, GridUnitType.Star);

            AllocConsole();
            Console.WriteLine(("Col0:" + Col0.Width).PadRight(10) + ("  Actual 0:" + Col0.ActualWidth).PadRight(10) + ("  MinWidth:" + Col0.MinWidth).PadRight(10) + ("  MaxWidth:" + Col0.MaxWidth).PadRight(10));
            Console.WriteLine(("Col1:" + Col1.Width).PadRight(10) + ("  Actual 1:" + Col1.ActualWidth).PadRight(10) + ("  MinWidth:" + Col1.MinWidth).PadRight(10) + ("  MaxWidth:" + Col1.MaxWidth).PadRight(10));
            Console.WriteLine(("Col2:" + Col2.Width).PadRight(10) + ("  Actual 2:" + Col2.ActualWidth).PadRight(10) + ("  MinWidth:" + Col2.MinWidth).PadRight(10) + ("  MaxWidth:" + Col2.MaxWidth).PadRight(10));
            Console.WriteLine();
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



        private void button_Click(object sender, RoutedEventArgs e)
        {
            GridLengthAnimation animation = new GridLengthAnimation();
            animation.From = new GridLength(0D);
            animation.To = Col1.Width;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));
            Storyboard.SetTarget(animation, Col0);

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
        }
    
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
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Col2.MaxWidth = MainGrid.ActualWidth;
            Col2.MinWidth = MainGrid.ActualWidth * 1.0/2.0;
        }
        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(("Col0:" + Col0.Width).PadRight(30) + ("  Actual 0:" + Col0.ActualWidth).PadRight(30) + ("  MinWidth:" + Col0.MinWidth).PadRight(15) + ("  MaxWidth:" + Col0.MaxWidth).PadRight(15));
            Console.WriteLine(("Col1:" + Col1.Width).PadRight(30) + ("  Actual 1:" + Col1.ActualWidth).PadRight(30) + ("  MinWidth:" + Col1.MinWidth).PadRight(15) + ("  MaxWidth:" + Col1.MaxWidth).PadRight(15));
            Console.WriteLine(("Col2:" + Col2.Width).PadRight(30) + ("  Actual 2:" + Col2.ActualWidth).PadRight(30) + ("  MinWidth:" + Col2.MinWidth).PadRight(15) + ("  MaxWidth:" + Col2.MaxWidth).PadRight(15));
            Console.WriteLine();


            
        }

        private void Test2Btn_Click(object sender, RoutedEventArgs e)
        {
            Col0.Width = new GridLength(1, GridUnitType.Auto);

            Console.WriteLine();
        }

        private void Test3Btn_Click(object sender, RoutedEventArgs e)
        {
            
            GridLengthAnimation gla3 =
                new GridLengthAnimation
                {
                    From = new GridLength(Col2.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(MainGrid.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            Col2.BeginAnimation(ColumnDefinition.WidthProperty, gla3);

           

            Console.WriteLine();
        }
    }
}
