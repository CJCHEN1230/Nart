using NartControl;
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
    using ExtensionMethods;
    using Model_Object;
    using System.IO;
    using System.Windows;
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
        /// <summary>
        /// 導引順序先開上顎or先開下顎
        /// </summary>
        private  string _firstNavigation = "Maxilla";

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
        private string _headOsp = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl";
        /// <summary>
        /// 下顎對稱面
        /// </summary>
        private string _mandibleOsp = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl";
        #endregion

        




        public NavigateViewModel(NavigateView navigateView)
        {
            _navigateView = navigateView;
            ModelSettingCommand = new RelayCommand(LoadSettingModel);            
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
        public string FirstNavigation
        {
            get
            {
                return _firstNavigation;
            }
            set
            {
                SetValue(ref _firstNavigation, value);
                //更新先導航順序
                MainViewModel.Data.FirstNavigation = value;
            }
        }
        public ICommand ModelSettingCommand { get; }

        private void SetBinding(object source, DependencyObject target, string propertyName, DependencyProperty dp, BindingMode mode)
        {
            Binding binding = new Binding(propertyName);
            binding.Source = source;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, dp, binding);
        }
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
            BoneModel head = new BoneModel
            {
                FilePath = HeadModel,
                MarkerId = "Head",
                BoneType = "Head",
                DiffuseColor = HeadDiffuseColor
            };
            head.LoadModel();

            //讀取原始上下顎 加上 規劃後的轉移矩陣
            Matrix plannedMatrix = ReadMatrixFile(_plannedMaxillaMatrix);
            BoneModel targetMaxilla = new BoneModel
            {
                FilePath = MaxillaModel,
                IsRendering = false,
                MarkerId = "",
                BoneType = "Maxilla",
                DiffuseColor = Color.FromArgb(255, 100, 100, 100),
                Transform = new MatrixTransform3D(plannedMatrix.ToMatrix3D())
            };
            targetMaxilla.LoadModel();

            Matrix plannedMandible = ReadMatrixFile(_plannedMandibleMatrix);
            BoneModel targetMandible = new BoneModel
            {
                FilePath = MandibleModel,
                IsRendering = false,
                MarkerId = "",
                BoneType = "Mandible",
                DiffuseColor = Color.FromArgb(255, 100, 100, 100),
                Transform = new MatrixTransform3D(plannedMandible.ToMatrix3D())
            };
            targetMandible.LoadModel();        

            MultiAngleViewModel.TargetCollection.Add(head);
            MultiAngleViewModel.TargetCollection.Add(targetMaxilla);
            MultiAngleViewModel.TargetCollection.Add(targetMandible);




            BoneModel oriMaxilla = new BoneModel
            {
                FilePath = MaxillaModel,
                BoneType = "Maxilla",
                MarkerId = "Splint",
                DiffuseColor = MaxillaDiffuseColor
            };
            oriMaxilla.LoadModel();

            BoneModel oriMandible = new BoneModel
            {
                FilePath = MandibleModel,
                BoneType = "Mandible",
                MarkerId = "Splint",
                DiffuseColor = MandibleDiffuseColor
            };
            oriMandible.LoadModel();
            
            MainViewModel.Data.BoneCollection.Add(oriMaxilla);
            MainViewModel.Data.BoneCollection.Add(oriMandible);

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

            //標記屬於上顎的ID
            DraggableTriangle maxillaTargetTriangle =new DraggableTriangle(targetMaxilla.ModelCenter)                
            {
                MarkerId = "Maxilla",
                IsRendering = false
            };
            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTargetTriangle);

            //標記屬於下顎的ID
            DraggableTriangle mandibleTargetTriangle =new DraggableTriangle(targetMandible.ModelCenter)                
            {
                MarkerId = "Mandible",
                IsRendering = false
            };
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTargetTriangle);

            //將導航三角形綁定到導航的上顎
            DraggableTriangle maxillaTriangle = new DraggableTriangle(oriMaxilla.ModelCenter)
            {
                MarkerId = "Maxilla",
                Transparency = 0.5f,
                IsRendering = false
            };
            SetBinding(oriMaxilla, maxillaTriangle,"Transform" ,HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty,BindingMode.OneWay);
            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTriangle);
      
            //將導航三角形綁定到導航的下顎
            DraggableTriangle mandibleTriangle = new DraggableTriangle(oriMandible.ModelCenter)
            {
                MarkerId = "Mandible",
                Transparency = 0.7f,
                IsRendering = false
            };
            SetBinding(oriMandible, mandibleTriangle, "Transform", HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty, BindingMode.OneWay);
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTriangle);



            MultiAngleViewModel.ResetCameraPosition();

            MainViewModel.Data.IsSet = true;

            _navigateView.Hide();
        }

        
    }
}
