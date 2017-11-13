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
using System.Windows.Shapes;

namespace Nart
{
    /// <summary>
    /// NavigateView.xaml 的互動邏輯
    /// </summary>    
    public partial class NavigateView : Window
    {

        public NavigateViewModel _navigateViewModel;
        public NavigateView()
        {
            InitializeComponent();
            _navigateViewModel = new NavigateViewModel(this);
            this.DataContext = _navigateViewModel; //將_multiAngleViewModel的資料環境傳給此DataContext  
        }

    }
}
