using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Nart.Model_Object
{
    public class BallModel : MeshGeometryModel3D ,  INotifyPropertyChanged
    {
        /// <summary>
        /// 紀錄模型中心
        /// </summary>
        public Vector3 Center;

        public ModelType ModelType;
        private string _ballName;
        private string _ballInfo;


        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D ballGeometry;
        public HelixToolkit.Wpf.SharpDX.MeshGeometry3D pipeGeometry;

        public BallModel()
                    : this(new Point3D(0,0,0))
        {
        }
        public BallModel(Vector3 center)
                    : this(new Point3D(center.X, center.Y, center.Z))
        {
        }
        public BallModel(Point3D center)
        {
           
        }

        public string BallName
        {
            get
            {
                return _ballName;
            }
            set
            {
                SetValue(ref _ballName, value);
            }
        }
        public string BallInfo
        {
            get
            {
                return _ballInfo;
            }
            set
            {
                SetValue(ref _ballInfo, value);
            }
        }

        



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
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
