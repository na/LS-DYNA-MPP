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
        public static readonly DependencyProperty WatermarkProperty = 
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(BrowseFileTextbox));

        public BrowseFileTextbox()
        {
            InitializeComponent();
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
    }
}
