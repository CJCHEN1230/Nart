using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Nart
{
    public class Database
    {
        public struct MarkerData
        {
            public double[] ThreeLength;
            public string ID;
        }
        public  List<MarkerData> MarkerInfo = new List<MarkerData>(10);
        /// <summary>
        ///頭在Database中引數，定義在CreateDatabase
        /// </summary>
        public int HeadIndex = -1;
        /// <summary>
        ///咬板在Database中引數，定義在CreateDatabase
        /// </summary>
        public int SplintIndex = -1;
        /// <summary>
        ///上顎在Database中引數
        /// </summary>
        private int MaxillaIndex = -1;
        /// <summary>
        ///下顎在Database中引數
        /// </summary>
        private int MandibleIndex = -1;

        public Database()
        {
            
            CreateDatabase();

        }
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

                    XmlNodeList markerLength1 = OneNode.ChildNodes;


                    double[] data1 = new double[3] { Convert.ToDouble(markerLength1[0].InnerText)
                                                  , Convert.ToDouble(markerLength1[1].InnerText)
                                                  , Convert.ToDouble(markerLength1[2].InnerText)};
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
                    markerdata.ThreeLength = data1;
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
