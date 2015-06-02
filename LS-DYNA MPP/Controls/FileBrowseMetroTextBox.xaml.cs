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
using ReactiveUI;
using Predictive.Lsdyna.Mpp.ViewModels;
using Microsoft.Win32;

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
            DataContext = this;
            
            BrowseFile = ReactiveCommand.Create();
            
            // file dialog 
            var dlg = new OpenFileDialog();

            this.BrowseFile.Subscribe(_ => {
                dlg.FileName = FilePath.Text;
                var result = dlg.ShowDialog();
                if (result == true)
                {
                    this.FilePath.Text = dlg.FileName;
                    dlg.CheckFileExists = false;
                }
            });

        }

        public ReactiveCommand<object> BrowseFile { get; protected set; }

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
