using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reactive.Subjects;
using ReactiveUI;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using Predictive.Lsdyna.Mpp.Models;
using Predictive.StringExtensions;

namespace Predictive.Lsdyna.Mpp
{
    public class SettingsViewModel : ReactiveObject
    {
        public SettingsViewModel()
        {
            this.WhenAnyValue(x => x.LicenseTypeIndex).Select(x => x.Equals(1)).ToProperty(this, x => x.IsNetworkLicense, out _isNetworkLicense );
            this.WhenAnyValue(x => x.LicenseTypeIndex).Select(x => x.Equals(0)).ToProperty(this, x => x.IsLocalLicense, out _isLocalLicense);
            LicenseType = new LsmppOption("License Type", "-env lstc_license ");
            LicenseFile = new LsmppOption("Local License File", "-env lstc_file ");
            LicenseServer = new LsmppOption("License Server", "-env lstc_license_server ");
            LicensePort = new LsmppOption("Network License Port", "-env lstc_license_port ");

            this.WhenAnyValue(x => x.LicenseType.Value).Where(x => !String.IsNullOrWhiteSpace(x)).Subscribe(x => 
            {
                Properties.Settings.Default.LSTC_LICENSE = x;
                Properties.Settings.Default.Save(); 
            });

            this.WhenAnyValue(x => x.LicenseFile.Value).Where(x => !String.IsNullOrWhiteSpace(x)).Throttle(TimeSpan.FromSeconds(3)).Subscribe(x =>
            {
                Properties.Settings.Default.LSTC_FILE = x;
                Properties.Settings.Default.Save();
            });

            this.WhenAnyValue(x => x.LicensePort.Value).Where(x => !String.IsNullOrWhiteSpace(x)).Throttle(TimeSpan.FromSeconds(3)).Subscribe(x =>
            {
                Properties.Settings.Default.LSTC_LICENSE_SERVER_PORT = x;
                Properties.Settings.Default.Save();
            });

            this.WhenAnyValue(x => x.LicenseServer.Value).Where(x => !String.IsNullOrWhiteSpace(x)).Throttle(TimeSpan.FromSeconds(3)).Subscribe(x =>
            {
                Properties.Settings.Default.LSTC_LICENSE_SERVER = x;
                Properties.Settings.Default.Save();
            });
        }

        private int _licenseTypeIndex;
        public int LicenseTypeIndex
        {
            get { return _licenseTypeIndex; }
            set { this.RaiseAndSetIfChanged(ref _licenseTypeIndex, value); }
        }

        ObservableAsPropertyHelper<bool> _isNetworkLicense;
        public bool IsNetworkLicense
        {
            get { return _isNetworkLicense.Value; }
        }

        ObservableAsPropertyHelper<bool> _isLocalLicense;
        public bool IsLocalLicense
        {
            get { return _isLocalLicense.Value; }
        }

        private LsmppOption _licenseServer;
        public LsmppOption LicenseServer
        {
            get { return _licenseServer; }
            set { this.RaiseAndSetIfChanged(ref _licenseServer, value); }
        }

        private LsmppOption _licensePort;
        public LsmppOption LicensePort
        {
            get { return _licensePort; }
            set { this.RaiseAndSetIfChanged(ref _licensePort, value); }
        }

        private LsmppOption _licenseFile;
        public LsmppOption LicenseFile
        {
            get { return _licenseFile; }
            set { this.RaiseAndSetIfChanged(ref _licenseFile, value); }
        }

        private LsmppOption _licenseType;
        public LsmppOption LicenseType
        {
            get { return _licenseType; }
            set { this.RaiseAndSetIfChanged(ref _licenseType, value); }
        }
    }
}
