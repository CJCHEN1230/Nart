using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Nart.ExtensionMethods;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace Nart.Model
{
    using Color = System.Windows.Media.Color;
    public class OSPModel : MeshGeometryModel3D
    {
        /// <summary>
        /// 讀檔時存模型的容器，方便之後算BoundingBox
        /// </summary>
        public Model3DGroup ModelContainer;
        /// <summary>
        /// 已從ModelDataCollection中移除則會變成true，在model setting 的頁面中按"-"時會改變成true
        /// </summary>
        public bool IsRemoved = false;
        /// <summary>
        /// 有被加進去模型清單才為True
        /// </summary>
        public bool IsAdded = false;
        /// <summary>
        /// 模型是不是OSP
        /// </summary>
        public bool IsOSP = false;
        /// <summary>
        /// 模型有Load進去則為true
        /// </summary>
        public new bool IsLoaded = false;
        /// <summary>
        /// OSP才有的Normal
        /// </summary>
        public Vector3D OSPCurrentNormal;
        /// <summary>
        /// OSP原始的Normal
        /// </summary>
        public Vector3D OSPOriNormal;
        /// <summary>
        /// OSP才有的平面點
        /// </summary>
        public Point3D OSPPlanePoint;
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

        public OSPModel()
        {
        }      
        
        public bool Highlight
        {
            set
            {
                if (highlight == value) { return; }
                highlight = value;
                if (highlight)
                {                   
                    this.Material = new PhongMaterial { EmissiveColor = SharpDX.Color.Yellow };
                }
                else
                {
                    this.Material = new PhongMaterial { EmissiveColor = SharpDX.Color.Transparent };
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

                Matrix3DExtension.AddMatrix3D(ref _totalModelTransform, ref item);

                Matrix3DExtension.DivideMatrix3D(ref _totalModelTransform, Count, ref _finalModelTransform);

            }
            else
            {
                Matrix3DExtension.SubtractMatrix3D(ref _totalModelTransform, ref _modelTransformSet[CurrentIndex]);

                Matrix3DExtension.AddMatrix3D(ref _totalModelTransform, ref item);

                Matrix3DExtension.DivideMatrix3D(ref _totalModelTransform, Count, ref _finalModelTransform);
            }

            _modelTransformSet[CurrentIndex] = item;

            CurrentIndex++;
            CurrentIndex = CurrentIndex % _modelTransformSet.Length;

        }
        public void SetOSPMaterial()
        {

            if (DiffuseColor != null)
            {
                this.Material = new PhongMaterial();
               
                Color4Collection Colors = new Color4Collection();

                for (int i = 0; i < this.Geometry.Positions.Count; i++)
                {
                    Colors.Add(DiffuseColor.ToColor4());
                }

                this.Geometry.Colors = Colors;
            }   
        }
        public void LoadOSP()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (this.IsLoaded || !System.IO.File.Exists(FilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            this.ModelContainer = reader.Read(FilePath);

            var geometryModel = this.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            this.OSPOriNormal = new Vector3D(mesh.Normals[0].X, mesh.Normals[0].Y, mesh.Normals[0].Z);
            this.OSPOriNormal.Normalize();//將上述向量正規化
            this.OSPPlanePoint = mesh.Positions[0];

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


            modelGeometry.Colors = new Color4Collection();
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                modelGeometry.Colors.Add(new Color4(0.1f, 0.1f, 0.8f, 0.2f));
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

            this.Geometry = modelGeometry;

            SetOSPMaterial();

            this.Transform = new MatrixTransform3D();

            this.IsOSP = false;

            this.IsLoaded = true;
        }
        //private void CreatePoint(int index0, int index1, int index2, int index3, Vector3Collection positions, Point3D[] ospPoint)
        //{
        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index0].X)
        //            , Convert.ToSingle(ospPoint[index0].Y)
        //            , Convert.ToSingle(ospPoint[index0].Z)));

        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index1].X)
        //            , Convert.ToSingle(ospPoint[index1].Y)
        //            , Convert.ToSingle(ospPoint[index1].Z)));

        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index3].X)
        //            , Convert.ToSingle(ospPoint[index3].Y)
        //            , Convert.ToSingle(ospPoint[index3].Z)));

        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index1].X)
        //            , Convert.ToSingle(ospPoint[index1].Y)
        //            , Convert.ToSingle(ospPoint[index1].Z)));

        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index2].X)
        //            , Convert.ToSingle(ospPoint[index2].Y)
        //            , Convert.ToSingle(ospPoint[index2].Z)));

        //    positions.Add(new Vector3(
        //              Convert.ToSingle(ospPoint[index3].X)
        //            , Convert.ToSingle(ospPoint[index3].Y)
        //            , Convert.ToSingle(ospPoint[index3].Z)));
        //}
        //private void CreateNormal(int index0, int index1, int index2, Vector3Collection normals, Point3D[] ospPoint)
        //{
        //    Vector3 normal = Vector3.Cross(new Vector3(Convert.ToSingle(ospPoint[index1].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index1].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index1].Z - ospPoint[index0].Z))
        //        , new Vector3(Convert.ToSingle(ospPoint[index2].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index2].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index2].Z - ospPoint[index0].Z)));


        //    normals.Add(normal);
        //    normals.Add(normal);
        //    normals.Add(normal);
        //    normals.Add(normal);
        //    normals.Add(normal);
        //    normals.Add(normal);
        //}
        public void SetTransformMatrix()
        {
            this.Transform = new MatrixTransform3D(_finalModelTransform);
            if (IsOSP)
            {
                OSPCurrentNormal = this.Transform.Transform(OSPOriNormal);
                OSPCurrentNormal.Normalize();
            }
        }
        /// <summary>
        /// 重設此類別的Shader
        /// </summary>
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }
    }
}
