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
    public class NavigateViewModel : ObservableObject
    {
        #region intermediate部分
        /// <summary>
        /// 中間上顎轉移矩陣
        /// </summary>
        private String ITMMaxillaMatrix = "";
        /// <summary>
        /// 中間下顎轉移矩陣
        /// </summary>
        private String ITMMandibleMatrix = "";
        /// <summary>
        /// 中間上顎規劃後模型
        /// </summary>
        private String ITMMaxillaPlanModel = "";
        /// <summary>
        /// 中間下顎規劃後模型
        /// </summary>
        private String ITMMandiblePlanModel = "";
        #endregion
        
        #region final部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String finalMaxillaMatrix = "";
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String finalMandibleMatrix = "";
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private String finalMaxillaPlanModel = "";
        /// <summary>
        /// 最後下顎規劃後模型
        /// </summary>
        private String finalMandiblePlanModel = "";
        #endregion

        #region 模型設定部分
        /// <summary>
        /// 最後上顎轉移矩陣
        /// </summary>
        private String headModel = "";
        /// <summary>
        /// 最後下顎轉移矩陣
        /// </summary>
        private String maxillaModel = "";
        /// <summary>
        /// 最後上顎規劃後模型
        /// </summary>
        private String mandibleModel = "";
        #endregion
        
        /// <summary>
        /// 頭部模型顏色
        /// </summary>
        private Color headDiffuseColor;
        /// <summary>
        /// 上顎模型顏色
        /// </summary>
        private Color maxillaDiffuseColor;
        /// <summary>
        /// 下顎模型顏色
        /// </summary>
        private Color mandibleDiffuseColor;
        private string firstNavigation;







        public NavigateViewModel()
        {
          
           
        }







        
        public String IntermediateMaxillaMatrix
        {
            get
            {
                return ITMMaxillaMatrix;
            }
            set
            {
                SetValue(ref ITMMaxillaMatrix, value);
            }
        }

        public String IntermediateMandibleMatrix
        {
            get
            {
                return ITMMandibleMatrix;
            }
            set
            {
                SetValue(ref ITMMandibleMatrix, value);
            }
        }

        public String IntermediateMaxillaPlanModel
        {
            get
            {
                return ITMMaxillaPlanModel;
            }
            set
            {
                SetValue(ref ITMMaxillaPlanModel, value);
            }
        }


        public String IntermediateMandiblePlanModel
        {
            get
            {
                return ITMMandiblePlanModel;
            }
            set
            {
                SetValue(ref ITMMandiblePlanModel, value);
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

        public String FinalMandibleMatrix
        {
            get
            {
                return finalMandibleMatrix;
            }
            set
            {
                SetValue(ref finalMandibleMatrix, value);
            }
        }

        public String FinalMaxillaPlanModel
        {
            get
            {
                return finalMaxillaPlanModel;
            }
            set
            {
                SetValue(ref finalMaxillaPlanModel, value);
            }
        }

        public String FinalMandiblePlanModel
        {
            get
            {
                return finalMandiblePlanModel;
            }
            set
            {
                SetValue(ref finalMandiblePlanModel, value);
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
