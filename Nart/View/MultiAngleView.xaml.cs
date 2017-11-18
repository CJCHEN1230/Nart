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
using System.Collections.ObjectModel;
using Nart.Model_Object;

namespace Nart
{
    /// <summary>
    /// MultiAngleView.xaml 的互動邏輯
    /// </summary>
    public partial class MultiAngleView : UserControl
    {
        public MultiAngleViewModel _multiAngleViewModel;

        public MultiAngleView()
        {
            InitializeComponent();         
            _multiAngleViewModel = new MultiAngleViewModel(this);
            this.DataContext = _multiAngleViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext                  

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
                    DraggableTriangle movedTriangle = MultiAngleViewModel.TriangleModelCollection[2] as DraggableTriangle;
            DraggableTriangle staTriangle = MultiAngleViewModel.TriangleModelCollection[2] as DraggableTriangle;
            Point3D red2 = movedTriangle.Transform.Transform(new Point3D(movedTriangle.positions[0].X, movedTriangle.positions[0].Y, movedTriangle.positions[0].Z));
            Point3D green2 = movedTriangle.Transform.Transform(new Point3D(movedTriangle.positions[1].X, movedTriangle.positions[1].Y, movedTriangle.positions[1].Z));
            Point3D blue2 = movedTriangle.Transform.Transform(new Point3D(movedTriangle.positions[2].X, movedTriangle.positions[2].Y, movedTriangle.positions[2].Z));

            Point3D red = new Point3D(staTriangle.positions[0].X, staTriangle.positions[0].Y, staTriangle.positions[0].Z);
            Point3D green = new Point3D(staTriangle.positions[1].X, staTriangle.positions[1].Y, staTriangle.positions[1].Z);
            Point3D blue = new Point3D(staTriangle.positions[2].X, staTriangle.positions[2].Y, staTriangle.positions[2].Z);


            HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D temp = new HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D();


                    var b1 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
                    b1.AddSphere(new Vector3(Convert.ToSingle(red2.X), Convert.ToSingle(red2.Y), Convert.ToSingle(red2.Z)), 15);
            b1.AddSphere(new Vector3(Convert.ToSingle(green2.X), Convert.ToSingle(green2.Y), Convert.ToSingle(green2.Z)), 15);
            b1.AddSphere(new Vector3(Convert.ToSingle(blue2.X), Convert.ToSingle(blue2.Y), Convert.ToSingle(blue2.Z)), 15);
            temp.Geometry = b1.ToMeshGeometry3D();
                     temp.Material = PhongMaterials.White;



            HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D temp2 = new HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D();


            var b2 = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            b2.AddSphere(new Vector3(Convert.ToSingle(red.X), Convert.ToSingle(red.Y), Convert.ToSingle(red.Z)), 15);
            b2.AddSphere(new Vector3(Convert.ToSingle(green.X), Convert.ToSingle(green.Y), Convert.ToSingle(green.Z)), 15);
            b2.AddSphere(new Vector3(Convert.ToSingle(blue.X), Convert.ToSingle(blue.Y), Convert.ToSingle(blue.Z)), 15);
            temp2.Geometry = b2.ToMeshGeometry3D();
            temp2.Material = PhongMaterials.Yellow;


            MultiAngleViewModel.NormalModelCollection.Add(temp);
            MultiAngleViewModel.NormalModelCollection.Add(temp2);
        }
    }
}
