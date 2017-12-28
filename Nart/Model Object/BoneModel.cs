using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    using System.Windows.Media;
    using Color = System.Windows.Media.Color;
    using Quaternion = System.Windows.Media.Media3D.Quaternion;
    public class BoneModel : MeshGeometryModel3D,  INotifyPropertyChanged
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
        /// 能否指派當前模型的轉移矩陣
        /// </summary>
        public bool IsTransformApplied = true;
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
        public ModelType ModelType;
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private readonly Matrix3D[] _modelTransformSet = new Matrix3D[10];
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color _boneDiffuseColor;
        /// <summary>
        /// 模型路徑
        /// </summary>
        private string _filePath;
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        public Matrix3D _finalModelTransform;
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private Matrix3D _totalModelTransform = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);


        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        public System.Windows.Media.Media3D.Quaternion _finalQuaternion;
        private System.Windows.Media.Media3D.Quaternion _totalQuaternion = new System.Windows.Media.Media3D.Quaternion(0, 0, 0, 0);
        private readonly System.Windows.Media.Media3D.Quaternion[] _modelQuaternionSet = new System.Windows.Media.Media3D.Quaternion[10];


        public Vector3D _finalTranslation;

        private Vector3D _totalTranslation = new Vector3D(0, 0, 0);
        private readonly Vector3D[] _modelVector3DSet = new Vector3D[10];










        /// <summary>
        /// Count是在ModelTransformSet中的累積數量
        /// </summary>
        private int _count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int _currentIndex = 0;

        private string _boneName;
        private bool _highlight = false;


        
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                SetValue(ref _filePath, value);
                if (!string.IsNullOrEmpty(_filePath))
                {
                    BoneName = Path.GetFileName(_filePath);
                }
            }
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
        public string BoneName
        {
            get
            {
                return _boneName;
            }
            set
            {
                SetValue(ref _boneName, value);
            }
        }
        public Color BoneDiffuseColor
        {
            get
            {
                return _boneDiffuseColor;
            }
            set
            {
                SetValue(ref _boneDiffuseColor, value);
                if (IsLoaded != true)
                    return;
                PhongMaterial  phongMaterial= this.Material as PhongMaterial;
                if (phongMaterial != null)
                    phongMaterial.DiffuseColor = _boneDiffuseColor.ToColor4();


            }
        }

        public void AddItem(Matrix3D item)
        {
            //item.OffsetX = 0;
            //item.OffsetY = 0;
            //item.OffsetZ = 0;

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

            //Console.WriteLine("\n\n");
            //if (_count == 10)
            //{

            //    for (int i = 0; i < 10; i++)
            //    {
            //        Console.WriteLine("\n\nInput[" + (_currentIndex + i)%10 + "]:\n" +
            //                          _modelTransformSet[(_currentIndex + i) % 10].OffsetX + "   " +
            //                          _modelTransformSet[(_currentIndex + i) % 10].OffsetY + "   " +
            //                          _modelTransformSet[(_currentIndex + i) % 10].OffsetZ + "   ");
            //    }
            //}
            //Console.WriteLine("\nfinal translation:\n" + _finalModelTransform.OffsetX + "   " + _finalModelTransform.OffsetY +"   " +_finalModelTransform.OffsetZ);

        }





        public void AddItem2(Matrix3D item)
        {
            double w;          
            double x;
            double y;
            double z;



            double tr = item.M11 + item.M22 + item.M33;

            if (tr > 0)
            {
                double S = Math.Sqrt(tr + 1.0) * 2; // S=4*qw 
                w = 0.25 * S;
                x = (item.M32 - item.M23) / S;
                y = (item.M13 - item.M31) / S;
                z = (item.M21 - item.M12) / S;
            }
            else if ((item.M11 > item.M22) & (item.M11 > item.M33))
            {
                double S = Math.Sqrt(1.0 + item.M11 - item.M22 - item.M33) * 2; // S=4*qx 
                w = (item.M32 - item.M23) / S;
                x = 0.25 * S;
                y = (item.M12 + item.M21) / S;
                z = (item.M13 + item.M31) / S;
            }
            else if (item.M22 > item.M33)
            {
                double S = Math.Sqrt(1.0 + item.M22 - item.M11 - item.M33) * 2; // S=4*qy
                w = (item.M13 - item.M31) / S;
                x = (item.M12 + item.M21) / S;
                y = 0.25 * S;
                z = (item.M23 + item.M32) / S;
            }
            else
            {
                double S = Math.Sqrt(1.0 + item.M33 - item.M11 - item.M22) * 2; // S=4*qz
                w = (item.M21 - item.M12) / S;
                x = (item.M13 + item.M31) / S;
                y = (item.M23 + item.M32) / S;
                z = 0.25 * S;
            }

            




            //Console.WriteLine("x  y  z  w:" + x.ToString("#.###E+0") + "  " + y.ToString("#.###E+0") + "  " + z.ToString("#.###E+0") + "  " + w.ToString("#.#E+0") + "  ");


            Quaternion items = new Quaternion(x, y, z, w);
            items.Normalize();

            Matrix3D temp = new Matrix3D();

            //數量少於陣列總長度則往後加入
            if (_count < _modelQuaternionSet.Length)
            {
                _count++;

                _totalQuaternion.W = _totalQuaternion.W + items.W;
                _totalQuaternion.X = _totalQuaternion.X + items.X;
                _totalQuaternion.Y = _totalQuaternion.Y + items.Y;
                _totalQuaternion.Z = _totalQuaternion.Z + items.Z;

                _finalQuaternion.W = _totalQuaternion.W / _count;
                _finalQuaternion.Y = _totalQuaternion.Y / _count;
                _finalQuaternion.X = _totalQuaternion.X / _count;
                _finalQuaternion.Z = _totalQuaternion.Z / _count;
                _finalQuaternion.Normalize();

                _totalTranslation.X = _totalTranslation.X + item.OffsetX;
                _totalTranslation.Y = _totalTranslation.Y + item.OffsetY;
                _totalTranslation.Z = _totalTranslation.Z + item.OffsetZ;

                _finalTranslation.X = _totalTranslation.X / _count;
                _finalTranslation.Y = _totalTranslation.Y / _count;
                _finalTranslation.Z = _totalTranslation.Z / _count;
            }
            else
            {
                _totalQuaternion.W = _totalQuaternion.W - _modelQuaternionSet[_currentIndex].W;
                _totalQuaternion.X = _totalQuaternion.X - _modelQuaternionSet[_currentIndex].X;
                _totalQuaternion.Y = _totalQuaternion.Y - _modelQuaternionSet[_currentIndex].Y;
                _totalQuaternion.Z = _totalQuaternion.Z - _modelQuaternionSet[_currentIndex].Z;

                _totalQuaternion.W = _totalQuaternion.W + items.W;
                _totalQuaternion.X = _totalQuaternion.X + items.X;
                _totalQuaternion.Y = _totalQuaternion.Y + items.Y;
                _totalQuaternion.Z = _totalQuaternion.Z + items.Z;

                _finalQuaternion.W = _totalQuaternion.W / _count;
                _finalQuaternion.Y = _totalQuaternion.Y / _count;
                _finalQuaternion.X = _totalQuaternion.X / _count;
                _finalQuaternion.Z = _totalQuaternion.Z / _count;
                _finalQuaternion.Normalize();


                _totalTranslation.X = _totalTranslation.X - _modelVector3DSet[_currentIndex].X;
                _totalTranslation.Y = _totalTranslation.Y - _modelVector3DSet[_currentIndex].Y;
                _totalTranslation.Z = _totalTranslation.Z - _modelVector3DSet[_currentIndex].Z;

                _totalTranslation.X = _totalTranslation.X + item.OffsetX;
                _totalTranslation.Y = _totalTranslation.Y + item.OffsetY;
                _totalTranslation.Z = _totalTranslation.Z + item.OffsetZ;

                _finalTranslation.X = _totalTranslation.X / _count;
                _finalTranslation.Y = _totalTranslation.Y / _count;
                _finalTranslation.Z = _totalTranslation.Z / _count;


            }

            _modelQuaternionSet[_currentIndex] = items;
            _modelVector3DSet[_currentIndex] = new Vector3D(item.OffsetX,item.OffsetY,item.OffsetZ);



            _currentIndex++;
            _currentIndex = _currentIndex % _modelQuaternionSet.Length;


            //temp.Rotate(items);

            //double test;
            //test = temp.M12;
            //temp.M12 = temp.M21;
            //temp.M21 = test;

            //test = temp.M13;
            //temp.M13 = temp.M31;
            //temp.M31 = test;

            //test = temp.M23;
            //temp.M23 = temp.M32;
            //temp.M32 = test;


            //_finalModelTransform = temp;

            //_finalModelTransform.OffsetX = item.OffsetX;
            //_finalModelTransform.OffsetY = item.OffsetY;
            //_finalModelTransform.OffsetZ = item.OffsetZ;
            //Console.WriteLine("\n\nInput:\n" + _modelVector3DSet[_currentIndex]);
            //Console.WriteLine("\nfinal translation:\n" + _finalTranslation);
        }



        public void SetQuaternion()
        {
            if (IsTransformApplied)
            {
                Matrix3D temp = new Matrix3D();
                temp.Rotate(_finalQuaternion);
                
                double test;
                test = temp.M12;
                temp.M12 = temp.M21;
                temp.M21 = test;

                test = temp.M13;
                temp.M13 = temp.M31;
                temp.M31 = test;

                test = temp.M23;
                temp.M23 = temp.M32;
                temp.M32 = test;

                temp.OffsetX = _finalTranslation.X;
                temp.OffsetY = _finalTranslation.Y;
                temp.OffsetZ = _finalTranslation.Z;

                Transform = new MatrixTransform3D(temp );
            }
        }









        public void SetTransformMatrix()
        {
            if (IsTransformApplied)
            {
                Transform = new MatrixTransform3D(_finalModelTransform);
            }
        }
        /// <summary>
        /// 設定模型材質
        /// </summary>        
        public void SetBoneMaterial()
        {

            PhongMaterial material = new PhongMaterial
            {
                ReflectiveColor = SharpDX.Color.Black,
                AmbientColor = new SharpDX.Color(0.0f, 0.0f, 0.0f, 1.0f),
                EmissiveColor = SharpDX.Color.Black,
                SpecularColor = new SharpDX.Color(90, 90, 90, 255),
                SpecularShininess = 60,
                DiffuseColor = BoneDiffuseColor.ToColor4()
            };         

            this.Material = material;
        }
        /// <summary>
        /// load進模型檔案
        /// </summary>
        public void LoadModel()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (IsLoaded || !File.Exists(FilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new StLReader();

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
                var geometryModel = model as GeometryModel3D;

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
                Geometry = mesh,
                Transform = Transform
            };


            StlExporter export = new StlExporter();
            string name = Path.GetFileName(FilePath);
            using (var fileStream = File.Create("D:\\Desktop\\test\\"+ name+".stl"))
            {
                export.Export(model, fileStream);
            }

       

        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        protected bool SetValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")
        {
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            this.OnPropertyChanged(propertyName);
            return true;
        }

}
}
