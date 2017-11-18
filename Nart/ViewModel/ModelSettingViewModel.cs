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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Nart.Model_Object;
using HelixToolkit.Wpf.SharpDX;
using System.Windows.Data;
using SharpDX;

namespace Nart
{
    using Matrix3D = System.Windows.Media.Media3D.Matrix3D;

    public class ModelSettingViewModel : ObservableObject
    {
        
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
            
            //如果ModelInfoCollection為空的
            if (ModelSettingCollection == null || ModelSettingCollection.Count == 0)
            {
                ModelSettingViewModel.ModelSettingCollection = new ObservableCollection<ModelSettingItem>();

                ModelSettingCollection = ModelSettingViewModel.ModelSettingCollection;


                ModelSettingCollection.Add(new ModelSettingItem
                {
                    MarkerID = "Head"
                                                                                ,
                    BoneFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                                                                                ,
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(50, 255, 0, 0)
                                                                                ,
                    BoneDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                });

                ModelSettingCollection.Add(new ModelSettingItem
                {
                 
                    MarkerID = "C"
                                                                                ,
                    BoneFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl"
                    //ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                                                                               ,
                    OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl"
                                                                                ,
                    BoneDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
                    OSPDiffuseColor = System.Windows.Media.Color.FromArgb(50, 0, 255, 0)

                });


                ModelSettingCollection.Add(new ModelSettingItem
                {

                    MarkerID = "A"
                                                                                ,
                    BoneFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                    // ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                    //                                                           ,
                    //OSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    BoneDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });


            }
        }


        public static ObservableCollection<ModelSettingItem> ModelSettingCollection
        {
            get
            {
                return modelSettingCollection;
            }
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
                SelectedModelItem.Bone.IsRemoved = true;
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
          
            //確保所有模型資訊都有set進去ModelInfo的資料
            for (int i = 0; i < ModelSettingCollection.Count; i++)
            {
                //Load模型檔，內部有防呆防止重複Load
                ModelSettingCollection[i].Load();

                OSPModel ospModel = ModelSettingCollection[i].OSP;
                BoneModel boneModel = ModelSettingCollection[i].Bone;

                //確認有Load過且有沒有被加進去modelDataCollection
                if (ospModel.IsLoaded && !ospModel.IsAdded) 
                {
                    MultiAngleViewModel.OSPModelCollection.Add(ModelSettingCollection[i].OSP);
                    ModelSettingCollection[i].OSP.IsAdded = true;
                }
                //確認有Load過且有沒有被加進去modelDataCollection
                if (boneModel.IsLoaded && !boneModel.IsAdded) 
                {
                    MultiAngleViewModel.BoneModelCollection.Add(ModelSettingCollection[i].Bone);
                    ModelSettingCollection[i].Bone.IsAdded = true;
                    //除了頭部以外需要guide
                    if (!boneModel.MarkerID.Equals("Head")&& !boneModel.MarkerID.Equals("C"))
                    {
                        ModelSettingCollection[i].Guide = new DraggableTriangle(boneModel.ModelCenter);

                        MultiAngleViewModel.TriangleModelCollection.Add(ModelSettingCollection[i].Guide);

                    }
                }
                //做bone 跟 osp transform的binding
                if (boneModel.IsLoaded && ospModel.IsAdded)
                {
                    var binding = new Binding("Transform");
                    binding.Source = boneModel;
                    binding.Mode = BindingMode.OneWay;
                    BindingOperations.SetBinding(ospModel, Model3D.TransformProperty, binding);
                }
                //做bone 的轉移矩陣綁到 DraggableTriangle的ModelTransform上面
                if (boneModel.IsLoaded && ModelSettingCollection[i].Guide != null)
                {
                    var binding = new Binding("Transform");
                    binding.Source = boneModel;
                    binding.Mode = BindingMode.OneWay;
                    BindingOperations.SetBinding(ModelSettingCollection[i].Guide, HelixToolkit.Wpf.SharpDX.GroupModel3D.TransformProperty, binding);
                }

            }
            //刪除modelDataCollection中已經從ModelInfoCollection移除的模型，
            for (int i = 0; i < MultiAngleViewModel.BoneModelCollection.Count; i++)
            {
                BoneModel boneModel = MultiAngleViewModel.BoneModelCollection[i] as BoneModel;
                //模型如果透過 - 移除 或是 因為換錯誤檔名造成IsLoaded 為false則直接移除
                if (boneModel.IsRemoved || !boneModel.IsLoaded)
                {
                    MultiAngleViewModel.BoneModelCollection.RemoveAt(i);
                    boneModel.IsAdded = false;
                    i--;
                }
            }


            //刪除modelDataCollection中已經從ModelInfoCollection移除的模型，
            for (int i = 0; i < MultiAngleViewModel.OSPModelCollection.Count; i++)
            {
                OSPModel ospModel = MultiAngleViewModel.OSPModelCollection[i] as OSPModel;
                //模型如果透過 - 移除 或是 因為換錯誤檔名造成IsLoaded 為false則直接移除
                if (ospModel.IsRemoved || !ospModel.IsLoaded)
                {
                    MultiAngleViewModel.OSPModelCollection.RemoveAt(i);
                    ospModel.IsAdded = false;
                    i--;
                }
            }
            
            Console.WriteLine("OSP 數量:"+ MultiAngleViewModel.OSPModelCollection.Count);
            Console.WriteLine("Bone  數量:" + MultiAngleViewModel.BoneModelCollection.Count);


            
            MultiAngleViewModel.ResetCameraPosition();

            _modelSettingView.Hide();
        }



        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string info = "")
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(null, new PropertyChangedEventArgs(info));
            }
        }
        protected static bool SetStaticValue<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = "")//CallerMemberName主要是.net4.5後定義好的caller訊息，能將訊息傳給後者的變數，目的在使用時不用特地傳入"Property"名稱
        {
            if (object.Equals(oldValue, newValue))
            {
                return false;
            }
            oldValue = newValue;
            OnStaticPropertyChanged(propertyName);
            return true;
        }

    }
}
