using Predictive.Lsdyna.Mpp.Models;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using RunProcessAsTask;

namespace Predictive.Lsdyna.Mpp
{
    public class SettingsViewModel : ReactiveObject
    {
        const string licenseFileFilter = "License File (*lstc_file*, *server_data*, *lstc.*)| *lstc_file*;*server_data*;*lstc.*";

        public SettingsViewModel()
        {
            this.WhenAnyValue(x => x.LicenseTypeIndex).Select(x => x.Equals(1)).ToProperty(this, x => x.IsNetworkLicense, out _isNetworkLicense);
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

            this.WhenAnyValue(x => x.LSTCPath).Where(x => !String.IsNullOrWhiteSpace(x)).Throttle(TimeSpan.FromSeconds(3)).Subscribe(x =>
            {
                Properties.Settings.Default.LSTC_PATH = x;
                Properties.Settings.Default.Save();
            });

            //StartLSTCLM = ReactiveCommand.Create();
            //StopLSTCLM = ReactiveCommand.Create();
            LaunchLSTCLM = ReactiveCommand.Create();

            ImportLicense = ReactiveCommand.CreateAsyncTask(async _ => {
                //await Task.Delay(60);
                await CopyLicense();
                });

            ImportLicense.ThrownExceptions.Subscribe(ex =>
            {
                MessageBox.Show("Error");
            });

            StartLSTCLM = ReactiveCommand.CreateAsyncTask(_ => LstcLM("start"));
            StopLSTCLM = ReactiveCommand.CreateAsyncTask(_ => LstcLM("stop"));

            StartLSTCLM.ThrownExceptions.Select(ex => new UserError("Error Starting License Manager", "Check LSTC path")).Subscribe(x => UserError.Throw(x));

            _SpinnerVisibility = ImportLicense.IsExecuting
                .Select(x => x ? Visibility.Visible : Visibility.Hidden)
                .ToProperty(this, x => x.SpinnerVisibility, Visibility.Visible);
        }

        public ReactiveCommand<Unit> ImportLicense { get; set; }
        public ReactiveCommand<bool> StartLSTCLM { get; protected set; }
        public ReactiveCommand<bool> StopLSTCLM { get; protected set; }
        public ReactiveCommand<object> LaunchLSTCLM { get; protected set; }
        public ReactiveCommand<Unit> RestartLM { get; protected set; }

        ObservableAsPropertyHelper<Visibility> _SpinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get { return _SpinnerVisibility.Value; }
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

        private String _LSTCPath;
        /// <summary>
        /// The path to the LS-DYNA® install directory
        /// </summary>
        public String LSTCPath
        {
            get { return _LSTCPath; }
            set { this.RaiseAndSetIfChanged(ref _LSTCPath, value); }
        }


        /// <summary>
        /// Task CopyLicense displays a select file dialog and then copies the selected file to 'server_data' in the program folder of <see cref="LSTCPath">LSTCPath</see>.
        /// </summary>
        public async Task CopyLicense()
        {
            var dlg = new OpenFileDialog();
            string destination = this.LSTCPath + "program\\server_data";
            var result = dlg.ShowDialog();

            if (result == true)
            {
                await CopyFileAsync(dlg.FileName, destination);
            }
        }

        /// <summary>
        /// Async task to copy a file from source to destination 
        /// file to LSTC_PATH\program\server_data
        /// </summary>
        /// <param name="source">The file to copy</param>
        /// <param name="destination">The path of the new file</param>
        /// <returns></returns>
        public async Task CopyFileAsync(string source, string destination)
        {
            using (FileStream SourceStream = File.Open(source, FileMode.Open))
            {
                using (FileStream DestinationStream = File.Create(destination))
                {
                    await SourceStream.CopyToAsync(DestinationStream);
                }
            }
        }


        public async Task<bool> LstcLM(string action)
        {
            if (action.Equals("restart"))
            {
                await StopLSTCLM.ExecuteAsync();
                await StartLSTCLM.ExecuteAsync();
                return await Task.FromResult<bool>(true);
            }
            else
            {
                // there is no non-generic TaskCompletionSource
                var tcs = new TaskCompletionSource<bool>();

                var process = new Process
                {
                    StartInfo = {
                    FileName = "cmd.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = string.Format("/k {0}program\\lstclm.exe {1}", LSTCPath, action)
                },
                    EnableRaisingEvents = true
                };

                process.Exited += (sender, args) =>
                {
                    tcs.SetResult(true);
                    process.Dispose();
                };

                process.Start();

                return await tcs.Task;
            }
        }

        //static Task RunProcessAsync(string fileName)
        //{
        //    // there is no non-generic TaskCompletionSource
        //    var tcs = new TaskCompletionSource<bool>();

        //    var process = new Process
        //    {
        //        StartInfo = { FileName = fileName },
        //        EnableRaisingEvents = true
        //    };

        //    process.Exited += (sender, args) =>
        //    {
        //        tcs.SetResult(true);
        //        process.Dispose();
        //    };

        //    process.Start();

        //    return tcs.Task;
        //}
    }
}