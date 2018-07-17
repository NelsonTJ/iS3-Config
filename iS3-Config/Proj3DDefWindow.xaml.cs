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
using System.Windows.Shapes;
using IS3.Unity.Webplayer;
using IS3.Core;
using IS3.Unity.Webplayer.UnityCore;

namespace iS3.Config
{
    /// <summary>
    /// Proj3DDefWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Proj3DDefWindow : Window
    {
        ProjectDefinition _projdef;
        public UnityLayer unityLayer;
        public void UnityLayerListener(object sender, UnityLayer unityLayer)
        {
            this.unityLayer = unityLayer;
            treeView.ItemsSource = unityLayer.UnityLayerModel.childs;
        }
        public Proj3DDefWindow(ProjectDefinition projDef)
        {
            InitializeComponent();
            _projdef = projDef;
        }

        private void import3D_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = ".unity3d(*.*)|*.unity3d";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                u3dTB.Text = file;
                EngineeringMap map = new EngineeringMap();
                map.LocalMapFileName = dialog.SafeFileName;

                U3DView u3DView = new U3DView(new Project() { projDef = _projdef }, map);
                u3DView.UnityLayerHanlder += UnityLayerListener;
                ViewHolder.Children.Add(u3DView);
                u3DView.view.loadPredefinedLayers();


            }
        }
        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            
            // finish 
            DialogResult = true;
            Close();
        }
    }
}
