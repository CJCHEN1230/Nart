using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace Nart
{
    class CamParam
    {
        public Matrix3D ExtParam; //exteranl parameters
        public Matrix3D InvExtParam; //exteranl parameters inverse
        public Matrix3D RotationInvert;
        

        public double FocalLength;
        public double Kappa1;
        public double Cx;
        public double Cy;
        public double Sx;

        public double Dx;//0.00375
        public double Dy;//0.00375
        public double Ncx;//1296
        public double Nfx;//1280
        public double Dpx;

        public CamParam(string path)
        {
            Dx = 0.0044;//0.00375
            Dy = 0.0044;//0.00375
            Ncx = 1628;//1296
            Nfx = 1600;//1280
            Dpx = Dx * Ncx / Nfx;
            LoadCalibrationFile(path);
            
        }


        private void LoadCalibrationFile(string path)
        {
            try
            {
                string fileContent = File.ReadAllText(path);//"../../../data/CaliR_L.txt"
                string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);

                string[] data = new string[20];
                Array.Copy(contentArray, 3, data, 0, 20);

                double[] cameraData = Array.ConvertAll(data, double.Parse);

                ExtParam = new Matrix3D(cameraData[0], cameraData[3], cameraData[6], 0,
                                       cameraData[1], cameraData[4], cameraData[7], 0,
                                       cameraData[2], cameraData[5], cameraData[8], 0,
                                       cameraData[9], cameraData[10], cameraData[11], 1);

                InvExtParam = new Matrix3D(cameraData[0], cameraData[3], cameraData[6], 0,
                                       cameraData[1], cameraData[4], cameraData[7], 0,
                                       cameraData[2], cameraData[5], cameraData[8], 0,
                                       cameraData[9], cameraData[10], cameraData[11], 1);
                InvExtParam.Invert();



                RotationInvert = new Matrix3D(cameraData[0], cameraData[1], cameraData[2], 0,
                                        cameraData[3], cameraData[4], cameraData[5], 0,
                                        cameraData[6], cameraData[7], cameraData[8], 0,
                                                    0,             0,             0, 1);


                FocalLength = cameraData[12];
                Kappa1 = cameraData[13];
                Cx = cameraData[14];                             
                Cy = cameraData[15];
                Sx = cameraData[19];

            }
            catch
            {
                MessageBox.Show("校正檔案路徑錯誤");
            }

            Console.WriteLine(ExtParam);
            Console.WriteLine(FocalLength);
            Console.WriteLine(Kappa1);
            Console.WriteLine(Cx);
            Console.WriteLine(Cy);
            Console.WriteLine(Sx);
            Console.WriteLine("\n\n\n");           
        }

        
    }
}
