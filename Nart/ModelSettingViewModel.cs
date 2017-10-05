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
       
        private static ObservableCollection<ModelSettingItem> modelInfoCollection;
        public static ObservableCollection<ModelSettingItem> ModelInfoCollection
        {
            get { return modelInfoCollection; }
            set
            {
                SetStaticValue(ref modelInfoCollection, value);
            }
        }
        public static ObservableCollection<ModelData> modelDataCollection;

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
        public ModelSettingView ModelSetView;
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
                MainViewModel.ModelInfoCollection = new ObservableCollection<ModelSettingItem>();

                ModelInfoCollection = MainViewModel.ModelInfoCollection;





                ModelInfoCollection.Add(new ModelSettingItem
                {
                    //CMode = CullMode.Back
                    //                                                            ,
                    //IvtNormal = false
                    //                                                            ,
                    //FrontCounterClockwise = true
                    //                                                            ,
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

                ModelInfoCollection.Add(new ModelSettingItem
                {
                    //CMode = CullMode.Back
                    //                                                            ,
                    //IvtNormal = false
                    //                                                            ,
                    //FrontCounterClockwise = true
                    //                                                            ,
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

                ModelInfoCollection.Add(new ModelSettingItem
                {
                    //CMode = CullMode.Back
                    //                                                            ,
                    //IvtNormal = false
                    //                                                            ,
                    //FrontCounterClockwise = true
                    //                                                            ,
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

            int temp = ModelInfoCollection.Count;
            ModelInfoCollection.Add(new ModelSettingItem());
            ModelSetView.ModelListView.SelectedIndex = temp;
            ListViewItem item = ModelSetView.ModelListView.ItemContainerGenerator.ContainerFromIndex(temp-1) as ListViewItem;
            if (item!=null)
            {
                item.Focus();
            }
        }
        /// <summary>
        ///移除[ModelInfoCollection中的物件
        /// </summary>
        public void RemoveItem()
        {
            if (ModelSetView.ModelListView.SelectedItem != null)
            {
                //選擇的ModelItem
                ModelSettingItem SelectedModelItem=(ModelSettingItem)ModelSetView.ModelListView.SelectedItem;

                //設定所選模型物件為"已移除"，按下OK之後會刪除
                SelectedModelItem.Model.IsRemoved = true;          
                SelectedModelItem.OSP.IsRemoved = true;
   
                int temp = ModelSetView.ModelListView.SelectedIndex;
              
                ModelInfoCollection.Remove(SelectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ModelInfoCollection.Count == temp)
                {
                    ModelSetView.ModelListView.SelectedIndex = ModelInfoCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    ModelSetView.ModelListView.SelectedIndex = temp;
                }
                
                ListViewItem item = ModelSetView.ModelListView.ItemContainerGenerator.ContainerFromIndex(ModelSetView.ModelListView.SelectedIndex) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }


            }
        }
        public void LoadSettingModel()
        {
            if (modelDataCollection == null)
                modelDataCollection = new ObservableCollection<ModelData>();

            Console.WriteLine("ModelInfoCollection count:" + ModelInfoCollection.Count);
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < ModelInfoCollection.Count; i++) 
            {               
                 //Load模型檔，內部有防呆防止重複Load
                ModelInfoCollection[i].Load();

                //確認有Load過且有沒有被加進去modelDataCollection
                if (ModelInfoCollection[i].IsOSPLoaded)
                {
                    modelDataCollection.Insert(0, ModelInfoCollection[i].OSP);
                }
                //確認有Load過且有沒有被加進去modelDataCollection
                if (ModelInfoCollection[i].IsModelLoaded)
                {
                    modelDataCollection.Add(ModelInfoCollection[i].Model);
                }
                
            }
            //刪除modelDataCollection中已經從ModelInfoCollection移除的模型，
            for (int i = 0; i < modelDataCollection.Count; i++)
            {              
                if (modelDataCollection[i].IsRemoved)
                {
                    modelDataCollection.RemoveAt(i);
                    i --;
                }           
            }
            
        }        
    }
}
