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

        private CamParam[] _camParam = new CamParam[2];

        public Point4D[] LensCenter = new Point4D[2];


        public CalcCoord()
        {
            _camParam[0] = new CamParam("../../../data/CaliR_L.txt");
            _camParam[1] = new CamParam("../../../data/CaliR_R.txt");
            CalcLensCenter();
        }
        
        private void CalcLensCenter()
        {
            LensCenter[0] = new Point4D(0, 0, 0, 1);
            LensCenter[1] = new Point4D(0, 0, 0, 1);
        
            //Console.WriteLine("Rotation:\n" +
            //    _camParam[0].Rotation.M11 + "\t" + _camParam[0].Rotation.M12 + "\t" + _camParam[0].Rotation.M13 + "\t" + _camParam[0].Rotation.M14 + "\n" +
            //    _camParam[0].Rotation.M21 + "\t" + _camParam[0].Rotation.M22 + "\t" + _camParam[0].Rotation.M23 + "\t" + _camParam[0].Rotation.M24 + "\n" +
            //    _camParam[0].Rotation.M31 + "\t" + _camParam[0].Rotation.M32 + "\t" + _camParam[0].Rotation.M33 + "\t" + _camParam[0].Rotation.M34 + "\n" +
            //    _camParam[0].Rotation.OffsetX + "\t" + _camParam[0].Rotation.OffsetY + "\t" + _camParam[0].Rotation.OffsetZ + "\t" + _camParam[0].Rotation.M44 + "\n");

            //Console.WriteLine("Translation invert之前:\n" +
            //    _camParam[0].Translation.M11 + "\t" + _camParam[0].Translation.M12 + "\t" + _camParam[0].Translation.M13 + "\t" + _camParam[0].Translation.M14 + "\n" +
            //    _camParam[0].Translation.M21 + "\t" + _camParam[0].Translation.M22 + "\t" + _camParam[0].Translation.M23 + "\t" + _camParam[0].Translation.M24 + "\n" +
            //    _camParam[0].Translation.M31 + "\t" + _camParam[0].Translation.M32 + "\t" + _camParam[0].Translation.M33 + "\t" + _camParam[0].Translation.M34 + "\n" +
            //    _camParam[0].Translation.OffsetX + "\t" + _camParam[0].Translation.OffsetY + "\t" + _camParam[0].Translation.OffsetZ + "\t" + _camParam[0].Translation.M44 + "\n");


            _camParam[0].extParam.Invert();
            _camParam[1].extParam.Invert();

            LensCenter[0] = _camParam[0].extParam.Transform(LensCenter[0]);
            LensCenter[1] = _camParam[1].extParam.Transform(LensCenter[1]);
            


        }
    }

    
}
