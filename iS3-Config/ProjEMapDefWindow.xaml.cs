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
using System.Windows.Forms;
using System.IO;
using IS3.Core;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Data;

namespace iS3.Config
{
    // GdbLayer is used for UI binding and selection operations.
    // The first three members are for UI binding.
    // The last two members are for selection operations.
    //
    public class GdbLayer
    {
        public string Name { get; set; }
        public bool Visibility { get; set; }
        public GdbLayer LayerObject { get; set; }
        public GeodatabaseFeatureTable FeatureTable { get; set; }
        public Esri.ArcGISRuntime.Geometry.Envelope Extent { get; set; }
    }

    /// <summary>
    /// Interaction logic for ProjEMapDefWindow.xaml
    /// </summary>
    public partial class ProjEMapDefWindow : Window
    {
        ProjectDefinition ProjDef;

        public ProjEMapDefWindow(ProjectDefinition projDef)
        {
            InitializeComponent();

            ProjDef = projDef;
            foreach (EngineeringMap emap in projDef.EngineeringMaps)
                EMapsLB.Items.Add(emap);

            MyMapView.Loaded += MyMapView_Loaded;
            Loaded += ProjEMapDefWindow_Loaded;
        }

        private void ProjEMapDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EngineeringMap firstEMap = ProjDef.EngineeringMaps.FirstOrDefault();
            if (firstEMap != null)
            {
                EMapsLB.SelectedIndex = 0;
                // refresh UI
                EMapGrd.DataContext = firstEMap;
            }
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap != null)
            {
                LoadTiledLayer1(emap);
                ReloadGeoDb(emap);
            }
        }

        private void LocalTileBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            string file = emap.LocalTileFileName1;
            string path = ProjDef.LocalTilePath;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path;
            dialog.Filter = "Tile Packages (.tpk)|*.tpk";
            dialog.FileName = file;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                emap.LocalTileFileName1 = dialog.SafeFileName;
                LoadTiledLayer1(emap);

                // refresh UI
                EMapGrd.DataContext = null;
                EMapGrd.DataContext = emap;
            }
        }

        private void LocalGeoDBBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            string file = emap.LocalTileFileName1;
            string path = ProjDef.LocalFilePath;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path;
            dialog.Filter = "Geo-Database(.geodatabase)|*.geodatabase";
            dialog.FileName = file;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Load new Local GeoDB layers
                emap.LocalGeoDbFileName = dialog.SafeFileName;
                ReloadGeoDb(emap);

                // refresh UI
                EMapGrd.DataContext = null;
                EMapGrd.DataContext = emap;
            }

        }

        private void LayerCB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chkBox = sender as System.Windows.Controls.CheckBox;

            // Click => Switch the layer on/off
            GdbLayer gdbLyr = chkBox.Tag as GdbLayer;
            Layer lyr = Map.Layers[gdbLyr.Name];
            lyr.IsVisible = chkBox.IsChecked.Value;

            // Update layer definition
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;
            LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == gdbLyr.Name);
            if (lyrDef == null)
                return;
            lyrDef.IsVisible = chkBox.IsChecked.Value;
        }

        private void LayrCB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.CheckBox chkBox = sender as System.Windows.Controls.CheckBox;

            // Double click => Zoom to the layer
            GdbLayer gdbLayer = chkBox.Tag as GdbLayer;
            MyMapView.SetView(gdbLayer.Extent);
        }

        private void AddEMap_Click(object sender, RoutedEventArgs e)
        {
            // new emap
            EngineeringMap emap = new EngineeringMap();
            emap.MapID = "Map" + ProjDef.EngineeringMaps.Count.ToString();
            emap.MapType = EngineeringMapType.FootPrintMap;

            // update project definition
            ProjDef.EngineeringMaps.Add(emap);

            // refresh UI
            EMapsLB.Items.Add(emap);
            EMapsLB.SelectedItem = emap;
            EMapGrd.DataContext = null;
            EMapGrd.DataContext = emap;

            // refresh map
            ReloadMap(emap);
        }

        private void RemoveEMap_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            // Update project definition
            ProjDef.EngineeringMaps.Remove(emap);

            // Refresh UI
            EMapsLB.Items.Remove(emap);
            EMapGrd.DataContext = null;

            // Refresh map
            ReloadMap(null);
        }

        private void EMapsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;

            // refresh UI
            EMapGrd.DataContext = null;
            EMapGrd.DataContext = emap;

            // refresh map
            ReloadMap(emap);
        }

        private async void LyrSetting_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap == null)
                return;

            GdbLayer gdbLyr = GeoDBLayrLB.SelectedItem as GdbLayer;
            if (gdbLyr == null)
                return;

            LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == gdbLyr.Name);

            LayerDefWindow lyrDefWnd = new LayerDefWindow(lyrDef);
            lyrDefWnd.Owner = this;
            lyrDefWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? success = lyrDefWnd.ShowDialog();
            if (success != null && success.Value == true)
            {
                Layer lyr = Map.Layers[gdbLyr.Name];
                if (lyr != null)
                {
                    // Reload the layer
                    Map.Layers.Remove(lyr);
                    await GdbHelper.addGeodatabaseLayer(Map, lyrDef, gdbLyr.FeatureTable);
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // finish 
            DialogResult = true;
            Close();
        }

        // Load ArcGISLocalTiledLayer for the specified engineering map
        //
        void LoadTiledLayer1(EngineeringMap emap)
        {
            ArcGISLocalTiledLayer tileLayr1 = Map.Layers["TiledLayer1"] as ArcGISLocalTiledLayer;
            if (tileLayr1 != null)
            {
                Map.Layers.Remove(tileLayr1);
            }

            string file = ProjDef.LocalTilePath + "\\" + emap.LocalTileFileName1;
            if (File.Exists(file))
            {
                ArcGISLocalTiledLayer newLayr = new ArcGISLocalTiledLayer(file);
                newLayr.ID = "TiledLayer1";
                newLayr.DisplayName = "TileLayer1";
                Map.Layers.Add(newLayr);

                MyMapView.SetView(newLayr.FullExtent);
            }
        }

        // Reload the specified engineering map, clear the map view at first.
        //
        void ReloadMap(EngineeringMap emap)
        {
            Map.Layers.Clear();
            if (emap != null)
            {
                LoadTiledLayer1(emap);
                ReloadGeoDb(emap);
            }
        }

        // Load the specifiled emap's geodatabase, and add all the features layers to the map.
        //
        async void ReloadGeoDb(EngineeringMap emap)
        {
            // Clear existing Local GeoDB layers
            if (GeoDBLayrLB.ItemsSource != null)
            {
                foreach (var item in GeoDBLayrLB.ItemsSource)
                {
                    GdbLayer lyr = item as GdbLayer;
                    Layer mapLyr = Map.Layers[lyr.Name];
                    if (mapLyr != null)
                        Map.Layers.Remove(mapLyr);
                }
                GeoDBLayrLB.ItemsSource = null;
            }

            // Load new
            string file = ProjDef.LocalFilePath + "\\" + emap.LocalGeoDbFileName;
            if (File.Exists(file))
            {
                // Open geodatabase
                Geodatabase gdb = await Geodatabase.OpenAsync(file);
                IEnumerable<GeodatabaseFeatureTable> featureTables =
                    gdb.FeatureTables;
                List<GdbLayer> gdbLayers = new List<GdbLayer>();
                foreach (var table in featureTables)
                {
                    // GdbLayer is used for UI binding and selection operations.
                    GdbLayer layer = new GdbLayer();
                    layer.Name = table.Name;
                    layer.Visibility = true;
                    layer.LayerObject = layer;
                    layer.FeatureTable = table;
                    layer.Extent = table.Extent;
                    gdbLayers.Add(layer);

                    // Search LayerDef, use default if not found.
                    LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == table.Name);
                    if (lyrDef == null)
                    {
                        lyrDef = GdbHelper.GenerateDefaultLayerDef(table);
                        emap.LocalGdbLayersDef.Add(lyrDef);
                    }

                    // Add the feature layer to the map
                    await GdbHelper.addGeodatabaseLayer(Map, lyrDef, table);
                }

                // Refresh UI
                GeoDBLayrLB.ItemsSource = gdbLayers;
            }
        }

    }
}
