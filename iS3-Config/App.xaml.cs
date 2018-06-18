using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string iS3Path = "";
        public string myDataPath = "";

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            string exeLocation = Assembly.GetExecutingAssembly().Location;
            string exePath = System.IO.Path.GetDirectoryName(exeLocation);

            iS3Path = exePath;
            myDataPath = iS3Path + "\\Data";

            if (!Directory.Exists(myDataPath))
                myDataPath = iS3Path;
        }
    }
}
