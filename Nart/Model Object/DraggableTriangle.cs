using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Input;

namespace Nart.Model_Object
{
    using MatrixTransform3D = System.Windows.Media.Media3D.MatrixTransform3D;
    using Matrix = SharpDX.Matrix;
    using HelixToolkit.Wpf;
    using Matrix3DExtensions = Nart.ExtensionMethods.Matrix3DExtensions;
    using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;

    public class DraggableTriangle : GroupModel3D, IHitable
    {

        public int length = 100;
        //一開始三顆球的初始位置
        private Vector3[] positions;
        private DraggableGeometryModel3D[] cornerHandles = new DraggableGeometryModel3D[3];
        private MeshGeometryModel3D[] edgeHandles = new MeshGeometryModel3D[3];

        private bool isCaptured;
        private Viewport3DX viewport;
        private Camera camera;
        private System.Windows.Media.Media3D.Point3D lastHitPos;
        private MatrixTransform3D dragTransform;
        private double InitialAngle = 30;

        private static Geometry3D NodeGeometry;
        private static Geometry3D EdgeGeometry;

        static DraggableTriangle()
        {
            //建立實體幾何
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0.0f, 0.0f, 0), 8);
            NodeGeometry = b1.ToMeshGeometry3D();

            var b2 = new MeshBuilder();
            b2.AddCylinder(new Vector3(0, 0, 0), new Vector3(1, 0, 0), 3, 32, true, true);
            EdgeGeometry = b2.ToMeshGeometry3D();
        }

    
        public DraggableTriangle()
        {
            this.Material = PhongMaterials.White;
            positions = new Vector3[3]
                 {

                    new Vector3(Convert.ToSingle(Math.Cos(InitialAngle/180f*Math.PI))*length,Convert.ToSingle(Math.Sin(InitialAngle/180f*Math.PI))*length,0),
                    new Vector3(Convert.ToSingle(Math.Cos((InitialAngle+120)/180f*Math.PI))*length,Convert.ToSingle(Math.Sin((InitialAngle+120)/180f*Math.PI))*length,0),
                    new Vector3(Convert.ToSingle(Math.Cos((InitialAngle+240)/180f*Math.PI))*length,Convert.ToSingle(Math.Sin((InitialAngle+240)/180f*Math.PI))*length,0),
                    
                 };
            for (int i = 0; i < 3; i++)
            {
                //平移圓球
                var translateMat = Matrix3DExtensions.Translate3D(positions[i]);
                cornerHandles[i] = new DraggableGeometryModel3D()
                {
                    Visibility = Visibility.Visible,
                    Material = this.Material,
                    Geometry = NodeGeometry,
                    Transform = new MatrixTransform3D(translateMat),
                };
                //定義球的滑鼠事件
                this.cornerHandles[i].MouseMove3D += OnNodeMouse3DMove;
                this.cornerHandles[i].MouseUp3D += OnNodeMouse3DUp;
                this.cornerHandles[i].MouseDown3D += OnNodeMouse3DDown;

           
                //兩點組成向量
                Vector3 v1 = positions[(i + 1) % 3] - positions[i];
                //圓柱初始擺放位置在X軸上，所以向量為X
                Vector3 xBar = new Vector3(1, 0, 0);
                Vector3 rotateAxis = Vector3.Cross(xBar, v1);
                rotateAxis.Normalize();
                if (rotateAxis.IsZero)
                {
                    rotateAxis.X = 0; rotateAxis.Y = 0; rotateAxis.Z = 1;
                }

                float theta = Convert.ToSingle(Math.Acos(Vector3.Dot(xBar, v1) / (v1.Length() * xBar.Length())));
                Matrix rotateMat = Matrix.RotationAxis(rotateAxis, theta);
                Matrix m = Matrix.Scaling(v1.Length(), 1, 1) * rotateMat * Matrix.Translation(positions[i]);
                this.edgeHandles[i] = new MeshGeometryModel3D()
                {
                    Geometry = EdgeGeometry,
                    Material = this.Material,
                    Visibility = Visibility.Visible,
                    Transform = new MatrixTransform3D(m.ToMatrix3D())
                };
                this.edgeHandles[i].MouseMove3D += OnEdgeMouse3DMove;
                this.edgeHandles[i].MouseUp3D += OnEdgeMouse3DUp;
                this.edgeHandles[i].MouseDown3D += OnEdgeMouse3DDown;


                this.Children.Add(cornerHandles[i]);
                this.Children.Add(edgeHandles[i]);
            }

            cornerHandles[0].Material = PhongMaterials.Red;
            cornerHandles[1].Material = PhongMaterials.Green;
            cornerHandles[2].Material = PhongMaterials.Blue;

            this.dragTransform = new MatrixTransform3D(this.Transform.Value);
        }


        private void OnNodeMouse3DDown(object sender, RoutedEventArgs e)
        {
            var args = e as Mouse3DEventArgs;
            if (args == null) return;
            if (args.Viewport == null) return;

            this.isCaptured = true;
        }

        private void OnNodeMouse3DUp(object sender, RoutedEventArgs e)
        {

            if (this.isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Arrow;               
            }
        }

        private void OnNodeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (this.isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Hand;
                UpdateTransforms(sender);
            }
        }

        private void OnEdgeMouse3DDown(object sender, RoutedEventArgs e)
        {
            var args = e as Mouse3DEventArgs;
            if (args == null) return;
            if (args.Viewport == null) return;

            this.isCaptured = true;
            this.viewport = args.Viewport;
            this.camera = args.Viewport.Camera;
            this.lastHitPos = args.HitTestResult.PointHit;
        }

        private void OnEdgeMouse3DUp(object sender, RoutedEventArgs e)
        {
            if (this.isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
                this.isCaptured = false;
                this.camera = null;
                this.viewport = null;
            }
        }

        private void OnEdgeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (this.isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.SizeAll;
                Mouse3DEventArgs args = e as Mouse3DEventArgs;


                var normal = this.camera.LookDirection;
                //輸入一個3D點，加一個向量以定義出一個平面，再利用輸入滑鼠新的位置，得到投影在該平面上的新3D做標點
                var newHit = this.viewport.UnProjectOnPlane(args.Position, lastHitPos, normal);
                if (newHit.HasValue)
                {
                    var offset = (newHit.Value - lastHitPos);
                    var trafo = this.Transform.Value;

                    if (this.DragX)
                        trafo.OffsetX += offset.X;

                    if (this.DragY)
                        trafo.OffsetY += offset.Y;

                    if (this.DragZ)
                        trafo.OffsetZ += offset.Z;
                    
                    this.dragTransform.Matrix = trafo;
                    this.Transform = this.dragTransform;
                    this.lastHitPos = newHit.Value;
                }
            }
        }

        private void UpdateTransforms(object sender)
        {
            var cornerTrafos = this.cornerHandles.Select(x => (x.Transform as MatrixTransform3D)).ToArray();
            var cornerMatrix = cornerTrafos.Select(x => (x).Value).ToArray();
            positions = cornerMatrix.Select(x => x.ToMatrix().TranslationVector).ToArray();

            for (int i = 0; i < 3; i++)
            {
                var translateMat = Matrix3DExtensions.Translate3D(positions[i]);

                Vector3 v1 = positions[(i + 1) % 3] - positions[i];
                Vector3 xBar = new Vector3(1, 0, 0);
                Vector3 rotateAxis = Vector3.Cross(xBar, v1);
                rotateAxis.Normalize();

                if (rotateAxis.IsZero)
                {
                    rotateAxis.X = 0; rotateAxis.Y = 0; rotateAxis.Z = 1;
                }
                float theta = Convert.ToSingle(Math.Acos(Vector3.Dot(xBar, v1) / (v1.Length() * xBar.Length())));


                Matrix rotateMat = Matrix.RotationAxis(rotateAxis, theta);


                var m = Matrix.Scaling(v1.Length(), 1, 1) * rotateMat * Matrix.Translation(positions[i]);


                ((MatrixTransform3D)edgeHandles[i].Transform).Matrix = (m.ToMatrix3D());

            }
            
        }


        public static readonly DependencyProperty DragXProperty =
            DependencyProperty.Register("DragX", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));

        public static readonly DependencyProperty DragYProperty =
            DependencyProperty.Register("DragY", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));

        public static readonly DependencyProperty DragZProperty =
            DependencyProperty.Register("DragZ", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));

      
        public bool DragX
        {
            get { return (bool)this.GetValue(DragXProperty); }
            set { this.SetValue(DragXProperty, value); }
        }
        public bool DragY
        {
            get { return (bool)this.GetValue(DragYProperty); }
            set { this.SetValue(DragYProperty, value); }
        }
        public bool DragZ
        {
            get { return (bool)this.GetValue(DragZProperty); }
            set { this.SetValue(DragZProperty, value); }
        }

     

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(DraggableTriangle), new UIPropertyMetadata(MaterialChanged));

        /// <summary>
        /// 
        /// </summary>
        private static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PhongMaterial)
            {
                foreach (var item in ((GroupModel3D)d).Items)
                {
                    var model = item as MaterialGeometryModel3D;
                    if (model != null)
                    {
                        model.Material = e.NewValue as PhongMaterial;
                    }
                }
            }
        }


    }
}
