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
            this.WhenAnyValue(x => x.LicenseServer).Throttle(TimeSpan.FromMilliseconds(800)).Where(x => !string.IsNullOrEmpty(x)).Subscribe(x => System.Environment.SetEnvironmentVariable("LSTC_LICENSE_SERVER",x,EnvironmentVariableTarget.User));
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

        private string _licenseServer;
        public string LicenseServer
        {
            get { return _licenseServer; }
            set { this.RaiseAndSetIfChanged(ref _licenseServer, value); }
        }

        private string _licensePort;
        public string LicensePort
        {
            get { return _licensePort; }
            set { this.RaiseAndSetIfChanged(ref _licensePort, value); }
        }

        private string _localLicenseFile;
        public string LocalLicenseFile
        {
            get { return _localLicenseFile; }
            set { this.RaiseAndSetIfChanged(ref _localLicenseFile, value); }
        }
    }
}
