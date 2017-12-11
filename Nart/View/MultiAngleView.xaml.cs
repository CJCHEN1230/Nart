﻿using System;
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


        private void button_Click(object sender, RoutedEventArgs e)
        {
            //確定已經註冊的情況
            if (MainViewModel.Data.IsSet&&CameraControl.TrackToggle)
            {
                string firstNavigation = MainViewModel.Data.FirstNavigation;

                for (int i = 0; i < MultiAngleViewModel.TriangleModelCollection.Count; i++) 
                {
                    DraggableTriangle model = MultiAngleViewModel.TriangleModelCollection[i] as DraggableTriangle;
                    //顯示第一階段的三角導引物
                    if (model.MarkerId.Equals(firstNavigation))
                    {
                        
                        model.IsRendering = true;
                    }
                }

                var boneCollection = MainViewModel.Data.BoneCollection;
                for (int i = 0; i < boneCollection.Count; i++)
                {
                    BoneModel model = boneCollection[i] as BoneModel;
                    //骨骼名稱是上顎
                    if (!model.BoneType.Equals(firstNavigation))
                    {
                        //model.IsRendering = false;
                        model.Visibility = Visibility.Hidden;
                    }
                }

                for (int i = 0; i < MultiAngleViewModel.TargetCollection.Count; i++)
                {
                    BoneModel model = MultiAngleViewModel.TargetCollection[i] as BoneModel;
                    if (!model.BoneType.Equals("Head"))
                    {
                        //骨骼名稱是上顎
                        if (model.BoneType.Equals(firstNavigation))
                        {
                            model.IsRendering = true;
                        }
                        else
                        {
                            model.IsRendering = false;
                        }
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

                for (int i = 0; i < MultiAngleViewModel.TriangleModelCollection.Count; i++)
                {
                    DraggableTriangle model = MultiAngleViewModel.TriangleModelCollection[i] as DraggableTriangle;

                    model.IsRendering = !model.IsRendering;
                  
                }
                var boneCollection = MainViewModel.Data.BoneCollection;

                for (int i = 0; i < boneCollection.Count; i++)
                {
                    BoneModel model = boneCollection[i] as BoneModel;
                    //model.IsRendering = !model.IsRendering;
                    if (model.Visibility == Visibility.Hidden)
                    {
                        model.Visibility = Visibility.Visible;
                    }
                    else 
                    {
                        model.Visibility = Visibility.Hidden;
                    }
                }

                for (int i = 0; i < MultiAngleViewModel.TargetCollection.Count; i++)
                {

                    BoneModel model = MultiAngleViewModel.TargetCollection[i] as BoneModel;

                    if (!model.BoneType.Equals("Head"))
                    {
                        model.IsRendering = !model.IsRendering;
                    }
                   
                }
            }
        }
    }
}
