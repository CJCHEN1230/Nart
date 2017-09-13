using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NartControl;
using System.Windows.Media;

namespace Nart
{
    class ModelSettingViewModel : ObservableObject
    {
        /// <summary>
        /// 每個列中的Model資料集合
        /// </summary>
        private ObservableCollection<ModelInfo> modelInfoCollection;
        public ObservableCollection<ModelInfo> ModelInfoCollection
        {
            get
            {
                return modelInfoCollection;
            }
            set
            {
                SetValue(ref modelInfoCollection, value);

                MainViewModel.allModelInfo = modelInfoCollection;
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
            if (MainViewModel.allModelInfo == null)
            {
                ObservableCollection<ModelInfo> allModelInfo = new ObservableCollection<ModelInfo>();

                allModelInfo.Add(new ModelInfo
                {
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\maxilla_0.4.stl"
                                                                                ,
                    BSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
                    BSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });
                allModelInfo.Add(new ModelInfo
                {
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\mandible_digital_segment_BVRO_0.4.stl"
                                                                                ,
                    BSPFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\max_OSP.stl"
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                                                                                ,
                    BSPDiffuseColor = System.Windows.Media.Color.FromArgb(100, 40, 181, 187)
                });
                allModelInfo.Add(new ModelInfo
                {
                    ModelFilePath = "D:\\Desktop\\研究資料\\蔡慧君_15755388_20151231\\註冊\\skull_wo_maxilla_w_ramus_BVRO_4.stl"
                                                                                ,
                    BSPFilePath = ""
                                                                                ,
                    ModelDiffuseColor = System.Windows.Media.Color.FromArgb(255, 40, 181, 187)
                });

                ModelInfoCollection = allModelInfo;
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

    }
}
