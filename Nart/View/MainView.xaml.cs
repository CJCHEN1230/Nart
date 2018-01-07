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
using NartControl;
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

        private MainViewModel _mainViewModel = null;
        public MainView()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel(this);

            Thickness margin = buttonList.Margin;
            margin.Left = 0;
            margin.Right = 0;
            buttonList.Margin = margin;

           


            this.DataContext = _mainViewModel;

            AllocConsole();
            //int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            //int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            //Console.WriteLine("螢幕解析度為 " + screenWidth.ToString() + "*" + screenHeight.ToString());
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
                _mainViewModel.InitCamCtrl();
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

        private void settingButton1_Click(object sender, RoutedEventArgs e)
        {      
            var storyboard = new Storyboard();

            Col2.Width = new GridLength(Col2.ActualWidth, GridUnitType.Pixel);
            
            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(Col1.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };


            Storyboard.SetTargetProperty(gla2, new PropertyPath(ColumnDefinition.WidthProperty));
            Storyboard.SetTarget(gla2, Col1);
            
            storyboard.Children.Add(gla2);
           
            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(Col0.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(Col1.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 200),
                    FillBehavior = FillBehavior.HoldEnd,

                };
            Storyboard.SetTargetProperty(gla, new PropertyPath(ColumnDefinition.WidthProperty));
            Storyboard.SetTarget(gla, Col0);


            storyboard.Children.Add(gla);
   
            this.BeginStoryboard(storyboard);
           
        }

        private void settingButton2_Click(object sender, RoutedEventArgs e)
        {
            Col2.Width = new GridLength(Col2.ActualWidth, GridUnitType.Pixel);

            GridLengthAnimation gla =
                new GridLengthAnimation
                {
                    From = new GridLength(Col0.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(0, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

            GridLengthAnimation gla2 =
                new GridLengthAnimation
                {
                    From = new GridLength(Col1.ActualWidth, GridUnitType.Pixel),
                    To = new GridLength(Col0.ActualWidth, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 150),
                    FillBehavior = FillBehavior.HoldEnd
                };

         
            Col0.BeginAnimation(ColumnDefinition.WidthProperty, gla);
            Col1.BeginAnimation(ColumnDefinition.WidthProperty, gla2);
             
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {


        
            FileStream myFileStream = new FileStream(@"D:\Desktop\ttt2.xml", FileMode.Create);
            //建立 BinaryFormatter 物件
            SoapFormatter myBinaryFormatter = new SoapFormatter();

            myBinaryFormatter.Serialize(myFileStream, MainViewModel.Data);

            myFileStream.Close();

            // 反序列化，從檔案取出資料成員           
             myFileStream = new FileStream(@"D:\Desktop\ttt2.xml", FileMode.Open);
             myBinaryFormatter = new SoapFormatter();

            ProjectData mydata = (ProjectData)
                myBinaryFormatter.Deserialize(myFileStream);
            
            myFileStream.Close();
            // 於控制臺輸出資料			
            Console.WriteLine(mydata.Name);
            Console.WriteLine(mydata.ID);
            Console.WriteLine(mydata.Institution);




            //Console.WriteLine("\n\n\nCol2 MaxWidth:" + Col2.MaxWidth);
            //Console.WriteLine("\nMain  Width:" + MainGrid.Width);
            //Console.WriteLine("\nMain  ActualWidth:" + MainGrid.ActualWidth);
            //Console.WriteLine("\nCol0  Width:" + Col0.Width);            
            //Console.WriteLine("\nCol0  ActualWidth:" + Col0.ActualWidth);
            //Console.WriteLine("\nCol1  Width:" + Col1.Width);
            //Console.WriteLine("\nCol1  ActualWidth:" + Col1.ActualWidth);
            //Console.WriteLine("\nCol2  Width:" + Col2.Width);
            //Console.WriteLine("\nCol2  ActualWidth:" + Col2.ActualWidth);
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            //ball的模型
            Model3DGroup ballGroup = new Model3DGroup();
            foreach (BallModel model in MainViewModel.Data.BallCollection)
            {
                System.Windows.Media.Media3D.MeshGeometry3D ballMesh = new System.Windows.Media.Media3D.MeshGeometry3D
                {
                    Positions = new Point3DCollection(),
                    Normals = new Vector3DCollection(),
                    TriangleIndices = new Int32Collection()
                };
                HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = model.ballGeometry as HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

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



            //cylinder的模型
            Model3DGroup pipeGroup = new Model3DGroup();
            foreach (BallModel model in MainViewModel.Data.BallCollection)
            {
                System.Windows.Media.Media3D.MeshGeometry3D pipeMesh = new System.Windows.Media.Media3D.MeshGeometry3D
                {
                    Positions = new Point3DCollection(),
                    Normals = new Vector3DCollection(),
                    TriangleIndices = new Int32Collection()
                };
                HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = model.pipeGeometry as HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

                if (geometry == null)
                    return;

                foreach (Vector3 position in geometry.Positions)
                {
                    pipeMesh.Positions.Add(new Point3D(position.X, position.Y, position.Z));
                }
                foreach (Vector3 normal in geometry.Normals)
                {
                    pipeMesh.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));
                }
                foreach (int triangleindice in geometry.TriangleIndices)
                {
                    pipeMesh.TriangleIndices.Add(triangleindice);
                }
                System.Windows.Media.Media3D.GeometryModel3D pipeModel = new System.Windows.Media.Media3D.GeometryModel3D
                {
                    Geometry = pipeMesh,
                };

                pipeGroup.Children.Add(pipeModel);
            }

            StlExporter export2 = new StlExporter();
            string name2 = "pipe.stl";
            using (var fileStream = File.Create("D:\\Desktop\\test\\" + name2))
            {
                export2.Export(pipeGroup, fileStream);
            }


            //存球資料
            FileStream fs = new FileStream("D:\\Desktop\\test\\balldata.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (BallModel model in MainViewModel.Data.BallCollection)
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
            Col2.MaxWidth = MainGrid.ActualWidth;
            Col2.MinWidth = Screen.PrimaryScreen.Bounds.Width*2.0/3.0;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Nart_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".nart",
                DefaultExt = ".nart",
                Filter = "Nart Project Files (.nart)|*.nart"
            };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // 建立暫存目錄
                string fullFilePath = dlg.FileName;//完成路徑+副檔名

                string projectName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);//檔名不包含副檔名
                string filePath = System.IO.Path.GetDirectoryName(fullFilePath);//路徑
                string tempDirectory = System.IO.Path.Combine(filePath, projectName);//路徑+檔名(不包含副檔名)

                //先創建一個資料夾
                if (System.IO.Directory.Exists(tempDirectory) == true)
                {
                    System.IO.Directory.Delete(tempDirectory);
                }
                else
                {
                    System.IO.Directory.CreateDirectory(tempDirectory);
                }

                // 專案檔輸出                             
                string xmlFilePah  = System.IO.Path.Combine(tempDirectory, projectName) + ".xml";
                                  
                using (FileStream myFileStream = new FileStream(xmlFilePah, FileMode.Create))
                {
                    SoapFormatter soapFormatter = new SoapFormatter();
                    soapFormatter.Serialize(myFileStream, MainViewModel.Data);
                    myFileStream.Close();
                }

                foreach (BoneModel boneModel in MainViewModel.Data.BoneCollection)
                {
                    boneModel.SaveModel(tempDirectory, false);
                }


                ZipFile.CreateFromDirectory(tempDirectory, fullFilePath);
                System.IO.Directory.Delete(tempDirectory, true);                
            }
        }
        private void LoadClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".nart",
                Multiselect = false,
                Filter = "Nart Project Files (.nart)|*.nart"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                string fullFilePath = dlg.FileName;

                switch (System.IO.Path.GetExtension(fullFilePath).ToLower())
                {
                    //
                    case ".nart":
                        {

                            string projectName = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);//檔名不包含副檔名
                            string filePath = System.IO.Path.GetDirectoryName(fullFilePath);//路徑
                            string tempDirectory = System.IO.Path.Combine(filePath, projectName);//路徑+檔名(不包含副檔名)
                            
                            System.IO.Directory.CreateDirectory(tempDirectory);
                            ZipFile.ExtractToDirectory(fullFilePath, tempDirectory);

                            string xmlFilePath = System.IO.Path.Combine(tempDirectory, projectName + ".xml");
                            if (!File.Exists(xmlFilePath))
                            {
                                return;
                            }

                            ProjectData projectData;
                            using (FileStream myFileStream = new FileStream(xmlFilePath, FileMode.Open))
                            {
                                SoapFormatter soapFormatter = new SoapFormatter();
                                projectData = (ProjectData)soapFormatter.Deserialize(myFileStream);
                                myFileStream.Close();
                            }
                            
                            foreach (BoneModel boneModel in projectData.BoneCollection)
                            {
                                boneModel.FilePath = System.IO.Path.Combine(tempDirectory, boneModel.SafeFileName);
                                boneModel.LoadModel();
                            }
                            projectData.Name = "大家好";
                            projectData.ID = "我很好";
                            projectData.Institution = "成大!!!!!!!!";
                            MainViewModel.Data.UpdateData(projectData);


                            MultiAngleViewModel.ResetCameraPosition();
                            //_mainViewModel



                            System.IO.Directory.Delete(tempDirectory, true);
                            break;
                        }

                }
            }


            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            



        }
    }
}
