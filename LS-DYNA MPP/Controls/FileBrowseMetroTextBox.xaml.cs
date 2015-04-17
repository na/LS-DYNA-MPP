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
    /// Interaction logic for FileBrowseTextBox.xaml
    /// </summary>
    public partial class FileBrowseMetroTextBox : UserControl
    {
        public FileBrowseMetroTextBox()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(FileBrowseMetroTextBox), new PropertyMetadata(""));
    }
}
