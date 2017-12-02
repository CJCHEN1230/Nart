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
        }

        void OnStretchedHeaderTemplateLoaded(object sender, RoutedEventArgs e)
        {
            Border rootElem = sender as Border;

            ContentPresenter contentPres =
                rootElem.TemplatedParent as ContentPresenter;

            contentPres.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

    }
}

