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

            ambientLightColor = Color.FromScRgb(1.0f, 0.4f, 0.4f, 0.4f);
            directionalLightColor1 = Color.FromScRgb(1.0f, 0.1f, 0.1f, 0.1f);
            directionalLightColor2 = Color.FromScRgb(1.0f, 0.1f, 0.1f, 0.1f);
            directionalLightColor3 = Color.FromScRgb(1.0f, 0.1f, 0.1f, 0.1f);

            LightSetting();
        }

        internal void SetCamera()
        {

            var rect3d = _window.mainModelVisual.FindBounds(_window.mainModelVisual.Transform);

            Point3D Center = new Point3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);
            _window.OrthographicCam.Position = new Point3D(Center.X, Center.Y - (rect3d.SizeY), Center.Z);
            _window.OrthographicCam.UpDirection = new Vector3D(0, 0, 1);
            _window.OrthographicCam.LookDirection= new Vector3D(0, rect3d.SizeY, 0);
            _window.OrthographicCam.NearPlaneDistance = -500;
            _window.OrthographicCam.Width = rect3d.SizeX + 150;
            
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

        /// <summary>
        /// Directional Light 2
        /// </summary>
        private Color directionalLightColor3;
        /// <summary>
        /// Directional Light 2
        /// </summary>
        public Color DirectionalLightColor3
        {
            set
            {
                directionalLightColor3 = value;
                DirectionalLight light = ((Model3DGroup)_window.LightModel.Content).Children[2] as DirectionalLight;
                light.Color = directionalLightColor3;
            }
            get
            {
                return directionalLightColor3;
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
        /// <summary>
        /// 設定光位置
        /// </summary>
        private void LightSetting()
        {
            AmbientLight myAmbientLight = new AmbientLight();
            myAmbientLight.Color = AmbientLightColor;

            DirectionalLight myDirectionalLight1 = new DirectionalLight();            
            myDirectionalLight1.Color = DirectionalLightColor1;
            myDirectionalLight1.Direction = new Vector3D(-1.0, 0.5, -0.6);
            
            DirectionalLight myDirectionalLight2 = new DirectionalLight();
            myDirectionalLight2.Color = DirectionalLightColor2;
            myDirectionalLight2.Direction = new Vector3D(1.0, -0.5, 0.6);

            DirectionalLight myDirectionalLight3 = new DirectionalLight();
            myDirectionalLight3.Color = DirectionalLightColor3;
            myDirectionalLight3.Direction = new Vector3D(1.0, -0.5, -0.6);



            Model3DGroup myLightGroup = new Model3DGroup();
            myLightGroup.Children.Add(myAmbientLight);
            myLightGroup.Children.Add(myDirectionalLight1);
            myLightGroup.Children.Add(myDirectionalLight2);

            _window.LightModel.Content = myLightGroup;

            _window.helixViewport.Children.Add(_window.LightModel);
        }


    }
}
