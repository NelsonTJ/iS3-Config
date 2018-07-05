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
    public class GdbLayer
    {
        public string Name { get; set; }
        public bool Visibility { get; set; }
        public GeodatabaseFeatureTable FeatureTable { get; set; }
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
                EMapChanged(firstEMap);
            }
        }

        void EMapChanged(EngineeringMap emap)
        {
            // refresh UI
            EMapGrd.DataContext = null;
            EMapGrd.DataContext = emap;
            if (emap.MapType == EngineeringMapType.FootPrintMap)
                EMapTypeCB.SelectedIndex = 0;
            else if (emap.MapType == EngineeringMapType.GeneralProfileMap)
                EMapTypeCB.SelectedIndex = 1;
            else if (emap.MapType == EngineeringMapType.Map3D)
                EMapTypeCB.SelectedIndex = 2;
        }

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

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            EngineeringMap emap = EMapsLB.SelectedItem as EngineeringMap;
            if (emap != null)
            {
                LoadTiledLayer1(emap);
                LoadGeoDb(emap);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            EngineeringMap map = ProjDef.EngineeringMaps.FirstOrDefault();
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
                emap.LocalGeoDbFileName = dialog.SafeFileName;
                LoadGeoDb(emap);

                // refresh UI
                EMapGrd.DataContext = null;
                EMapGrd.DataContext = emap;
            }

        }

        async void LoadGeoDb(EngineeringMap emap)
        {
            string file = ProjDef.LocalFilePath + "\\" + emap.LocalGeoDbFileName;
            if (File.Exists(file))
            {
                Geodatabase gdb = await Geodatabase.OpenAsync(file);
                IEnumerable<GeodatabaseFeatureTable> featureTables =
                    gdb.FeatureTables;
                List<GdbLayer> gdbLayers = new List<GdbLayer>();
                foreach (var table in featureTables)
                {
                    GdbLayer layer = new GdbLayer();
                    layer.Name = table.Name;
                    layer.Visibility = false;
                    layer.FeatureTable = table;
                    gdbLayers.Add(layer);

                    LayerDef lyrDef = emap.LocalGdbLayersDef.Find(x => x.Name == table.Name);
                    if (lyrDef == null)
                    {
                        lyrDef = GdbHelper.GenerateDefaultLayerDef(table);
                        emap.LocalGdbLayersDef.Add(lyrDef);
                    }
                    await GdbHelper.addGeodatabaseLayer(Map, lyrDef, table);
                }

                GeoDBLayrList.ItemsSource = gdbLayers;
            }
        }

        private void LayerCB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chkBox = sender as System.Windows.Controls.CheckBox;

            if (chkBox.IsChecked.Value)
            { }
        }
    }
}
