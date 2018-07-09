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
using System.Data;
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for DomainDefWindow.xaml
    /// </summary>
    public partial class DomainDefWindow : Window
    {
        ProjectDefinition _prjDef;
        Project _prj;
        List<EMapLayers> _eMapLayersList;

        public DomainDefWindow(ProjectDefinition prjDef, Project prj,
            List<EMapLayers> eMapLayersList)
        {
            InitializeComponent();

            _prjDef = prjDef;
            _prj = prj;
            _eMapLayersList = eMapLayersList;
            DomainListLB.ItemsSource = prj.domains;
            Loaded += DomainDefWindow_Loaded;
        }

        private void DomainDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_prj.domains.Count > 0)
                DomainListLB.SelectedIndex = 0;

            List<string> types = ObjectTypeHelper.GetDObjectTypes();
            TypeCB.ItemsSource = types;
        }

        private void DomainListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear DObjsLB at first
            DObjsLB.ItemsSource = null;

            KeyValuePair<string, Domain> item = (KeyValuePair<string, Domain>)DomainListLB.SelectedItem;
            Domain domain = _prj.domains[item.Key];
            Dictionary<string, DGObjectsDefinition> objsDef = domain.objsDefinitions;
            DObjsLB.ItemsSource = objsDef;

            if (domain.objsDefinitions.Count > 0)
                DObjsLB.SelectedIndex = 0;
        }

        private void DObjsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if this is triggered by clearing event
            if (DObjsLB.ItemsSource == null)
            {
                DObjsDefGrid.DataContext = null;
                return;
            }

            KeyValuePair<string, DGObjectsDefinition> item =
                (KeyValuePair<string, DGObjectsDefinition>)DObjsLB.SelectedItem;
            DGObjectsDefinition DObjsDef = item.Value;
            DObjsDefGrid.DataContext = DObjsDef;
        }

        private void TableNameSQLBtn_Click(object sender, RoutedEventArgs e)
        {
            string dbFile = _prjDef.LocalFilePath + "\\" + _prjDef.LocalDatabaseName;
            List<string> names = DbHelper.GetDbTablenames(dbFile);

            SelectTableNamesWindow selectTableNamesWnd = new SelectTableNamesWindow(names, TableNameTB.Text);
            selectTableNamesWnd.Owner = this;
            selectTableNamesWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = selectTableNamesWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                TableNameTB.Text = selectTableNamesWnd.SelectedName;
                DGObjectsDefinition DObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
                DObjsDef.TableNameSQL = selectTableNamesWnd.SelectedName;
            }
        }
        private void PreviewTableBtn_Click(object sender, RoutedEventArgs e)
        {
            string dbFile = _prjDef.LocalFilePath + "\\" + _prjDef.LocalDatabaseName;
            DGObjectsDefinition dObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
            DataSet dataSet = DbHelper.LoadTable(dbFile, 
                dObjsDef.TableNameSQL, dObjsDef.ConditionSQL, dObjsDef.OrderSQL);

            PreviewTableWindow previewTblWnd = new PreviewTableWindow(TableNameTB.Text, dataSet);
            previewTblWnd.Owner = this;
            previewTblWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            previewTblWnd.ShowDialog();
        }

        private void TwoDimLayerBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectEMapLayersWindow selectEMapLayersWnd = new SelectEMapLayersWindow(_eMapLayersList);
            selectEMapLayersWnd.Owner = this;
            selectEMapLayersWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? ok = selectEMapLayersWnd.ShowDialog();
            if (ok != null && ok.Value == true)
            {
                if (selectEMapLayersWnd.SelectedLayerName != null)
                {
                    LayerNameTB.Text = selectEMapLayersWnd.SelectedLayerName;
                    DGObjectsDefinition DObjsDef = DObjsDefGrid.DataContext as DGObjectsDefinition;
                    DObjsDef.GISLayerName = selectEMapLayersWnd.SelectedLayerName;
                }
            }
        }

        private void ThreeDimLayerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // finish 
            DialogResult = true;
            Close();
        }

    }
}
