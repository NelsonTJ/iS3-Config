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
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjInfoWindow.xaml
    /// </summary>
    public partial class ProjInfoWindow : Window
    {
        PictureMarkerSymbol _pinMarkerSymbol = new PictureMarkerSymbol();

        public string ProjName;
        public double ProjLocX;
        public double ProjLocY;

        public ProjInfoWindow()
        {
            InitializeComponent();

            StepLB.SelectedIndex = 0;
            InitializePictureMarkerSymbol();
            MyMapView.MouseDown += MyMapView_MouseDown;
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

        private void StepLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int step = StepLB.SelectedIndex;
            if (step == 0)
            {
                prompt.Text = "Input project title in the title text box.";
                ProjectTitleTB.Visibility = System.Windows.Visibility.Visible;
            }
            else if (step == 1)
            {
                prompt.Text = "Drop the project location on the map.";
                ProjectTitleTB.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int step = StepLB.SelectedIndex;
            if (step == 0)
                return;

            GraphicsLayer gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;
            gLayer.Graphics.Clear();

            Point screenPoint = e.GetPosition(MyMapView);
            MapPoint loc = MyMapView.ScreenToLocation(screenPoint);
            Graphic g = new Graphic()
            {
                Geometry = new MapPoint(loc.X, loc.Y),
                Symbol = _pinMarkerSymbol,
            };
            gLayer.Graphics.Add(g);

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // finish 
            ProjName = ProjectTitleTB.Text;
            ProjLocX = 0;
            ProjLocY = 0;

            GraphicsLayer gLayer = Map.Layers["ProjectGraphicsLayer"] as GraphicsLayer;
            Graphic g = gLayer.Graphics.FirstOrDefault();
            if (g != null)
            {
                MapPoint loc = g.Geometry as MapPoint;
                if (loc != null)
                {
                    ProjLocX = loc.X;
                    ProjLocY = loc.Y;
                }
            }
        }

    }
}
