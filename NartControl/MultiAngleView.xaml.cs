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
        MultiAngleViewModel _multiAngleViewModel ;
       
        public MultiAngleView()
        {
            InitializeComponent();
            _multiAngleViewModel = new MultiAngleViewModel(this);
            this.DataContext = _multiAngleViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext
           
            //_multiAngleViewModel = (MultiAngleViewModel)base.DataContext;
        }
        /// <summary>
        /// 增加模型進去Container當中，並重設相機位置
        /// </summary>
        public void AddModel(MeshGeometryModel3D Model)
        {
            sharedContainer.Items.Add(Model);
            _multiAngleViewModel.ResetCameraPosition();
        }       
    }
}
