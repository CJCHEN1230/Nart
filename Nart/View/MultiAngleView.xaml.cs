using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using System.Collections.ObjectModel;
using System.IO;
using Nart.Model_Object;

namespace Nart
{
    /// <summary>
    /// MultiAngleView.xaml 的互動邏輯
    /// </summary>
    public partial class MultiAngleView : UserControl
    {
        public MultiAngleViewModel _multiAngleViewModel;

        public MultiAngleView()
        {
            InitializeComponent();         
            _multiAngleViewModel = new MultiAngleViewModel(this);
            this.DataContext = _multiAngleViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext                  

        }

        

   
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (extension.Equals(".stl"))
                    {

                        BoneModel model = new BoneModel();
                      
                        model.FilePath = files[i];
                        model.MarkerID = "";
                        model.DiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187);
                        model.LoadModel();
                        model.Transform = new System.Windows.Media.Media3D.MatrixTransform3D();

                        MultiAngleViewModel.BoneModelCollection.Add(model);
                    }
                }
                MultiAngleViewModel.ResetCameraPosition();
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (extension.Equals(".stl"))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]).ToLower();
                    if (extension.Equals(".stl"))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //確定已經註冊的情況
            if (NavigateViewModel.IsSet&&CameraControl.TrackToggle)
            {
                string firstNavigation = NavigateViewModel.FirstNavigation;

                for (int i = 0; i < MultiAngleViewModel.TriangleModelCollection.Count; i++) 
                {
                    DraggableTriangle model = MultiAngleViewModel.TriangleModelCollection[i] as DraggableTriangle;
                    //確保是第一階段的導引物
                    if (model.MarkerID.Equals(firstNavigation))
                    {
                        
                        model.IsRendering = true;
                    }
                }

                for (int i = 0; i < MultiAngleViewModel.BoneModelCollection.Count; i++)
                {
                    BoneModel model = MultiAngleViewModel.BoneModelCollection[i] as BoneModel;
                    //骨骼名稱是上顎
                    if (model.BoneName.Equals("Maxilla"))
                    {
                        model.Stage = "intermediate";
                    }
                }

                for (int i = 0; i < MultiAngleViewModel.NavigationTargetCollection.Count; i++)
                {
                    BoneModel model = MultiAngleViewModel.NavigationTargetCollection[i] as BoneModel;
                    //骨骼名稱是上顎
                    if (model.BoneName.Equals("Maxilla"))
                    {
                        model.IsRendering = true;
                    }
                }
                NavigateViewModel.firstStageDone = true;
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            //確定已經註冊的情況
            if (NavigateViewModel.IsSet && CameraControl.TrackToggle)
            {
                string firstNavigation = NavigateViewModel.FirstNavigation;

                for (int i = 0; i < MultiAngleViewModel.TriangleModelCollection.Count; i++)
                {
                    DraggableTriangle model = MultiAngleViewModel.TriangleModelCollection[i] as DraggableTriangle;

                    model.IsRendering = !model.IsRendering;
                  
                }

                for (int i = 0; i < MultiAngleViewModel.BoneModelCollection.Count; i++)
                {
                    BoneModel model = MultiAngleViewModel.BoneModelCollection[i] as BoneModel;
                    //骨骼名稱是上顎
                    if (model.BoneName.Equals("Maxilla"))
                    {
                        model.Stage = "final";
                    }
                }

                for (int i = 0; i < MultiAngleViewModel.NavigationTargetCollection.Count; i++)
                {


                    BoneModel model = MultiAngleViewModel.NavigationTargetCollection[i] as BoneModel;
                    if (!model.MarkerID.Equals("Head"))
                    {
                        model.IsRendering = !model.IsRendering;
                    }
                }
            }
        }
    }
}
