using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;

namespace NartControl
{
    /// <summary>
    /// MultiAngleView.xaml 的互動邏輯
    /// </summary>
    public partial class MultiAngleView : UserControl
    {


        MultiAngleViewModel _multiAngleViewModel = new MultiAngleViewModel();
       
        public MultiAngleView()
        {
            InitializeComponent();
            this.DataContext = _multiAngleViewModel;
            _multiAngleViewModel = (MultiAngleViewModel)base.DataContext;
        }

        //public void LoadSTL(string path)
        //{
 
                
               //////using Point3D = System.Windows.Media.Media3D.Point3D;
               //////using Vector3D = System.Windows.Media.Media3D.Vector3D;
        //    //利用helixtoolkit.wpf裡面提供的StlReader讀檔案，後續要轉成wpf.sharpdx可用的格式
        //    StLReader reader = new HelixToolkit.Wpf.StLReader();
        //    Model3DGroup model3dgroup = reader.Read(path);

        //    System.Windows.Media.Media3D.GeometryModel3D geometryModel = model3dgroup.Children[0] as System.Windows.Media.Media3D.GeometryModel3D;

        //    System.Windows.Media.Media3D.MeshGeometry3D mesh = geometryModel.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

        //    //設定模型材質
        //    PhongMaterial material = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
        //    material.AmbientColor = new Color4(Convert.ToSingle(0.349), Convert.ToSingle(0.349), Convert.ToSingle(0.349), 1);
        //    material.DiffuseColor = new Color4(Convert.ToSingle(0), Convert.ToSingle(0.5019), Convert.ToSingle(0), 1);
        //    material.SpecularColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);
        //    material.SpecularShininess = 12.8F;
        //    material.ReflectiveColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);


        //    //設定模型幾何形狀
        //    HelixToolkit.Wpf.SharpDX.MeshGeometry3D geometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

        //    geometry.Normals = new Vector3Collection();
        //    geometry.Positions = new Vector3Collection();
        //    geometry.Indices = new IntCollection();

        //    //將從stlreader讀到的資料轉入
        //    foreach (Point3D position in mesh.Positions)
        //    {
        //        geometry.Positions.Add(new Vector3(
        //              Convert.ToSingle(position.X)
        //            , Convert.ToSingle(position.Y)
        //            , Convert.ToSingle(position.Z)));
        //    }

        //    foreach (Vector3D normal in mesh.Normals)
        //    {
        //        geometry.Normals.Add(new Vector3(
        //              Convert.ToSingle(normal.X)
        //            , Convert.ToSingle(normal.Y)
        //            , Convert.ToSingle(normal.Z)));
        //    }

        //    foreach (Int32 triangleindice in mesh.TriangleIndices)
        //    {
        //        geometry.Indices.Add(triangleindice);
        //    }

        //    //將建立好的指派進helix.sharp格式
        //    MeshGeometry = new MeshGeometryModel3D();
        //    MeshGeometry.Material = material;
        //    MeshGeometry.Geometry = geometry;

        //    MeshGeometry.Transform = new TranslateTransform3D(0, 140, 180);
        //    //meshgeometry.Transform = new TranslateTransform3D(0, 140, 180);


        //    //this.ModelGeometry.Add(MeshGeometry);




        //    //meshgeometry.Attach(modelView.RenderHost); //viewport3dx

        //}
    }
}
