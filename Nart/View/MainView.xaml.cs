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
using Nart.ExtensionMethods;
using Nart.Model_Object;
using NartControl;
using System.Windows.Media.Animation;

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

            //System.Windows.Controls.Button temp = new System.Windows.Controls.Button();
            //temp.Height = 100;
            //System.Windows.Controls.Button temp1 = new System.Windows.Controls.Button();
            //temp1.Height = 100;
            //System.Windows.Controls.Button temp2 = new System.Windows.Controls.Button();
            //temp2.Height = 100;
            //System.Windows.Controls.Button temp3 = new System.Windows.Controls.Button();
            //temp3.Height = 100;
            //System.Windows.Controls.Button temp4 = new System.Windows.Controls.Button();
            //temp4.Height = 100;

            //expander_TargetModel.expItem.Children.Add(temp);
            //expander_TargetModel.expItem.Children.Add(temp2);
            //expander_TargetModel.expItem.Children.Add(temp1);
            //expander_TargetModel.expItem.Children.Add(temp3);
            //expander_TargetModel.expItem.Children.Add(temp4);


        }

        private void Translate_Click(object sender, RoutedEventArgs e)
        {

            //this.Closed
            //Matrix3D TEST = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -100, -100, 100, 1);


            //BoneModel test = MultiAngleViewModel.BoneModelCollection[0] as BoneModel;

            //test.Transform= new MatrixTransform3D(TEST);
            //MainViewModel.ModelDataCollection = multiAngleView._multiAngleViewModel.ModelDataCollection;

            //SharpDX.Vector3 temp = MainViewModel.ModelDataCollection[0].ModelGeometry.Positions[0];

            //Point3D oldPoint = new Point3D(temp.X,temp.Y,temp.Z);

            //Console.WriteLine("oldPoint:" + oldPoint);

            //for (int i = 0; i < MainViewModel.ModelDataCollection.Count; i++)
            //{
            //    MainViewModel.ModelDataCollection[i].ModelTransform = new MatrixTransform3D(TEST);
            //}

            //SharpDX.Vector3 temp2 = MainViewModel.ModelDataCollection[0].ModelGeometry.Positions[0];

            //Point3D newPoint = new Point3D(temp2.X, temp2.Y, temp2.Z);


            //Console.WriteLine("newPoint:" + newPoint);

            //multiAngleView._multiAngleViewModel.ModelDataCollection = MainViewModel.ModelDataCollection;


            // Console.WriteLine(MainViewModel.ModelDataCollection[0].ModelGeometry.Positions[0].X);

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
            animation.Duration = new Duration( TimeSpan.FromSeconds(0.2));

            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));
            Storyboard.SetTarget(animation, Col0);

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);


            //GridLengthAnimation animation2 = new GridLengthAnimation();
            //animation2.From = Col1.Width;
            //animation2.To = Col0.Width;
            //animation2.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            //Col1.BeginAnimation(System.Windows.Controls.Button.WidthProperty, animation2);
        }

        private void settingButton1_Click(object sender, RoutedEventArgs e)
        {
            //// double ratio = Col1.ActualWidth / (mainGrid.ActualWidth - Col3.ActualWidth);
            //double ratio = Col1.ActualWidth * 5 / Col2.ActualWidth;
            //GridLengthAnimation gla = new GridLengthAnimation();
            //gla.From = Col0.Width;
            //gla.To = /*Col1.Width*/ new GridLength(ratio, GridUnitType.Star);
            //gla.Duration = new TimeSpan(0, 0, 0, 0, 200);
            //gla.FillBehavior = FillBehavior.Stop;
            //gla.Completed += (s, e1) =>
            //{
            //    Col0.Width = gla.To;
            //};
            //mainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gla);


            //GridLengthAnimation gla2 = new GridLengthAnimation();
            //gla2.From = Col1.Width;
            ////gla2.To = Col0.Width;
            //gla2.To = new GridLength(0, GridUnitType.Star);
            //gla2.Duration = new TimeSpan(0, 0, 0, 0, 200);
            //gla2.FillBehavior = FillBehavior.Stop;
            //gla2.Completed += (s, e1) =>
            //{
            //    Col1.Width = gla2.To;
            //};
            //mainGrid.ColumnDefinitions[1].BeginAnimation(ColumnDefinition.WidthProperty, gla2);

        }

        private void settingButton2_Click(object sender, RoutedEventArgs e)
        {

            
            //GridLengthAnimation gla = new GridLengthAnimation();
            //gla.From = Col0.Width;
            //gla.To =new GridLength(0, GridUnitType.Star);
            //gla.Duration = new TimeSpan(0, 0, 0, 0, 200);
            //gla.FillBehavior = FillBehavior.Stop;
            //gla.Completed += (s, e1) =>
            //{
            //    Col0.Width = gla.To;
            //};
            //mainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gla);


            //GridLengthAnimation gla2 = new GridLengthAnimation();
            //gla2.From = Col1.Width;
            //gla2.To = new GridLength(0, GridUnitType.Auto);
            //gla2.Duration = new TimeSpan(0, 0, 0, 0, 200);
            //gla2.FillBehavior = FillBehavior.Stop;
            //gla2.Completed += (s, e1) =>
            //{
            //    Col1.Width = gla2.To;
            //};
            //mainGrid.ColumnDefinitions[1].BeginAnimation(ColumnDefinition.WidthProperty, gla2);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("\n\n\nCol0  Width:" + Col0.Width);            
            Console.WriteLine("\nCol0  ActualWidth:" + Col0.ActualWidth);
            Console.WriteLine("\nCol1  Width:" + Col1.Width);
            Console.WriteLine("\nCol1  ActualWidth:" + Col1.ActualWidth);
            Console.WriteLine("\nCol2  Width:" + Col2.Width);
            Console.WriteLine("\nCol2  ActualWidth:" + Col2.ActualWidth);
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            BoneModel bone = MultiAngleViewModel.BoneModelCollection[0] as BoneModel;
            BoneModel bone1 = MultiAngleViewModel.BoneModelCollection[1] as BoneModel;
            //BoneModel bone2= MultiAngleViewModel.BoneModelCollection[2] as BoneModel;


            bone?.SaveModel();
            bone1?.SaveModel();
            //bone2?.SaveModel();
        }
    }
}
