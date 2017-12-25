using HelixToolkit.Wpf.SharpDX;
using Nart.Model_Object;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public class Projectata : ObservableObject
    {
        //病人資訊
        private string _name = "蔡慧君";
        private string _id = "123456";
        private string _institution = "成大";
        private string _regFilePath = "../../../data/蔡慧君測試用.txt";

        public bool IsRegInitialized = false;
        public bool IsNavigationSetted = false;
        public bool IsFirstStage = false;       
        public bool IsSecondStage = false;
        public string FirstNavigation = "Maxilla";
       

        private bool _canSelectPoints = false;
        private string _selectPointState = "OFF";
        private  ObservableCollection<BallModel> _ballCollection=  new ObservableCollection<BallModel>();
        private  ObservableCollection<BoneModel> _boneCollection =  new ObservableCollection<BoneModel>();

        public Projectata()
        {            
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
        public string Id
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
