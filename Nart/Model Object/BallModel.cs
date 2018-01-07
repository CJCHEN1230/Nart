using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Nart.Model_Object
{
    [Serializable]
    public class BallModel : MeshGeometryModel3D ,  INotifyPropertyChanged , ISerializable
    {
        /// <summary>
        /// 紀錄模型中心
        /// </summary>
        public Vector3 BallCenter;

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
        public BallModel(SerializationInfo info, StreamingContext context)
        {
            IsRendering = (bool)info.GetValue("IsRendering", typeof(bool));
            Vector3 ballCenter = new Vector3();
            ballCenter.X = (float)info.GetValue("BallCenter_X", typeof(float));
            ballCenter.Y = (float)info.GetValue("BallCenter_Y", typeof(float));
            ballCenter.Z = (float)info.GetValue("BallCenter_Z", typeof(float));
            BallCenter = ballCenter;

            BallName = (string)info.GetValue("BallName", typeof(string));
            BallInfo = (string)info.GetValue("BallInfo", typeof(string));
            ModelType = (ModelType)info.GetValue("ModelType", typeof(ModelType));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsRendering", IsRendering);
            info.AddValue("BallCenter_X", BallCenter.X);
            info.AddValue("BallCenter_Y", BallCenter.Y);
            info.AddValue("BallCenter_Z", BallCenter.Z);
            info.AddValue("BallName", BallName);
            info.AddValue("BallInfo", BallInfo);
            info.AddValue("ModelType", ModelType);
        }

        public void CreateBall()
        {
            var ballContainer = new HelixToolkit.Wpf.SharpDX.MeshBuilder();

            ballContainer.AddSphere(BallCenter, 1.5);

            Geometry = ballContainer.ToMeshGeometry3D();

            this.Material = PhongMaterials.White;            
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
