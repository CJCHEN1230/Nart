using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Nart
{
    /// <summary>
    /// 此類別讀進面部資訊的檔案Go Me FH
    /// </summary>
    public class CraniofacialInfo
    {
        public Point3D PoL = new Point3D();
        public Point3D PoR = new Point3D();
        public Point3D OrL = new Point3D();
        public Point3D OrR = new Point3D();
        public Point3D Me = new Point3D();
        public Point3D GoL = new Point3D();
        public Point3D GoR = new Point3D();
        public Point3D GoIntersection;   // Go點連線與下顎對稱面的交點


        public CraniofacialInfo(string path)
        {
            LoadCranioFacialInfo(path);
        }

        private void LoadCranioFacialInfo(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);

                string[] data = new string[7] { "PoL", "PoR", "OrL", "OrR", "Me", "GoL", "GoR" };

                Point3D[] pointData = new Point3D[7] { this.PoL, this.PoR, this.OrL, this.OrR, this.Me, this.GoL, this.GoR };

                int comma1 = -1;
                int comma2 = -1;
                int comma3 = -1;
                int lineTerminator = -1;

                for (int i = 0; i < pointData.Length; i++)
                {
                    int dataIndex = fileContent.IndexOf(data[i]);

                    comma1 = fileContent.IndexOf(", ", dataIndex);
                    comma2 = fileContent.IndexOf(", ", comma1 + 1);
                    comma3 = fileContent.IndexOf(", ", comma2 + 1);
                    lineTerminator = fileContent.IndexOf("\r\n", comma3 + 1);
                    pointData[i].X = Convert.ToDouble(fileContent.Substring(comma1 + 2, comma2 - comma1 - 2));
                    pointData[i].Y = Convert.ToDouble(fileContent.Substring(comma2 + 2, comma3 - comma2 - 2));
                    pointData[i].Z = Convert.ToDouble(fileContent.Substring(comma3 + 2, lineTerminator - comma3 - 2));                    
                }


                this.PoL = pointData[0];
                this.PoR = pointData[1];
                this.OrL = pointData[2];
                this.OrR = pointData[3];
                this.Me = pointData[4];
                this.GoL = pointData[5];
                this.GoR = pointData[6];
                
            }
            catch
            {
                MessageBox.Show("Ceph檔案錯誤");
            }
            
        }
    }
}
