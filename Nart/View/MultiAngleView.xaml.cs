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
using Nart.Model_Object;

namespace Nart
{
    /// <summary>
    /// MultiAngleView.xaml 的互動邏輯
    /// </summary>
    public partial class MultiAngleView : UserControl
    {
        public MultiAngleViewModel MultiAngleViewModel;

        public MultiAngleView()
        {
            InitializeComponent();         
            MultiAngleViewModel = new MultiAngleViewModel(this);
            this.DataContext = MultiAngleViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext                  

        }

        /// <summary>
        /// 這個階段主要要顯示出所設定的第一階段的上or下顎，且顯示三角形模型
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //確定已經設定導航資訊，且已經有按Tracking的情形
            if (MainViewModel.Data.IsSet&&CameraControl.TrackToggle)
            {
                string firstNavigation = MainViewModel.Data.FirstNavigation;
                foreach (Element3D model in MultiAngleViewModel.TriangleModelCollection)
                {
                    DraggableTriangle dragModel = model as DraggableTriangle;
                    //顯示第一階段的三角導引物
                    if (dragModel != null && dragModel.MarkerId.Equals(firstNavigation))
                    {                        
                        dragModel.IsRendering = true;
                    }
                }

                var boneCollection = MainViewModel.Data.BoneCollection;
                foreach (BoneModel model in boneCollection)
                {
                    //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話不讓他改變位置
                    if (!model.BoneType.Equals(firstNavigation))
                    {
                        model.IsTransformApplied = false;
                    }
                }

                //顯示出所選擇的目標模型
                foreach (Element3D targetModel in MultiAngleViewModel.TargetCollection)
                {
                    BoneModel model = targetModel as BoneModel;
                    //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話則不顯示
                    if (model != null && !model.BoneType.Equals("Head"))
                    {
                        //骨骼名稱是上顎
                        model.IsRendering = model.BoneType.Equals(firstNavigation);
                    }
                }
                MainViewModel.Data.FirstStageDone = true;
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            //確定已經註冊的情況
            if (MainViewModel.Data.IsSet && CameraControl.TrackToggle)
            {
                string firstNavigation = MainViewModel.Data.FirstNavigation;

                foreach (Element3D dragModel in MultiAngleViewModel.TriangleModelCollection)
                {
                    DraggableTriangle model = dragModel as DraggableTriangle;

                    model.IsRendering = !model.IsRendering;
                }
                var boneCollection = MainViewModel.Data.BoneCollection;
                
                foreach (BoneModel model in boneCollection)
                {
                    //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話則計算位置
                    //if (!model.BoneType.Equals(firstNavigation))
                    //{
                        model.IsTransformApplied = !model.IsTransformApplied;
                    //}
                }



                foreach (Element3D targetModel in MultiAngleViewModel.TargetCollection)
                {
                    BoneModel model = targetModel as BoneModel;

                    if (!model.BoneType.Equals("Head"))
                    {
                        model.IsRendering = !model.IsRendering;
                    }
                }
            }
        }
    }
}
