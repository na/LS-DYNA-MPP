using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Predictive.Lsdyna.Mpp.Controls
{
    /// <summary>
    /// Interaction logic for FileBrowseTextBox.xaml
    /// </summary>
    public partial class FileBrowseTextBox : UserControl
    {
        public FileBrowseTextBox()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileBrowseTextBox), new PropertyMetadata(""));

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(FileBrowseTextBox), new PropertyMetadata(""));

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = this.SelectedPath;
            dialog.CheckFileExists = false;
            var result = dialog.ShowDialog();
            if ((result.HasValue) && (result.Value))
            {
                this.SelectedPath = dialog.FileName;
            }
        }
    }
}
