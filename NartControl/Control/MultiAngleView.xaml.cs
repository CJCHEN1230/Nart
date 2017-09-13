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

namespace NartControl.Control
{
    /// <summary>
    /// MultiAngleView.xaml 的互動邏輯
    /// </summary>
    public partial class MultiAngleView : UserControl
    {
        MultiAngleViewModel _multiAngleViewModel;

        public MultiAngleView()
        {
            InitializeComponent();
            _multiAngleViewModel = new MultiAngleViewModel(this);
            this.DataContext = _multiAngleViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext      


            //test2.LoadSTL("D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl");
            //_multiAngleViewModel.ModelInfoCollection.Add(test2);
        }
        /// <summary>
        /// 增加模型進去Container當中，並重設相機位置
        /// </summary>
        public void PutModellnView(MeshGeometryModel3D Model)
        {
            sharedContainer.Items.Add(Model);
            _multiAngleViewModel.ResetCameraPosition();
        }


        public Nart.ModelInfo test2 = new Nart.ModelInfo
        {
            ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                                                                                ,
            BSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
            ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
            BSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
        };

        public System.Windows.Media.Media3D.Transform3DGroup MA = new System.Windows.Media.Media3D.Transform3DGroup();
        private void button_Click(object sender, RoutedEventArgs e)
        {
            ////////////////模型累加移動
            Matrix3D TEST = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -100, -100, 100, 1);
            MatrixTransform3D tr = new MatrixTransform3D(TEST);


            MA.Children.Add(tr);

            _multiAngleViewModel.mdata1.ModelTransform = MA;

        }
    }
}
