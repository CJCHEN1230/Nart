using Nart.Model_Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nart.Control
{
    /// <summary>
    /// Expander_NavigationBalls.xaml 的互動邏輯
    /// </summary>
    public partial class Expander_NavigationBalls : Expander
    {

      
        public Expander_NavigationBalls()
        {
            InitializeComponent();
            this.DataContext = this;
        }

     
        
        public void BindBallCollection(Projectata data)
        {
            Binding binding = new Binding("BallCollection");
            binding.Source = data;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(BallListView, ItemsControl.ItemsSourceProperty, binding);

            Binding binding2 = new Binding("CanSelectPoints");
            binding2.Source = data;
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(SelectTB, ToggleButton.IsCheckedProperty, binding2);

            Binding binding3 = new Binding("SelectPointState");
            binding3.Source = data;
            binding3.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(stateTB, TextBlock.TextProperty, binding3);

        }
    }
}

