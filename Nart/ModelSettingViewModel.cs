using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NartControl;
using System.Windows.Media;
using SharpDX.Direct3D11;

namespace Nart
{
    public class ModelSettingViewModel : ObservableObject
    {
        /// <summary>
        /// 每個列中的Model資料集合
        /// </summary>
       
        private static ObservableCollection<ModelInfo> modelInfoCollection;
        public static ObservableCollection<ModelInfo> ModelInfoCollection
        {
            get { return modelInfoCollection; }
            set
            {
                SetStaticValue(ref modelInfoCollection, value);
            }
        }
        /// <summary>
        /// 病人名字
        /// </summary>
        private static String patientName = "蔡慧君";
        public static String PatientName
        {
            get
            {
                return patientName;
            }
            set
            {
                SetStaticValue(ref patientName, value);
            }
        }
        /// <summary>
        /// 病人ID
        /// </summary>
        private static String patierntID;
        public static String PatierntID
        {
            get
            {
                return patierntID;
            }
            set
            {
                SetStaticValue(ref patierntID, value);
            }
        }
        /// <summary>
        /// 醫院名字
        /// </summary>
        private static String hospital;
        public static String Hospital
        {
            get
            {
                return hospital;
            }
            set
            {
                SetStaticValue(ref hospital, value);
            }
        }
        /// <summary>
        /// 註冊檔名
        /// </summary>
        private static String regPath = "../../../data/reg20170713.txt";
        public static String RegPath
        {
            get
            {
                return regPath;
            }
            set
            {
                SetStaticValue(ref regPath, value);
            }
        }
        /// <summary>
        /// ModelSettingView的物件
        /// </summary>
        public ModelSettingView ModelSetView
        {
            get;
            set;
        }
        /// <summary>
        /// 建構子，如果ModelInfoCollection是空的，則自製部分預設項目
        /// </summary>
        public ModelSettingViewModel(ModelSettingView _modelSettingView)
        {
            ModelSetView = _modelSettingView;



            ModelInfoCollection = MainViewModel.ModelInfoCollection;
            //如果ModelInfoCollection為空的
            if (ModelInfoCollection == null || ModelInfoCollection.Count == 0)
            {
                MainViewModel.ModelInfoCollection = new ObservableCollection<ModelInfo>();

                ModelInfoCollection = MainViewModel.ModelInfoCollection;


                ModelInfoCollection.Add(new ModelInfo
                {
                    CMode = CullMode.Back
                                                                                ,
                    IvtNormal = false
                                                                                ,
                    FrontCounterClockwise = true
                                                                                ,
                    MarkerID = "C"
                                                                                ,
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl"
                    //ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                                                                                ,
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP2.stl"
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)

                });

                //ModelInfoCollection.Add(new ModelInfo
                //{
                //    CMode = CullMode.Back
                //                                                                ,
                //    IvtNormal = true
                //                                                                ,
                //    FrontCounterClockwise = false
                //                                                                ,
                //    MarkerID = "A"
                //                                                                ,
                //    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                //                                                                // ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                //                                                                ,
                //    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                //                                                                ,
                //    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                //                                                                ,
                //    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(30, 40, 181, 187)
                //});

                ModelInfoCollection.Add(new ModelInfo
                {
                    CMode = CullMode.Back
                                                                                ,
                    IvtNormal = false
                                                                                ,
                    FrontCounterClockwise = true
                                                                                ,
                    MarkerID = "A"
                                                                                ,
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                    // ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                                                                                ,
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });


                //ModelInfoCollection.Add(new ModelInfo
                //{
                //    CMode = CullMode.Back
                //                                                                   ,
                //    IvtNormal = false
                //                                                                ,
                //    FrontCounterClockwise = false
                //                                                                ,
                //    MarkerID = "A"
                //                                                                ,
                //    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                //                                                                ,
                //    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                //                                                                ,
                //    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                //                                                                ,
                //    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                //});






                //ModelInfoCollection.Add(new ModelInfo
                //{
                //    CMode = CullMode.Back
                //                                                                                      ,
                //    MarkerID = "Head"
                //                                                                ,
                //    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                //                                                                ,
                //    OSPFilePath = ""
                //                                                                ,
                //    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                //});
                
            }
        }
        /// <summary>
        /// 新增ModelInfo進ModelInfoCollection
        /// </summary>
        public void AddItem()
        {
            ModelInfoCollection.Add(new ModelInfo());
            ModelSetView.ModelListView.SelectedIndex = ModelInfoCollection.Count - 1;
        }
        /// <summary>
        ///移除[ModelInfoCollection中的物件
        /// </summary>
        public void RemoveItem()
        {
            if (ModelSetView.ModelListView.SelectedItem != null)
            {
                ModelInfoCollection.Remove((ModelInfo)ModelSetView.ModelListView.SelectedItem);
                ModelSetView.ModelListView.SelectedIndex = ModelInfoCollection.Count - 1;
            }
        }

        public void LoadSettingModel()
        {
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < ModelInfoCollection.Count; i++)
            {
                //檢查模型有無load，有換過檔名就變成沒有Load
                if (!ModelInfoCollection[i].IsLoaded)
                {
                    //不知道為什麼整個ModelInfo一定要重建，理論上應該只要LoatSTL有走過就好
                    ModelInfoCollection[i] = new ModelInfo(MainViewModel.ModelInfoCollection[i]);                  
                    ModelInfoCollection[i].LoadSTL();
                }

            }
            
        }
    }
}
