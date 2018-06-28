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
        int step = 1;
        PictureMarkerSymbol _pinMarkerSymbol = new PictureMarkerSymbol();

        public string ProjName;
        public double ProjLocX;
        public double ProjLocY;

        public ProjInfoWindow()
        {
            InitializeComponent();

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

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (step == 1)
            {
                prompt.Text = ">>>Step 2 of 2: Drop the project location on the map.";
                NextBtn.Content = "Finish";
                step = 2;
                return;
            }
            if (step == 2)
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

        void MyMapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (step == 1)
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

    }
}
