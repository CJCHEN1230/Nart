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
using System.Windows.Input;

namespace Nart
{
    public class ModelSettingViewModel : ObservableObject
    {
        /// <summary>
        /// 轉換成模型清單的資料集合
        /// </summary>       
        public static ObservableCollection<ModelData> ModelDataCollection;
        /// <summary>
        /// 項目中每個Item的資料集合
        /// </summary>       
        private static ObservableCollection<ModelSettingItem> modelSettingCollection;        
        /// <summary>
        /// 病人名字
        /// </summary>
        private static String patientName = "蔡X君";        
        /// <summary>
        /// 病人ID
        /// </summary>
        private static String patientID;        
        /// <summary>
        /// 醫院名字
        /// </summary>
        private static String hospital;        
        /// <summary>
        /// 註冊檔名
        /// </summary>
        private static String regPath = "../../../data/reg20170713.txt";               
        /// <summary>
        /// ModelSettingView的物件
        /// </summary>
        private ModelSettingView _modelSettingView;
        
        
        /// <summary>
        /// 建構子，如果ModelInfoCollection是空的，則自製部分預設項目
        /// </summary>
        public ModelSettingViewModel(ModelSettingView modelSettingView)
        {
            this._modelSettingView = modelSettingView;

            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
            ModelSettingCommand = new RelayCommand(LoadSettingModel);

            ModelSettingCollection = MainViewModel.ModelSettingCollection;
            //如果ModelInfoCollection為空的
            if (ModelSettingCollection == null || ModelSettingCollection.Count == 0)
            {
                MainViewModel.ModelSettingCollection = new ObservableCollection<ModelSettingItem>();

                ModelSettingCollection = MainViewModel.ModelSettingCollection;


                ModelSettingCollection.Add(new ModelSettingItem
                {              
                    MarkerID = "Head"
                                                                                ,
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                                                                               
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                });

                ModelSettingCollection.Add(new ModelSettingItem
                {
                 
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


                ModelSettingCollection.Add(new ModelSettingItem
                {

                    MarkerID = "A"
                                                                                ,
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                    // ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                                                                               ,
                    //OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                    //                                                            ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });


            }
        }


        public static ObservableCollection<ModelSettingItem> ModelSettingCollection
        {
            get { return modelSettingCollection; }
            set
            {
                SetStaticValue(ref modelSettingCollection, value);
            }
        }
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
        /// 設定頁面按下+後新增模型的Command
        /// </summary>
        public ICommand AddItemCommand { private set; get; }
        /// <summary>
        /// 設定頁面按下-後移除模型的Command
        /// </summary>
        public ICommand RemoveItemCommand { private set; get; }
        /// <summary>
        /// 設定頁面按下ok後執行的Command
        /// </summary>
        public ICommand ModelSettingCommand { private set; get; }
        /// <summary>
        /// 新增ModelInfo進ModelInfoCollection
        /// </summary>
        public void AddItem(object o)
        {

            int temp = ModelSettingCollection.Count;
            ModelSettingCollection.Add(new ModelSettingItem());
            _modelSettingView.ModelListView.SelectedIndex = temp;
            ListViewItem item = _modelSettingView.ModelListView.ItemContainerGenerator.ContainerFromIndex(temp - 1) as ListViewItem;
            //設置成點選狀態
            if (item != null)
            {
                item.Focus();
            }
        }
        /// <summary>
        ///移除[ModelInfoCollection中的物件
        /// </summary>
        public void RemoveItem(object o)
        {
            if (_modelSettingView.ModelListView.SelectedItem != null)
            {
                //選擇的ModelItem
                ModelSettingItem SelectedModelItem = (ModelSettingItem)_modelSettingView.ModelListView.SelectedItem;

                //設定所選模型物件為"已移除"，按下OK之後會刪除
                SelectedModelItem.Model.IsRemoved = true;
                SelectedModelItem.OSP.IsRemoved = true;

                int temp = _modelSettingView.ModelListView.SelectedIndex;

                ModelSettingCollection.Remove(SelectedModelItem);

                //刪減之後數量若跟舊的索引值一樣，代表選項在最後一個
                if (ModelSettingCollection.Count == temp)
                {
                    _modelSettingView.ModelListView.SelectedIndex = ModelSettingCollection.Count - 1;
                }
                else//不是的話則維持原索引值
                {
                    _modelSettingView.ModelListView.SelectedIndex = temp;
                }

                ListViewItem item = _modelSettingView.ModelListView.ItemContainerGenerator.ContainerFromIndex(_modelSettingView.ModelListView.SelectedIndex) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                }


            }
        }
        /// <summary>
        ///將設置清單的每個Item Load進檔案，並轉成模型清單
        /// </summary>
        public void LoadSettingModel(object o)
        {
            if (ModelDataCollection == null)
                ModelDataCollection = new ObservableCollection<ModelData>();

            
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < ModelSettingCollection.Count; i++)
            {
                //Load模型檔，內部有防呆防止重複Load
                ModelSettingCollection[i].Load();

                //確認有Load過且有沒有被加進去modelDataCollection
                if (ModelSettingCollection[i].OSP.IsLoaded && !ModelSettingCollection[i].OSP.IsAdded) 
                {
                    ModelDataCollection.Add(ModelSettingCollection[i].OSP);
                    ModelSettingCollection[i].OSP.IsAdded = true;
                }
                //確認有Load過且有沒有被加進去modelDataCollection
                if (ModelSettingCollection[i].Model.IsLoaded && !ModelSettingCollection[i].Model.IsAdded) 
                {
                    ModelDataCollection.Insert(0, ModelSettingCollection[i].Model);
                    ModelSettingCollection[i].Model.IsAdded = true;
                }

            }
            //刪除modelDataCollection中已經從ModelInfoCollection移除的模型，
            for (int i = 0; i < ModelDataCollection.Count; i++)
            {
                //模型如果透過 - 移除 或是 因為換錯誤檔名造成IsLoaded 為false則直接移除
                if (ModelDataCollection[i].IsRemoved || !ModelDataCollection[i].IsLoaded)
                {
                    ModelDataCollection.RemoveAt(i);
                    i--;
                }
            }
            Console.WriteLine(" 模型總數:  " + ModelDataCollection.Count);
            _modelSettingView.Hide();
        }
    }
}
