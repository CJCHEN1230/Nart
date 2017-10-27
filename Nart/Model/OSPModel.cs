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
        /// 模型有Load進去則為true
        /// </summary>
        public new bool  IsLoaded = false;
        /// <summary>
        /// 模型是不是OSP
        /// </summary>
        public bool IsOSP = false;
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


        public Color DiffuseColor;

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
  

        public OSPModel()
        {

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
        public void SetTransformMatrix()
        {
            Transform = new MatrixTransform3D(_finalModelTransform);
            if (IsOSP)
            {
                OSPCurrentNormal = Transform.Transform(OSPOriNormal);
                OSPCurrentNormal.Normalize();
            }
        }     
        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }
        public void SetOSPMaterial()
        {

            if (DiffuseColor != null)
            {
                this.Material = new PhongMaterial();

                this.Geometry.Colors = new Color4Collection();

                for (int i = 0; i < this.Geometry.Positions.Count; i++)
                {
                    this.Geometry.Colors.Add(DiffuseColor.ToColor4());
                }
            }
            //material.ReflectiveColor = SharpDX.Color.Black;
            //float ambient = 0.0f;
            //material.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
            //material.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
            //int Specular = 90;
            //material.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
            //material.SpecularShininess = 60;
            //material.DiffuseColor = OSPDiffuseColor.ToColor4();

            //this.Material = material;

        }
        //public void LoadOSP()
        //{
        //    //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
        //    if (this.IsLoaded || !System.IO.File.Exists(FilePath))
        //    {
        //        return;
        //    }

        //    //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
        //    StLReader reader = new HelixToolkit.Wpf.StLReader();

        //    this.ModelContainer = reader.Read(FilePath);

        //    var geometryModel = this.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

        //    var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

        //    if (mesh.Positions.Count != 6)
        //    {
        //        return;
        //    }

        //    this.OSPOriNormal = new Vector3D(mesh.Normals[0].X, mesh.Normals[0].Y, mesh.Normals[0].Z);
        //    this.OSPOriNormal.Normalize();//將上述向量正規化
        //    this.OSPPlanePoint = mesh.Positions[0];
        //    ////設定模型材質
        //    //SetOSPMaterial();

        //    //定義整個對稱面八個點
        //    Point3D[] ospPoint = new Point3D[8];
        //    int[] repeatIndex = new int[2] { -1, -1 };
        //    int[] repeatIndex2 = new int[2] { -1, -1 };

        //    //前半網格點
        //    for (int i = 0, k = 0; i < mesh.Positions.Count / 2; i++)
        //    { //後半網格點
        //        for (int j = 3; j < mesh.Positions.Count; j++)
        //        {   //尋找前半跟後半有哪些重複存下引數在repeatIndex  repeatIndex2
        //            double error = Math.Abs(Math.Pow(mesh.Positions[i].X - mesh.Positions[j].X, 2)) + Math.Abs(Math.Pow(mesh.Positions[i].Y - mesh.Positions[j].Y, 2)) + Math.Abs(Math.Pow(mesh.Positions[i].Z - mesh.Positions[j].Z, 2));
        //            if (error < 10E-008)
        //            {
        //                repeatIndex[k] = i;
        //                repeatIndex2[k] = j;
        //                k++;
        //            }
        //        }
        //    }
        //    //找出沒有重複的那個引數字
        //    int thirdIndex = 3 - repeatIndex[0] - repeatIndex[1];
        //    int thirdIndex2 = 12 - repeatIndex2[0] - repeatIndex2[1];


        //    if (thirdIndex == 0)
        //    {
        //        ospPoint[0] = mesh.Positions[0];
        //        ospPoint[1] = mesh.Positions[1];
        //        ospPoint[2] = mesh.Positions[thirdIndex2];
        //        ospPoint[3] = mesh.Positions[2];
        //    }
        //    else if (thirdIndex == 1)
        //    {
        //        ospPoint[0] = mesh.Positions[1];
        //        ospPoint[1] = mesh.Positions[2];
        //        ospPoint[2] = mesh.Positions[thirdIndex2];
        //        ospPoint[3] = mesh.Positions[0];
        //    }
        //    else
        //    {
        //        ospPoint[0] = mesh.Positions[2];
        //        ospPoint[1] = mesh.Positions[0];
        //        ospPoint[2] = mesh.Positions[thirdIndex2];
        //        ospPoint[3] = mesh.Positions[1];
        //    }


        //    for (int i = 4; i < ospPoint.Length; i++)
        //    {
        //        ospPoint[i] = new Point3D(ospPoint[i - 4].X - 0.1 * mesh.Normals[0].X,
        //                                                          ospPoint[i - 4].Y - 0.1 * mesh.Normals[0].Y,
        //                                                          ospPoint[i - 4].Z - 0.1 * mesh.Normals[0].Z);
        //    }

        //    for (int i = 0; i < ospPoint.Length / 2; i++)
        //    {
        //        ospPoint[i] = new Point3D(ospPoint[i].X + 0.1 * mesh.Normals[0].X,
        //                                                          ospPoint[i].Y + 0.1 * mesh.Normals[0].Y,
        //                                                          ospPoint[i].Z + 0.1 * mesh.Normals[0].Z);
        //    }



        //    HelixToolkit.Wpf.SharpDX.MeshGeometry3D ospGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
        //    ospGeometry.Normals = new Vector3Collection();
        //    ospGeometry.Positions = new Vector3Collection();
        //    ospGeometry.Indices = new IntCollection();

        //    // 0 1 2 3
        //    CreatePoint(0, 1, 2, 3, ospGeometry.Positions, ospPoint);

        //    CreateNormal(0, 1, 2, ospGeometry.Normals, ospPoint);

        //    ////4 0 3 7
        //    //CreatePoint(4, 0, 3, 7, ospGeometry2.Positions, ospPoint);
        //    //CreateNormal(4, 0, 3, ospGeometry2.Normals, ospPoint);

        //    ////5 1 0 4
        //    //CreatePoint(5, 1, 0, 4, ospGeometry2.Positions, ospPoint);
        //    //CreateNormal(5, 1, 0, ospGeometry2.Normals, ospPoint);

        //    ////1 5 6 2 
        //    //CreatePoint(1, 5, 6, 2, ospGeometry2.Positions, ospPoint);
        //    //CreateNormal(1, 5, 6, ospGeometry2.Normals, ospPoint);

        //    ////3 2 6 7
        //    //CreatePoint(3, 2, 6, 7, ospGeometry2.Positions, ospPoint);
        //    //CreateNormal(3, 2, 6, ospGeometry2.Normals, ospPoint);

        //    ////5 4 7 6
        //    CreatePoint(5, 4, 7, 6, ospGeometry.Positions, ospPoint);
        //    CreateNormal(5, 4, 7, ospGeometry.Normals, ospPoint);

        //    for (int i = 0; i < ospGeometry.Positions.Count; i++)
        //    {
        //        ospGeometry.Indices.Add(i);
        //    }

        //    this.Geometry = ospGeometry;

        //    SetOSPMaterial();

        //    this.Transform = new MatrixTransform3D();

        //    this.IsOSP = true;

        //    this.IsLoaded = true;

        //}

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

            //設定模型材質
            //SetMaterial(ref Model.ModelMaterial, modelDiffuseColor);
            //SetModelMaterial(ModelDiffuseColor);

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


        //public void SetOSPMaterial()
        //{
        //    if (DiffuseColor != null)
        //    {
        //        HelixToolkit.Wpf.SharpDX.PhongMaterial material = new PhongMaterial();

        //        material.ReflectiveColor = SharpDX.Color.Black;
        //        float ambient = 0.0f;
        //        material.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
        //        material.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
        //        int Specular = 90;
        //        material.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
        //        material.SpecularShininess = 60;
        //        material.DiffuseColor = DiffuseColor.ToColor4();

        //        this.Material = material;
        //    }
        //}


        private void CreatePoint(int index0, int index1, int index2, int index3, Vector3Collection positions, Point3D[] ospPoint)
        {
            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index0].X)
                    , Convert.ToSingle(ospPoint[index0].Y)
                    , Convert.ToSingle(ospPoint[index0].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index1].X)
                    , Convert.ToSingle(ospPoint[index1].Y)
                    , Convert.ToSingle(ospPoint[index1].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index3].X)
                    , Convert.ToSingle(ospPoint[index3].Y)
                    , Convert.ToSingle(ospPoint[index3].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index1].X)
                    , Convert.ToSingle(ospPoint[index1].Y)
                    , Convert.ToSingle(ospPoint[index1].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index2].X)
                    , Convert.ToSingle(ospPoint[index2].Y)
                    , Convert.ToSingle(ospPoint[index2].Z)));

            positions.Add(new Vector3(
                      Convert.ToSingle(ospPoint[index3].X)
                    , Convert.ToSingle(ospPoint[index3].Y)
                    , Convert.ToSingle(ospPoint[index3].Z)));
        }
        private void CreateNormal(int index0, int index1, int index2, Vector3Collection normals, Point3D[] ospPoint)
        {
            Vector3 normal = Vector3.Cross(new Vector3(Convert.ToSingle(ospPoint[index1].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index1].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index1].Z - ospPoint[index0].Z))
                , new Vector3(Convert.ToSingle(ospPoint[index2].X - ospPoint[index0].X), Convert.ToSingle(ospPoint[index2].Y - ospPoint[index0].Y), Convert.ToSingle(ospPoint[index2].Z - ospPoint[index0].Z)));


            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }

    }
}
