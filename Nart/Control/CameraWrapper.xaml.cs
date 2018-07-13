using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIS.Imaging;

namespace Nart.Control
{
    /// <summary>
    /// CameraWrapper.xaml 的互動邏輯
    /// </summary>
    public partial class CameraWrapper : UserControl
    {

        public TIS.Imaging.ICImagingControl IcImagingControl = new TIS.Imaging.ICImagingControl();
        public bool IsActivated = false;        
        public CameraWrapper()
        {
            InitializeComponent();            
        }

        public void InitializeCamSetting(double width , double height)
        {
            DefaultControlSetting(width, height);
            this.CamHost.Child = IcImagingControl;
        }
        /// <summary>
        /// 相機控制項的初始化設定
        /// </summary>
        private void DefaultControlSetting(double width, double height)
        {
            ((System.ComponentModel.ISupportInitialize)(IcImagingControl)).BeginInit();
            //IcImagingControl.Size = new System.Drawing.Size((int)width, (int)height);
            //ResetCameraSize(width, height);
            IcImagingControl.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
            IcImagingControl.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
            IcImagingControl.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
            IcImagingControl.LiveDisplayPosition = new System.Drawing.Point(0,0);
            IcImagingControl.ImageRingBufferSize = 8;
            IcImagingControl.BackColor = System.Drawing.Color.Black;
            IcImagingControl.LiveDisplayDefault = false; //如果設定為true，將無法改變顯示視窗大小，所以下面的icImagingControl.Height將無法使用
            IcImagingControl.LiveCaptureContinuous = true; //LiveCaptureContinuous = True means that every frame is copied to the ring buffer.
            IcImagingControl.LiveCaptureLastImage = false;
            IcImagingControl.LiveDisplay = false; //設定為false才能將影像處理顯示在control
            //IcImagingControl.LiveDisplayHeight = IcImagingControl.Height;
            //IcImagingControl.LiveDisplayWidth = IcImagingControl.Width;
            ResetCameraSize(width, height);
            IcImagingControl.MemoryCurrentGrabberColorformat = ICImagingControlColorformats.ICY800;
            ((System.ComponentModel.ISupportInitialize)(IcImagingControl)).EndInit();
        }
        public void ResetCameraSize(double width, double height)
        {
            IcImagingControl.Size = new System.Drawing.Size((int)width, (int)height);
            IcImagingControl.LiveDisplayHeight = IcImagingControl.Height;
            IcImagingControl.LiveDisplayWidth = IcImagingControl.Width;
        }
    }
}
