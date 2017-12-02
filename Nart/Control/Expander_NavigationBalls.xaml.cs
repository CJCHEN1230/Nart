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
    public partial class Expander_NavigationBalls : Expander, INotifyPropertyChanged
    {
        private ObservableCollection<BallModel> ballCollection = new ObservableCollection<BallModel>();
        public Expander_NavigationBalls()
        {
            InitializeComponent();

            for (int i =0;i<5 ;i++)
            {
                BallModel ball = new BallModel();
                ball.BallName = i.ToString();
                ball.BallInfo = "!!!!!!!!!!!!!!!!!!!!!!!!!!";
                BallCollection.Add(ball);
            }
          

            this.DataContext = this;
        }

        public ObservableCollection<BallModel> BallCollection
        {
            get
            {
                return ballCollection;
            }
            set
            {
                SetValue(ref ballCollection, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string info = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        protected bool SetValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")
        {
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}

