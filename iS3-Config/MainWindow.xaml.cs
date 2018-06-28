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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App app;

        public MainWindow()
        {
            InitializeComponent();

            app = App.Current as App;

            iS3Labl.Content = app.iS3Path;
            myLabl.Content = app.myDataPath;
        }


        private void iS3LocBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = app.iS3Path;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                app.iS3Path = dialog.SelectedPath;
                iS3Labl.Content = app.iS3Path;

                app.myDataPath = app.iS3Path + "\\Data";
                if (!Directory.Exists(app.myDataPath))
                    app.myDataPath = app.iS3Path;
                myLabl.Content = app.myDataPath;
            }
        }

        private void myDatBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = app.myDataPath;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                app.myDataPath = dialog.SelectedPath;
                myLabl.Content = app.myDataPath;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
