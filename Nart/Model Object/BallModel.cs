﻿using HelixToolkit.Wpf;
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
        /// 球伴隨的模型
        /// </summary>
        public ModelType ModelType;
        /// <summary>
        /// 球的名稱，主要用來Binding在Expander的資料的
        /// </summary>
        private string _ballName;
        /// <summary>
        /// 也是模型的中心，主要用來Binding在Expander裡面的X座標
        /// </summary>
        private string _ballXCoord;
        /// <summary>
        /// 也是模型的中心，主要用來Binding在Expander裡面的的Y座標
        /// </summary>
        private string _ballYCoord;
        /// <summary>
        /// 也是模型的中心，主要用來Binding在Expander裡面的的Z座標
        /// </summary>
        private string _ballZCoord;
        /// <summary>
        /// 紀錄模型中心
        /// </summary>
        private Vector3 _ballCenter;
        /// <summary>
        /// 導引之後移動的距離
        /// </summary>
        private Vector3 _ballDistance;
        
      
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
            BallXCoord = (string)info.GetValue("BallXCoord", typeof(string));
            BallYCoord = (string)info.GetValue("BallYCoord", typeof(string));
            BallZCoord = (string)info.GetValue("BallZCoord", typeof(string));

            Vector3 ballDistance = new Vector3();
            ballDistance.X = (float)info.GetValue("BallDistanceX", typeof(float));
            ballDistance.Y = (float)info.GetValue("BallDistanceY", typeof(float));
            ballDistance.Z = (float)info.GetValue("BallDistanceZ", typeof(float));
            BallDistance = ballDistance;

            ModelType = (ModelType)info.GetValue("ModelType", typeof(ModelType));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsRendering", IsRendering);
            info.AddValue("BallCenter_X", BallCenter.X);
            info.AddValue("BallCenter_Y", BallCenter.Y);
            info.AddValue("BallCenter_Z", BallCenter.Z);
            info.AddValue("BallName", BallName);
            info.AddValue("BallXCoord", BallXCoord);
            info.AddValue("BallYCoord", BallYCoord);
            info.AddValue("BallZCoord", BallZCoord);
            info.AddValue("ModelType", ModelType);
            info.AddValue("BallDistanceX", BallDistance.X);
            info.AddValue("BallDistanceY", BallDistance.Y);
            info.AddValue("BallDistanceZ", BallDistance.Z);
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
        public string BallXCoord
        {
            get
            {
                return _ballXCoord;
            }
            set
            {
                SetValue(ref _ballXCoord, value);
            }
        }

        public string BallYCoord
        {
            get
            {
                return _ballYCoord;
            }
            set
            {
                SetValue(ref _ballYCoord, value);
            }
        }

        public string BallZCoord
        {
            get
            {
                return _ballZCoord;
            }
            set
            {
                SetValue(ref _ballZCoord, value);
            }
        }

        public Vector3 BallDistance
        {
            get
            {
                return _ballDistance;
            }
            set
            {
                SetValue(ref _ballDistance, value);
            }
        }
        public Vector3 BallCenter
        {
            get
            {
                return _ballCenter;
            }
            set
            {
                SetValue(ref _ballCenter, value);

                BallXCoord = "X:" + Math.Round(_ballCenter.X, 2);
                BallYCoord = "Y:" + Math.Round(_ballCenter.Y, 2);
                BallZCoord = "Z:" + Math.Round(_ballCenter.Z, 2);
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
