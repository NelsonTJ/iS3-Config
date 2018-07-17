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
using System.IO;

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
                //移动目录
                MoveFile(_projdef.LocalFilePath + "\\" + dialog.SafeFileName, file);
                //新建emap
                EngineeringMap map = new EngineeringMap();
                map.LocalMapFileName = dialog.SafeFileName;

                //添加三维视图-unitywebplayer
                U3DView u3DView = new U3DView(new Project() { projDef = _projdef }, map);
                ViewHolder.Children.Add(u3DView);
                //添加图层获取事件
                u3DView.UnityLayerHanlder += UnityLayerListener;
                u3DView.view.loadPredefinedLayers();
            }
        }
        //判断工程目录下是否存在文件，若不存在，则复制到工程目录下
        public void MoveFile(string aimPath,string nowPath)
        {
            if (!File.Exists(aimPath))
            {
                File.Copy(nowPath, aimPath);
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
