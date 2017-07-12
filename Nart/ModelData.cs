using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Nart
{
    public class ModelData
    {
        private String _filename;

        private ModelVisual3D _modelVisual = new ModelVisual3D();

        private Model3DGroup _modleGroup;

        private Matrix3D ModelTransform;

        public int DatabaseIndex = -1;

        public ModelData(String filename)
        {
            _filename = filename;

            Display3d(_filename);

            Rect3D rect3d = _modleGroup.Bounds;

            BoundingCenter = new Point3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);

            GeometryModel3D Geomodel = _modleGroup.Children[0] as GeometryModel3D;

            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(40, 181, 187)));

            Geomodel.Material = material;

            Geomodel.BackMaterial = material;

            _modelVisual.Content = _modleGroup;
        }

        public Point3D BoundingCenter
        {
            get;
            set;
        }

        public ModelVisual3D ModelVisual
        {
            get
            {
                return _modelVisual;
            }
            set
            {
                _modelVisual = value;
            }
        }

        public String Filename
        {
            get;
            set;
        }
             

        private void Display3d(string model)
        {            
            try
            {                
                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //Load the 3D model file
                _modleGroup = import.Load(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            
        }
    }
}
