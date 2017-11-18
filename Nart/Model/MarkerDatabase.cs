using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Nart
{
    public class MarkerDatabase
    {
        /// <summary>
        ///使用一個Struct專門記錄三邊長與ID
        /// </summary>
        public struct MarkerData
        {
            public double[] ThreeLength;
            public string ID;
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
        /// Marker的ID資料組成的List
        /// </summary>
        public static List<String> MarkerIDList = new List<String>();
        public MarkerDatabase()
        {            
            CreateDatabase();
        }
        /// <summary>
        /// 輸入Marker的data(三邊長)並將資料存進MarkerDB裡面
        /// </summary>
        private void CreateDatabase()
        {
            
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("../../../data/MarkerDatabase(1).xml");

                //挑出Maxilla下面的Marker element
                XmlNodeList markerList = doc.SelectNodes("BWMarker/Marker");
                foreach (XmlNode OneNode in markerList)
                {
                    String ID = OneNode.Attributes["ID"].Value;
                    //將新讀到的MarkerID加到清單裡面
                    MarkerIDList.Add(ID);

                    XmlNodeList markerLength = OneNode.ChildNodes;

                    double[] lengthData = new double[3] { Convert.ToDouble(markerLength[0].InnerText)
                                                  , Convert.ToDouble(markerLength[1].InnerText)
                                                  , Convert.ToDouble(markerLength[2].InnerText)};
                    //存下Head Marker在Database中的Index
                    if (ID.Equals("Head"))
                    {
                        HeadIndex = MarkerInfo.Count;
                    }
                    //存下Head Marker在Database中的Index
                    if (ID.Equals("Splint"))
                    {
                        SplintIndex = MarkerInfo.Count;
                    }

                    MarkerData markerdata = new MarkerData();
                    markerdata.ThreeLength = lengthData;
                    markerdata.ID = ID;
                    MarkerInfo.Add(markerdata);
                }
            }
            catch
            {
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
                if (MarkerInfo[i].ID.Equals("Head"))
                {
                    HeadIndex = i;
                }
                //存下Head Marker在Database中的Index
                if (MarkerInfo[i].ID.Equals("Splint"))
                {
                    SplintIndex = i;
                }
            }
        }
    }


    

}
