using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ReactiveUI;


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

            this.Bind(ViewModel, vm => vm.LicenseServer, v => v.LicenseServer.Text);
            this.Bind(ViewModel, vm => vm.LicensePort, v => v.LicensePort.Text);
            this.Bind(ViewModel, vm => vm.LicenseTypeIndex, v => v.LicenseType.SelectedIndex);
            this.Bind(ViewModel, vm => vm.IsNetworkLicense, v => v.LicenseServer.IsEnabled);
            this.Bind(ViewModel, vm => vm.IsNetworkLicense, v => v.LicensePort.IsEnabled);
            this.Bind(ViewModel, vm => vm.IsLocalLicense, v => v.LocalLicenseFile.IsEnabled);
            this.Bind(ViewModel, vm => vm.LocalLicenseFile, v => v.LocalLicenseFile.Text);

            this.LicenseType.SelectedValue = System.Environment.GetEnvironmentVariable("LSTC_LICENSE");
            this.LicenseServer.Text = System.Environment.GetEnvironmentVariable("LSTC_LICENSE_SERVER");
            this.LicensePort.Text = System.Environment.GetEnvironmentVariable("LSTC_LICENSE_SERVER_PORT");
            this.LocalLicenseFile.Text = System.Environment.GetEnvironmentVariable("LSTC_FILE");        
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
