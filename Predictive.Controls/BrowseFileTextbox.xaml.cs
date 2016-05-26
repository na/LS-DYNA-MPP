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
using Microsoft.Win32;

namespace Predictive.Controls
{
    /// <summary>
    /// Interaction logic for AdvancedOptionsInputFile.xaml
    /// </summary>
    public partial class BrowseFileTextbox : UserControl
    {
        public BrowseFileTextbox()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            // file dialogs 
            var dlg = new OpenFileDialog();
            dlg.FileName = this.Value;
            var result = dlg.ShowDialog();
            if (result == true)
            {
                this.Value = dlg.FileName;
            }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(BrowseFileTextbox), new PropertyMetadata(""));

        //public bool IsEnabled
        //{
        //    get { return (bool)GetValue(IsEnabledProperty); }
        //    set { SetValue(IsEnabledProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsEnabledProperty =
        //    DependencyProperty.Register("IsEnabled", typeof(bool), typeof(BrowseFileTextbox), new PropertyMetadata(false));
    }
}
