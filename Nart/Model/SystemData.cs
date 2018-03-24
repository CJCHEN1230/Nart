using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public static class SystemData
    {
        public static bool IsFirstStage = false;
        public static bool IsSecondStage = false;
        public static bool IsFinished = false;
        public static MarkerDatabase MarkerData;
        /// <summary>
        /// 註冊狀態，註冊當下會開啟，註冊完之後會自動關閉
        /// </summary>
        public static bool RegToggle = false;
        /// <summary>
        /// 追蹤狀態
        /// </summary>
        public static bool TrackToggle = false;
        ///<summary>
        ///頭在註冊資料中引數
        /// </summary>
        public static int RegHeadIndex = -1;
        ///<summary>
        ///咬板在註冊資料中引數
        /// </summary>
        public static int RegSplintIndex = -1;
        ///<summary>
        ///兩相機參數
        ///</summary>
        public static  CamParam[] CameraParam = new CamParam[2];
        static SystemData()
        {
            CameraParam[0] = new CamParam("./data/CaliR_L.txt");
            CameraParam[1] = new CamParam("./data/CaliR_R.txt");
            MarkerData = new MarkerDatabase();
        }

    }
}
