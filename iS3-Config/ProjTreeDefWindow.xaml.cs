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
using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for ProjTreeDefWindow.xaml
    /// </summary>
    public partial class ProjTreeDefWindow : Window
    {
        Project _prj;

        public ProjTreeDefWindow(Project prj)
        {
            InitializeComponent();

            _prj = prj;
            Loaded += ProjTreeDefWindow_Loaded;
        }

        private void ProjTreeDefWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Domain dm in _prj.domains.Values)
            {
                TreeUserControl treeCtrl = new TreeUserControl(dm.root);
                treeCtrl.OnTreeSelected += TreeCtrl_OnTreeSelected;

                TabItem tab = new TabItem();
                tab.Header = dm.name;
                TreeTabHolder.Items.Add(tab);
                tab.Content = treeCtrl;
            }

            if (_prj.domains.Count > 0)
                TreeTabHolder.SelectedIndex = 0;
        }

        private void TreeTabHolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = TreeTabHolder.SelectedItem as TabItem;
            string name = tab.Header as string;
            Domain domain = _prj.domains[name];

            DObjsLB.ItemsSource = domain.objsDefinitions.Keys;
        }

        private void TreeCtrl_OnTreeSelected(object sender, object e)
        {
            Tree tree = e as Tree;
            TreeItemGrid.DataContext = tree;
        }

        private void DomainListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear DObjsLB at first
            DObjsLB.ItemsSource = null;

            string name = "";
            Domain domain = _prj.domains[name];
            Dictionary<string, DGObjectsDefinition> objsDef = domain.objsDefinitions;
            DObjsLB.ItemsSource = objsDef;

            if (domain.objsDefinitions.Count > 0)
                DObjsLB.SelectedIndex = 0;
        }


        private void DObjsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
