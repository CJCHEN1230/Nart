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
    public partial class ExpanderNavigationBalls : Expander
    {

      
        public ExpanderNavigationBalls()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (BallListView.SelectedItem != null)
            {
                //選擇的BallModel
                BallModel selectedModelItem = (BallModel)BallListView.SelectedItem;

                int temp = BallListView.SelectedIndex;

                ObservableCollection<BallModel> ballCollection = MainViewModel.Data.BallCollection;

                ballCollection.Remove(selectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ballCollection.Count == temp)
                {
                    BallListView.SelectedIndex = ballCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    BallListView.SelectedIndex = temp;
                }

                ListViewItem item = BallListView.ItemContainerGenerator.ContainerFromIndex(BallListView.SelectedIndex) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }

            }
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

