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
using System.IO;
using System.Xml.Linq;
using System.Windows.Markup;
using System.Windows.Forms;
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjGnrDefWindow.xaml
    /// </summary>
    public partial class ProjGnrDefWindow : Window
    {
        string _projID = "";
        string _dataPath = "";

        public ProjectDefinition ProjDef;

        public ProjGnrDefWindow(string projID, string dataPath)
        {
            InitializeComponent();

            _projID = projID;
            _dataPath = dataPath;

            ProjDef = ConfigCore.LoadProjectDefinition(_dataPath, _projID);
            if (ProjDef == null)
                ProjDef = ConfigCore.CreateProjectDefinition(_dataPath, _projID);

            GeneralGrd.DataContext = ProjDef;
        }

        private void PathBtn_Click(object sender, RoutedEventArgs e)
        {
            string path;
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            if (btn.Name == "LocalPathBtn")
                path = ProjDef.LocalFilePath;
            else
                path = ProjDef.LocalTilePath;

            if (!Directory.Exists(path))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                catch (Exception error)
                {
                    System.Windows.MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                }
            }

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = path;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (btn.Name == "LocalPathBtn")
                    ProjDef.LocalFilePath = dialog.SelectedPath;
                else
                    ProjDef.LocalTilePath = dialog.SelectedPath;
                
                // refresh UI
                GeneralGrd.DataContext = null;
                GeneralGrd.DataContext = ProjDef;
            }
        }

        private void LocalDbBtn_Click(object sender, RoutedEventArgs e)
        {
            string file = ProjDef.LocalDatabaseName;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = ProjDef.LocalFilePath;
            dialog.FileName = file;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ProjDef.LocalDatabaseName = dialog.SafeFileName;

                // refresh UI
                GeneralGrd.DataContext = null;
                GeneralGrd.DataContext = ProjDef;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // finish 
            DialogResult = true;
            Close();
        }

    }
}
