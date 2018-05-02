using HelixToolkit.Wpf.SharpDX;
using Nart.Model_Object;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    [Serializable]
    public class ProjectData : ObservableObject , ISerializable
    {        
        public bool IsRegInitialized = false;
        public bool IsNavDone = true;

        public float Stage1Red;
        public float Stage1Green;
        public float Stage1Blue;
        public float Stage2Red;
        public float Stage2Green;
        public float Stage2Blue;

        public float DA;    //DeviationAngle
        public float FDA;   // FrontalDeviationAngle
        public float HDA;   //HorizontalDeviationAngle
        public float DD;    //DeviationDistance
        public float PDD;   //PosteriorDeviationDistance;

         //病人資訊
        private string _name = "蔡慧君";
        private string _id = "123456";
        private string _institution = "成大醫院";
        private string _regFilePath = "./data/蔡慧君測試用.txt";
        private string _firstNavigation = "Maxilla";
        private bool _canSelectPoints = true;
        private string _selectPointState = "ON";
        private bool _isNavSet = false;
        private bool _isRegistered = false;
        private ObservableCollection<BallModel> _ballCollection=  new ObservableCollection<BallModel>();
        private ObservableCollection<BoneModel> _boneCollection =  new ObservableCollection<BoneModel>();
        private ObservableCollection <BoneModel> _targetCollection = new ObservableCollection<BoneModel>();
        private ObservableCollection<Element3D> _ospCollection = new ObservableCollection<Element3D>();

        static ProjectData()
        {
            //MarkerDatabase MarkerData = new MarkerDatabase();
        }
        public ProjectData()
        {            
        }

        public ProjectData(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            ID = (string)info.GetValue("ID", typeof(string));
            Institution = (string)info.GetValue("Institution", typeof(string));
            RegFilePath = (string)info.GetValue("RegFilePath", typeof(string));
            IsRegInitialized = (bool)info.GetValue("IsRegInitialized", typeof(bool));
            IsNavSet = (bool)info.GetValue("IsNavSet", typeof(bool));
            Stage1Red = (float)info.GetValue("Stage1Red", typeof(float));
            Stage1Green = (float)info.GetValue("Stage1Green", typeof(float));
            Stage1Blue = (float)info.GetValue("Stage1Blue", typeof(float));
            Stage2Red = (float)info.GetValue("Stage2Red", typeof(float));
            Stage2Green = (float)info.GetValue("Stage2Green", typeof(float));
            Stage2Blue = (float)info.GetValue("Stage2Blue", typeof(float));
            DA = (float)info.GetValue("DA", typeof(float));
            FDA = (float)info.GetValue("FDA", typeof(float));
            HDA = (float)info.GetValue("HDA", typeof(float));
            DD = (float)info.GetValue("DD", typeof(float));
            PDD = (float)info.GetValue("PDD", typeof(float));
            FirstNavigation = (string)info.GetValue("FirstNavigation", typeof(string));
            IsNavDone = (bool)info.GetValue("IsNavDone", typeof(bool));

            int count = (int)info.GetValue("BallCollection_Count", typeof(int));
            ObservableCollection<BallModel> ballCollection = new ObservableCollection<BallModel>();
            for (int i = 0; i < count; i++)
            {
                BallModel ballModel = (BallModel)info.GetValue("BallCollection_" + i, typeof(BallModel));
                ballModel.CreateBall();
                ballCollection.Add(ballModel);
            }
            BallCollection=ballCollection;

            count = (int)info.GetValue("BoneCollection_Count", typeof(int));
            ObservableCollection<BoneModel> boneCollection = new ObservableCollection<BoneModel>();
            for (int i = 0; i < count; i++)
            {
                BoneModel boneModel = (BoneModel)info.GetValue("BoneCollection_"+i, typeof(BoneModel));
                boneCollection.Add(boneModel);
            }
            BoneCollection = boneCollection;

            count = (int)info.GetValue("TargetCollection_Count", typeof(int));
            ObservableCollection<BoneModel> targetCollection = new ObservableCollection<BoneModel>();
            for (int i = 0; i < count; i++)
            {
                BoneModel targetModel = (BoneModel)info.GetValue("TargetCollection_" + i, typeof(BoneModel));
                targetCollection.Add(targetModel);
            }
            TargetCollection = targetCollection;

        }
        public void UpdateData(ProjectData projectData)
        {

            this.Name = projectData.Name;
            this.ID = projectData.ID;
            this.Institution = projectData.Institution;
            this.RegFilePath = projectData.RegFilePath;
            this.IsRegInitialized = projectData.IsRegInitialized;
            this.IsNavSet = projectData._isNavSet;
            this.Stage1Red = projectData.Stage1Red;
            this.Stage1Green = projectData.Stage1Green;
            this.Stage1Blue = projectData.Stage1Blue;
            this.Stage2Red = projectData.Stage2Red;
            this.Stage2Green = projectData.Stage2Green;
            this.Stage2Blue = projectData.Stage2Blue;

            this.DA = projectData.DA;
            this.FDA = projectData.FDA;
            this.HDA = projectData.HDA;
            this.DD = projectData.DD;
            this.PDD = projectData.PDD;


            this.FirstNavigation = projectData.FirstNavigation;
            this.IsNavDone = projectData.IsNavDone;
            this.BallCollection.Clear();
            for (int i = 0; i < projectData.BallCollection.Count; i++)
            {
                BallModel ballModel = projectData.BallCollection[i];
                this.BallCollection.Add(ballModel);
            }

            this.BoneCollection.Clear();
            for (int i = 0; i < projectData.BoneCollection.Count; i++)
            {
                BoneModel boneModel = projectData.BoneCollection[i];
                this.BoneCollection.Add(boneModel);
            }

            this.TargetCollection.Clear();
            for (int i = 0; i < projectData.TargetCollection.Count; i++)
            {
                BoneModel targetModel = projectData.TargetCollection[i];
                this.TargetCollection.Add(targetModel);
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("ID", ID);
            info.AddValue("Institution", Institution);
            info.AddValue("RegFilePath", RegFilePath);
            info.AddValue("IsRegInitialized", IsRegInitialized);
            info.AddValue("IsNavSet", _isNavSet);

            info.AddValue("Stage1Red", Stage1Red);
            info.AddValue("Stage1Green", Stage1Green);
            info.AddValue("Stage1Blue", Stage1Blue);

            info.AddValue("Stage2Red", Stage2Red);
            info.AddValue("Stage2Green", Stage2Green);
            info.AddValue("Stage2Blue", Stage2Blue);

            info.AddValue("DA", DA);
            info.AddValue("FDA", FDA);
            info.AddValue("HDA", HDA);
            info.AddValue("DD", DD);
            info.AddValue("PDD", PDD);

            info.AddValue("FirstNavigation", FirstNavigation);
            info.AddValue("IsNavDone", IsNavDone);

            info.AddValue("BallCollection_Count", BallCollection.Count);
            for (int i = 0; i < BallCollection.Count; i++)
            {
                info.AddValue("BallCollection_" + i, BallCollection[i]);
            }

            info.AddValue("BoneCollection_Count", BoneCollection.Count);
            for (int i = 0; i < BoneCollection.Count; i++) 
            {
                info.AddValue("BoneCollection_"+ i, BoneCollection[i]);
            }

            info.AddValue("TargetCollection_Count", TargetCollection.Count);
            for (int i = 0; i < TargetCollection.Count; i++)
            {
                info.AddValue("TargetCollection_" + i, TargetCollection[i]);
            }
        }


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value);
            }
        }
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                SetValue(ref _id, value);
            }
        }
        public string Institution
        {
            get
            {
                return _institution;
            }
            set
            {
                SetValue(ref _institution, value);
            }
        }
        public string RegFilePath
        {
            get
            {
                return _regFilePath;
            }
            set
            {
                SetValue(ref _regFilePath, value);
            }
        }
        public bool CanSelectPoints
        {
            get
            {
                return _canSelectPoints;
            }
            set
            {
                SetValue(ref _canSelectPoints, value);
                SelectPointState = _canSelectPoints ? "ON" : "OFF";
            }
        }
        public string SelectPointState
        {
            get
            {
                return _selectPointState;
            }
            set
            {
                SetValue(ref _selectPointState, value);
            }
        }
        public string FirstNavigation
        {
            get
            {
                return _firstNavigation;
            }
            set
            {
                SetValue(ref _firstNavigation, value);
            }
        }
        public bool IsNavSet
        {
            get
            {
                return _isNavSet;
            }
            set
            {
                SetValue(ref _isNavSet, value);
            }
        }
        public bool IsRegistered
        {
            get
            {
                return _isRegistered;
            }
            set
            {
                SetValue(ref _isRegistered, value);
            }
        }
        public  ObservableCollection<BallModel> BallCollection
        {
            get
            {
                return _ballCollection;
            }
            set
            {
                SetValue(ref _ballCollection, value);
            }
        }
        public  ObservableCollection<BoneModel> BoneCollection
        {
            get
            {
                return _boneCollection;
            }
            set
            {
                SetValue(ref _boneCollection, value);
            }
        }
        public ObservableCollection<BoneModel> TargetCollection
        {
            get
            {
                return _targetCollection;
            }
            set
            {
                SetValue(ref _targetCollection, value);
            }
        }
    }
}
