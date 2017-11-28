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

        public static bool IsSet = false;
        public static bool firstStageDone = false;
        /// <summary>
        /// 導引順序先開上顎or先開下顎
        /// </summary>
        private static String firstNavigation = "Maxilla";
        #region N-Art計畫部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String plannedMaxillaMatrix = "../../../data/蔡慧君/plan-maxilla-matrix.txt";
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String plannedMandibleMatrix = "../../../data/蔡慧君/plan-mandible-matrix.txt";
        #endregion       
        #region 原始模型設定部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String headModel = "../../../data/蔡慧君/head.stl";
        /// <summary>
        /// 頭部模型顏色
        /// </summary>
        private Color headDiffuseColor = Color.FromArgb(255, 11, 243, 243);
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String maxillaModel = "../../../data/蔡慧君/pre-maxilla.stl";
        /// <summary>
        /// 上顎模型顏色
        /// </summary>
        private Color maxillaDiffuseColor = Color.FromArgb(255, 11, 243, 243);
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private String mandibleModel = "../../../data/蔡慧君/pre-mandible.stl";
        /// <summary>
        /// 下顎模型顏色
        /// </summary>
        private Color mandibleDiffuseColor = Color.FromArgb(255, 2, 231, 2);
        #endregion


        #region OSP部分
        /// <summary>
        /// 頭部對稱面
        /// </summary>
        private String headOSP = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl";
        /// <summary>
        /// 下顎對稱面
        /// </summary>
        private String mandibleOSP = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl";
        #endregion

        /// <summary>
        /// View頁面
        /// </summary>
        private NavigateView _navigateView;




        public NavigateViewModel(NavigateView navigateView)
        {
            _navigateView = navigateView;
            ModelSettingCommand = new RelayCommand(LoadSettingModel);            
        }



        public String PlannedMaxillaMatrix
        {
            get
            {
                return plannedMaxillaMatrix;
            }
            set
            {
                SetValue(ref plannedMaxillaMatrix, value);
            }
        }
        public String PlannedMandibleMatrix
        {
            get
            {
                return plannedMandibleMatrix;
            }
            set
            {
                SetValue(ref plannedMandibleMatrix, value);
            }
        }

        public String HeadModel
        {
            get
            {
                return headModel;
            }
            set
            {
                SetValue(ref headModel, value);
            }
        }
        public String MaxillaModel
        {
            get
            {
                return maxillaModel;
            }
            set
            {
                SetValue(ref maxillaModel, value);
            }
        }
        public String MandibleModel
        {
            get
            {
                return mandibleModel;
            }
            set
            {
                SetValue(ref mandibleModel, value);
            }
        }
        public Color HeadDiffuseColor
        {
            get
            {
                return headDiffuseColor;
            }
            set
            {
                SetValue(ref headDiffuseColor, value);
            }
        }
        public Color MaxillaDiffuseColor
        {
            get
            {
                return maxillaDiffuseColor;
            }
            set
            {
                SetValue(ref maxillaDiffuseColor, value);
            }
        }
        public Color MandibleDiffuseColor
        {
            get
            {
                return mandibleDiffuseColor;
            }
            set
            {
                SetValue(ref mandibleDiffuseColor, value);
            }
        }

        public String HeadOSP
        {
            get
            {
                return headOSP;
            }
            set
            {
                SetValue(ref headOSP, value);
            }
        } 
     
        public String MandibleOSP
        {
            get
            {
                return mandibleOSP;
            }
            set
            {
                SetValue(ref mandibleOSP, value);
            }
        } 






        public static string FirstNavigation
        {
            get
            {
                return firstNavigation;
            }
            set
            {
                SetStaticValue(ref firstNavigation, value);
            }
        }
        public ICommand ModelSettingCommand { private set; get; }

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
                string fileContent = File.ReadAllText(path);//"../../../data/CaliR_L.txt"
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

            BoneModel Head = new BoneModel();
            Head.FilePath = HeadModel;
            Head.MarkerID = "Head";
            Head.BoneName = "Head";
            Head.DiffuseColor = HeadDiffuseColor;
            Head.LoadModel();

            //讀取原始上下顎 加上 規劃後的轉移矩陣
            Matrix plannedMatrix = ReadMatrixFile(plannedMaxillaMatrix);
            BoneModel targetMaxilla = new BoneModel();
            targetMaxilla.FilePath = MaxillaModel;
            targetMaxilla.IsRendering = false;
            targetMaxilla.MarkerID = "";
            targetMaxilla.BoneName = "Maxilla";
            targetMaxilla.DiffuseColor = Color.FromArgb(255, 100, 100, 100);
            targetMaxilla.LoadModel();
            targetMaxilla.Transform = new System.Windows.Media.Media3D.MatrixTransform3D(plannedMatrix.ToMatrix3D()); 

            Matrix plannedMandible = ReadMatrixFile(plannedMandibleMatrix);
            BoneModel targetMandible = new BoneModel();
            targetMandible.FilePath = MandibleModel;
            targetMandible.IsRendering = false;
            targetMandible.MarkerID = "";
            targetMandible.BoneName = "Mandible";
            targetMandible.DiffuseColor = Color.FromArgb(255, 100, 100, 100);
            targetMandible.LoadModel();        
            targetMandible.Transform = new System.Windows.Media.Media3D.MatrixTransform3D(plannedMandible.ToMatrix3D());

            MultiAngleViewModel.NavigationTargetCollection.Add(Head);
            MultiAngleViewModel.NavigationTargetCollection.Add(targetMaxilla);
            MultiAngleViewModel.NavigationTargetCollection.Add(targetMandible);




            BoneModel oriMaxilla = new BoneModel();
            oriMaxilla.FilePath = MaxillaModel;
            oriMaxilla.BoneName = "Maxilla";
            oriMaxilla.MarkerID = "Splint";
            oriMaxilla.DiffuseColor = MaxillaDiffuseColor;
            oriMaxilla.LoadModel();

            BoneModel oriMandible = new BoneModel();
            oriMandible.FilePath = MandibleModel;
            oriMandible.BoneName = "Mandible";
            oriMandible.MarkerID = "Splint";
            oriMandible.DiffuseColor = MandibleDiffuseColor;
            oriMandible.LoadModel();

            MultiAngleViewModel.BoneModelCollection.Add(oriMaxilla);
            MultiAngleViewModel.BoneModelCollection.Add(oriMandible);

            //載入OSP模型
            OSPModel headOSP = new OSPModel();
            headOSP.MarkerID = "Head";
            headOSP.FilePath = HeadOSP;
            headOSP.DiffuseColor = System.Windows.Media.Color.FromArgb(50, 11, 243, 243);
            headOSP.LoadOSP();

            OSPModel mandibleOSP = new OSPModel();
            mandibleOSP.MarkerID = "C";
            mandibleOSP.FilePath = MandibleOSP;
            mandibleOSP.DiffuseColor = System.Windows.Media.Color.FromArgb(50, 2, 231, 2);
            mandibleOSP.LoadOSP();
            SetBinding(oriMandible, mandibleOSP, "Transform", HelixToolkit.Wpf.SharpDX.Model3D.TransformProperty, BindingMode.OneWay);
            MultiAngleViewModel.OSPModelCollection.Add(headOSP);
            MultiAngleViewModel.OSPModelCollection.Add(mandibleOSP);

            //標記屬於上顎的ID
            DraggableTriangle maxillaTargetTriangle = new DraggableTriangle(targetMaxilla.ModelCenter);
            maxillaTargetTriangle.MarkerID = "Maxilla";
            maxillaTargetTriangle.IsRendering = false;
            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTargetTriangle);

            //標記屬於下顎的ID
            DraggableTriangle mandibleTargetTriangle = new DraggableTriangle(targetMandible.ModelCenter);
            mandibleTargetTriangle.MarkerID = "Mandible";
            mandibleTargetTriangle.IsRendering = false;
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTargetTriangle);

            //將導航三角形綁定到導航的上顎
            DraggableTriangle maxillaTriangle = new DraggableTriangle(oriMaxilla.ModelCenter);
            maxillaTriangle.MarkerID = "Maxilla";
            maxillaTriangle.Transparency = 0.5f;
            maxillaTriangle.IsRendering = false;
            SetBinding(oriMaxilla, maxillaTriangle,"Transform" ,HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty,BindingMode.OneWay);
            MultiAngleViewModel.TriangleModelCollection.Add(maxillaTriangle);
      
            //將導航三角形綁定到導航的下顎
            DraggableTriangle mandibleTriangle = new DraggableTriangle(oriMandible.ModelCenter);
            mandibleTriangle.MarkerID = "Mandible";
            mandibleTriangle.Transparency = 0.7f;
            mandibleTriangle.IsRendering = false;
            SetBinding(oriMandible, mandibleTriangle, "Transform", HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty, BindingMode.OneWay);
            MultiAngleViewModel.TriangleModelCollection.Add(mandibleTriangle);

            MultiAngleViewModel.ResetCameraPosition();

            IsSet = true;

            _navigateView.Hide();
        }






        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string info = "")
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(null, new PropertyChangedEventArgs(info));
            }
        }
        protected static bool SetStaticValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")//CallerMemberName主要是.net4.5後定義好的caller訊息，能將訊息傳給後者的變數，目的在使用時不用特地傳入"Property"名稱
        {
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            OnStaticPropertyChanged(propertyName);
            return true;
        }

    }
}
