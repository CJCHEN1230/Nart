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

namespace NartControl
{
    /// <summary>
    /// RegistrationView.xaml 的互動邏輯
    /// </summary>
    public partial class RegistrationView : Window
    {

        RegistrationViewModel _registrationViewModel;

        public RegistrationView()
        {
            InitializeComponent();
            //_registrationViewModel = new RegistrationViewModel(this);
            //this.DataContext = _registrationViewModel;

        }

        private void AddItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void ReomoveItemClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

