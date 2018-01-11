using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;


namespace Nart
{
    using Model_Object;
    using SharpDX.Direct3D11;
    using System.Windows;
    using System.Windows.Data;
    using Color = System.Windows.Media.Color;
    /// <summary>
    /// 模型設置清單中Listview中的項目
    /// </summary>
    public class ModelSettingItem : ObservableObject
    {
        /// <summary>
        /// 設置列當中的OSP模型
        /// </summary>
        public OspModel Osp = new OspModel();
        /// <summary>
        /// 設置列當中的Bone模型
        /// </summary>
        public BoneModel Bone = new BoneModel();
        /// <summary>
        /// 導引用的guide
        /// </summary>
        public DraggableTriangle Guide = new DraggableTriangle();
        /// <summary>
        /// 模型名稱
        /// </summary>
        private string _boneFilePath;
        /// <summary>
        /// OSP名稱
        /// </summary>
        private string _ospFilePath;
        /// <summary>
        /// Model 顏色
        /// </summary>
        private Color _boneDiffuseColor;
        /// <summary>
        /// OSP 顏色
        /// </summary>
        private Color _ospDiffuseColor;
        /// <summary>
        /// MarkerID 的值
        /// </summary>
        private string _markerId;
        /// <summary>
        /// combobox選項的內容
        /// </summary>
        private List<string> _comboboxList = MarkerDatabase.MarkerIdList;
        /// <summary>
        /// 設定好模型之後Load進去模型資料所用
        /// </summary>
        public void Load()
        {
            
            Bone.FilePath = BoneFilePath;
            Bone.MarkerId = MarkerId;
            Bone.BoneDiffuseColor = BoneDiffuseColor;

            Osp.FilePath = OspFilePath;
            Osp.MarkerId = MarkerId;
            Osp.DiffuseColor = OspDiffuseColor;
            if (!Osp.IsLoaded)
            {
                Osp.LoadOsp();
            }

            if (!Bone.IsLoaded)
            {
                Bone.LoadModel();
            }

        }
        public string BoneFilePath
        {
            get
            {
                return _boneFilePath;
            }
            set
            {
                SetValue(ref _boneFilePath, value);
                Bone.IsLoaded = false;                
            }
        }
        public string OspFilePath
        {
            get
            {
                return _ospFilePath;
            }
            set
            {
                SetValue(ref _ospFilePath, value);
                Osp.IsLoaded = false;
            }
        }
        public Color BoneDiffuseColor
        {
            get
            {
                return _boneDiffuseColor;
            }
            set
            {
                SetValue(ref _boneDiffuseColor, value);
                if (Bone.IsLoaded != true)
                    return;
                Bone.BoneDiffuseColor = _boneDiffuseColor;
                Bone.SetBoneMaterial();
            }
        }
        public Color OspDiffuseColor
        {
            get
            {
                return _ospDiffuseColor;
            }
            set
            {
                //如果透明度太低則降低到50
                if (value.A>150)
                {
                    value.A = 50;
                }
                SetValue(ref _ospDiffuseColor, value);
                if (Osp.IsLoaded != true)
                    return;
                Osp.DiffuseColor = _ospDiffuseColor;
                Osp.SetOspMaterial();
            }
        }
        public string MarkerId
        {
            get
            {
                return _markerId;
            }
            set
            {
                SetValue(ref _markerId, value);
            }
        }
        public List<string> ComboBoxList
        {
            get
            {
                return _comboboxList;
            }
            set
            {
                SetValue(ref _comboboxList, value);
            }
        }
    }
}
