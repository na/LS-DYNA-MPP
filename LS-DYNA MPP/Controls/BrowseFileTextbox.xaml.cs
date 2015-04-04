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

namespace Predictive.Lsdyna.Mpp.Controls
{
    /// <summary>
    /// Interaction logic for BrowseFileTextbox.xaml
    /// </summary>
    public partial class BrowseFileTextbox : UserControl
    {


        public BrowseFileTextbox()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public string WatermarkLabel
        {
            get { return (string)GetValue(WatermarkLabelProperty); }
            set { SetValue(WatermarkLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkLabelProperty =
            DependencyProperty.Register("WatermarkLabel", typeof(string), typeof(BrowseFileTextbox), new PropertyMetadata(string.Empty));
    }
}
