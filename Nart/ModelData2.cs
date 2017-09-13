using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using NartControl;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    public class ModelData2
    {
        /// <summary>
        /// 儲存模型資料，包括矩陣轉移、材質、網格
        /// </summary>        
        public MeshGeometryModel3D meshGeometry { get; private set; }
        /// <summary>
        /// 模型名稱
        /// </summary>
        public String Filename{get;set;}
        /// <summary>
        /// 此Model的最終轉換矩陣
        /// </summary>
        private SharpDX.Matrix ModelTransform = new SharpDX.Matrix();
        /// <summary>
        /// 用來累加的矩陣
        /// </summary>
        private SharpDX.Matrix TotalModelTransform = new SharpDX.Matrix();
        /// <summary>
        /// 防止抖動，用來存放所有矩陣，10是累積總數量
        /// </summary>
        private SharpDX.Matrix[] ModelTransformSet = new SharpDX.Matrix[10];
        /// <summary>
        /// CurrenIndex是當前要儲存在ModelTransformSet裡面位置的索引
        /// </summary>
        private int CurrenIndex = 0;
        public int DatabaseIndex{ get;set;}
        public int Count {get;set;}                                 
        public ModelData2(String filename)
        {
            Filename = filename;

            Count = 0;

            DatabaseIndex = -1;

            TotalModelTransform = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            ModelTransform = new Matrix(1, 0, 0, 0
                                                                     , 0, 1, 0, 0
                                                                     , 0, 0, 1, 0
                                                                     , 0, 0, 0, 1); //單位矩陣

            meshGeometry = LoadSTL(Filename);
            
            //Console.WriteLine("0");
        }        
        public MeshGeometryModel3D LoadSTL(string path)
        {
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();
            Model3DGroup model3dgroup = reader.Read(path);

            MultiAngleViewModel.AllModelGroup.Children.Add(model3dgroup);

            System.Windows.Media.Media3D.GeometryModel3D geometryModel = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            PhongMaterial material = new HelixToolkit.Wpf.SharpDX.PhongMaterial();

            material.ReflectiveColor = Color.Black;
            float ambient = 0.0f;
            material.AmbientColor = new Color(ambient, ambient, ambient, 1.0f);
            material.DiffuseColor = new Color(40, 181, 187, 100);
            material.EmissiveColor = Color.Black; //這是自己發光的顏色
            float Specular = 0.5f;
            material.SpecularColor = new Color(Specular, Specular, Specular, 255);
            material.SpecularShininess = 80;

            

            //設定模型幾何形狀
            HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

            geometry.Normals = new Vector3Collection();
            geometry.Positions = new Vector3Collection();
            geometry.Indices = new IntCollection();

            //將從stlreader讀到的資料轉入
            foreach (Point3D position in mesh.Positions)
            {
                geometry.Positions.Add(new Vector3(
                      Convert.ToSingle(position.X)
                    , Convert.ToSingle(position.Y)
                    , Convert.ToSingle(position.Z)));
            }

            foreach (Vector3D normal in mesh.Normals)
            {
                geometry.Normals.Add(new Vector3(
                      Convert.ToSingle(normal.X)
                    , Convert.ToSingle(normal.Y)
                    , Convert.ToSingle(normal.Z)));
            }

            foreach (Int32 triangleindice in mesh.TriangleIndices)
            {
                geometry.Indices.Add(triangleindice);
            }

            MeshGeometryModel3D tempMeshGeometry = new MeshGeometryModel3D();
            //將建立好的指派進helix.sharp格式的MeshGeometryModel3D
            tempMeshGeometry.Material = material;
            tempMeshGeometry.Geometry = geometry;
            tempMeshGeometry.Transform = new TranslateTransform3D(0, 0, 0);

            //meshGeometry.PushMatrix(new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -100, -100, 100, 1));

            return tempMeshGeometry;

        }

   

    }
}
