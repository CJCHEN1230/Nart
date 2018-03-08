using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public static class SystemData
    {

        ///<summary>
        ///頭在註冊資料中引數
        /// </summary>
        public static int RegHeadIndex = -1;
        ///<summary>
        ///咬板在註冊資料中引數
        /// </summary>
        public static int RegSplintIndex = -1;
        public static  CamParam[] CameraParam = new CamParam[2];
        public static MarkerDatabase MarkerData = new MarkerDatabase();

        static SystemData()
        {
            CameraParam[0] = new CamParam("../../../data/CaliR_L.txt");
            CameraParam[1] = new CamParam("../../../data/CaliR_R.txt");
        }

    }
}
