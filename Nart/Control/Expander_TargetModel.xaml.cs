using Nart.Model_Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Expander_TargetModel.xaml 的互動邏輯
    /// </summary>
    public partial class ExpanderTargetModel : Expander
    {

        public ExpanderTargetModel()
        {
            InitializeComponent();
        }

        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (BoneListView.SelectedItem != null)
            {
                //選擇的BoneModel
                BoneModel selectedModelItem = (BoneModel)BoneListView.SelectedItem;

                int temp = BoneListView.SelectedIndex;

                var ballCollection = MainViewModel.Data.BoneCollection;

                ballCollection.Remove(selectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ballCollection.Count == temp)
                {
                    BoneListView.SelectedIndex = ballCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    BoneListView.SelectedIndex = temp;
                }

                ListViewItem item = BoneListView.ItemContainerGenerator.ContainerFromIndex(BoneListView.SelectedIndex) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }

            }
        }
        public void BindBoneCollection(Projectata data)
        {
            //將data中的BoneCollection綁到此控制項的item上面   
            Binding binding = new Binding("BoneCollection");
            binding.Source = data;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(BoneListView, ItemsControl.ItemsSourceProperty, binding);            
        }
    }
}
