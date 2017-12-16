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
    using Matrix3D = System.Windows.Media.Media3D.Matrix3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Matrix = SharpDX.Matrix;
    using HelixToolkit.Wpf;
    using Matrix3DExtensions = Nart.ExtensionMethods.Matrix3DExtensions;
    using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class DraggableTriangle : GroupModel3D, IHitable
    {
        public static readonly DependencyProperty DragXProperty = DependencyProperty.Register("DragX", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));
        public static readonly DependencyProperty DragYProperty = DependencyProperty.Register("DragY", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));
        public static readonly DependencyProperty DragZProperty = DependencyProperty.Register("DragZ", typeof(bool), typeof(DraggableTriangle), new UIPropertyMetadata(true));
        public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register("Material", typeof(Material), typeof(DraggableTriangle), new UIPropertyMetadata(MaterialChanged));
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        public String MarkerId;
        public ModelType ModelType;
        /// <summary>
        /// 整個三角形中心位置
        /// </summary>
        public Point3D Center;
        /// <summary>
        /// 一開始三顆球的初始位置:紅色 綠色 藍色
        /// </summary>
        public Vector3[] Positions;
        /// <summary>
        /// 單顆球幾何
        /// </summary>
        private static readonly Geometry3D NodeGeometry;
        /// <summary>
        /// 單根桿子
        /// </summary>
        private static readonly Geometry3D EdgeGeometry;
        /// <summary>
        /// 直接調整透明度
        /// </summary>
        private float _transparency = 1.0f;
        /// <summary>
        /// 三顆球
        /// </summary>
        private readonly MeshGeometryModel3D[] _ballHandles = new MeshGeometryModel3D[3];
        /// <summary>
        /// 三根桿
        /// </summary>
        private readonly MeshGeometryModel3D[] _cylinderHandles = new MeshGeometryModel3D[3];
        /// <summary>
        /// 三頂點於中心的距離
        /// </summary>
        private int _length = 100;
        /// <summary>
        /// 初始三角形角度
        /// </summary>
        private double _initialAngle = 30;
        /// <summary>
        /// 初始三角形角度
        /// </summary>
        private bool _isCaptured;
        /// <summary>
        /// 當前的viewport，點下Down時候會存下
        /// </summary>
        private Viewport3DX _viewport;
        /// <summary>
        /// 當前的相機，點下Down時候會存下
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// 紀錄點下Down時的3D座標點
        /// </summary>
        private Point3D _lastHitPos;
        /// <summary>
        /// 定義球跟桿子
        /// </summary>
        
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
                    : this(new Point3D(0,0,0))
        {
        }
        public DraggableTriangle(Vector3 center)
                    : this(new Point3D(center.X, center.Y, center.Z))
        {
        }
        public DraggableTriangle(Point3D center)
        {
          

            Center = center;
            Positions = new Vector3[3]
                 {
                    new Vector3(Convert.ToSingle(Center.X+ Math.Cos( _initialAngle/180f*Math.PI)           *_length),Convert.ToSingle(Center.Y+ Math.Sin(_initialAngle/180f*Math.PI)             *_length),Convert.ToSingle(Center.Z)),
                    new Vector3(Convert.ToSingle(Center.X+ Math.Cos((_initialAngle+120)/180f*Math.PI)*_length),Convert.ToSingle(Center.Y+Math.Sin((_initialAngle+120)/180f*Math.PI)*_length),Convert.ToSingle(Center.Z)),
                    new Vector3(Convert.ToSingle(Center.X+ Math.Cos((_initialAngle+240)/180f*Math.PI)*_length),Convert.ToSingle(Center.Y+Math.Sin((_initialAngle+240)/180f*Math.PI)*_length),Convert.ToSingle(Center.Z)),
                 };
            for (int i = 0; i < 3; i++)
            {
                //平移圓球
                var translateMat = Matrix3DExtensions.Translate3D(Positions[i]);
                //三顆球顏色不同 材料設置在迴圈外面，並沒有在這邊先設置
                _ballHandles[i] = new MeshGeometryModel3D()
                {
                    Visibility = Visibility.Visible,
                    Geometry = NodeGeometry,
                    Transform = new MatrixTransform3D(translateMat),
                };
                //定義球的滑鼠事件
                this._ballHandles[i].MouseMove3D += OnNodeMouse3DMove;
                this._ballHandles[i].MouseUp3D += OnNodeMouse3DUp;
                this._ballHandles[i].MouseDown3D += OnNodeMouse3DDown;

           
                //兩點組成向量
                Vector3 v1 = Positions[(i + 1) % 3] - Positions[i];
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
                Matrix m = Matrix.Scaling(v1.Length(), 1, 1) * rotateMat * Matrix.Translation(Positions[i]);
                _cylinderHandles[i] = new MeshGeometryModel3D()
                {
                    Geometry = EdgeGeometry,
                    Material = SetMaterial(Color4.White),
                    Visibility = Visibility.Visible,
                    Transform = new MatrixTransform3D(m.ToMatrix3D())
                };
                //定義桿子的滑鼠事件
                _cylinderHandles[i].MouseMove3D += OnEdgeMouse3DMove;
                _cylinderHandles[i].MouseUp3D += OnEdgeMouse3DUp;
                _cylinderHandles[i].MouseDown3D += OnEdgeMouse3DDown;

            }
            //因為會畫透明物件，所以在上面迴圈外先將球一次加進去
            foreach (MeshGeometryModel3D ball in _ballHandles)
            {
                Children.Add(ball);
            }
            //加完球再加桿子
            foreach (MeshGeometryModel3D cylinder in _cylinderHandles)
            {
                Children.Add(cylinder);
            }


            //設定三顆球的顏色
            _ballHandles[0].Material = SetMaterial(new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            _ballHandles[1].Material = SetMaterial(new Color4(0.0f, 1.0f, 0.0f, 1.0f));
            _ballHandles[2].Material = SetMaterial(new Color4(0.0f, 0.0f, 1.0f, 1.0f));

        }






        public float Transparency
        {
            get
            {
                return _transparency;
            }
            set
            {
                if (Math.Abs(value - _transparency) > 10E-10)
                {
                    _transparency = value;


                    //將三角導引件內部所有球、桿的透明度更改成新的值
                    foreach (var element3D in Children)
                    {
                        var model = (MeshGeometryModel3D) element3D;
                        PhongMaterial material = model?.Material as PhongMaterial;
                        if (material == null)
                            continue;
                        Color4 color = material.DiffuseColor;

                        color.Alpha = _transparency;
                        material.DiffuseColor = color;
                    }
                }
            }
        }
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
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }


        public PhongMaterial SetMaterial(Color4 color)
        {
           
            HelixToolkit.Wpf.SharpDX.PhongMaterial material = new PhongMaterial();

            material.ReflectiveColor = SharpDX.Color.Black;
            float ambient = 0.0f;
            material.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
            material.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            int specular = 90;
            material.SpecularColor = new SharpDX.Color(specular, specular, specular, 255);
            material.SpecularShininess = 60;
            material.DiffuseColor = color;

            return material;
           
        }
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
        private void OnNodeMouse3DDown(object sender, RoutedEventArgs e)
        {
            var args = e as Mouse3DEventArgs;
            if (args == null || args.Viewport == null)
                return;

            this._isCaptured = true;
            this._viewport = args.Viewport;
            this._camera = args.Viewport.Camera;
            this._lastHitPos = args.HitTestResult.PointHit;
        }
        private void OnNodeMouse3DUp(object sender, RoutedEventArgs e)
        {

            if (this._isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
            }
        }
        private void OnNodeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (this._isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Hand;

                var args = e as Mouse3DEventArgs;

                var normal = this._camera.LookDirection;

                var newHit = this._viewport.UnProjectOnPlane(args.Position, _lastHitPos, normal);
                if (newHit.HasValue)
                {
                    MeshGeometryModel3D corner = sender as MeshGeometryModel3D;
                    var offset = (newHit.Value - _lastHitPos);
                    var localTransform = corner.Transform.Value;
                    Matrix3D groupTransform = this.Transform.Value;


                    groupTransform.Invert();

                    offset = groupTransform.Transform(offset);

                    //將w正規化為1
                    if (this.DragX)
                        localTransform.OffsetX += offset.X / groupTransform.M44;

                    if (this.DragY)
                        localTransform.OffsetY += offset.Y / groupTransform.M44;

                    if (this.DragZ)
                        localTransform.OffsetZ += offset.Z / groupTransform.M44;

                    this._lastHitPos = newHit.Value;
                    corner.Transform = new MatrixTransform3D(localTransform);

                }


                UpdateTransforms();
            }
        }
        private void OnEdgeMouse3DDown(object sender, RoutedEventArgs e)
        {
            var args = e as Mouse3DEventArgs;
            if (args == null || args.Viewport == null)
                return;

            this._isCaptured = true;
            this._viewport = args.Viewport;
            this._camera = args.Viewport.Camera;
            this._lastHitPos = args.HitTestResult.PointHit;
        }
        private void OnEdgeMouse3DUp(object sender, RoutedEventArgs e)
        {
            if (this._isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
                this._isCaptured = false;
                this._camera = null;
                this._viewport = null;
            }
        }
        private void OnEdgeMouse3DMove(object sender, RoutedEventArgs e)
        {
            if (this._isCaptured)
            {
                Application.Current.MainWindow.Cursor = Cursors.SizeAll;
                Mouse3DEventArgs args = e as Mouse3DEventArgs;


                var normal = this._camera.LookDirection;
                //輸入一個3D點，加一個向量以定義出一個平面，再利用輸入滑鼠新的位置，得到投影在該平面上的新3D做標點
                var newHit = this._viewport.UnProjectOnPlane(args.Position, _lastHitPos, normal);
                if (newHit.HasValue)
                {
                    var offset = (newHit.Value - _lastHitPos);
                    var trafo = this.Transform.Value;

                    if (this.DragX)
                        trafo.OffsetX += offset.X;

                    if (this.DragY)
                        trafo.OffsetY += offset.Y;

                    if (this.DragZ)
                        trafo.OffsetZ += offset.Z;

                    this.Transform = new MatrixTransform3D(trafo);
                    this._lastHitPos = newHit.Value;
                }
            }
        }
        private void UpdateTransforms()
        {
            Matrix3D[] cornerMatrix = this._ballHandles.Select(x => (x.Transform.Value)).ToArray();
            //Matrix3D[] cornerMatrix = cornerTrafos.Select(x => (x).Value).ToArray();
            Positions = cornerMatrix.Select(x => x.ToMatrix().TranslationVector).ToArray();

            for (int i = 0; i < 3; i++)
            {
                var translateMat = Matrix3DExtensions.Translate3D(Positions[i]);

                Vector3 v1 = Positions[(i + 1) % 3] - Positions[i];
                Vector3 xBar = new Vector3(1, 0, 0);
                Vector3 rotateAxis = Vector3.Cross(xBar, v1);
                rotateAxis.Normalize();

                if (rotateAxis.IsZero)
                {
                    rotateAxis.X = 0; rotateAxis.Y = 0; rotateAxis.Z = 1;
                }
                float theta = Convert.ToSingle(Math.Acos(Vector3.Dot(xBar, v1) / (v1.Length() * xBar.Length())));

                Matrix rotateMat = Matrix.RotationAxis(rotateAxis, theta);

                var m = Matrix.Scaling(v1.Length(), 1, 1) * rotateMat * Matrix.Translation(Positions[i]);

                ((MatrixTransform3D)_cylinderHandles[i].Transform).Matrix = (m.ToMatrix3D());
            }
        }
    }
}
