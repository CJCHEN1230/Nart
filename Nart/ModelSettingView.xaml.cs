using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// RegistrationView.xaml 的互動邏輯
    /// </summary>
    public partial class ModelSettingView : Window
    {
        /// <summary>
        /// ModelSettingView的ViewModel
        /// </summary>
        private ModelSettingViewModel _modelSettingViewModel;
        public ModelSettingView()
        {
            InitializeComponent();
            _modelSettingViewModel = new ModelSettingViewModel(this);
            this.DataContext = _modelSettingViewModel;
        }
        /// <summary>
        /// 增加清單中的Model項目
        /// </summary>
        private void AddItemClick(object sender, RoutedEventArgs e)
        {
            _modelSettingViewModel.AddItem();
        }
        /// <summary>
        /// 減少清單中的Model項目
        /// </summary>
        private void ReomoveItemClick(object sender, RoutedEventArgs e)
        {
            _modelSettingViewModel.RemoveItem();
        }
        /// <summary>
        /// 設定頁面按下ok
        /// </summary>
        private void OKClick(object sender, RoutedEventArgs e)
        {
            _modelSettingViewModel.LoadSettingModel();
            this.Hide();
        }
        /// <summary>
        /// 設定頁面按下紅色按紐
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

