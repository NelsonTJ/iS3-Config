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

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for Proj3DViewDefWindow.xaml
    /// </summary>
    public partial class Proj3DViewDefWindow : Window
    {
        string _dataPath;
        string _projID;

        string _u3dFile;
        string _u3dFilePath;

        public Proj3DViewDefWindow(string dataPath, string projID)
        {
            InitializeComponent();

            _dataPath = dataPath;
            _projID = projID;
            _u3dFile = _projID + ".unity3d";
            _u3dFilePath = dataPath + "\\" + projID + "\\" + _u3dFile;

            Model3DTB.Text = _u3dFile;

            Loaded += Proj3DViewDefWindow_Loaded;
        }

        private void Proj3DViewDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_u3dFilePath))
            {
                u3dPlayerControl.LoadScence(_u3dFilePath);
            }
            else
            {
                PromptTB.Text = "The 3d model file does not exist.";
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
