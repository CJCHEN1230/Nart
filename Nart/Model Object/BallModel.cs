using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nart.Model_Object
{
    public class BallModel : MeshGeometryModel3D ,  INotifyPropertyChanged
    {

        private string ballName;
        private string ballInfo;
        public BallModel()
        {
           
        }

        public string BallName
        {
            get
            {
                return ballName;
            }
            set
            {
                SetValue(ref ballName, value);
            }
        }
        public string BallInfo
        {
            get
            {
                return ballInfo;
            }
            set
            {
                SetValue(ref ballInfo, value);
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
