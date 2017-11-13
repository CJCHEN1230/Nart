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
    using Model_Object;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Color = System.Windows.Media.Color;
    public class NavigateViewModel : ObservableObject
    {

        #region N-Art計畫部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String plannedMaxillaMatrix = "../../../data/蔡慧君/原始位置轉到規劃後位置上顎_matrix.txt";
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String plannedMandibleMatrix = "../../../data/蔡慧君/原始位置轉到規劃後位置下顎_matrix.txt";
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private String plannedMaxilla = "../../../data/蔡慧君/nart規劃後 finalmaxilla.stl";
        /// <summary>
        /// 最後下顎規劃後模型
        /// </summary>
        private String plannedMandible = "D://Desktop//Nart//data//蔡慧君//nart規劃後 finalmandible.stl";
        #endregion


        #region 原始模型設定部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String headModel = "../../../data/蔡慧君/skull.stl";
        /// <summary>
        /// 頭部模型顏色
        /// </summary>
        private Color headDiffuseColor = Color.FromArgb(255, 40, 181, 187);
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String maxillaModel = "../../../data/蔡慧君/original_maxilla.stl";
        /// <summary>
        /// 上顎模型顏色
        /// </summary>
        private Color maxillaDiffuseColor = Color.FromArgb(255, 40, 181, 187);
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private String mandibleModel = "../../../data/蔡慧君/original_mandible.stl";
        /// <summary>
        /// 下顎模型顏色
        /// </summary>
        private Color mandibleDiffuseColor = Color.FromArgb(255, 40, 181, 187);
        #endregion


        /// <summary>
        /// 中間咬板對位的轉移矩陣
        /// </summary>
        private String intermediateMaxillaMatrix = "../../../data/蔡慧君/intermediatemoved_Matrix.txt";
        /// <summary>
        /// 最後咬板對位的轉移矩陣
        /// </summary>
        private String finalMaxillaMatrix = "../../../data/蔡慧君/finalmoved_Matrix.txt";
        /// <summary>
        /// 導引順序先開上顎or先開下顎
        /// </summary>
        private string firstNavigation = "Maxilla";
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
        public String PlannedMaxilla
        {
            get
            {
                return plannedMaxilla;
            }
            set
            {
                SetValue(ref plannedMaxilla, value);
            }
        }
        public String PlannedMandible
        {
            get
            {
                return plannedMandible;
            }
            set
            {
                SetValue(ref plannedMandible, value);
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
        public String ITMMaxillaMatrix
        {
            get
            {
                return intermediateMaxillaMatrix;
            }
            set
            {
                SetValue(ref intermediateMaxillaMatrix, value);
            }
        }
        public String FinalMaxillaMatrix
        {
            get
            {
                return finalMaxillaMatrix;
            }
            set
            {
                SetValue(ref finalMaxillaMatrix, value);
            }
        }
        public string FirstNavigation
        {
            get
            {
                return firstNavigation;
            }
            set
            {
                SetValue(ref firstNavigation, value);
            }
        }
        public ICommand ModelSettingCommand { private set; get; }

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

            BoneModel Bone1 = new BoneModel();
            Bone1.FilePath = HeadModel;
            Bone1.MarkerID = "Head";
            Bone1.DiffuseColor = HeadDiffuseColor;
            Bone1.LoadModel();

            BoneModel Bone2 = new BoneModel();
            Bone2.FilePath = PlannedMaxilla;
            Bone2.MarkerID = "A";
            Bone2.DiffuseColor = Color.FromArgb(255, 100, 100, 100);
            Bone2.LoadModel();

            BoneModel Bone3 = new BoneModel();
            Bone3.FilePath = PlannedMandible;
            Bone3.MarkerID = "Splint";
            Bone3.DiffuseColor = Color.FromArgb(255, 100, 100, 100);
            Bone3.LoadModel();



            BoneModel Bone4 = new BoneModel();
            Bone4.FilePath = MaxillaModel;
            Bone4.MarkerID = "A";
            Bone4.DiffuseColor = MaxillaDiffuseColor;
            Bone4.LoadModel();
            

            BoneModel Bone5 = new BoneModel();
            Bone5.FilePath = MandibleModel;
            Bone5.MarkerID = "C";
            Bone5.DiffuseColor = MandibleDiffuseColor;
            Bone5.LoadModel();



            Matrix plannedMatrix = ReadMatrixFile(plannedMaxillaMatrix);
            Matrix plannedMandible = ReadMatrixFile(plannedMandibleMatrix);

            
            Bone5.PushMatrix(plannedMandible);

            System.Windows.Media.Media3D.Matrix3D temp = new System.Windows.Media.Media3D.Matrix3D();

            temp.M11 = 1; temp.M12 = 0; temp.M13 = 0; temp.M14 = 0;
            temp.M21 = 0; temp.M22 = 1; temp.M23 = 0; temp.M24 = 0;
            temp.M31 = 0; temp.M32 = 0; temp.M33 = 1; temp.M34 = 0;
            temp.OffsetX = 100; temp.OffsetY = -100; temp.OffsetZ = 100; temp.M44 = 1;


            //Bone4.Transform = new System.Windows.Media.Media3D.MatrixTransform3D(temp);

            Bone4.PushMatrix(plannedMatrix);

            MultiAngleViewModel.BoneModelCollection.Add(Bone1);
          //  MultiAngleViewModel.BoneModelCollection.Add(Bone2);
            MultiAngleViewModel.BoneModelCollection.Add(Bone3);
            MultiAngleViewModel.BoneModelCollection.Add(Bone4);
            MultiAngleViewModel.BoneModelCollection.Add(Bone5);







            MultiAngleViewModel.ResetCameraPosition();

            _navigateView.Hide();
        }
        

    }
}
