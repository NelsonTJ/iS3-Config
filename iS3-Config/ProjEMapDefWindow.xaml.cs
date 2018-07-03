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
    /// Interaction logic for ProjEMapDefWindow.xaml
    /// </summary>
    public partial class ProjEMapDefWindow : Window
    {
        ProjectDefinition ProjDef;

        public ProjEMapDefWindow(ProjectDefinition projDef)
        {
            InitializeComponent();

            ProjDef = projDef;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
