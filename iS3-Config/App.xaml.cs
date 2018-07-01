using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string iS3Path = "";
        string dataPath = "";

        string projName = "";
        double projLocX = 0;
        double projLocY = 0;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // open a background window that start the configuration
            Window backgroundWnd = new Window();
            this.MainWindow = backgroundWnd;

            bool ok = StartConfig();
            if (ok)
            {
                // do something
            }
            else
            {

            }

            Shutdown();
        }

        bool StartConfig()
        {
            bool? success;

            ConfPathWindow mainWnd = new ConfPathWindow();
            success = mainWnd.ShowDialog();
            if (success == null || success.Value == false)
            {
                return false;
            }
            iS3Path = mainWnd.ExePath;
            dataPath = mainWnd.DataPath;

            ProjInfoWindow infoWnd = new ProjInfoWindow();
            success = infoWnd.ShowDialog();
            if (success == null || success.Value == false)
            {
                return false;
            }
            projName = infoWnd.ProjName;
            projLocX = infoWnd.ProjLocX;
            projLocY = infoWnd.ProjLocY;

            return true;
        }
    }
}
