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
using System.Windows.Navigation;
using System.Windows.Shapes;

using IS3.Core;

namespace iS3.Config
{
    /// <summary>
    /// Interaction logic for TreeUserControl.xaml
    /// </summary>
    public partial class TreeUserControl : UserControl
    {
        public TreeUserControl(Tree tree)
        {
            InitializeComponent();

            MyTreeView.ItemsSource = tree.Children;
        }

        public event EventHandler<object> OnTreeSelected;

        private void MyTreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (OnTreeSelected != null)
                OnTreeSelected(this, e.NewValue);
        }
    }
}
