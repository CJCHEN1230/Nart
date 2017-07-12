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

namespace NartControl
{
    /// <summary>
    /// CameraWrapper.xaml 的互動邏輯
    /// </summary>
    public partial class CameraWrapper : WindowsFormsHost
    {

        public TIS.Imaging.ICImagingControl icImagingControl = new TIS.Imaging.ICImagingControl();

        
        public CameraWrapper()
        {
            InitializeComponent();
            defaultControlSetting(this.ActualWidth, this.ActualHeight);
        }

        private void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            this.Child = icImagingControl;
            Console.WriteLine("  ActualHeight:" + this.ActualHeight+ "  ActualWidth:" + this.ActualWidth);
            Console.WriteLine("  height:" + this.Height + "  width:" + this.Width);

            
        }
        private void defaultControlSetting(double width, double height)
        {
            ((System.ComponentModel.ISupportInitialize)(icImagingControl)).BeginInit();
            icImagingControl.Size = new System.Drawing.Size((int)width, (int)height);
            icImagingControl.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
            icImagingControl.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
            icImagingControl.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
            icImagingControl.LiveDisplayPosition = new System.Drawing.Point(0,0);
            icImagingControl.ImageRingBufferSize = 8;
            icImagingControl.BackColor = System.Drawing.Color.Black;
            icImagingControl.LiveDisplayDefault = false; //如果設定為true，將無法改變顯示視窗大小，所以下面的icImagingControl.Height將無法使用
            icImagingControl.LiveCaptureContinuous = true; //LiveCaptureContinuous = True means that every frame is copied to the ring buffer.
            icImagingControl.LiveCaptureLastImage = false;
            icImagingControl.LiveDisplay = true; //設定為false才能將影像處理顯示在control
            icImagingControl.LiveDisplayHeight = icImagingControl.Height;
            icImagingControl.LiveDisplayWidth = icImagingControl.Width;
            icImagingControl.MemoryCurrentGrabberColorformat = ICImagingControlColorformats.ICY800;
            ((System.ComponentModel.ISupportInitialize)(icImagingControl)).BeginInit();
        }
    }
}
