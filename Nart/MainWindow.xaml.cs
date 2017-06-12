using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIS.Imaging;
using UseCVLibrary;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;




namespace Nart
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private TIS.Imaging.ICImagingControl icImagingControl1;
        private TIS.Imaging.ICImagingControl icImagingControl2;
        private ImageBuffer DisplayBuffer1 = null;
        private ImageBuffer DisplayBuffer2 = null;
        private CornerPointFilter cornerPointFilter;
        private const string MODEL_PATH = "Model.stl";
        private ModelVisual3D device3D;


        private delegate void ShowBufferDelegate(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl1);

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();

            device3D = new ModelVisual3D();
            //device3D.Content = Display3d(MODEL_PATH);

            this.viewPort3d.Background = new SolidColorBrush(Colors.Black);


            viewPort3d.Children.Add(device3D);

            cornerPointFilter = new CornerPointFilter();
            icImagingControl1 = new TIS.Imaging.ICImagingControl();
            icImagingControl2 = new TIS.Imaging.ICImagingControl();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).BeginInit();

            this.icImagingControl1.Size = new System.Drawing.Size((int)CamHost1.Width, (int)CamHost1.Height);
            this.icImagingControl2.Size = new System.Drawing.Size((int)CamHost2.Width, (int)CamHost2.Height);

            this.defaultCameraSetting(icImagingControl1);
            this.defaultCameraSetting(icImagingControl2);

            this.icImagingControl1.ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl1_ImageAvailable);
            this.icImagingControl2.ImageAvailable += new System.EventHandler<TIS.Imaging.ICImagingControl.ImageAvailableEventArgs>(this.icImagingControl2_ImageAvailable);


            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icImagingControl2)).EndInit();




            CamHost1.Child = icImagingControl1;
            CamHost2.Child = icImagingControl2;

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

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

        private void icImagingControl1_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            DisplayBuffer1 = icImagingControl1.ImageBuffers[e.bufferIndex];

            unsafe
            {


                byte* data = DisplayBuffer1.Ptr;
                //Console.WriteLine((int)data);
                cornerPointFilter.GetCornerPoint(data);
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), DisplayBuffer1, icImagingControl1);
                // Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Label, int>(updateControl), label, result);
                //Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<ImageBuffer, TIS.Imaging.ICImagingControl>(ShowImageBuffer), DisplayBuffer1, icImagingControl1);


                //this.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), DisplayBuffer1, icImagingControl1);
            }
        }

        private void icImagingControl2_ImageAvailable(object sender, TIS.Imaging.ICImagingControl.ImageAvailableEventArgs e)
        {
            DisplayBuffer2 = icImagingControl2.ImageBuffers[e.bufferIndex];

            unsafe
            {

                byte* data = DisplayBuffer2.Ptr;
                cornerPointFilter.GetCornerPoint(data);
                Dispatcher.BeginInvoke(new ShowBufferDelegate(ShowImageBuffer), DisplayBuffer2, icImagingControl2);

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (!icImagingControl1.DeviceValid)
            {
                icImagingControl1.ShowDeviceSettingsDialog();

                if (!icImagingControl1.DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            if (!icImagingControl2.DeviceValid)
            {
                icImagingControl2.ShowDeviceSettingsDialog();

                if (!icImagingControl2.DeviceValid)
                {
                    System.Windows.Forms.MessageBox.Show("No device was selected.", "Grabbing an Image",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            icImagingControl1.LiveStart();
            icImagingControl2.LiveStart();
        }

        private void ShowImageBuffer(TIS.Imaging.ImageBuffer buffer, TIS.Imaging.ICImagingControl icImagingControl)
        {
            icImagingControl.DisplayImageBuffer(buffer);
        }



        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                //Adding a gesture here
                //viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //Load the 3D model file
                device = import.Load(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not find the 3D model file
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Title = "New Project";
            dlg.DefaultExt = ".stl";
            dlg.Multiselect = false;
            dlg.Filter = "STL File (.stl)|*.stl";


            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {

                //importFiles(dlg.FileNames);
                device3D.Content = Display3d(dlg.FileNames[0]);
            }
        }

        private void NewCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
