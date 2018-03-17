using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Nart
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private void LaunchApplication(object sender, StartupEventArgs e)
        {
            string filename = string.Empty;

            //使用command line同時開啟的化，只開最後一個.nart附檔名的檔案
            foreach (string temp in e.Args)
            {
                if (System.IO.File.Exists(temp) && System.IO.Path.GetExtension(temp).ToLower() == ".nart")
                {
                    filename = temp;
                }
            }

            
            MainView mainView = new MainView();
           
            if (filename != string.Empty)
            {
                mainView.MainViewModel.ImportProject(filename);
            }
            mainView.Show();

            
        }
    }
}
