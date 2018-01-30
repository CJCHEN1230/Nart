using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;

namespace Nart.Experiment
{
    /// <summary>
    /// CtrlRotPlatform.xaml 的互動邏輯
    /// </summary>
    public partial class CtrlRotPlatform : Window
    {
        private SerialPort myport = new SerialPort();
        //public static bool GetMarkerToggle = false;
        private  List<Marker3D> CurWorldPoints = new List<Marker3D>(10);
        public CtrlRotPlatform()
        {
            InitializeComponent();
            string[] serialPorts = SerialPort.GetPortNames(); //取得所有COM Port名稱

            List<string> ComboBoxList = new List<string>();
            for (int i=0; i<serialPorts.Length;i++)
            {
                ComboBoxList.Add(serialPorts[i]);
            }
            cbSerialPorts.ItemsSource = ComboBoxList;
            inputTB.Text = "";
        }
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                myport.BaudRate = 9600;
                myport.PortName = cbSerialPorts.SelectedItem.ToString();
                myport.Open();
                MessageBox.Show("Serial port connect successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open() error: " + ex.Message);
            }
        }
        private void EnterClick(object sender, RoutedEventArgs e)
        {
            myport.Write(inputTB.Text);
            Thread.Sleep(1000);
            inputTB.Text = "";
        }

        private void DoExperiemntClick(object sender, RoutedEventArgs e)
        {
            
            while (true) 
            {
                if (CalcCoord.ForExperiment.Count == 1)
                {
                    CurWorldPoints.Clear();
                    CurWorldPoints.Add(CalcCoord.ForExperiment[0]);

                    Console.WriteLine("\n1:" + CurWorldPoints[0].ThreeLength[0]);
                    Console.WriteLine("\n2:" + CurWorldPoints[0].ThreeLength[1]);
                    Console.WriteLine("\n3:" + CurWorldPoints[0].ThreeLength[2]);

                     


                    break;
                }
            }



        }
    }
}
