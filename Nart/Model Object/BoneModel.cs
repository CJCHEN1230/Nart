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
using GeometryModel3D = System.Windows.Media.Media3D.GeometryModel3D;
using MeshGeometry3D = System.Windows.Media.Media3D.MeshGeometry3D;
using Model3D = System.Windows.Media.Media3D.Model3D;

namespace Nart.Model_Object
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Color = System.Windows.Media.Color;
    public class BoneModel : MeshGeometryModel3D
    {
        /// <summary>
        /// 讀檔時存模型的容器，方便之後算BoundingBox
        /// </summary>
        public Model3DGroup ModelContainer;
        /// <summary>
        /// 紀錄模型中心
        /// </summary>
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
        public string MarkerId;
        /// <summary>
        /// 所屬綁定的骨頭部位
        /// </summary>
        public string BoneName = "";
        /// <summary>
        /// 模型顏色
        /// </summary>
        public Color DiffuseColor;
        /// <summary>
        /// 模型路徑
        /// </summary>
        public string FilePath;
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
        private int _count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int _currentIndex = 0;
        private bool _highlight = false;




        public BoneModel()
        {

        }
        public bool Highlight
        {
            set
            {
                if (_highlight == value) { return; }
                _highlight = value;
                PhongMaterial material = this.Material as PhongMaterial;
                if (_highlight)
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
                return _highlight;
            }
        }
        public void AddItem(Matrix3D item)
        {
            //數量少於陣列總長度則往後加入
            if (_count < _modelTransformSet.Length)
            {
                _count++;

                ExtensionMethods.Matrix3DExtensions.AddMatrix3D(ref _totalModelTransform, ref item);

                ExtensionMethods.Matrix3DExtensions.DivideMatrix3D(ref _totalModelTransform, _count, ref _finalModelTransform);

            }
            else
            {
                ExtensionMethods.Matrix3DExtensions.SubtractMatrix3D(ref _totalModelTransform, ref _modelTransformSet[_currentIndex]);

                ExtensionMethods.Matrix3DExtensions.AddMatrix3D(ref _totalModelTransform, ref item);

                ExtensionMethods.Matrix3DExtensions.DivideMatrix3D(ref _totalModelTransform, _count, ref _finalModelTransform);
            }

            _modelTransformSet[_currentIndex] = item;

            _currentIndex++;
            _currentIndex = _currentIndex % _modelTransformSet.Length;

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
                int specular = 90;
                material.SpecularColor = new SharpDX.Color(specular, specular, specular, 255);
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

            HelixToolkit.Wpf.SharpDX.MeshGeometry3D modelGeometry =new HelixToolkit.Wpf.SharpDX.MeshGeometry3D
                {
                    Normals = new Vector3Collection(),
                    Positions = new Vector3Collection(),
                    Indices = new IntCollection()
                };



            foreach (Model3D model in ModelContainer.Children)
            {
                var geometryModel = model as System.Windows.Media.Media3D.GeometryModel3D;

                MeshGeometry3D mesh = geometryModel?.Geometry as MeshGeometry3D;

                if (mesh == null)
                    continue;

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
            }

            SetBoneMaterial();

            Geometry = modelGeometry;

            Transform = new MatrixTransform3D();

            IsLoaded = true;
        }

        public void SaveModel()
        {
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry= Geometry as HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

            if (geometry == null)
                return;


            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection(),
                Normals = new Vector3DCollection(),
                TriangleIndices = new Int32Collection()
            };

            foreach (Vector3 position in geometry.Positions)
            {
                mesh.Positions.Add(new Point3D(position.X,position.Y,position.Z));                
            }
            foreach (Vector3 normal in geometry.Normals)
            {
                mesh.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));
            }
            foreach (int triangleindice in geometry.TriangleIndices)
            {
                mesh.TriangleIndices.Add(triangleindice);
            }

            GeometryModel3D model = new GeometryModel3D
            {
                Geometry = mesh
            };


            StlExporter export = new StlExporter();
            string name = Path.GetFileName(FilePath);
            using (var fileStream = File.Create("D:\\Desktop\\test\\"+ name+".stl"))
            {
                export.Export(model, fileStream);
            }

       

        }
    }
}
