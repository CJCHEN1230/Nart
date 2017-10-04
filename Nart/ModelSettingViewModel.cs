using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NartControl;
using System.Windows.Media;
using SharpDX.Direct3D11;
using System.Windows.Controls;

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
        private static ObservableCollection<ModelData> modelDataCollection;
        public static ObservableCollection<ModelData> ModelDataCollection
        {
            get { return modelDataCollection; }
            set
            {
                SetStaticValue(ref modelDataCollection, value);
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
        private static String patientID;
        public static String PatientID
        {
            get
            {
                return patientID;
            }
            set
            {
                SetStaticValue(ref patientID, value);
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
                    MarkerID = "Head"
                                                                                ,
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                                                                                ,
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                });

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
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl"
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
                                                                                
                    //OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });


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
                //選擇的ModelInfo
                ModelInfo SelectedModelInfo=(ModelInfo)ModelSetView.ModelListView.SelectedItem;
                //確認ModelInfo中的Model有沒有被load過(有load過才有實體化)，有的話標記removed為true
                if (SelectedModelInfo.Model!=null)
                {
                    SelectedModelInfo.Model.IsRemoved = true;
                }
                //確認ModelInfo中的OSP有沒有被load過(有load過才有實體化)，有的話標記removed為true
                if (SelectedModelInfo.OSP != null)
                {
                    SelectedModelInfo.OSP.IsRemoved = true;
                }
                ModelInfoCollection.Remove(SelectedModelInfo);
                ModelSetView.ModelListView.SelectedIndex = ModelInfoCollection.Count - 1;

                ListViewItem item = ModelSetView.ModelListView.ItemContainerGenerator.ContainerFromIndex(ModelSetView.ModelListView.SelectedIndex) as ListViewItem;
                item.Focus();

                
            }
        }
        public void LoadSettingModel()
        {
            if (ModelDataCollection == null)
                ModelDataCollection = new ObservableCollection<ModelData>();

            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < ModelInfoCollection.Count; i++) 
            {               
                 //不知道為什麼整個ModelInfo一定要重建，理論上應該只要LoatSTL有走過就好
                //ModelInfoCollection[i] = new ModelInfo(MainViewModel.ModelInfoCollection[i]);                  
                ModelInfoCollection[i].Load();

                //確認有Load過且有沒有被加進去
                if (ModelInfoCollection[i].IsOSPLoaded && !ModelInfoCollection[i].OSP.IsAdded)
                {
                    ModelDataCollection.Insert(0, ModelInfoCollection[i].OSP);
                    ModelInfoCollection[i].OSP.IsAdded = true;
                }
                //確認有Load過且有沒有被加進去
                if (ModelInfoCollection[i].IsModelLoaded && !ModelInfoCollection[i].Model.IsAdded)
                {
                    ModelDataCollection.Add(ModelInfoCollection[i].Model);
                    ModelInfoCollection[i].Model.IsAdded = true;
                }
                
            }

            for (int i = 0; i < ModelDataCollection.Count; i++)
            {
              
                if (ModelDataCollection[i].IsRemoved)
                {
                    ModelDataCollection.RemoveAt(i);
                }
           
            }


            Console.WriteLine("Model data count: " + ModelDataCollection.Count);
        }        
    }
}
