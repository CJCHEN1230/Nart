using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    using Converter;
    using ExtensionMethods;
    using Model_Object;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;
    using Color = System.Windows.Media.Color;
    public class NavigateViewModel : ObservableObject
    {

        /// <summary>
        /// NavigateView頁面
        /// </summary>
        private readonly NavigateView _navigateView;

        #region N-Art計畫部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private string _plannedMaxillaMatrix = "../../../data/蔡慧君/plan-maxilla-matrix.txt";
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private string _plannedMandibleMatrix = "../../../data/蔡慧君/plan-mandible-matrix.txt";
        #endregion       
        #region 原始模型設定部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private string _headModel = "../../../data/蔡慧君/head.stl";
        /// <summary>
        /// 頭部模型顏色
        /// </summary>
        private Color _headDiffuseColor = Color.FromArgb(255, 11, 243, 243);
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private string _maxillaModel = "../../../data/蔡慧君/pre-maxilla.stl";
        /// <summary>
        /// 上顎模型顏色
        /// </summary>
        private Color _maxillaDiffuseColor = Color.FromArgb(255, 11, 243, 243);
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private string _mandibleModel = "../../../data/蔡慧君/pre-mandible.stl";
        /// <summary>
        /// 下顎模型顏色
        /// </summary>
        private Color _mandibleDiffuseColor = Color.FromArgb(255, 2, 231, 2);
        #endregion
        #region OSP部分
        /// <summary>
        /// 頭部對稱面
        /// </summary>
        private string _headOsp = "../../../data/蔡慧君/max_OSP.stl";
        /// <summary>
        /// 下顎對稱面
        /// </summary>
        private string _mandibleOsp = "../../../data/蔡慧君/man_OSP.stl";
        #endregion

        public NavigateViewModel(NavigateView navigateView)
        {
            _navigateView = navigateView;
            ModelSettingCommand = new RelayCommand(LoadSettingModel);
            //直接綁FirstNavigation到專案資料裡面
            BindFirstNavigation();
        }



        public string PlannedMaxillaMatrix
        {
            get
            {
                return _plannedMaxillaMatrix;
            }
            set
            {
                SetValue(ref _plannedMaxillaMatrix, value);
            }
        }
        public string PlannedMandibleMatrix
        {
            get
            {
                return _plannedMandibleMatrix;
            }
            set
            {
                SetValue(ref _plannedMandibleMatrix, value);
            }
        }
        public string HeadModel
        {
            get
            {
                return _headModel;
            }
            set
            {
                SetValue(ref _headModel, value);
            }
        }
        public string MaxillaModel
        {
            get
            {
                return _maxillaModel;
            }
            set
            {
                SetValue(ref _maxillaModel, value);
            }
        }
        public string MandibleModel
        {
            get
            {
                return _mandibleModel;
            }
            set
            {
                SetValue(ref _mandibleModel, value);
            }
        }
        public Color HeadDiffuseColor
        {
            get
            {
                return _headDiffuseColor;
            }
            set
            {
                SetValue(ref _headDiffuseColor, value);
            }
        }
        public Color MaxillaDiffuseColor
        {
            get
            {
                return _maxillaDiffuseColor;
            }
            set
            {
                SetValue(ref _maxillaDiffuseColor, value);
            }
        }
        public Color MandibleDiffuseColor
        {
            get
            {
                return _mandibleDiffuseColor;
            }
            set
            {
                SetValue(ref _mandibleDiffuseColor, value);
            }
        }
        public string HeadOsp
        {
            get
            {
                return _headOsp;
            }
            set
            {
                SetValue(ref _headOsp, value);
            }
        }      
        public string MandibleOsp
        {
            get
            {
                return _mandibleOsp;
            }
            set
            {
                SetValue(ref _mandibleOsp, value);
            }
        } 
     
        public ICommand ModelSettingCommand { get; }

        public Matrix ReadMatrixFile(string  path)
        {            
            try
            {
                string fileContent = File.ReadAllText(path);
                string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
                float[] matrixInfo = Array.ConvertAll(contentArray, float.Parse);                
                if (matrixInfo.Length!=16)
                {
                    throw new Exception();
                }
                
                return new Matrix(matrixInfo);
            }
            catch
            {
                MessageBox.Show("矩陣檔案讀取錯誤");
                return new Matrix();
            }

         
        }
        public void LoadSettingModel(object o)
        {
            MainViewModel.ProjData.TargetCollection.Clear();
            MainViewModel.ProjData.BoneCollection.Clear();
            MultiAngleViewModel.OspModelCollection.Clear();
            MultiAngleViewModel.TriangleModelCollection.Clear();

            


            //讀取原始上下顎 加上 規劃後的轉移矩陣
            Matrix plannedMatrix = ReadMatrixFile(_plannedMaxillaMatrix);
            BoneModel targetMaxilla = new BoneModel
            {
                FilePath = MaxillaModel,
                IsRendering = false,
                MarkerId = "",
                ModelType = ModelType.TargetMaxilla,
                BoneDiffuseColor = Color.FromArgb(255, 100, 100, 100),
                Transform = new MatrixTransform3D(plannedMatrix.ToMatrix3D())
            };
            targetMaxilla.LoadModel();

            Matrix plannedMandible = ReadMatrixFile(_plannedMandibleMatrix);
            BoneModel targetMandible = new BoneModel
            {
                FilePath = MandibleModel,
                IsRendering = false,
                MarkerId = "",
                ModelType = ModelType.TargetMandible,
                BoneDiffuseColor = Color.FromArgb(255, 100, 100, 100),
                Transform = new MatrixTransform3D(plannedMandible.ToMatrix3D())
            };
            targetMandible.LoadModel();        

            //MainViewModel.Data.TargetCollection.Add(head);
            MainViewModel.ProjData.TargetCollection.Add(targetMaxilla);
            MainViewModel.ProjData.TargetCollection.Add(targetMandible);

            BoneModel head = new BoneModel
            {
                FilePath = HeadModel,
                MarkerId = "Head",
                ModelType = ModelType.Head,
                BoneDiffuseColor = HeadDiffuseColor
            };
            head.LoadModel();
            BoneModel oriMaxilla = new BoneModel
            {
                FilePath = MaxillaModel,
                ModelType = ModelType.MovedMaxilla,
                MarkerId = "Splint",
                BoneDiffuseColor = MaxillaDiffuseColor
            };
            oriMaxilla.LoadModel();            
            BoneModel oriMandible = new BoneModel
            {
                FilePath = MandibleModel,
                ModelType = ModelType.MovedMandible,
                MarkerId = "Splint",
                BoneDiffuseColor = MandibleDiffuseColor
            };
            oriMandible.LoadModel();

            MainViewModel.ProjData.BoneCollection.Add(head);
            MainViewModel.ProjData.BoneCollection.Add(oriMaxilla);
            MainViewModel.ProjData.BoneCollection.Add(oriMandible);

            //載入OSP模型
            OspModel headOsp = new OspModel
            {
                MarkerId = "Head",
                FilePath = HeadOsp,
                DiffuseColor = Color.FromArgb(50, 11, 243, 243)
            };
            headOsp.LoadOsp();

            OspModel mandibleOsp = new OspModel
            {
                MarkerId = "C",
                FilePath = MandibleOsp,
                DiffuseColor = Color.FromArgb(50, 2, 231, 2)
            };
            mandibleOsp.LoadOsp();

            //綁定下顎對稱面到下顎模型
            SetBinding(oriMandible, mandibleOsp, "Transform", HelixToolkit.Wpf.SharpDX.Model3D.TransformProperty, BindingMode.OneWay);
            MultiAngleViewModel.OspModelCollection.Add(headOsp);
            MultiAngleViewModel.OspModelCollection.Add(mandibleOsp);

            //標記屬於上顎的ID，綁定到目標上顎
            DraggableTriangle maxillaTargetTriangle =new DraggableTriangle(targetMaxilla.ModelCenter)                
            {
                MarkerId = "Maxilla",
                IsRendering = false,
                Transform = targetMaxilla.Transform,
                ModelType = ModelType.TargetMaxillaTriangle,
            };

            //標記屬於下顎的ID，綁定到目標下顎
            DraggableTriangle mandibleTargetTriangle =new DraggableTriangle(targetMandible.ModelCenter)                
            {
                MarkerId = "Mandible",
                IsRendering = false,
                Transform = targetMandible.Transform,
                ModelType = ModelType.TargetMandibleTriangle,
            };




            //將導航三角形綁定到導航的上顎
            DraggableTriangle maxillaTriangle = new DraggableTriangle(oriMaxilla.ModelCenter)
            {
                MarkerId = "Maxilla",
                Transparency = 0.5f,
                IsRendering = false,
                ModelType = ModelType.MovedMaxillaTriangle,
            };
            SetBinding(oriMaxilla, maxillaTriangle,"Transform" ,HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty,BindingMode.OneWay);

            
      
            //將導航三角形綁定到導航的下顎
            DraggableTriangle mandibleTriangle = new DraggableTriangle(oriMandible.ModelCenter)
            {
                MarkerId = "Mandible",
                Transparency = 0.5f,
                IsRendering = false,
                ModelType = ModelType.MovedMandibleTriangle,
            };
            SetBinding(oriMandible, mandibleTriangle, "Transform", HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty, BindingMode.OneWay);

            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTargetTriangle);
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTargetTriangle);
            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTriangle);
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTriangle);



            MultiAngleViewModel.ResetCameraPosition();

            MainViewModel.ProjData.IsNavSet = true;

            _navigateView.Hide();
        }
        private void SetBinding(object source, DependencyObject target, string propertyName, DependencyProperty dp, BindingMode mode)
        {
            Binding binding = new Binding(propertyName);
            binding.Source = source;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, dp, binding);
        }
        /// <summary>
        /// 綁定Naviagation View的FirstNavigation到ProjData
        /// </summary>
        private void BindFirstNavigation()
        {
            //這邊主要是綁定一組RadioButton的IsChecked到FirstNavigation
            EnumMatchToBooleanConverter converter = new EnumMatchToBooleanConverter();
            Binding binding = new Binding("FirstNavigation");
            binding.Source = MainViewModel.ProjData;
            binding.Mode = BindingMode.TwoWay;            
            binding.Converter = converter;
            binding.ConverterParameter = "Maxilla";
            BindingOperations.SetBinding(_navigateView.MaxRadioButton, RadioButton.IsCheckedProperty, binding);

            Binding binding2 = new Binding("FirstNavigation");
            binding2.Source = MainViewModel.ProjData;
            binding2.Mode = BindingMode.TwoWay;
            binding2.Converter = converter;
            binding2.ConverterParameter = "Mandible";
            BindingOperations.SetBinding(_navigateView.ManRadioButton, RadioButton.IsCheckedProperty, binding2);
        }

    }
}
