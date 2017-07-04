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
        /// <summary>
        /// 相片寬度
        /// </summary>
        private int _width;
        /// <summary>
        /// 相片長度
        /// </summary>
        private int _height;
         /// <summary>
        /// 儲存兩相機的點資料
        /// </summary>
        List<BWMarker>[] OutputMarker = new List<BWMarker>[2];
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
        /// <summary>
        /// 顯示畫面的委派
        /// </summary>
        private delegate void ShowBufferDelegate();
        /// <summary>
        /// 顯示畫面的函數實作
        /// </summary>
        private Thread _displayThread ;
        /// <summary>
        /// 管理Thread通行
        /// </summary>
        private AutoResetEvent _are = new AutoResetEvent(false);
        /// <summary>
        /// 管理Thread記數
        /// </summary>
        private CountdownEvent _count = new CountdownEvent(2);//代表要計數兩次
        /// <summary>
        /// 計算座標的類別
        /// </summary>
        private CalcCoord _calcCoord = new CalcCoord();
        /// <summary>
        /// 傳進來的width跟height決定inImageControl的長寬
        /// </summary>
        public CameraControl(int width, int height)
        {
            icImagingControl[0] = new TIS.Imaging.ICImagingControl();
            icImagingControl[1] = new TIS.Imaging.ICImagingControl();

            _corPtFltr[0] = new CornerPointFilter(0);
            _corPtFltr[1] = new CornerPointFilter(1);           

            OutputMarker[0] = new List<BWMarker>(10);// 儲存的Marker空間，預設10個
            OutputMarker[1] = new List<BWMarker>(10);

            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).BeginInit();
           
            icImagingControl[0].Size = new System.Drawing.Size(width, height);
            icImagingControl[1].Size = new System.Drawing.Size(width, height);

            defaultControlSetting(icImagingControl[0]);
            defaultControlSetting(icImagingControl[1]);

            icImagingControl[0].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl1_ImageAvailable);
            icImagingControl[1].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl2_ImageAvailable);


            ((System.ComponentModel.ISupportInitialize)(icImagingControl[0])).EndInit();
            ((System.ComponentModel.ISupportInitialize)(icImagingControl[1])).EndInit();

            LoadCamSetting();


            _displayThread = new Thread(DisplayLoop);


            _displayThread.Start();


        }
        /// <summary>
        /// 實體化委派的顯示函數
        /// </summary>
        private void ShowImageBuffer()
        {
            
            Parallel.For(0, 2, i =>
            {
                icImagingControl[i].DisplayImageBuffer(_displayBuffer[i]);
            });

            //time_start = DateTime.Now;


            _calcCoord.Rectificaion(OutputMarker);

            //time_end = DateTime.Now;
            //string result2 = ((TimeSpan)(time_end - time_start)).TotalMilliseconds.ToString();

            //Console.WriteLine("time: " + result2);
            _calcCoord.MatchAndCalc3D(OutputMarker);


            

            _are.Set();
            _are.Set();
          
        }
        /// <summary>
        /// 雙相機控制項的初始化設定
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
        /// <summary>
        /// 從xml匯入相機參數
        /// </summary>
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
        /// <summary>
        /// 目的是開啟相機，但匯入相機參數檔失敗之後，會自動進入設定參數頁面
        /// </summary>
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

               
                Parallel.For(0, 2, i =>
                {
                    icImagingControl[i].LiveStart();
                });


            }
        }


        int count1 = 0;
        int count2 = 0;

        int buf1 = 0;
        int buf2 = 0;

        /// <summary>
        /// 相機拍攝的所觸發的事件函數
        /// </summary>  
        private void icImagingControl1_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            
            buf1 = e.bufferIndex;
            _displayBuffer[0] = icImagingControl[0].ImageBuffers[e.bufferIndex];

   
            unsafe
            {
                
                byte* data = _displayBuffer[0].Ptr;

                OutputMarker[0] = _corPtFltr[0].GetCornerPoint(_width, _height, data);

                Console.WriteLine("\n\n1: "+DateTime.Now.ToString("ss.ffff"));

                _count.Signal();

                _are.WaitOne();

            }
          
        }
        /// <summary>
        /// 相機拍攝的所觸發的事件函數
        /// </summary>
        private void icImagingControl2_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            buf2 = e.bufferIndex;
            _displayBuffer[1] = icImagingControl[1].ImageBuffers[e.bufferIndex];

            unsafe
            {               
                byte* data = _displayBuffer[1].Ptr;

                OutputMarker[1] = _corPtFltr[1].GetCornerPoint(_width, _height, data);

                Console.WriteLine("\n\n2: " + DateTime.Now.ToString("ss.ffff"));

                _count.Signal();

                _are.WaitOne();
              
            }
        }
        /// <summary>
        /// 在此顯示區域無限循環         
        /// </summary>
        private void DisplayLoop()
        {            
            while (true)
            {
                _count.Wait();            //等到兩個擷取畫面各執行一次Signal()後才通過  
                _count.Reset(2);          //重設定count為兩次

                Console.WriteLine("    ");
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer));


            }
        }

        static DateTime time_start;
        static DateTime time_end;

    }
}
