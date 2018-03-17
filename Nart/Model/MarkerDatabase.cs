using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Nart
{
    public class MarkerDatabase
    {
        public bool Isloaded = false;
        /// <summary>
        ///預設資料庫檔案路徑
        /// </summary>
        public string Filepath = "../../../data/MarkerDatabase.xml";
        /// <summary>
        ///使用一個Struct專門記錄三邊長與ID
        /// </summary>
        public struct MarkerData
        {
            public double[] ThreeLength;
            public string MarkerID;
        }
        /// <summary>
        ///使用一個List儲存Marker的三邊長與ID
        /// </summary>
        public List<MarkerData> MarkerInfo = new List<MarkerData>(10);
        /// <summary>
        ///頭在Database中引數，定義在CreateDatabase
        /// </summary>
        public int HeadIndex = -1;
        /// <summary>
        ///咬板在Database中引數，定義在CreateDatabase
        /// </summary>
        public int SplintIndex = -1;
        /// <summary>
        /// Marker的ID資料組成的List，創建來給ModelSettingItem中的combobox使用
        /// </summary>
        public static List<String> MarkerIdList = new List<String>();
        public MarkerDatabase()
        {            
            LoadDatabase();
        }
        /// <summary>
        /// 輸入Marker的data(三邊長)並將資料存進MarkerDB裡面
        /// </summary>
        public void LoadDatabase()
        {
            
            try
            {
                XmlDocument doc = new XmlDocument();
             
                doc.Load(Filepath);
                //先清空Marker資料內容
                MarkerInfo.Clear();
                //挑出Maxilla下面的Marker element
                XmlNodeList markerList = doc.SelectNodes("BWMarker/Marker");
                foreach (XmlNode oneNode in markerList)
                {
                    String id = oneNode.Attributes["ID"].Value;
                    //將新讀到的MarkerID加到清單裡面
                    MarkerIdList.Add(id);

                    XmlNodeList markerLength = oneNode.ChildNodes;

                    double[] lengthData = new double[3] { Convert.ToDouble(markerLength[0].InnerText)
                                                  , Convert.ToDouble(markerLength[1].InnerText)
                                                  , Convert.ToDouble(markerLength[2].InnerText)};
                    //存下Head Marker在Database中的Index
                    if (id.Equals("Head"))
                    {
                        HeadIndex = MarkerInfo.Count;
                    }
                    //存下Head Marker在Database中的Index
                    if (id.Equals("Splint"))
                    {
                        SplintIndex = MarkerInfo.Count;
                    }

                    MarkerData markerdata = new MarkerData();
                    markerdata.ThreeLength = lengthData;
                    markerdata.MarkerID = id;
                    MarkerInfo.Add(markerdata);
                }
                Isloaded = true;
            }
            catch
            {
                
                Isloaded = false;
                MessageBox.Show("資料庫檔案錯誤");
            }
            
        }
        /// <summary>
        /// 重製Head 跟 Splint的索引值
        /// </summary>
        public void ResetIndex()
        {
            HeadIndex = -1;
            SplintIndex = -1;
            for (int i = 0; i < MarkerInfo.Count; i++)
            {
                //存下Head Marker在Database中的Index
                if (MarkerInfo[i].MarkerID.Equals("Head"))
                {
                    HeadIndex = i;
                }
                //存下Splint Marker在Database中的Index
                if (MarkerInfo[i].MarkerID.Equals("Splint"))
                {
                    SplintIndex = i;
                }
            }
        }
    }


    

}
