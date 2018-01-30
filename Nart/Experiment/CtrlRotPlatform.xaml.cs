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



                    string pathFile = "D:\\Desktop\\test111111111";

                    // Excel.Application Excel_APP1= new Excel.Application();
                    // Excel._Workbook Excel_WB1 = Excel_APP1.Workbooks.Open(pathFile);
                    // Excel.Worksheet Excel_WS1 = new Excel.Worksheet();
                    // Excel_WS1 = Excel_WB1.Worksheets["11111"];


                    // Excel_APP1.Cells[3, 1] = "asdf";
                    // Excel_APP1.Cells[3, 2] = "qwe";

                    // Excel_WB1.Save();

                    // Excel_WS1 = null;
                    // Excel_WB1.Close();
                    // Excel_WB1 = null;
                    // Excel_WB1 = null;
                    // Excel_APP1 = null;


                    //// Excel_WB1.Save();

                    Excel.Application excelApp;
                    Excel._Workbook wBook;
                    Excel._Worksheet wSheet;
                    Excel.Range wRange;

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

                    try
                    {
                        // 引用第一個工作表
                        wSheet = (Excel._Worksheet)wBook.Worksheets[1];

                        // 命名工作表的名稱
                        wSheet.Name = "工作表測試";

                        // 設定工作表焦點
                        wSheet.Activate();

                        excelApp.Cells[1, 1] = "Excel測試";

                        // 設定第1列資料
                        excelApp.Cells[1, 1] = "名稱";
                        excelApp.Cells[1, 2] = "數量";
                        // 設定第1列顏色
                        wRange = wSheet.Range[wSheet.Cells[1, 1], wSheet.Cells[1, 2]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
                        wRange.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.DimGray);

                        // 設定第2列資料
                        excelApp.Cells[2, 1] = "AA";
                        excelApp.Cells[2, 2] = "10";

                        // 設定第3列資料
                        excelApp.Cells[3, 1] = "BB";
                        excelApp.Cells[3, 2] = "20";

                        // 設定第4列資料
                        excelApp.Cells[4, 1] = "CC";
                        excelApp.Cells[4, 2] = "30";

                        // 設定第5列資料
                        excelApp.Cells[5, 1] = "總計";
                        // 設定總和公式 =SUM(B2:B4)
                        excelApp.Cells[5, 2].Formula = string.Format("=SUM(B{0}:B{1})", 2, 4);
                        // 設定第5列顏色
                        wRange = wSheet.Range[wSheet.Cells[5, 1], wSheet.Cells[5, 2]];
                        wRange.Select();
                        wRange.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.Red);
                        wRange.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.Yellow);

                        // 自動調整欄寬
                        wRange = wSheet.Range[wSheet.Cells[1, 1], wSheet.Cells[5, 2]];
                        wRange.Select();
                        wRange.Columns.AutoFit();

                        try
                        {
                            //另存活頁簿
                            wBook.SaveAs(pathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                            Console.WriteLine("儲存文件於 " + Environment.NewLine + pathFile);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("產生報表時出錯！" + Environment.NewLine + ex.Message);
                        break;
                    }

                    //關閉活頁簿
                    wBook.Close(false, Type.Missing, Type.Missing);

                    //關閉Excel
                    excelApp.Quit();

                    //釋放Excel資源
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    wBook = null;
                    wSheet = null;
                    wRange = null;
                    excelApp = null;
                    GC.Collect();

                    Console.Read();
                    break;
                }
            }



        }
    }
}
