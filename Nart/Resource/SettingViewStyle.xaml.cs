using NartControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nart
{
    public partial class SettingViewStyle : ResourceDictionary
    {
        public SettingViewStyle()
        {
            InitializeComponent();          
        }
        

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Hide();
        }
    }
}
