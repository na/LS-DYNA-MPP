using MahApps.Metro.Controls;
using Microsoft.Win32;
using ReactiveUI;
using System.Diagnostics;
using System.Windows;
using System.ComponentModel;
using System;




namespace Predictive.Lsdyna.Mpp.Views
{
    /// <summary>
    /// Interaction logic for SettingsFlyout.xaml
    /// </summary>
    public partial class SettingsFlyout : Flyout, IViewFor<SettingsViewModel>
    {
        const string licenseFileFilter = "Executable (*.exe)| *.exe";

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
            this.BindCommand(ViewModel, vm => vm.StartLSTCLM, v => v.StartLSTCLMButton);
            this.BindCommand(ViewModel, vm => vm.StopLSTCLM, v => v.StopLSTCLMButton);
            this.BindCommand(ViewModel, vm => vm.LaunchLSTCLM, v => v.LaunchLSTCLMButton);

            // TODO: Implement RxUI commands for start, stop and lauch License Manager
            // Here is the RxUI way of doing commands but error handling isn't set up yet
            //this.WhenAnyObservable(x => x.ViewModel.StartLSTCLM)
            //    .Subscribe(_ =>
            //    {
            //        Process.Start(String.Format("{0}program\\lstmlm.exe", LSTCPath.Text), "start");  
            //    });

            //this.WhenAnyObservable(x => x.ViewModel.StopLSTCLM)
            //    .Subscribe(_ =>
            //    {
            //        Process.Start(String.Format("{0}program\\lstclm.exe", LSTCPath.Text), "stop");
            //    });

            //this.WhenAnyObservable(x => x.ViewModel.LaunchLSTCLM)
            //    .Subscribe(_ =>
            //    {
            //        Process.Start(String.Format("{0}program\\lstclmui.exe", LSTCPath.Text));
            //    });
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

        private void StartLSTCLM(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(string.Format("{0}program\\lstclm.exe", LSTCPath.Text), "start");
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(string.Format("Cannot find {0}program\\lstclm.exe. Please make sure LSTC Path is set to the install directory of LS-DYNA.", LSTCPath.Text), "Error starting LSTC License Manager", MessageBoxButton.OK);
            }
        }
        private void StopLSTCLM(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(string.Format("{0}program\\lstclm.exe", LSTCPath.Text), "stop");
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(string.Format("Cannot find {0}program\\lstclm.exe. Please make sure LSTC Path is set to the install directory of LS-DYNA.", LSTCPath.Text), "Error starting LSTC License Manager", MessageBoxButton.OK);
            }
        }
        private void LaunchLSTCLM(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(string.Format("{0}program\\lstclmui.exe", LSTCPath.Text));
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(string.Format("Cannot find {0}program\\lstclmui.exe. Please make sure LSTC Path is set to the install directory of LS-DYNA.", LSTCPath.Text), "Error starting LSTC License Manager", MessageBoxButton.OK);
            }
        }

    }
}
