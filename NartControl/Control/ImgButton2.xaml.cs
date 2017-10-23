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

namespace NartControl.Control
{
    /// <summary>
    /// ImgButton2.xaml 的互動邏輯
    /// </summary>
    public partial class ImgButton2 : Button
    {
        public ImgButton2()
        {
            InitializeComponent();
        }


        //第一個參數是和此DependencyProperty關聯的Property名稱
        //第二個用來指明此依賴屬性用來存儲什麼類型的值
        //第三個參數用來指明此依賴屬性的宿主是什麼類型或者說DependencyProperty.Register方法將把這個依賴屬性註冊關聯到哪個類型上。
        //第四個參數是用來設定一些Dependency Property的細節，例如是否要延續父層的屬性、是否會影響尺寸、預設值等等
        //第五個參數是當值改變的時候，要呼叫的方法。
        public static readonly DependencyProperty ButtonImageSourceProperty = DependencyProperty.Register("ButtonImageSource", typeof(Uri), typeof(ImgButton2), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSourceChanged)));

        public Uri ButtonImageSource
        {
            set
            {
                this.SetValue(ButtonImageSourceProperty, value);
            }
            get
            {
                return (Uri)GetValue(ButtonImageSourceProperty);
            }
        }

        private static void ImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ImgButton2 userControl = (ImgButton2)sender;

            userControl.ButtonImage.Source = new BitmapImage((Uri)e.NewValue);
        }
        #region ButtonLabel property


        public string ButtonLabel
        {
            set
            {
                SetValue(ButtonLabelProperty, value);
            }
            get
            {
                return (string)GetValue(ButtonLabelProperty);
            }
        }

        public static readonly DependencyProperty ButtonLabelProperty = DependencyProperty.Register("ButtonLabel", typeof(string), typeof(ImgButton2), new FrameworkPropertyMetadata("Default"));
        #endregion
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = new SolidColorBrush(Colors.LightYellow);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
