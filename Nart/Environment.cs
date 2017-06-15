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
    class Environment
    {

        private MainWindow _window = null;
        

        public Environment(MainWindow window)
        {
            _window = window;
            //backgroundColor = Colors.Black;
            SetCamera();
            LightSetting();
        }

        internal void SetCamera()
        {


            //_window.OrthographicCam.LookAt(environmentSetting.BoundingBox_center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 1.0);
            _window.OrthographicCam.LookAt(new Point3D(0,0,0), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 1.0);
            //double dx = environmentSetting.BoundingBox_max.X - environmentSetting.BoundingBox_min.X;
            //double dy = environmentSetting.BoundingBox_max.Y - environmentSetting.BoundingBox_min.Y;
            //double dz = environmentSetting.BoundingBox_max.Z - environmentSetting.BoundingBox_min.Z;

            //_window.OrthographicCam.Width = Math.Max(dx, Math.Max(dy, dz)) * 1.5;
            //_window.OrthographicCam.NearPlaneDistance = environmentSetting.BoundingBox_min.Y - dy * 15.0;

            _window.OrthographicCam.Width = 500;
            _window.OrthographicCam.NearPlaneDistance = -200;


            _window.helixViewport.Camera = _window.OrthographicCam;

        }

        private Color ambientLightColor;

        public Color AmbientLightColor
        {
            get
            {
                return ambientLightColor;
            }
            set
            {
                
                ambientLightColor = value;
                AmbientLight light = ((Model3DGroup)_window.LightModel.Content).Children[0] as AmbientLight;
                light.Color = ambientLightColor;
            }
        }

        /// <summary>
        /// Directional Light 1
        /// </summary>
        private Color directionalLightColor1;
        /// <summary>
        /// Directional Light 1
        /// </summary>
        public Color DirectionalLightColor1
        {
            set
            {
                directionalLightColor1 = value;
                DirectionalLight light = ((Model3DGroup)_window.LightModel.Content).Children[1] as DirectionalLight;
                light.Color = directionalLightColor1;
            }
            get
            {
                return directionalLightColor1;
            }
        }

        /// <summary>
        /// Directional Light 2
        /// </summary>
        private Color directionalLightColor2;
        /// <summary>
        /// Directional Light 2
        /// </summary>
        public Color DirectionalLightColor2
        {
            set
            {
                directionalLightColor2 = value;
                DirectionalLight light = ((Model3DGroup)_window.LightModel.Content).Children[2] as DirectionalLight;
                light.Color = directionalLightColor2;
            }
            get
            {
                return directionalLightColor2;
            }
        }


        private Color backgroundColor;

        public Color BackgroundColor
        {
            set
            {
                backgroundColor = value;
                // 同時修改背景色彩
                _window.helixViewport.Background = new SolidColorBrush(backgroundColor);
            }
            get
            {
                return backgroundColor;
            }
        }


        private Point3D boundingBoxCenter;

        public Point3D BoundingBoxCenter
        {
            set
            {
                boundingBoxCenter = value;
            }
            get
            {
                return boundingBoxCenter;
            }           
        }

        private void LightSetting()
        {
            AmbientLight myAmbientLight = new AmbientLight();
            myAmbientLight.Color = AmbientLightColor;

            DirectionalLight myDirectionalLight1 = new DirectionalLight();            
            myDirectionalLight1.Color = DirectionalLightColor1;
            myDirectionalLight1.Direction = new Vector3D(-1.0, 0.0, -0.6);

            DirectionalLight myDirectionalLight2 = new DirectionalLight();
            myDirectionalLight2.Color = DirectionalLightColor2;
            myDirectionalLight2.Direction = new Vector3D(1.0, 0.0, -0.6);

            Model3DGroup myLightGroup = new Model3DGroup();
            myLightGroup.Children.Add(myAmbientLight);
            myLightGroup.Children.Add(myDirectionalLight1);
            myLightGroup.Children.Add(myDirectionalLight2);

            _window.LightModel.Content = myLightGroup;

            _window.helixViewport.Children.Add(_window.LightModel);
        }


    }
}
