using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using Nart.ExtensionMethods;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace Nart.Model_Object
{
    using System.Windows;
    using Color = System.Windows.Media.Color;
    public class BoneModel : MeshGeometryModel3D
    {
        /// <summary>
        /// 讀檔時存模型的容器，方便之後算BoundingBox
        /// </summary>
        public Model3DGroup ModelContainer;

        public Vector3 ModelCenter;
        /// <summary>
        /// 已從ModelDataCollection中移除則會變成true，在model setting 的頁面中按"-"時會改變成true
        /// </summary>
        public bool IsRemoved = false;
        /// <summary>
        /// 有被加進去模型清單才為True
        /// </summary>
        public bool IsAdded = false;
        /// <summary>
        /// 模型有Load進去則為true
        /// </summary>
        public new bool IsLoaded = false;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        public String MarkerID;
        /// <summary>
        /// 模型顏色
        /// </summary>
        public Color DiffuseColor;
        /// <summary>
        /// 模型路徑
        /// </summary>
        public String FilePath;


        public GroupModel3D ballGroup = new GroupModel3D();
        
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private Matrix3D _finalModelTransform = new Matrix3D();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D _totalModelTransform = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private Matrix3D[] _modelTransformSet = new Matrix3D[10];
        /// <summary>
        /// Count是在ModelTransformSet中的累積數量
        /// </summary>
        private int Count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrentIndex = 0;
        private bool highlight = false;




        public BoneModel()
        {

        }
        public bool Highlight
        {
            set
            {
                if (highlight == value) { return; }
                highlight = value;
                PhongMaterial material = this.Material as PhongMaterial;
                if (highlight)
                {
                    material.EmissiveColor = SharpDX.Color.Yellow;
                }
                else
                {
                    material.EmissiveColor = SharpDX.Color.Black;
                }
            }
            get
            {
                return highlight;
            }
        }
        public void AddItem(Matrix3D item)
        {
            //數量少於陣列總長度則往後加入
            if (Count < _modelTransformSet.Length)
            {
                Count++;

                ExtensionMethods.Matrix3DExtensions.AddMatrix3D(ref _totalModelTransform, ref item);

                ExtensionMethods.Matrix3DExtensions.DivideMatrix3D(ref _totalModelTransform, Count, ref _finalModelTransform);

            }
            else
            {
                ExtensionMethods.Matrix3DExtensions.SubtractMatrix3D(ref _totalModelTransform, ref _modelTransformSet[CurrentIndex]);

                ExtensionMethods.Matrix3DExtensions.AddMatrix3D(ref _totalModelTransform, ref item);

                ExtensionMethods.Matrix3DExtensions.DivideMatrix3D(ref _totalModelTransform, Count, ref _finalModelTransform);
            }

            _modelTransformSet[CurrentIndex] = item;

            CurrentIndex++;
            CurrentIndex = CurrentIndex % _modelTransformSet.Length;

        }
        public void SetTransformMatrix()
        {
            Transform = new MatrixTransform3D(_finalModelTransform);
        }
        /// <summary>
        /// 設定模型材質
        /// </summary>        
        public void SetBoneMaterial()
        {
            if (DiffuseColor != null)
            {
                HelixToolkit.Wpf.SharpDX.PhongMaterial material = new PhongMaterial();

                material.ReflectiveColor = SharpDX.Color.Black;
                float ambient = 0.0f;
                material.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                material.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                int Specular = 90;
                material.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
                material.SpecularShininess = 60;
                material.DiffuseColor = DiffuseColor.ToColor4();

                this.Material = material;
            }
        }
        /// <summary>
        /// load進模型檔案
        /// </summary>
        public void LoadModel()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (this.IsLoaded || !System.IO.File.Exists(FilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            ModelContainer = reader.Read(FilePath);

            Rect3D bound = ModelContainer.Bounds;

            ModelCenter = new Vector3(Convert.ToSingle(bound.X + bound.SizeX / 2.0),
                Convert.ToSingle(bound.Y + bound.SizeY / 2.0),
                Convert.ToSingle(bound.Z + bound.SizeZ / 2.0));

            var geometryModel = ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型幾何形狀
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            modelGeometry.Normals = new Vector3Collection();
            modelGeometry.Positions = new Vector3Collection();
            modelGeometry.Indices = new IntCollection();
            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                modelGeometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                modelGeometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                modelGeometry.Indices.Add(triangleindice);
            }

            SetBoneMaterial();

            this.Geometry = modelGeometry;

            this.Transform = new MatrixTransform3D();

            this.IsLoaded = true;
        }

    }
}
