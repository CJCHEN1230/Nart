using Nart.Model_Object;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using TIS.Imaging;
using UseCVLibrary;

namespace Nart
{
    public class CameraControl :DispatcherObject //繼承此DispatcherObject才能使用Dispatch
    {       
         /// <summary>
        /// 儲存兩相機的點資料
        /// </summary>
        private readonly List<BWMarker>[] _outputMarker = new List<BWMarker>[2];
        /// <summary>
        /// 雙相機Buffer
        /// </summary>
        private readonly ImageBuffer[] _displayBuffer = new ImageBuffer[2];
        /// <summary>
        /// 角點影像處理
        /// </summary>
        private readonly CornerPointFilter[] _corPtFltr = new CornerPointFilter[2];
        /// <summary>
        /// 雙相機控制項
        /// </summary>
        internal TIS.Imaging.ICImagingControl[] IcImagingControl = new TIS.Imaging.ICImagingControl[2];       
        /// <summary>
        /// 顯示畫面的委派
        /// </summary>
        private delegate void ShowBufferDelegate();
        /// <summary>
        /// 顯示畫面的函數實作
        /// </summary>
        internal Thread DisplayThread ;
        /// <summary>
        /// 管理Thread通行
        /// </summary>
        private AutoResetEvent[] _are = new AutoResetEvent[2];//目前暫時沒有用到        
        /// <summary>
        /// 管理Thread記數
        /// </summary>
        private CountdownEvent _count = new CountdownEvent(2);//代表要計數兩次
        /// <summary>
        /// 計算座標的類別
        /// </summary>
        private CalcCoord _calcCoord ;
        /// <summary>
        /// 開啟Registration Button的功能
        /// </summary>
        public static bool RegToggle = false;
        /// <summary>
        /// 開啟Tracking Button的功能
        /// </summary>
        public static bool TrackToggle = false;
        /// <summary>
        /// 管理相機通過與否
        /// </summary>
        private bool[] _cameraToggle = new bool[2] {true,true};
        /// <summary>
        /// 傳進來的width跟height決定inImageControl的長寬
        /// </summary>      
        public CameraControl(TIS.Imaging.ICImagingControl[] cam)
        {
          
            _calcCoord = new CalcCoord();

            IcImagingControl = cam;

            _are[0] = new AutoResetEvent(false);
            _are[1] = new AutoResetEvent(false);

            _corPtFltr[0] = new CornerPointFilter(0);
            _corPtFltr[1] = new CornerPointFilter(1);

            _outputMarker[0] = new List<BWMarker>(10);// 儲存的Marker空間，預設10個
            _outputMarker[1] = new List<BWMarker>(10);

           
            IcImagingControl[0].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl1_ImageAvailable);
            IcImagingControl[1].ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl2_ImageAvailable);
          
            LoadCamSetting();

            DisplayThread = new Thread(DisplayLoop);
            DisplayThread.IsBackground = true;

        }            
        /// <summary>
        /// 從xml匯入相機參數
        /// </summary>
        private void LoadCamSetting()
        {
            try
            {
                IcImagingControl[0].LoadDeviceStateFromFile("../../../data/Cam1.xml", true);
                IcImagingControl[1].LoadDeviceStateFromFile("../../../data/Cam2.xml", true);
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
            //確認裝置有沒有效
            if (!IcImagingControl[0].DeviceValid)
            {
                IcImagingControl[0].ShowDeviceSettingsDialog();

                if (!IcImagingControl[0].DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                }
            }
            if (!IcImagingControl[1].DeviceValid)
            {
                IcImagingControl[1].ShowDeviceSettingsDialog();

                if (!IcImagingControl[1].DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                }
            }
            if (IcImagingControl[0].DeviceValid && IcImagingControl[1].DeviceValid) 
            {
                DisplayThread.Start();
                

                Parallel.For(0, 2, i =>
                {
                    IcImagingControl[i].LiveStart();
                });
            }
        }
        public void CameraClose()
        {
            DisplayThread.Abort();

            if (IcImagingControl[0].LiveVideoRunning)
            {
                
                IcImagingControl[0].LiveStop();
            }
            if (IcImagingControl[1].LiveVideoRunning)
            {
                IcImagingControl[1].LiveStop();
            }
            
        }
        private void MoveModel()
        {
            foreach (BoneModel boneModel in MainViewModel.Data.BoneCollection)
            {
                      //boneModel.SetTransformMatrix();
                 boneModel.SetTransformMatrix();
            }
        }
        /// <summary>
        /// 實體化委派的顯示函數
        /// </summary>
        private void ShowImageBuffer()
        {
           
            if (MainViewModel.TabIndex == 0)
            {
                Parallel.For(0, 2, i =>
                {                    
                    IcImagingControl[i].DisplayImageBuffer(_displayBuffer[i]);
                });
            }

            _calcCoord.Rectify(_outputMarker);
          
            _calcCoord.MatchAndCalc3D(_outputMarker);
            
            


            if (CameraControl.RegToggle)
            {
                _calcCoord.Registraion2();
            }

            if (CameraControl.TrackToggle) 
            {
                _calcCoord.CalcModelTransform2();

                MoveModel();

                _calcCoord.CalcCraniofacialInfo();

                _calcCoord.CalcBallDistance();
            }
        

            _cameraToggle[0] = true;
            _cameraToggle[1] = true;           
        }
        /// <summary>
        /// 相機拍攝的所觸發的事件函數
        /// </summary>  
        private void icImagingControl1_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e)
        {
            if (_cameraToggle[0])
            {
                _cameraToggle[0] = false;
                _displayBuffer[0] = IcImagingControl[0].ImageBuffers[e.bufferIndex];

                unsafe
                {

                    byte* data = _displayBuffer[0].Ptr;

                    _outputMarker[0] = _corPtFltr[0].GetCornerPoint(IcImagingControl[0].ImageSize.Width, IcImagingControl[0].ImageSize.Height, data);

                    _count.Signal();

                    //_are[0].WaitOne();

                }
            }
          
        }
        /// <summary>
        /// 相機拍攝的所觸發的事件函數
        /// </summary>
        private void icImagingControl2_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e)
        {
            if (_cameraToggle[1])
            {
                _cameraToggle[1] = false;
                _displayBuffer[1] = IcImagingControl[1].ImageBuffers[e.bufferIndex];

                unsafe
                {

                    byte* data = _displayBuffer[1].Ptr;

                    _outputMarker[1] = _corPtFltr[1].GetCornerPoint(IcImagingControl[0].ImageSize.Width, IcImagingControl[0].ImageSize.Height, data);
                 
                    _count.Signal();

                   // _are[1].WaitOne();

                }
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
                
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer));
            }
        }
    }
}
