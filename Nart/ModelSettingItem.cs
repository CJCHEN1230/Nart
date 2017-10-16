using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using NartControl;
using NartControl.Control;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;


namespace Nart
{
    using SharpDX.Direct3D11;
    using Color = System.Windows.Media.Color;
    /// <summary>
    /// 模型設置清單中Listview中的項目
    /// </summary>
    public class ModelSettingItem : ObservableObject
    {
        /// <summary>
        /// 設置列當中的模型物件
        /// </summary>
        public ModelData Model = new ModelData();
        /// <summary>
        /// 設置列當中的OSP物件
        /// </summary>
        public ModelData OSP = new ModelData();
        /// <summary>
        /// 模型名稱
        /// </summary>
        private String modelFilePath;
        /// <summary>
        /// OSP名稱
        /// </summary>
        private String ospFilePath;
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color modelDiffuseColor;
        /// <summary>
        /// OSP 顏色
        /// </summary>
        private Color ospDiffuseColor;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        private String markerID;
        /// <summary>
        /// combobox選項的內容
        /// </summary>
        private List<String> comboboxList = MarkerDatabase.MarkerIDList;

                               
        public ModelSettingItem()
        {                       
            
        }
        /// <summary>
        /// 設定模型材質
        /// </summary>        
        private void SetModelMaterial()
        {
            if (Model.ModelMaterial == null)
            {
                Model.ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
                Model.ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
                float ambient = 0.0f;
                Model.ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                Model.ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                int Specular = 90;
                Model.ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
                Model.ModelMaterial.SpecularShininess = 60;
            }

            Model.ModelMaterial.DiffuseColor = ModelDiffuseColor.ToColor4();
        }
        /// <summary>
        /// 設定OSP材質
        /// </summary>        
        private void SetOSPMaterial()
        {
            if (OSP.ModelMaterial == null)
            {
                OSP.ModelMaterial = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
                OSP.ModelMaterial.ReflectiveColor = SharpDX.Color.Black;
                float ambient = 0.0f;
                OSP.ModelMaterial.AmbientColor = new SharpDX.Color(ambient, ambient, ambient, 1.0f);
                OSP.ModelMaterial.EmissiveColor = SharpDX.Color.Black; //這是自己發光的顏色
                int Specular = 90;
                OSP.ModelMaterial.SpecularColor = new SharpDX.Color(Specular, Specular, Specular, 255);
                OSP.ModelMaterial.SpecularShininess = 60;
            }

            OSP.ModelMaterial.DiffuseColor = OSPDiffuseColor.ToColor4();
        }
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        public void Load()
        {            
            LoadModel();            
            LoadOSP();
        }
        /// <summary>
        /// load進模型檔案
        /// </summary>
        public void LoadModel()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (Model.IsLoaded || !System.IO.File.Exists(ModelFilePath))
            {
                return;
            }
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            Model.ModelContainer = reader.Read(ModelFilePath);

            var geometryModel = Model.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            //設定模型材質
            //SetMaterial(ref Model.ModelMaterial, modelDiffuseColor);
            SetModelMaterial();

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
            Model.MarkerID = this.MarkerID;

            Model.ModelGeometry = modelGeometry;

            Model.ModelTransform = new MatrixTransform3D();

            OSP.IsOSP = false;

            Model.IsLoaded = true;                        
        }
        /// <summary>
        /// load進OSP檔案
        /// </summary>
        public void LoadOSP()
        {
            //ModelGeometry已經有幾何模型存在內部 及 阻擋檔案不存在的情況
            if (OSP.IsLoaded || !System.IO.File.Exists(OSPFilePath))
            {
                return;
            }
            
            //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
            StLReader reader = new HelixToolkit.Wpf.StLReader();

            OSP.ModelContainer = reader.Read(OSPFilePath);
            
            var geometryModel = OSP.ModelContainer.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

            var mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

            if (mesh.Positions.Count != 6) 
            {
                return;
            }

            OSP.OSPOriNormal = new Vector3D(mesh.Normals[0].X, mesh.Normals[0].Y, mesh.Normals[0].Z);
            OSP.OSPOriNormal.Normalize();//將上述向量正規化
            OSP.OSPPlanePoint  = mesh.Positions[0];
            //設定模型材質
            SetOSPMaterial();
           
            //定義整個對稱面八個點
            Point3D[] ospPoint = new Point3D[8];
            int[] repeatIndex = new int[2] { -1, -1 };
            int[] repeatIndex2 = new int[2] { -1, -1 };

            //前半網格點
            for (int i=0,k=0;i<mesh.Positions.Count/2 ;i++)
            { //後半網格點
                for (int j = 3; j < mesh.Positions.Count; j++)
                {   //尋找前半跟後半有哪些重複存下引數在repeatIndex  repeatIndex2
                    double error = Math.Abs(Math.Pow(mesh.Positions[i].X - mesh.Positions[j].X, 2)) + Math.Abs(Math.Pow(mesh.Positions[i].Y - mesh.Positions[j].Y, 2)) + Math.Abs(Math.Pow(mesh.Positions[i].Z - mesh.Positions[j].Z, 2));
                    if (error < 10E-008)
                    {
                        repeatIndex[k] = i;
                        repeatIndex2[k] = j;
                        k++;
                    }                   
                 }
            }
             //找出沒有重複的那個引數字
            int thirdIndex = 3 - repeatIndex[0] - repeatIndex[1];
            int thirdIndex2 = 12 - repeatIndex2[0] - repeatIndex2[1];

          
            if (thirdIndex == 0)
            {
                ospPoint[0] = mesh.Positions[0];
                ospPoint[1] = mesh.Positions[1];
                ospPoint[2] = mesh.Positions[thirdIndex2];
                ospPoint[3] = mesh.Positions[2];
            }
            else if (thirdIndex == 1)
            {
                ospPoint[0] = mesh.Positions[1];
                ospPoint[1] = mesh.Positions[2];
                ospPoint[2] = mesh.Positions[thirdIndex2];
                ospPoint[3] = mesh.Positions[0];
            }
            else
            {
                ospPoint[0] = mesh.Positions[2];
                ospPoint[1] = mesh.Positions[0];
                ospPoint[2] = mesh.Positions[thirdIndex2];
                ospPoint[3] = mesh.Positions[1];
            }


            for (int i=4; i< ospPoint.Length; i++)
            {
                ospPoint[i] = new Point3D(ospPoint[i - 4].X - 0.1 * mesh.Normals[0].X, 
                                                                  ospPoint[i - 4].Y - 0.1 * mesh.Normals[0].Y,
                                                                  ospPoint[i - 4].Z - 0.1 * mesh.Normals[0].Z);
            }

            for (int i = 0; i < ospPoint.Length/2; i++)
            {
                ospPoint[i] = new Point3D(ospPoint[i].X + 0.1 * mesh.Normals[0].X,
                                                                  ospPoint[i].Y + 0.1 * mesh.Normals[0].Y,
                                                                  ospPoint[i].Z + 0.1 * mesh.Normals[0].Z);
            }



            HelixToolkit.Wpf.SharpDX.MeshGeometry3D ospGeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            ospGeometry.Normals = new Vector3Collection();
            ospGeometry.Positions = new Vector3Collection();
            ospGeometry.Indices = new IntCollection();


            //var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder(true, false, false);
            //var center = new Vector3(Convert.ToSingle(-0.463861), Convert.ToSingle(-127.7), Convert.ToSingle(-183.243));
            //builder.AddSphere(center, 2, 12, 12);
            //center = new Vector3(Convert.ToSingle(-0.473168), Convert.ToSingle(-127.317), Convert.ToSingle(-182.388));
            //builder.AddSphere(center, 2, 12, 12);

            

            ////center = new Vector3(Convert.ToSingle(0.622753), Convert.ToSingle(-196.831), Convert.ToSingle(-191.1));
            ////builder.AddSphere(center, 2, 12, 12);

            ////center = new Vector3(Convert.ToSingle(0.613464), Convert.ToSingle(-196.201), Convert.ToSingle(-191.183));
            ////builder.AddSphere(center, 2, 12, 12);

            //HelixToolkit.Wpf.SharpDX.MeshGeometry3D Sphere = builder.ToMeshGeometry3D();




            // 0 1 2 3
            CreatePoint(0, 1, 2, 3, ospGeometry.Positions, ospPoint);

            CreateNormal(0, 1, 2, ospGeometry.Normals, ospPoint);

            ////4 0 3 7
            //CreatePoint(4, 0, 3, 7, ospGeometry2.Positions, ospPoint);
            //CreateNormal(4, 0, 3, ospGeometry2.Normals, ospPoint);

            ////5 1 0 4
            //CreatePoint(5, 1, 0, 4, ospGeometry2.Positions, ospPoint);
            //CreateNormal(5, 1, 0, ospGeometry2.Normals, ospPoint);

            ////1 5 6 2 
            //CreatePoint(1, 5, 6, 2, ospGeometry2.Positions, ospPoint);
            //CreateNormal(1, 5, 6, ospGeometry2.Normals, ospPoint);

            ////3 2 6 7
            //CreatePoint(3, 2, 6, 7, ospGeometry2.Positions, ospPoint);
            //CreateNormal(3, 2, 6, ospGeometry2.Normals, ospPoint);

            ////5 4 7 6
            CreatePoint(5, 4, 7, 6, ospGeometry.Positions, ospPoint);
            CreateNormal(5, 4, 7, ospGeometry.Normals, ospPoint);

            for (int i = 0; i < ospGeometry.Positions.Count ; i++) 
            {
                ospGeometry.Indices.Add(i);
            }


            OSP.MarkerID = this.MarkerID;

            //if (OSPFilePath == "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl")
            //{
            //    OSP.ModelGeometry = /*ospGeometry;*/Sphere;
            //}
            //else
            //{
                OSP.ModelGeometry = ospGeometry;
            //}
            OSP.ModelTransform = new MatrixTransform3D();

            OSP.IsOSP = true;

            OSP.IsLoaded = true;
        }
        private void CreatePoint(int index0, int index1, int index2, int index3 , Vector3Collection positions,Point3D[]  ospPoint )
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

        


        public String ModelFilePath
        {
            get
            {
                return modelFilePath;
            }
            set
            {
                SetValue(ref modelFilePath, value);
                Model.IsLoaded = false;                
            }
        }
        public String OSPFilePath
        {
            get
            {
                return ospFilePath;
            }
            set
            {
                SetValue(ref ospFilePath, value);
                OSP.IsLoaded = false;
            }
        }
        public Color ModelDiffuseColor
        {
            get
            {
                return modelDiffuseColor;
            }
            set
            {
                SetValue(ref modelDiffuseColor, value);
                if (Model.IsLoaded == true)
                {
                    SetModelMaterial();
                }
            }
        }
        public Color OSPDiffuseColor
        {
            get
            {
                return ospDiffuseColor;
            }
            set
            {
                SetValue(ref ospDiffuseColor, value);
                if (OSP.IsLoaded == true)
                {
                    SetOSPMaterial();
                }
            }
        }
        public String MarkerID
        {
            get
            {
                return markerID;
            }
            set
            {
                SetValue(ref markerID, value);
            }
        }
        public List<String> ComboBoxList
        {
            get
            {
                return comboboxList;
            }
            set
            {
                SetValue(ref comboboxList, value);
            }
        }
    }
}
