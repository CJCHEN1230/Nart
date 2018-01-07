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
        //病人資訊
        private string _name = "蔡慧君";
        private string _id = "123456";
        private string _institution = "成大";
        private string _regFilePath = "../../../data/蔡慧君測試用.txt";

        public bool IsRegInitialized = false;
        public bool IsNavigationSet = false;
        public bool IsFirstStage = false;       
        public bool IsSecondStage = false;
        public bool IsFinished = false;
        public string FirstNavigation = "Maxilla";

        private bool _canSelectPoints = false;
        private string _selectPointState = "OFF";
        private  ObservableCollection<BallModel> _ballCollection=  new ObservableCollection<BallModel>();
        private  ObservableCollection<BoneModel> _boneCollection =  new ObservableCollection<BoneModel>();

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
            IsNavigationSet = (bool)info.GetValue("IsNavigationSet", typeof(bool));
            IsFirstStage = (bool)info.GetValue("IsFirstStage", typeof(bool));
            IsSecondStage = (bool)info.GetValue("IsSecondStage", typeof(bool));
            IsFinished = (bool)info.GetValue("IsFinished", typeof(bool));
            FirstNavigation = (string)info.GetValue("FirstNavigation", typeof(string));


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
        }

        public void UpdateData(ProjectData projectData)
        {

            this.Name = projectData.Name;
            this.ID = projectData.ID;
            this.Institution = projectData.Institution;
            this.RegFilePath = projectData.RegFilePath;
            this.IsRegInitialized = projectData.IsRegInitialized;
            this.IsNavigationSet = projectData.IsNavigationSet; 
            this.IsFirstStage = projectData.IsFirstStage;
            this.IsSecondStage = projectData.IsSecondStage;
            this.IsFinished = projectData.IsFinished;
            this.FirstNavigation = projectData.FirstNavigation;

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
            
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("ID", ID);
            info.AddValue("Institution", Institution);
            info.AddValue("RegFilePath", RegFilePath);
            info.AddValue("IsRegInitialized", IsRegInitialized);
            info.AddValue("IsNavigationSet", IsNavigationSet);
            info.AddValue("IsFirstStage", IsFirstStage);
            info.AddValue("IsSecondStage", IsSecondStage);
            info.AddValue("IsFinished", IsFinished);
            info.AddValue("FirstNavigation", FirstNavigation);

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
    }
}
