using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public class MainViewModel : INotifyPropertyChanged
    {




        public Environment EnvironmentSetting
        {
            get;
            set;
        }

        public CameraControl _camCtrl
        {
            get;
            set;
        }


        private string _pointNumber;

        public string PointNumber
        {
            get { return this._pointNumber; }
            set
            {
                if (this._pointNumber != value)
                {
                    this._pointNumber = value;
                    this.NotifyPropertyChanged("PointNumber");
                }
            }
        }

        //public static Model3DGroup _model3dgroup = new Model3DGroup();

        //public static List<ModelData> AllModelData = new List<ModelData>(5);
        //public Element3DCollection ModelGeometry { get; private set; }

        //public GroupModel3D GroupModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
