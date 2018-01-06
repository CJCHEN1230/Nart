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
            Id = (string)info.GetValue("Id", typeof(string));
            Institution = (string)info.GetValue("Institution", typeof(string));
            RegFilePath = (string)info.GetValue("RegFilePath", typeof(string));
            IsRegInitialized = (bool)info.GetValue("IsRegInitialized", typeof(bool));
            IsNavigationSet = (bool)info.GetValue("IsNavigationSet", typeof(bool));
            IsFirstStage = (bool)info.GetValue("IsFirstStage", typeof(bool));
            IsSecondStage = (bool)info.GetValue("IsSecondStage", typeof(bool));
            IsFinished = (bool)info.GetValue("IsFinished", typeof(bool));
            FirstNavigation = (string)info.GetValue("FirstNavigation", typeof(string));

        }


        private void SerializeBinary()
        {
            //Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.FileName = "Dsgn_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".dsgn";
            //dlg.DefaultExt = ".dsgn";
            //dlg.Filter = "GB Design Project Files (.dsgn)|*.dsgn";
            //Nullable<bool> result = dlg.ShowDialog();

            string FileName= "D:\\Desktop\\test.nart";
            // 建立檔案資料流物件
            using (FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                // 建立 BinaryFormatter 物件
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                // 將物件進行二進位序列化，並且儲存檔案
                binaryFormatter.Serialize(fileStream, this);
            }
            // 將序列化後的檔案內容呈現到表單畫面
            StringBuilder sbContent = new StringBuilder();

            foreach (var byteData in File.ReadAllBytes(FileName))
            {
                sbContent.Append(byteData);
                sbContent.Append(" ");
            }


            Console.WriteLine(File.ReadAllText(FileName));
        }
        /// <summary>
        /// 使用 BinaryFormatter 進行還原序列化
        /// </summary>
        /// <returns></returns>
        private ProjectData DeserializeBinary()
        {
            // 建立 ProjectData 類別物件
            ProjectData clsSerializable = null;

            string FileName = "D:\\Desktop\\test.nart";
            // 建立檔案資料流物件
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
            {
                // 建立 BinaryFormatter 物件
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                // 將檔案內容還原序列化成 Object 物件，並且進一步轉型成正確的型別
 
                clsSerializable = (ProjectData)binaryFormatter.Deserialize(fileStream);
            }
            return clsSerializable;
        }
      
        /// <summary>
        /// 使用 SoapFormatter 進行序列化
        /// </summary>
        private void SerializeSoap()
        {
            // 建立 ProjectData 類別物件
            ProjectData clsSerializable = new ProjectData();
            string FileName = "D:\\Desktop\\test.nart";
            // 建立檔案資料流物件
            using (FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                // 建立 SoapFormatter 物件
                SoapFormatter soapFormatter = new SoapFormatter();

                // 將物件進行 SOAP 序列化，並且儲存檔案
                soapFormatter.Serialize(fileStream, clsSerializable);
            }

            // 將序列化後的檔案內容呈現到表單畫面
            Console.WriteLine(File.ReadAllText(FileName));
        }

        /// <summary>
        /// 使用 SoapFormatter 進行還原序列化
        /// </summary>
        /// <returns></returns>
        private ProjectData DeserializeSoap()
        {
            // 建立 ProjectData 類別物件
            ProjectData clsSerializable = null;
            string FileName = "D:\\Desktop\\test.nart";
            // 建立檔案資料流物件
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
            {
                // 建立 SoapFormatter 物件
                SoapFormatter soapFormatter = new SoapFormatter();

                // 將檔案內容還原序列化成 Object 物件，並且進一步轉型成正確的型別
                  clsSerializable = (ProjectData)soapFormatter.Deserialize(fileStream);
                
            }
            return clsSerializable;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //private string _name = "蔡慧君";
            //private string _id = "123456";
            //private string _institution = "成大";
            //private string _regFilePath = "../../../data/蔡慧君測試用.txt";

            //public bool IsRegInitialized = false;
            //public bool IsNavigationSet = false;
            //public bool IsFirstStage = false;
            //public bool IsSecondStage = false;
            //public bool IsFinished = false;
            //public string FirstNavigation = "Maxilla";


            info.AddValue("Name", Name);
            info.AddValue("Id", Id);
            info.AddValue("Institution", Institution);
            info.AddValue("RegFilePath", RegFilePath);
            info.AddValue("IsRegInitialized", IsRegInitialized);
            info.AddValue("IsNavigationSet", IsNavigationSet);
            info.AddValue("IsFirstStage", IsFirstStage);
            info.AddValue("IsSecondStage", IsSecondStage);
            info.AddValue("IsFinished", IsFinished);
            info.AddValue("FirstNavigation", FirstNavigation);



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
