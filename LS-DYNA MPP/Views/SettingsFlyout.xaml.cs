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

        //// TODO: Make this a ReactiveCOMMAND and add proper error handling
        //private void StartLSTCLM(object sender, RoutedEventArgs e)
        //{
        //    LstcLM("start");
        //}

        //private void StopLSTCLM(object sender, RoutedEventArgs e)
        //{
        //    LstcLM("stop");
        //}

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

        private void LstcLM(string action)
        {
            Process process = new Process();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = string.Format("/k {0}program\\lstclm.exe {1}", LSTCPath.Text, action);
                Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(string.Format("Cannot find {0}program\\lstclm.exe. Please make sure LSTC Path is set to the install directory of LS-DYNA.", LSTCPath.Text), "Error starting LSTC License Manager", MessageBoxButton.OK);
            }
        }

    }
}
