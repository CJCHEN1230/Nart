using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
    [Serializable]
    public class BoneModel : MeshGeometryModel3D,  INotifyPropertyChanged, ISerializable
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
        public string MarkerId = "";
        /// <summary>
        /// 所屬綁定的骨頭部位
        /// </summary>
        public ModelType ModelType;
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color _boneDiffuseColor;
        /// <summary>
        /// 模型路徑
        /// </summary>
        private string _filePath;
        /// <summary>
        /// 純檔名  xxx.stl
        /// </summary>
        private string _safeFileName;
        private bool _highlight = false;
        /// <summary>
        /// 最後的旋轉矩陣
        /// </summary>
        private Quaternion _finalRotation;
        /// <summary>
        /// 最後的平移矩陣
        /// </summary>
        private Vector3D _finalTranslation;
        /// <summary>
        /// 累加的旋轉矩陣
        /// </summary>
        private Quaternion _totalRotation = new Quaternion(0, 0, 0, 0);
        /// <summary>
        /// 累加的平移矩陣
        /// </summary>
        private Vector3D _totalTranslation = new Vector3D(0, 0, 0);
        /// <summary>
        /// 做均值濾波儲存下來旋轉矩陣的集合
        /// </summary>
        private readonly Quaternion[] _rotationCollection = new Quaternion[10];
        /// <summary>
        /// 做均值濾波儲存下來平移矩陣的集合
        /// </summary>
        private readonly Vector3D[] _translationCollection = new Vector3D[10];
        /// <summary>
        /// Count是在ModelTransformSet中的累積數量
        /// </summary>
        private int _count = 0;
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int _currentIndex = 0;

        public BoneModel()
        {
        }
        /// <summary>
        /// 將BoneModel物件反序列化的過程
        /// </summary>
        public BoneModel(SerializationInfo info, StreamingContext context)
        {
            IsRendering = (bool)info.GetValue("IsRendering", typeof(bool));
            SafeFileName = (string)info.GetValue("SafeFileName", typeof(string));
            MarkerId = (string)info.GetValue("MarkerId", typeof(string));
            ModelCenter.X = (float)info.GetValue("ModelCenter_X", typeof(float));
            ModelCenter.Y = (float)info.GetValue("ModelCenter_Y", typeof(float));
            ModelCenter.Z = (float)info.GetValue("ModelCenter_Z", typeof(float));
            IsTransformApplied = (bool)info.GetValue("IsTransformApplied", typeof(bool));
            ModelType = (ModelType)info.GetValue("ModelType", typeof(ModelType));
            Matrix3D matrix = new Matrix3D();
            matrix.M11 = (double)info.GetValue("M11", typeof(double)); matrix.M12 = (double)info.GetValue("M12", typeof(double)); matrix.M13 = (double)info.GetValue("M13", typeof(double)); matrix.M14 = (double)info.GetValue("M14", typeof(double));
            matrix.M21 = (double)info.GetValue("M21", typeof(double)); matrix.M22 = (double)info.GetValue("M22", typeof(double)); matrix.M23 = (double)info.GetValue("M23", typeof(double)); matrix.M24 = (double)info.GetValue("M24", typeof(double));
            matrix.M31 = (double)info.GetValue("M31", typeof(double)); matrix.M32 = (double)info.GetValue("M32", typeof(double)); matrix.M33 = (double)info.GetValue("M33", typeof(double)); matrix.M34 = (double)info.GetValue("M34", typeof(double));
            matrix.OffsetX = (double)info.GetValue("M41", typeof(double)); matrix.OffsetY = (double)info.GetValue("M42", typeof(double)); matrix.OffsetZ = (double)info.GetValue("M43", typeof(double)); matrix.M44 = (double)info.GetValue("M44", typeof(double));
            Transform = new MatrixTransform3D(matrix);
            Color boneColor = new Color();
            boneColor.A = (byte)info.GetValue("boneColor_A", typeof(byte));
            boneColor.R = (byte)info.GetValue("boneColor_R", typeof(byte));
            boneColor.G = (byte)info.GetValue("boneColor_G", typeof(byte));
            boneColor.B = (byte)info.GetValue("boneColor_B", typeof(byte));
            BoneDiffuseColor = boneColor;
        }


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
                    SafeFileName = Path.GetFileName(_filePath);
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
        public string SafeFileName
        {
            get
            {
                return _safeFileName;
            }
            set
            {
                SetValue(ref _safeFileName, value);
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

        /// <summary>
        /// 將旋轉矩陣輸入，並計算最後結果在_finalTranslation、_finalRotation
        /// </summary>     
        public void AddItem(Matrix3D item)
        {

            //先計算旋轉矩陣的四元數表達法
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

            

            Quaternion rotation = new Quaternion(x, y, z, w);
            Vector3D translation = new Vector3D(item.OffsetX, item.OffsetY, item.OffsetZ);
            rotation.Normalize();
            

            //數量少於陣列總長度則往後加入
            if (_count < _rotationCollection.Length)
            {
                _count++;

                _totalRotation.W = _totalRotation.W + rotation.W;
                _totalRotation.X = _totalRotation.X + rotation.X;
                _totalRotation.Y = _totalRotation.Y + rotation.Y;
                _totalRotation.Z = _totalRotation.Z + rotation.Z;

                _finalRotation.W = _totalRotation.W / _count;
                _finalRotation.Y = _totalRotation.Y / _count;
                _finalRotation.X = _totalRotation.X / _count;
                _finalRotation.Z = _totalRotation.Z / _count;
                _finalRotation.Normalize();

                _totalTranslation.X = _totalTranslation.X + translation.X;
                _totalTranslation.Y = _totalTranslation.Y + translation.Y;
                _totalTranslation.Z = _totalTranslation.Z + translation.Z;

                _finalTranslation.X = _totalTranslation.X / _count;
                _finalTranslation.Y = _totalTranslation.Y / _count;
                _finalTranslation.Z = _totalTranslation.Z / _count;
            }
            else
            {
                _totalRotation.W = _totalRotation.W - _rotationCollection[_currentIndex].W;
                _totalRotation.X = _totalRotation.X - _rotationCollection[_currentIndex].X;
                _totalRotation.Y = _totalRotation.Y - _rotationCollection[_currentIndex].Y;
                _totalRotation.Z = _totalRotation.Z - _rotationCollection[_currentIndex].Z;

                _totalRotation.W = _totalRotation.W + rotation.W;
                _totalRotation.X = _totalRotation.X + rotation.X;
                _totalRotation.Y = _totalRotation.Y + rotation.Y;
                _totalRotation.Z = _totalRotation.Z + rotation.Z;

                _finalRotation.W = _totalRotation.W / _count;
                _finalRotation.Y = _totalRotation.Y / _count;
                _finalRotation.X = _totalRotation.X / _count;
                _finalRotation.Z = _totalRotation.Z / _count;
                _finalRotation.Normalize();


                _totalTranslation.X = _totalTranslation.X - _translationCollection[_currentIndex].X;
                _totalTranslation.Y = _totalTranslation.Y - _translationCollection[_currentIndex].Y;
                _totalTranslation.Z = _totalTranslation.Z - _translationCollection[_currentIndex].Z;

                _totalTranslation.X = _totalTranslation.X + translation.X;
                _totalTranslation.Y = _totalTranslation.Y + translation.Y;
                _totalTranslation.Z = _totalTranslation.Z + translation.Z;

                _finalTranslation.X = _totalTranslation.X / _count;
                _finalTranslation.Y = _totalTranslation.Y / _count;
                _finalTranslation.Z = _totalTranslation.Z / _count;


            }

            _rotationCollection[_currentIndex] = rotation;
            _translationCollection[_currentIndex] = translation;

            _currentIndex++;
            _currentIndex = _currentIndex % _rotationCollection.Length;

            
        }
        /// <summary>
        /// 設定轉移矩陣，將旋轉跟平移矩陣結合
        /// </summary>   
        public void SetTransformMatrix()
        {
            if (IsTransformApplied)
            {
                Matrix3D transform = new Matrix3D();
                //將四元數轉乘Matrix 剛轉完需要再轉置
                transform.Rotate(_finalRotation);
                
                double temp;
                temp = transform.M12;
                transform.M12 = transform.M21;
                transform.M21 = temp;

                temp = transform.M13;
                transform.M13 = transform.M31;
                transform.M31 = temp;

                temp = transform.M23;
                transform.M23 = transform.M32;
                transform.M32 = temp;

                transform.OffsetX = _finalTranslation.X;
                transform.OffsetY = _finalTranslation.Y;
                transform.OffsetZ = _finalTranslation.Z;

                Transform = new MatrixTransform3D(transform);
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
        /// <summary>
        /// 儲存模型檔案，第一個參數指定路徑，第二個參數指定是否要儲存轉移後的模型
        /// </summary>
        public void SaveModel(string filepath , bool isSavedTransform)
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

            };
            if (isSavedTransform)
            {
                model.Transform = Transform;
            }


            StlExporter export = new StlExporter();
            
            using (var fileStream = File.Create(System.IO.Path.Combine(filepath, SafeFileName)))
            {
                export.Export(model, fileStream);
            }

       

        }
        /// <summary>
        /// 序列化物件資料
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsRendering", IsRendering);
            info.AddValue("SafeFileName", SafeFileName);
            info.AddValue("MarkerId", MarkerId);
            info.AddValue("ModelCenter_X", ModelCenter.X);
            info.AddValue("ModelCenter_Y", ModelCenter.Y);
            info.AddValue("ModelCenter_Z", ModelCenter.Z);
            info.AddValue("IsTransformApplied", IsTransformApplied);
            info.AddValue("ModelType", ModelType);
            info.AddValue("M11", Transform.Value.M11); info.AddValue("M12", Transform.Value.M12); info.AddValue("M13", Transform.Value.M13); info.AddValue("M14", Transform.Value.M14);
            info.AddValue("M21", Transform.Value.M21); info.AddValue("M22", Transform.Value.M22); info.AddValue("M23", Transform.Value.M23); info.AddValue("M24", Transform.Value.M24);
            info.AddValue("M31", Transform.Value.M31); info.AddValue("M32", Transform.Value.M32); info.AddValue("M33", Transform.Value.M33); info.AddValue("M34", Transform.Value.M34);
            info.AddValue("M41", Transform.Value.OffsetX); info.AddValue("M42", Transform.Value.OffsetY); info.AddValue("M43", Transform.Value.OffsetZ); info.AddValue("M44", Transform.Value.M44);
            info.AddValue("boneColor_A", BoneDiffuseColor.A);
            info.AddValue("boneColor_R", BoneDiffuseColor.R);
            info.AddValue("boneColor_G", BoneDiffuseColor.G);
            info.AddValue("boneColor_B", BoneDiffuseColor.B);

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
