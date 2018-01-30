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
            if (!MainViewModel.Data.IsNavigationSet || !MainViewModel.Data.TrackToggle)
                return;

            string firstNavigation = MainViewModel.Data.FirstNavigation;
            foreach (Element3D model in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle dragModel = model as DraggableTriangle;
                if (dragModel == null)
                    return;

                switch (firstNavigation)
                {
                    case "Maxilla":
                        if (dragModel.ModelType == ModelType.TargetMaxillaTriangle ||
                            dragModel.ModelType == ModelType.MovedMaxillaTriangle)
                        {
                            dragModel.IsRendering = true;
                        }
                        else
                        {
                            dragModel.IsRendering = false;
                        }
                        break;
                    case "Mandible":
                        if (dragModel.ModelType == ModelType.TargetMandibleTriangle ||
                            dragModel.ModelType == ModelType.MovedMandibleTriangle)
                        {
                            dragModel.IsRendering = true;
                        }
                        else
                        {
                            dragModel.IsRendering = false;
                        }
                        break;
                }
            }

            var boneCollection = MainViewModel.Data.BoneCollection;
            foreach (BoneModel model in boneCollection)
            {
                switch (firstNavigation)
                {
                    //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話不讓他改變位置
                    case "Maxilla":
                        if (model.ModelType == ModelType.MovedMandible)
                        {
                            model.Transform = Transform3D.Identity;
                            model.IsTransformApplied = false;
                        }
                        break;
                    case "Mandible":
                        if (model.ModelType == ModelType.MovedMaxilla)
                        {
                            model.Transform = Transform3D.Identity;
                            model.IsTransformApplied = false;
                        }
                        break;
                }
            }

            var targetCollection = MainViewModel.Data.TargetCollection;
            //顯示出所選擇的目標模型
            foreach (BoneModel targetModel in targetCollection)
            {
             
                if (targetModel == null)                     
                    return;
                //骨骼名稱是不是所選的第一導引骨頭(上、下顎)，不是的話則不顯示
                switch (firstNavigation)
                {                        
                    case "Maxilla":
                        if (targetModel.ModelType == ModelType.TargetMaxilla)
                        {
                            targetModel.IsRendering = true;
                        }
                        break;
                    case "Mandible":
                        if (targetModel.ModelType == ModelType.TargetMandible)
                        {
                            targetModel.IsRendering = true;
                        }
                        break;
                }
            }
            //第一階段按下
            MainViewModel.Data.IsFirstStage = true;
        }
        /// <summary>
        /// 這個階段主要要顯示出所設定的第二階段的上or下顎，且顯示三角形模型
        /// </summary>
        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            //確定已經註冊的情況
            if (!MainViewModel.Data.IsNavigationSet || !MainViewModel.Data.IsFirstStage || !MainViewModel.Data.TrackToggle )
                return;
            string firstNavigation = MainViewModel.Data.FirstNavigation;

            //因為是第二階段，所以三角導引物是否顯示相反就好
            foreach (Element3D dragModel in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle model = dragModel as DraggableTriangle;

                if (model != null)
                    model.IsRendering = !model.IsRendering;
            }
            var boneCollection = MainViewModel.Data.BoneCollection;

            //因為是第二階段，所以模型是否可以更新位置相反就好
            foreach (BoneModel model in boneCollection)
            {
                model.IsTransformApplied = !model.IsTransformApplied;
            }



            foreach (BoneModel targetModel in MainViewModel.Data.TargetCollection)
            {              
                if (targetModel != null && 
                    (targetModel.ModelType.Equals(ModelType.TargetMandible) || targetModel.ModelType.Equals(ModelType.TargetMaxilla))) 
                {
                    targetModel.IsRendering = !targetModel.IsRendering;
                }
            }
            MainViewModel.Data.IsFirstStage = false;
            MainViewModel.Data.IsSecondStage = true;
        }


        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            foreach (BoneModel targetModel in MainViewModel.Data.TargetCollection)
            {
                if (targetModel != null &&
                    (targetModel.ModelType.Equals(ModelType.TargetMandible) || targetModel.ModelType.Equals(ModelType.TargetMaxilla)))
                {
                    targetModel.IsRendering = false;
                }
            }

            foreach (Element3D dragModel in MultiAngleViewModel.TriangleModelCollection)
            {
                DraggableTriangle model = dragModel as DraggableTriangle;

                if (model != null)
                    model.IsRendering = false;
            }

            MainViewModel.Data.RegToggle = false;
            MainViewModel.Data.TrackToggle = false;
            MainViewModel.Data.IsFirstStage = false;
            MainViewModel.Data.IsSecondStage = false;
        }
    }
}
