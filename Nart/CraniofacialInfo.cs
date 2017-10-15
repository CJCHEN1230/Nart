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

                string[] Data = new string[7] { "PoL", "PoR", "OrL", "OrR", "Me", "GoL", "GoR" };

                Point3D[] PointData = new Point3D[7] { this.PoL, this.PoR, this.OrL, this.OrR, this.Me, this.GoL, this.GoR };

                int comma1 = -1;
                int comma2 = -1;
                int comma3 = -1;
                int lineTerminator = -1;

                for (int i = 0; i < PointData.Length; i++)
                {
                    int DataIndex = fileContent.IndexOf(Data[i]);

                    comma1 = fileContent.IndexOf(", ", DataIndex);
                    comma2 = fileContent.IndexOf(", ", comma1 + 1);
                    comma3 = fileContent.IndexOf(", ", comma2 + 1);
                    lineTerminator = fileContent.IndexOf("\r\n", comma3 + 1);
                    PointData[i].X = Convert.ToDouble(fileContent.Substring(comma1 + 2, comma2 - comma1 - 2));
                    PointData[i].Y = Convert.ToDouble(fileContent.Substring(comma2 + 2, comma3 - comma2 - 2));
                    PointData[i].Z = Convert.ToDouble(fileContent.Substring(comma3 + 2, lineTerminator - comma3 - 2));                    
                }


                this.PoL = PointData[0];
                this.PoR = PointData[1];
                this.OrL = PointData[2];
                this.OrR = PointData[3];
                this.Me = PointData[4];
                this.GoL = PointData[5];
                this.GoR = PointData[6];
                
            }
            catch
            {
                MessageBox.Show("Ceph檔案錯誤");
            }
            
        }
    }
}
