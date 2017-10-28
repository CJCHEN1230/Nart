﻿using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using NartControl;
using System.Collections.ObjectModel;
using NartControl.Control;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace Nart
{
    /// <summary>
    /// 此類別用來記錄各種MainView的屬性
    /// </summary>
    public class MainViewModel : ObservableObject
    {

        public CameraControl CamCtrl;
        public NartServer Server= new NartServer();
        private static string _pointNumber;
        private static int _tabIndex = 1; //預設tab頁面索引值        
        private ModelSettingView _modelSettingdlg;
        private MainView _mainWindow;
        
        public MainViewModel(MainView mainWindow)
        {
            _mainWindow = mainWindow;
            SetModelCommand = new RelayCommand(SetModel);
        }
                
        public static int TabIndex
        {
            get { return _tabIndex; }
            set
            {
                SetStaticValue(ref _tabIndex, value);
            }
        }
        public static string PointNumber
        {
            get { return _pointNumber; }
            set
            {
                SetStaticValue(ref _pointNumber, value);
            }
        }
        public ICommand SetModelCommand { private set; get; }


        public void InitCamCtrl()
        {

            CamCtrl = new CameraControl(new TIS.Imaging.ICImagingControl[2] { _mainWindow.CamHost1.icImagingControl, _mainWindow.CamHost2.icImagingControl });
            CamCtrl.CameraStart();
        }
        /// <summary>
        /// 顯示設置好的各項模型資訊，按下Set Model 之後並且按ok後會走到這
        /// </summary>       
        private void SetModel(object o)
        {
            if (_modelSettingdlg == null)
            {
                _modelSettingdlg = new ModelSettingView();
            }
            _modelSettingdlg.Owner = _mainWindow;

            _modelSettingdlg.ShowDialog();
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