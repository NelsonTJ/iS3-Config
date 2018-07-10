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
        }

        private void DomainListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DObjsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
