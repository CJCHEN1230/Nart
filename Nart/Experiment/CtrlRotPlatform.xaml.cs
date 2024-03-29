﻿using System;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace Nart.Experiment
{
    /// <summary>
    /// CtrlRotPlatform.xaml 的互動邏輯
    /// </summary>
    public partial class CtrlRotPlatform : Window
    {
        private SerialPort myport = new SerialPort();
        private Excel.Application excelApp; //Excel程式
        private Excel._Workbook wBook; //活頁簿
        Excel._Worksheet wSheet;
        int currentLine = 2;
        int count = 0;

        private void DisplayLoop()
        {
            myport.Write("MoveXPosi");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveXPosi");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            
        }


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
            InitializeExcel();
            BtnState(false);
        }
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                myport.BaudRate = 9600;
                myport.PortName = cbSerialPorts.SelectedItem.ToString();
                myport.Open();
                BtnState(true);
                MessageBox.Show("Serial port connect successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open() error: " + ex.Message);
            }
        }
        private void EnterClick(object sender, RoutedEventArgs e)
        {
            string temp = inputTB.Text;

            new Thread(() =>
            {
      
                if (!(temp == ""))
                {
                    myport.Write(temp);
                    string get = myport.ReadLine();
                    Console.WriteLine("\n!!!!!!!!!:" + get);
                    //Thread.Sleep(2000);
                    //WriteLengthData();
                    
                }

            }).Start();
            inputTB.Text = "";
        }
        private void DoExperiemntClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++) 
            {
                myport.Write("10");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
                WriteLengthData();
            }

        }

        private void inputTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnterClick(sender, e);
            }
        }



        private void WriteLengthData()
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

                    try
                    {
                        // 設定第X列資料
                        excelApp.Cells[currentLine, 1] = (currentLine - 2) * 10;
                        excelApp.Cells[currentLine, 2] = CurWorldPoints[0].ThreeLength[0];
                        excelApp.Cells[currentLine, 3] = CurWorldPoints[0].ThreeLength[1];
                        excelApp.Cells[currentLine, 4] = CurWorldPoints[0].ThreeLength[2];

                        currentLine++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                        break;
                    }

                    count = 0;
                    break;
                }
                else
                {
                    count++;
                }

                if (count >= 20)
                {
                    excelApp.Cells[currentLine, 1] = (currentLine - 2) * 10;
                    excelApp.Cells[currentLine, 2] = "找不到";
                    excelApp.Cells[currentLine, 3] = "找不到";
                    excelApp.Cells[currentLine, 4] = "找不到";
                    currentLine++;
                    break;
                }
            }
        }

        private void WriteWorldCoord()
        {
            while (true)
            {
                if (CalcCoord.ForExperiment.Count == 1)
                {
                    CurWorldPoints.Clear();
                    CurWorldPoints.Add(CalcCoord.ForExperiment[0]);

                    Console.WriteLine("\nPoint1:" + CurWorldPoints[0].ThreePoints[0].X + "  " + CurWorldPoints[0].ThreePoints[0].Y + "  " + CurWorldPoints[0].ThreePoints[0].Z);
                    Console.WriteLine("\nPoint2:" + CurWorldPoints[0].ThreePoints[1].X + "  " + CurWorldPoints[0].ThreePoints[1].Y + "  " + CurWorldPoints[0].ThreePoints[1].Z);
                    Console.WriteLine("\nPoint3:" + CurWorldPoints[0].ThreePoints[2].X + "  " + CurWorldPoints[0].ThreePoints[2].Y + "  " + CurWorldPoints[0].ThreePoints[2].Z);

                    try
                    {
                        // 設定第X列資料
                        excelApp.Cells[currentLine, 1] = (currentLine - 2) * 10;
                        excelApp.Cells[currentLine, 2] = CurWorldPoints[0].ThreePoints[0].X;
                        excelApp.Cells[currentLine, 3] = CurWorldPoints[0].ThreePoints[0].Y;
                        excelApp.Cells[currentLine, 4] = CurWorldPoints[0].ThreePoints[0].Z;
                        excelApp.Cells[currentLine, 5] = CurWorldPoints[0].ThreePoints[1].X ;
                        excelApp.Cells[currentLine, 6] = CurWorldPoints[0].ThreePoints[1].Y;
                        excelApp.Cells[currentLine, 7] = CurWorldPoints[0].ThreePoints[1].Z;
                        excelApp.Cells[currentLine, 8] = CurWorldPoints[0].ThreePoints[2].X;
                        excelApp.Cells[currentLine, 9] = CurWorldPoints[0].ThreePoints[2].Y;
                        excelApp.Cells[currentLine, 10] = CurWorldPoints[0].ThreePoints[2].Z;

                        currentLine++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                        break;
                    }

                    count = 0;
                    break;
                }
                else
                {
                    count++;
                }

                if (count >= 20)
                {
                    excelApp.Cells[currentLine, 1] = (currentLine - 2) * 10;
                    excelApp.Cells[currentLine, 2] = "找不到";
                    excelApp.Cells[currentLine, 3] = "找不到";
                    excelApp.Cells[currentLine, 4] = "找不到";
                    excelApp.Cells[currentLine, 5] = "找不到";
                    excelApp.Cells[currentLine, 6] = "找不到";
                    excelApp.Cells[currentLine, 7] = "找不到";
                    excelApp.Cells[currentLine, 8] = "找不到";
                    excelApp.Cells[currentLine, 9] = "找不到";
                    excelApp.Cells[currentLine, 10] = "找不到";
                    currentLine++;
                    break;
                }
            }
        }

        private void InitializeExcel()
        {
            // 開啟一個新的應用程式
            excelApp = new Excel.Application();

            // 讓Excel文件可見
            excelApp.Visible = true;

            // 停用警告訊息
            excelApp.DisplayAlerts = false;

            // 加入新的活頁簿
            excelApp.Workbooks.Add(Type.Missing);

            // 引用第一個活頁簿
            wBook = excelApp.Workbooks[1];

            // 設定活頁簿焦點
            wBook.Activate();

            // 引用第一個工作表
            wSheet = (Excel._Worksheet)wBook.Worksheets[1];

            // 命名工作表的名稱
            wSheet.Name = "角度精度驗證";

            // 設定工作表焦點
            wSheet.Activate();

            // 設定第1列資料
            excelApp.Cells[1, 1] = "第一邊";
            excelApp.Cells[1, 2] = "第二邊";
            excelApp.Cells[1, 3] = "第三邊";
        }
      
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            string pathFile = "D:\\Desktop\\Hello";
            try
            {
                //另存活頁簿
                wBook.SaveAs(pathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Console.WriteLine("儲存文件於 " + Environment.NewLine + pathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
            }

            //關閉活頁簿
            wBook.Close(false, Type.Missing, Type.Missing);
            //關閉Excel
            excelApp.Quit();
            //釋放Excel資源
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            wBook = null;
            excelApp = null;
            wSheet = null;
            GC.Collect();
        }

        private void BtnState(bool State) //Button狀態
        {
            button_Copy3.IsEnabled = State;
            button_Copy1.IsEnabled = State;
            button.IsEnabled = State;
      
        }

        private void WriteLengthDataClick(object sender, RoutedEventArgs e)
        {
            WriteLengthData();
        }

        private void WritePointsDataClick(object sender, RoutedEventArgs e)
        {
            WriteWorldCoord();
        }

        private void XPosiBtn_Click(object sender, RoutedEventArgs e)
        {

            myport.Write("MoveXPosi");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveXPosi");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void XNegBtn_Click(object sender, RoutedEventArgs e)
        {
            myport.Write("MoveXNeg");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveXNeg");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void YPosiBtn_Click(object sender, RoutedEventArgs e)
        {
            myport.Write("MoveYPosi");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveYPosi");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void YNegBtn_Click(object sender, RoutedEventArgs e)
        {
            myport.Write("MoveYNeg");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveYNeg");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void X5PosiBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i =0;i<5 ;i++)
            {
                myport.Write("MoveXPosi");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }

        private void X5NegBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                myport.Write("MoveXNeg");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }

        private void Y5PosiBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                myport.Write("MoveYPosi");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }

        private void Y5NegBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                myport.Write("MoveYNeg");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }

        private void ZPosiBtn_Click(object sender, RoutedEventArgs e)
        {
            myport.Write("MoveZPosi");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveZPosi");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void ZNegBtn_Click(object sender, RoutedEventArgs e)
        {
            myport.Write("MoveZNeg");
            string get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
            myport.Write("MoveZNeg");
            get = myport.ReadLine();
            Console.WriteLine("\n!!!!!!!!!:" + get);
        }

        private void Z5PosiBtn(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                myport.Write("MoveZPosi");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }

        private void Z5NegBtn(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                myport.Write("MoveZNeg");
                string get = myport.ReadLine();
                Console.WriteLine("\n!!!!!!!!!:" + get);
            }
        }
    }
}
