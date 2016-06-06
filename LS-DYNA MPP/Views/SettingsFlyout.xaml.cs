using MahApps.Metro.Controls;
using ReactiveUI;
using System.Windows;

namespace Predictive.Lsdyna.Mpp.Views
{
    /// <summary>
    /// Interaction logic for SettingsFlyout.xaml
    /// </summary>
    public partial class SettingsFlyout : Flyout, IViewFor<SettingsViewModel>
    {
        public SettingsFlyout()
        {
            ViewModel = new SettingsViewModel();
            DataContext = ViewModel;
            InitializeComponent();

            //Bind View elements to ViewModel
            this.Bind(ViewModel, vm => vm.LicenseServer.Value, v => v.LicenseServer.Text);
            this.Bind(ViewModel, vm => vm.LicensePort.Value, v => v.LicensePort.Text);
            this.Bind(ViewModel, vm => vm.LicenseTypeIndex, v => v.LicenseType.SelectedIndex);
            this.Bind(ViewModel, vm => vm.IsNetworkLicense, v => v.LicenseServer.IsEnabled);
            this.Bind(ViewModel, vm => vm.IsNetworkLicense, v => v.LicensePort.IsEnabled);
            this.Bind(ViewModel, vm => vm.IsLocalLicense, v => v.LicenseFile.IsEnabled);
            this.Bind(ViewModel, vm => vm.LicenseFile.Value, v => v.LicenseFile.Text);
            this.Bind(ViewModel, vm => vm.LicenseType.Value, v => v.LicenseType.SelectedValue);
            this.Bind(ViewModel, vm => vm.LSTCPath, v => v.LSTCPath.Text);
            this.Bind(ViewModel, vm => vm.SpinnerVisibility, v => v.ImportProgress.Visibility);

            // load saved settings
            ViewModel.LSTCPath = Properties.Settings.Default.LSTC_PATH;
            ViewModel.LicenseType.Value = Properties.Settings.Default.LSTC_LICENSE;
            ViewModel.LicenseServer.Value = Properties.Settings.Default.LSTC_LICENSE_SERVER;
            ViewModel.LicensePort.Value = Properties.Settings.Default.LSTC_LICENSE_SERVER_PORT;
            ViewModel.LicenseFile.Value = Properties.Settings.Default.LSTC_FILE;

            // Bind commands to buttons
            this.BindCommand(ViewModel, vm => vm.ImportLicense, v => v.ImportLicenseButton);
            this.BindCommand(ViewModel, vm => vm.StartLicenseManager, v => v.StartLicenseManagerButton);
            this.BindCommand(ViewModel, vm => vm.StopLicenseManager, v => v.StopLicenseManagerButton);
            this.BindCommand(ViewModel, vm => vm.LaunchLicenseManager, v => v.LaunchLicenseManagerButton);
        }

        public SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SettingsViewModel), typeof(SettingsFlyout), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SettingsViewModel)value; }
        }
    }
}
