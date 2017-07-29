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
        public CameraControl CamCtrl
        {
            get;
            set;
        }
        private MainView _window;
        public MainView MainWindow
        {
            get { return this._window; }
            set { this._window = value; }
        }
        public MainViewModel(MainView window)
        {
            MainWindow = window;
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
