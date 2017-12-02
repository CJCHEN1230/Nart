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

namespace Nart.Control
{
    /// <summary>
    /// Expander_Info.xaml 的互動邏輯
    /// </summary>
    public partial class Expander_Info : Expander
    {
        public Expander_Info()
        {
            InitializeComponent();
        }

        public void SetInformation(Projectata data)
        {

            SetBinding(data, NameTB, "Name", TextBlock.TextProperty, BindingMode.TwoWay);
            SetBinding(data, IDTB, "ID", TextBlock.TextProperty, BindingMode.TwoWay);
            SetBinding(data, InstitutionTB, "Institution", TextBlock.TextProperty, BindingMode.TwoWay);

        }

        private void SetBinding(object source, DependencyObject target, string propertyName, DependencyProperty dp, BindingMode mode)
        {
            Binding binding = new Binding(propertyName);
            binding.Source = source;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, dp, binding);
        }



            
    }
}
