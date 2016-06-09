using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Predictive.StringExtensions;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Data;

namespace Predictive.Lsdyna.Mpp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainViewModel>
    {
        // File filters for openFileDialogs
        const string inputFileFilter = "Input Data Files (*.dyn,*.d,*.dat,*.k,*.key)|*.dyn;*.d;*.dat;*.k;*.key|All files (*.*)|*.*";
        const string restartFileFilter = "Restart Data Files (d3dump*, d3full*)|d3dump*.*;d3full*|All files (*.*)|*.*";
        const string allFileFilter = "All files (*.*)|*.*";
        const string exeFileFilter = "Executable (*.exe)| *.exe";

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(AdvancedOptions.ViewModel, Settings.ViewModel);
            DataContext = ViewModel;
            
            // Data binding
            this.Bind(ViewModel, vm => vm.InputFile.Value, v => v.InputFile.Text);
            this.Bind(ViewModel, vm => vm.OutputFile.Value, v => v.OutputFile.Text);
            this.Bind(ViewModel, vm => vm.RestartFile.Value, v => v.RestartFile.Text);
            this.Bind(ViewModel, vm => vm.Solver.Value, v => v.Solver.Text);
            this.Bind(ViewModel, vm => vm.Memory.Value, v => v.Memory.Text);
            this.Bind(ViewModel, vm => vm.Memory2.Value, v => v.Memory2.Text);
            this.Bind(ViewModel, vm => vm.ExtraMPICommands.Value, v => v.ExtraMPICommands.Text);
            this.Bind(ViewModel, vm => vm.ExtraMPPCommands.Value, v => v.ExtraMPPCommands.Text);
            this.Bind(ViewModel, vm => vm.Affinity.IsActive, v => v.Affinity.IsChecked);
            this.Bind(ViewModel, vm => vm.Processors.Value, v => v.Processors.Value);
            //this.Bind(ViewModel, vm => vm.MemorySize, v => v.MemorySelector.Text);

            // Command Binding
            this.BindCommand(ViewModel, vm => vm.BrowseInputFile, v => v.InputFileButton);
            this.BindCommand(ViewModel, vm => vm.BrowseRestartFile, v => v.RestartFileButton);
            this.BindCommand(ViewModel, vm => vm.BrowseOutputFile, v => v.OutputFileButton);
            this.BindCommand(ViewModel, vm => vm.BrowseSolver, v => v.SolverButton);
            this.BindCommand(ViewModel, vm => vm.Run, v => v.RunButton);
            this.BindCommand(ViewModel, vm => vm.SWASCIIFlush, v => v.SWASCIIFlushButton);
            this.BindCommand(ViewModel, vm => vm.SWPlotState, v => v.SWPlotStateButton);
            this.BindCommand(ViewModel, vm => vm.SWRestartContinue, v => v.SWRestartContinueButton);
            this.BindCommand(ViewModel, vm => vm.SWRestartStop, v => v.SWRestartStopButton);
            this.BindCommand(ViewModel, vm => vm.SWRezonerToggle, v => v.SWRezonerToggleButton);
            this.BindCommand(ViewModel, vm => vm.SWTimeAndCycle, v => v.SWTimeAndCycleButton);
            this.BindCommand(ViewModel, vm => vm.SWVisToggle, v => v.SWVisToggleButton);
            this.BindCommand(ViewModel, vm => vm.SWStop, v => v.SWStopButton);

            this.Processors.Maximum = Environment.ProcessorCount;
            ViewModel.MpiExe.Value = "mpiexec.exe";
           
            // Load saved settings
            ViewModel.Solver.Value = Properties.Settings.Default.mppPath;
            ViewModel.Processors.Value = Environment.ProcessorCount.ToString();
            Affinity.IsChecked = Properties.Settings.Default.AFFINITY_IsChecked;
            ViewModel.Memory.Value = Properties.Settings.Default.Memory.ToString();
            ViewModel.Memory2.Value = Properties.Settings.Default.Memory2.ToString();
            ViewModel.ExtraMPICommands.Value = Properties.Settings.Default.Extra_MPI_commands.ToString();
            ViewModel.ExtraMPPCommands.Value = Properties.Settings.Default.Extra_MPP_commands.ToString();

            // file dialogs 
            var dlg = new OpenFileDialog();

            // The following code wires up the OpenFileDialog boxes for the inputfile, outputfile and solver
            // Subjects for OpenFileDialog Results
            //var inputFilePath = new Subject<string>();
            //var outputFilePath = new Subject<string>();
            var solverFilePath = new Subject<string>();

            // Input File Dialog Box. This calls the OpenFileDialog to allow the user to select an input file, then sets the MainView.inputfile to
            // selected file to the textbox and if outfile is empty it adds a value to this field
            this.WhenAnyObservable(x => x.ViewModel.BrowseInputFile)
                .Subscribe(_ => 
                  {
                      dlg.Filter = inputFileFilter;
                      dlg.FileName = InputFile.Text;
                      var result = dlg.ShowDialog();
                      if (result == true)
                      {
                          //inputFilePath.OnNext(dlg.FileName);
                          this.InputFile.Text = dlg.FileName;
                          dlg.CheckFileExists = false;
                          var output = dlg.FileName.Directory() + "\\d3hsp";
                          File.Create(output);
                          this.OutputFile.Text = output;
                      }                    
                  });
            //inputFilePath.Subscribe(x => this.ViewModel.InputFile = x);


            // Restart File Dialog Box
            this.WhenAnyObservable(x => x.ViewModel.BrowseRestartFile)
                .Subscribe(_ =>
                {
                    dlg.CheckFileExists = false;
                    dlg.Filter = restartFileFilter;
                    dlg.FileName = RestartFile.Text;
                    var result = dlg.ShowDialog();
                    if (result == true)
                    {
                        this.RestartFile.Text = dlg.FileName;
                        var output = dlg.FileName.Directory() + "\\d3hsp";
                        if (!File.Exists(output))
                        {
                            File.Create(output).Dispose();
                        }
                        this.OutputFile.Text = output;
                    }
                });

            // Output File Dialog Box
            this.WhenAnyObservable(x => x.ViewModel.BrowseOutputFile)
                .Subscribe(_ => 
                  {
                      dlg.CheckFileExists = false;
                      dlg.Filter = allFileFilter;
                      dlg.FileName = OutputFile.Text;
                      var result = dlg.ShowDialog();
                      this.OutputFile.Text = dlg.FileName.GetShortPathName();
                  });

            // Solver Dialog Box
            this.WhenAnyObservable(x => x.ViewModel.BrowseSolver)
                .Subscribe(_ =>
                {
                    dlg.CheckFileExists = true;
                    dlg.Filter = exeFileFilter;
                    dlg.FileName = Solver.Text;
                    //dlg.InitialDirectory = Path.GetDirectoryName(Solver.Text);
                    var result = dlg.ShowDialog();
                    Properties.Settings.Default.mppPath = dlg.FileName;
                    Properties.Settings.Default.Save();
                    solverFilePath.OnNext(dlg.FileName);
                });
            solverFilePath.Subscribe(x => this.ViewModel.Solver.Value = x);

            // Run Command
            this.WhenAnyObservable(x => x.ViewModel.Run)
                .Subscribe(_ => StartCommand());

            // Sense Switch Commands
            // SW1
            this.WhenAnyObservable(x => x.ViewModel.SWRestartStop)
                .Subscribe(_ => InsertSenseSwitch("SW1"));

            // SW2
            this.WhenAnyObservable(x => x.ViewModel.SWTimeAndCycle)
                .Subscribe(_ => InsertSenseSwitch("SW2"));

            // SW3
            this.WhenAnyObservable(x => x.ViewModel.SWRestartContinue)
                .Subscribe(_ => InsertSenseSwitch("SW3"));

            // SW4
            this.WhenAnyObservable(x => x.ViewModel.SWPlotState)
                .Subscribe(_ => InsertSenseSwitch("SW4"));

            // SW5
            this.WhenAnyObservable(x => x.ViewModel.SWVisToggle)
                .Subscribe(_ => InsertSenseSwitch("SW5"));

            // SWA
            this.WhenAnyObservable(x => x.ViewModel.SWASCIIFlush)
                .Subscribe(_ => InsertSenseSwitch("SWA"));

            //stop
            this.WhenAnyObservable(x => x.ViewModel.SWStop)
                .Subscribe(_ => InsertSenseSwitch("stop"));

            //var errorHandler = UserError.RegisterHandler(error => {
            //    //ShowErrorDialog(error);

            //    return error.RecoveryCommands
            //        .Select(x => x.Select(_ => x.RecoveryOptionResult))
            //        .merge()
            //        .ObserveOn(RxApp.MainThreadScheduler);
            //});

            UserError.RegisterHandler(err => {
                ShowMessage("Check LSTC Path", err.ErrorMessage);

                // This is what the ViewModel should do in response to the user's decision
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        private static string FindMPI()
        {
            var possiblePaths = new string[]{
                @"C:\Program Files\Microsoft MPI\Bin\mpiexec.exe"
            };

            var path = possiblePaths.FirstOrDefault(p => File.Exists(p));
            return path;
        }

        private void StartCommand()
        {
            var proc = new ProgramHelper("cmd");
            Properties.Settings.Default.lastWorkingDir = this.ViewModel.WorkingDir;
            Properties.Settings.Default.lastCommand = this.ViewModel.mppCommand;
            Properties.Settings.Default.Save();
            proc.StartProgram(String.Format("/k {0}", this.ViewModel.mppCommand), this.ViewModel.WorkingDir);
        }

        public IValueConverter PathConverter { get; set; }

        private static void LineOutputter(string line)
        {
            Debug.Print(line);
        }

        private void InsertSenseSwitch(string senseSwitch)
        {
            File.WriteAllText(String.Format("{0}\\D3KIL", this.ViewModel.WorkingDir), senseSwitch);
        }

        private void ShowAdvancedFlyout(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(0);
        }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Batch file (*.bat)|*.bat";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, String.Format("cmd /k {0}",this.ViewModel.mppCommand));
        }

        private void LaunchPEWebsite(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.predictiveengineering.com/"); ;
        }

        private void LaunchHelp(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.predictiveengineering.com/content/free-ls-dyna-mpp-program-manager-windows"); ;
        }

        private void RunLast(object sender, RoutedEventArgs e)
        {
            var proc = new ProgramHelper("cmd");
            var lastWorkingDir = Properties.Settings.Default.lastWorkingDir.ToString();
            var lastCommand = Properties.Settings.Default.lastCommand.ToString();
            proc.StartProgram(String.Format("/k {0}", lastCommand), lastWorkingDir);
        }
        
        private void ShowSettings(object sender, RoutedEventArgs e) 
        {
            this.ToggleFlyout(1);
        }

        ///http://stackoverflow.com/questions/1600962/displaying-the-build-date
        private DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }

        public string AppTitle
        {
            get {return String.Format("LS-DYNA® MPP Program Manager – {0} BETA", RetrieveLinkerTimestamp().ToShortDateString());}
        }

        private async void ShowMessage(string title, string message) {
            await this.ShowMessageAsync(title, message);
        }
    }

    public sealed class ProgramHelper
    {
        private readonly Process _program = new Process();
        //public IObservable<string> ObservableOutput { get; private set; }

        public ProgramHelper(string programPath)
        {
            _program.StartInfo.FileName = programPath;
            _program.EnableRaisingEvents = true;
            _program.StartInfo.UseShellExecute = false;
            _program.StartInfo.RedirectStandardOutput = false;
        }

        public void StartProgram(string programArgs, string workingDir)
        {
            _program.StartInfo.WorkingDirectory = workingDir;
            _program.StartInfo.Arguments = programArgs;
            _program.Start();

            //ObservableOutput = _program.StandardOutputObservable();
        }
    }

    public class PathLongtoShortConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().GetShortPathName();
        }
        public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().GetLongPathName();
        }
    }


}
