using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//using System.Threading.Tasks;
using System.Windows.Threading;
using TIS.Imaging;
using UseCVLibrary;

namespace Nart
{
    public class CameraControl :DispatcherObject //繼承此DispatcherObject才能使用Dispatch
    {

        private int _width;

        private int _height;
        /// <summary>
        /// 儲存兩相機的點資料
        /// </summary>
        List<List<PointF>>[] OutputCorPt = new List<List<PointF>>[2];
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
        private CornerPointFilter[] _corPtFltr = new CornerPointFilter[2];


        public Thread[] _camThread = new Thread[2];
        /// <summary>
        /// 顯示畫面的委派
        /// </summary>
        private delegate void   ShowBufferDelegate(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl1);
        /// <summary>
        /// 顯示畫面的函數實作
        /// </summary>
        private Thread sample ;
        public CameraControl(int width, int height)
        {
            icImagingControl[0] = new TIS.Imaging.ICImagingControl();
            icImagingControl[1] = new TIS.Imaging.ICImagingControl();

            _corPtFltr[0] = new CornerPointFilter(0);
            _corPtFltr[1] = new CornerPointFilter(1);

            OutputCorPt[0] = new List<List<PointF>>();
            OutputCorPt[1] = new List<List<PointF>>();


            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).BeginInit();

            icImagingControl[0].Name = "cam1";
            icImagingControl[1].Name = "cam2";


            icImagingControl[0].Size = new System.Drawing.Size(width, height);
            icImagingControl[1].Size = new System.Drawing.Size(width, height);

            defaultControlSetting(icImagingControl[0]);
            defaultControlSetting(icImagingControl[1]);

            icImagingControl[0].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl1_ImageAvailable);
            icImagingControl[1].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl2_ImageAvailable);


            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).EndInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).EndInit();

            LoadCamSetting();


            sample = new Thread(_ThreadFunction);


            sample.Start();


        }

        private void ShowImageBuffer(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl)
        {
            icImagingControl.DisplayImageBuffer(buffer);
        }


        private delegate void ShowBufferDelegate2(TIS.Imaging.ImageBuffer buffer1, TIS.Imaging.ImageBuffer buffer2);
        private void ShowImageBuffer2(TIS.Imaging.ImageBuffer buffer1, TIS.Imaging.ImageBuffer buffer2)
        {
            
            Parallel.For(0, 2, i =>
            {
                icImagingControl[i].DisplayImageBuffer(_displayBuffer[i]);
            });
            //Console.WriteLine("show buffer: "+Thread.CurrentThread.ManagedThreadId);
            mre.Set();
            mre.Set();

            //icImagingControl[0].DisplayImageBuffer(buffer1);
            //icImagingControl[1].DisplayImageBuffer(buffer2);

        }

        /// <summary>
        /// 雙相機的初始化設定
        /// </summary>
        private void defaultControlSetting(TIS.Imaging.ICImagingControl icImagingControl)
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

        private void LoadCamSetting()
        {
            try
            {
                icImagingControl[0].LoadDeviceStateFromFile("../../../data/Cam1.xml", true);
                icImagingControl[1].LoadDeviceStateFromFile("../../../data/Cam2.xml", true);
            }
            catch (Exception)
            {
                MessageBox.Show("Load device setting failed.");
            }
           
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
               
                _width = icImagingControl[0].ImageSize.Width;
                _height = icImagingControl[0].ImageSize.Height;

                icImagingControl[0].LiveStart();
                icImagingControl[1].LiveStart();
            }
        }
        private void DisplayTest()
        {
            if (_camThread[0] is Thread && _camThread[1] is Thread)
            {
                if (_camThread[1].ThreadState != ThreadState.Unstarted)
                {
                    _camThread[1].Join();
                    Dispatcher.BeginInvoke(new ShowBufferDelegate2(ShowImageBuffer2), _displayBuffer[0], _displayBuffer[1]);
                }
            }
          
            
           
        }

        public AutoResetEvent mre = new AutoResetEvent(false);
        public CountdownEvent count = new CountdownEvent(2);

        private void icImagingControl1_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            _displayBuffer[0] = icImagingControl[0].ImageBuffers[e.bufferIndex];

   
            unsafe
            {
                //Console.WriteLine("Thread A: "+Thread.CurrentThread.ManagedThreadId);
                byte* data = _displayBuffer[0].Ptr;
                
                OutputCorPt[0] = _corPtFltr[0].GetCornerPoint(_width, _height, data);
                //Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), _displayBuffer[0], icImagingControl[0]);
                //Thread.Sleep(1000);
                count.Signal();
                //Console.WriteLine("\n\nThreadA:" + count.CurrentCount);
                mre.WaitOne();

            }
          
        }

        private void icImagingControl2_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            _displayBuffer[1] = icImagingControl[1].ImageBuffers[e.bufferIndex];

            unsafe
            {

                //Console.WriteLine("Thread B: " + Thread.CurrentThread.ManagedThreadId);
                byte* data = _displayBuffer[1].Ptr;
                //Thread.Sleep(1000);
                OutputCorPt[1] = _corPtFltr[1].GetCornerPoint(_width, _height, data);
                //Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), _displayBuffer[1], icImagingControl[1]);
                count.Signal();
                //Console.WriteLine("\n\nThreadB:" + count.CurrentCount);

                mre.WaitOne();
               
                
                //Console.WriteLine("Thread State:"+sample.ThreadState);
                //DisplayTest();
                //_camThread[1].Join();

            }
        }




        private void _ThreadFunction()
        {
            
            while (true)
            {
                //Console.WriteLine("\n\n\n\nThreadC0: " + count.CurrentCount);

                count.Wait();
                //Console.WriteLine("\n\nThreadC1: " + count.CurrentCount);
                count.Reset(2);
                //Console.WriteLine("\n\nThreadC2: " + count.CurrentCount);

                //mre.Reset();
                //Console.WriteLine("third: " + Thread.CurrentThread.ManagedThreadId);
                Dispatcher.BeginInvoke(new ShowBufferDelegate2(ShowImageBuffer2), _displayBuffer[0], _displayBuffer[1]);


                //Thread.Sleep(1000);

                //Console.Read();
            }
        }
    }
}
