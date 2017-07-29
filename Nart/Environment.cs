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
    public class Environment
    {

        private MainView _window = null;

        private ModelVisual3D LightModel = new ModelVisual3D();
        public Environment(MainView window)
        {
            _window = window;
            //backgroundColor = Colors.Black;
            SetCamera();

            ambientLightColor = Color.FromScRgb(1.0f, 0.3f, 0.3f, 0.3f);
            directionalLightColor1 = Color.FromScRgb(1.0f, 0.7f, 0.7f, 0.7f);
            directionalLightColor2 = Color.FromScRgb(1.0f, 0.7f, 0.7f, 0.7f);
            directionalLightColor3 = Color.FromScRgb(1.0f, 0.7f, 0.7f, 0.7f);

            LightSetting();
        }

        internal void SetCamera()
        {

            var rect3d = _window.mainModelVisual.FindBounds(_window.mainModelVisual.Transform);

            Point3D Center = new Point3D(rect3d.X + rect3d.SizeX / 2.0, rect3d.Y + rect3d.SizeY / 2.0, rect3d.Z + rect3d.SizeZ / 2.0);
            _window.OrthographicCam1.Position = new Point3D(Center.X, Center.Y - (rect3d.SizeY), Center.Z);
            _window.OrthographicCam1.UpDirection = new Vector3D(0, 0, 1);
            _window.OrthographicCam1.LookDirection= new Vector3D(0, rect3d.SizeY, 0);
            _window.OrthographicCam1.NearPlaneDistance = -500;
            _window.OrthographicCam1.Width = rect3d.SizeX + 110;


            var rect3d2 = _window.mainModelVisual1.FindBounds(_window.mainModelVisual.Transform);

            Point3D Center2 = new Point3D(rect3d2.X + rect3d2.SizeX / 2.0, rect3d2.Y + rect3d2.SizeY / 2.0, rect3d2.Z + rect3d2.SizeZ / 2.0);
            _window.OrthographicCam2.Position = new Point3D(Center2.X, Center2.Y, Center2.Z + (rect3d2.SizeZ));
            _window.OrthographicCam2.UpDirection = new Vector3D(0, 1, 0);
            _window.OrthographicCam2.LookDirection = new Vector3D(0, 0, -rect3d2.SizeZ);
            _window.OrthographicCam2.NearPlaneDistance = -500;
            _window.OrthographicCam2.Width = rect3d.SizeX + 110;



            var rect3d3 = _window.mainModelVisual1.FindBounds(_window.mainModelVisual.Transform);
            Point3D Center3 = new Point3D(rect3d3.X + rect3d3.SizeX / 2.0, rect3d3.Y + rect3d3.SizeY / 2.0, rect3d3.Z + rect3d3.SizeZ / 2.0);
            _window.OrthographicCam3.Position = new Point3D(Center3.X - (rect3d3.SizeX), Center3.Y, Center3.Z);
            _window.OrthographicCam3.UpDirection = new Vector3D(0, 0, 1);
            _window.OrthographicCam3.LookDirection = new Vector3D(rect3d3.SizeX, 0, 0);
            _window.OrthographicCam3.NearPlaneDistance = -500;
            _window.OrthographicCam3.Width = rect3d.SizeX + 110;



        }

        private Color ambientLightColor;

        public Color AmbientLightColor
        {
            set
            {
                ambientLightColor = value;
                AmbientLight light = ((Model3DGroup)LightModel.Content).Children[0] as AmbientLight;
                light.Color = ambientLightColor;
            }
            get
            {
                return ambientLightColor;
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
                DirectionalLight light = ((Model3DGroup)LightModel.Content).Children[1] as DirectionalLight;
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
                DirectionalLight light = ((Model3DGroup)LightModel.Content).Children[2] as DirectionalLight;
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
                DirectionalLight light = ((Model3DGroup)LightModel.Content).Children[2] as DirectionalLight;
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
            myDirectionalLight3.Direction = new Vector3D(-0.5, -0.5, -0.6);


            DirectionalHeadLight headlight1 = new DirectionalHeadLight();
            headlight1.Brightness = 20;
            headlight1.Position = new Point3D(30, 0, 10);


            DirectionalHeadLight headlight2 = new DirectionalHeadLight();
            headlight1.Brightness =20;
            headlight1.Position = new Point3D(30, 0, 10);


            _window.helixViewport.Children.Add(headlight1);


            DirectionalHeadLight headlight3 = new DirectionalHeadLight();
            headlight3.Brightness = 20;
            headlight3.Position = new Point3D(30, 0, 10);
            _window.helixViewport2.Children.Add(headlight3);



            DirectionalHeadLight headlight5 = new DirectionalHeadLight();
            headlight5.Brightness = 20;
            headlight5.Position = new Point3D(30, 0, 10);
            _window.helixViewport3.Children.Add(headlight5);
            //DirectionalHeadLight headlight4 = new DirectionalHeadLight();
            //headlight1.Brightness = 20;
            //headlight1.Position = new Point3D(80, 0, 80);
            //_window.helixViewport3.Children.Add(headlight4);




            //_window.helixViewport.Children.Add(headlight2);

            //Model3DGroup myLightGroup = new Model3DGroup();
            //myLightGroup.Children.Add(myAmbientLight);
            //myLightGroup.Children.Add(myDirectionalLight1);
            //myLightGroup.Children.Add(myDirectionalLight2);
            ////myLightGroup.Children.Add(myDirectionalLight3);
            //_window.LightModel.Content = myLightGroup;

            //_window.helixViewport.Children.Add(_window.LightModel);
        }


    }
}
