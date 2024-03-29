﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static ObservableCollection<ModelSettingItem> _modelSettingCollection;      
        /// <summary>
        /// 病人名字
        /// </summary>
        private static String _patientName = "蔡X君";        
        /// <summary>
        /// 病人ID
        /// </summary>
        private static String _patientId;        
        /// <summary>
        /// 醫院名字
        /// </summary>
        private static String _hospital;        
        /// <summary>
        /// 註冊檔名
        /// </summary>
        private static String _regPath = "./data/reg20170713.txt";
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
                    MarkerId = "Head"
                                                                                ,
                    BoneFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                                                                                ,
                    OspFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    OspDiffuseColor = System.Windows.Media.Color.FromArgb(50, 255, 0, 0)
                                                                                ,
                    BoneDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                });

                ModelSettingCollection.Add(new ModelSettingItem
                {
                 
                    MarkerId = "C"
                                                                                ,
                    BoneFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl"
                    //ModelFilePath = "D:\\Desktop\\c2lpk7avgum8-E-45-Aircraft\\E-45-Aircraft\\E 45 Aircraft_stl.stl"
                                                                               ,
                    OspFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\man_OSP.stl"
                                                                                ,
                    BoneDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
                    OspDiffuseColor = System.Windows.Media.Color.FromArgb(50, 0, 255, 0)

                });


                ModelSettingCollection.Add(new ModelSettingItem
                {

                    MarkerId = "A"
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
                return _modelSettingCollection;
            }
            set
            {
                SetStaticValue(ref _modelSettingCollection, value);
            }
        }
        public static String PatientName
        {
            get
            {
                return _patientName;
            }
            set
            {
                SetStaticValue(ref _patientName, value);
            }
        }
        public static String PatientId
        {
            get
            {
                return _patientId;
            }
            set
            {
                SetStaticValue(ref _patientId, value);                
            }
        }
        public static String Hospital
        {
            get
            {
                return _hospital;
            }
            set
            {
                SetStaticValue(ref _hospital, value);
            }
        }
        public static String RegPath
        {
            get
            {
                return _regPath;
            }
            set
            {
                SetStaticValue(ref _regPath, value);
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
                ModelSettingItem selectedModelItem = (ModelSettingItem)_modelSettingView.ModelListView.SelectedItem;

                //設定所選模型物件為"已移除"，按下OK之後會刪除
                selectedModelItem.Bone.IsRemoved = true;
                selectedModelItem.Osp.IsRemoved = true;

                int temp = _modelSettingView.ModelListView.SelectedIndex;

                ModelSettingCollection.Remove(selectedModelItem);

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

                OspModel ospModel = ModelSettingCollection[i].Osp;
                BoneModel boneModel = ModelSettingCollection[i].Bone;

                //確認有Load過且有沒有被加進去modelDataCollection
                if (ospModel.IsLoaded && !ospModel.IsAdded) 
                {
                    MultiAngleViewModel.OspModelCollection.Add(ModelSettingCollection[i].Osp);
                    ModelSettingCollection[i].Osp.IsAdded = true;
                }
                //確認有Load過且有沒有被加進去modelDataCollection
                if (boneModel.IsLoaded && !boneModel.IsAdded) 
                {
                    MainViewModel.ProjData.BoneCollection.Add(ModelSettingCollection[i].Bone);
                    ModelSettingCollection[i].Bone.IsAdded = true;
                    //除了頭部以外需要guide
                    if (!boneModel.MarkerId.Equals("Head")&& !boneModel.MarkerId.Equals("C"))
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
            for (int i = 0; i < MainViewModel.ProjData.BoneCollection.Count; i++)
            {
                BoneModel boneModel = MainViewModel.ProjData.BoneCollection[i] as BoneModel;
                //模型如果透過 - 移除 或是 因為換錯誤檔名造成IsLoaded 為false則直接移除
                if (boneModel.IsRemoved || !boneModel.IsLoaded)
                {
                    MainViewModel.ProjData.BoneCollection.RemoveAt(i);
                    boneModel.IsAdded = false;
                    i--;
                }
            }


            //刪除modelDataCollection中已經從ModelInfoCollection移除的模型，
            for (int i = 0; i < MultiAngleViewModel.OspModelCollection.Count; i++)
            {
                OspModel ospModel = MultiAngleViewModel.OspModelCollection[i] as OspModel;
                //模型如果透過 - 移除 或是 因為換錯誤檔名造成IsLoaded 為false則直接移除
                if (ospModel.IsRemoved || !ospModel.IsLoaded)
                {
                    MultiAngleViewModel.OspModelCollection.RemoveAt(i);
                    ospModel.IsAdded = false;
                    i--;
                }
            }
            
            Console.WriteLine("OSP 數量:"+ MultiAngleViewModel.OspModelCollection.Count);
            Console.WriteLine("Bone  數量:" + MainViewModel.ProjData.BoneCollection.Count);


            
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
