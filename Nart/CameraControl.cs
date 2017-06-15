using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using System.Threading.Tasks;
using System.Windows.Threading;
using TIS.Imaging;
using UseCVLibrary;

namespace Nart
{
    public class CameraControl :DispatcherObject //繼承此DispatcherObject才能使用Dispatch
    {
        
        public CameraControl(int width,int height)
        {
            icImagingControl[0] = new TIS.Imaging.ICImagingControl();
            icImagingControl[1] = new TIS.Imaging.ICImagingControl();

            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).BeginInit();

            icImagingControl[0].Size = new System.Drawing.Size(width, height);
            icImagingControl[1].Size = new System.Drawing.Size(width, height);

            defaultCameraSetting(icImagingControl[0]);
            defaultCameraSetting(icImagingControl[1]);

            icImagingControl[0].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl1_ImageAvailable);
            icImagingControl[1].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl2_ImageAvailable);


            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).EndInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).EndInit();
        }

        /// <summary>
        /// 雙相機控制項
        /// </summary>
        internal TIS.Imaging.ICImagingControl[] icImagingControl = new TIS.Imaging.ICImagingControl[2];
        /// <summary>
        /// 雙相機Buffer
        /// </summary>
        private ImageBuffer[] _displayBuffer = new ImageBuffer[2];
        /// <summary>
        /// 角點影像處理
        /// </summary>
        private CornerPointFilter _corPtFltr = new CornerPointFilter();
        /// <summary>
        /// 顯示畫面的委派
        /// </summary>
        private delegate void ShowBufferDelegate(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl1);
        /// <summary>
        /// 顯示畫面的函數實作
        /// </summary>
        private void ShowImageBuffer(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl)
        {
            icImagingControl.DisplayImageBuffer(buffer);
        }
        /// <summary>
        /// 雙相機的初始化設定
        /// </summary>
        private void defaultCameraSetting(TIS.Imaging.ICImagingControl icImagingControl)
        {
            icImagingControl.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
            icImagingControl.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
            icImagingControl.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
            icImagingControl.ImageRingBufferSize = 10;
            icImagingControl.BackColor = System.Drawing.Color.Black;
            icImagingControl.LiveDisplayDefault = false; //如果設定為true，將無法改變顯示視窗大小，所以下面的icImagingControl.Height將無法使用
            icImagingControl.LiveCaptureContinuous = true; //LiveCaptureContinuous = True means that every frame is copied to the ring buffer.
            icImagingControl.LiveCaptureLastImage = false;
            icImagingControl.LiveDisplay = false; //設定為false才能將影像處理顯示在control
            icImagingControl.LiveDisplayHeight = icImagingControl.Height;
            icImagingControl.LiveDisplayWidth = icImagingControl.Width;
            icImagingControl.MemoryCurrentGrabberColorformat = ICImagingControlColorformats.ICY800;
        }

        public void CameraStart()
        {
            if (!icImagingControl[0].DeviceValid)
            {
                icImagingControl[0].ShowDeviceSettingsDialog();

                if (!icImagingControl[0].DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
            }
            if (!icImagingControl[1].DeviceValid)
            {
                icImagingControl[1].ShowDeviceSettingsDialog();

                if (!icImagingControl[1].DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
            }
            if (icImagingControl[0].DeviceValid && icImagingControl[1].DeviceValid) 
            {
                icImagingControl[0].LiveStart();
                icImagingControl[1].LiveStart();
            }
        }
        
        private void icImagingControl1_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            _displayBuffer[0] = icImagingControl[0].ImageBuffers[e.bufferIndex];

            unsafe
            {

                byte* data = _displayBuffer[0].Ptr;
                _corPtFltr.GetCornerPoint(data);
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), _displayBuffer[0], icImagingControl[0]);

            }
        }

        private void icImagingControl2_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            _displayBuffer[1] = icImagingControl[1].ImageBuffers[e.bufferIndex];

            unsafe
            {

                byte* data = _displayBuffer[1].Ptr;
                _corPtFltr.GetCornerPoint(data);
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), _displayBuffer[1], icImagingControl[1]);

            }
        }
    }
}
