using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    public class CalcCoord
    {

        public  Matrix3D exParam;

        public double FocalLength;
        public double Kappa1;
        public double Cx;
        public double Cy;
        public double Sx;

        public CalcCoord()
        {
            
            
        }
        public void  LoadCalibrationFile()
        {
            string fileContent = File.ReadAllText("../../../data/CaliR_L.txt");
            string[] contentArray = fileContent.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
            
            string[] data = new string[20];
            Array.Copy(contentArray, 3, data, 0, 20);

            double[] cameraData = Array.ConvertAll(data, double.Parse);

            

            exParam = new Matrix3D(cameraData[0], cameraData[1], cameraData[2], 0,
                                   cameraData[3], cameraData[4], cameraData[5], 0,
                                   cameraData[6], cameraData[7], cameraData[8], 0,
                                   cameraData[9], cameraData[10], cameraData[11], 1);


            FocalLength = cameraData[12];						
            Kappa1 = cameraData[13]; 
            Cx = cameraData[14]; 
            Cy = cameraData[15];
            Sx = cameraData[19];



            Console.WriteLine(exParam);
            Console.WriteLine(FocalLength);
            Console.WriteLine(Kappa1);
            Console.WriteLine(Cx);
            Console.WriteLine(Cy);
            Console.WriteLine(Sx);

            Console.Read();


        }

    }
}
