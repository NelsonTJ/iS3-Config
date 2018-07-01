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
using System.Diagnostics;
using System.IO;
using System.Windows.Markup;
using System.Xml.Serialization;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjInfoWindow.xaml
    /// </summary>
    public partial class ProjInfoWindow : Window
    {
        public string ProjName = "";
        public double ProjLocX = 0;
        public double ProjLocY = 0;

        string _projListFile;
        ProjectList _projList;
        GraphicsLayer _gLayer;
        PictureMarkerSymbol _pinMarkerSymbol = new PictureMarkerSymbol();

        public ProjInfoWindow(string projListFile)
        {
            InitializeComponent();

            _projListFile = projListFile;

            InitializePictureMarkerSymbol();
            MyMapView.Loaded += MyMapView_Loaded;
            MyMapView.MouseDown += MyMapView_MouseDown;

            LoadProjectList();
            if (_projList == null)
                return;
            foreach (ProjectLocation loc in _projList.Locations)
            {
                ProjectListLB.Items.Add(loc);
            }
        }

        private async void InitializePictureMarkerSymbol()
        {
            try
            {
                await _pinMarkerSymbol.SetSourceAsync(
                    new Uri("pack://application:,,,/IS3.Config;component/Images/pin_red.png"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void LoadProjectList()
        {
            try
            {
                StreamReader reader = new StreamReader(_projListFile);
                object obj = XamlReader.Load(reader.BaseStream);
                _projList = obj as ProjectList;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        void WriteProjectList()
        {
            try
            {
                //Stream writer = new FileStream(_projListFile + "1", FileMode.Create);
                //XmlSerializer s = new XmlSerializer(_projList.GetType());
                //s.Serialize(writer, _projList);
                //writer.Close();

                StreamWriter writer = new StreamWriter(_projListFile + "1");

                string s = "";
                foreach (ProjectLocation loc in _projList.Locations)
                    s += XamlWriter.Save(loc) + "\n";


            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            _gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;
            if (_projList == null)
                return;
            foreach (ProjectLocation loc in _projList.Locations)
            {
                AddProjectLocationOnMap(loc);
            }
        }

        void AddProjectLocationOnMap(ProjectLocation loc)
        {
            Graphic g = new Graphic()
            {
                Geometry = new MapPoint(loc.X, loc.Y),
                Symbol = _pinMarkerSymbol,
            };
            g.Attributes["ID"] = loc.ID;
            g.Attributes["DefinitionFile"] = loc.DefinitionFile;
            g.Attributes["Description"] = loc.Description;

            _gLayer.Graphics.Add(g);
        }

        private void ProjectListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
            {
                ProjectTitleTB.Visibility = Visibility.Hidden;
                _gLayer.Graphics.Clear();
                return;
            }

            ProjectTitleTB.Visibility = Visibility.Visible;
            ProjectTitleTB.Text = loc.Description;
            ProjectTitleTB.IsReadOnly = true;

            _gLayer.Graphics.Clear();
            AddProjectLocationOnMap(loc);

            PromptTB.Text = "Select operations from the configuration.";
            StepLB.SelectedIndex = -1;
        }

        private void StepLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step == 0)
            {
                PromptTB.Text = "Input project title in the title text box.";
                ProjectTitleTB.IsReadOnly = false;
                ProjectTitleTB.Focus();
                ProjectTitleTB.SelectAll();
                ProjectTitleTB.Visibility = System.Windows.Visibility.Visible;
                _gLayer.Graphics.Clear();
            }
            else if (step == 1)
            {
                PromptTB.Text = "Drop the project location on the map.";
                ProjectTitleTB.Visibility = System.Windows.Visibility.Hidden;
                AddProjectLocationOnMap(loc);
            }
        }

        private void ProjectTitleTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            loc.Description = ProjectTitleTB.Text;
        }

        void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            int step = StepLB.SelectedIndex;
            if (step != 1)
                return;

            Point screenPoint = e.GetPosition(MyMapView);
            MapPoint coord = MyMapView.ScreenToLocation(screenPoint);
            _gLayer.Graphics.Clear();

            loc.X = coord.X;
            loc.Y = coord.Y;
            AddProjectLocationOnMap(loc);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddProjWindow addProjWnd = new AddProjWindow();
            addProjWnd.Owner = this;
            addProjWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = addProjWnd.ShowDialog();
            if (ok!=null && ok.Value==true)
            {
                ProjectLocation loc = new ProjectLocation();
                loc.ID = addProjWnd.IdTB.Text;
                loc.Description = addProjWnd.DescTB.Text;
                loc.DefinitionFile = loc.ID + ".xml";
                loc.X = 0;
                loc.Y = 0;
                _projList.Locations.Add(loc);

                ProjectListLB.Items.Add(loc);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ProjectLocation loc = ProjectListLB.SelectedItem as ProjectLocation;
            if (loc == null)
                return;

            _projList.Locations.Remove(loc);
            ProjectListLB.Items.Remove(loc);

            ProjectListLB.SelectedIndex = -1;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            WriteProjectList();
            // finish 
            DialogResult = true;
            Close();
        }

    }
}
