using System;
using System.IO;
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
using System.Reactive.Subjects;
using System.Reactive.Linq;
using MahApps.Metro.Controls;
using ReactiveUI;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reactive.Disposables;
using Predictive.ProcessExtensions;

namespace Predictive.Lsdyna.Mpp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainViewModel>
    {
        // File filters for openFileDialogs
        const string inputFileFilter = "Input Data Files (*.dyn,*.d,*.dat,*.k,*.key)|*.dyn;*.d;*.dat;*.k*.key|All files (*.*)|*.*";
        const string allFileFilter = "All files (*.*)|*.*";
        const string exeFileFilter = "Executable (*.exe)| *.exe";
        
        public MainWindow()
        {
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            InitializeComponent();

            // Data binding
            this.Bind(ViewModel, vm => vm.InputFile, v => v.InputFile.Text);
            this.Bind(ViewModel, vm => vm.OutputFile, v => v.OutputFile.Text);
            this.Bind(ViewModel, vm => vm.Solver, v => v.Solver.Text);
            this.Bind(ViewModel, vm => vm.Memory, v => v.Memory.Text);
            this.Bind(ViewModel, vm => vm.Memory2, v => v.Memory2.Text);
            this.Bind(ViewModel, vm => vm.ExtraCommands, v => v.ExtraCommands.Text);
            
            //this.OneWayBind(ViewModel, x => x.Processors, x => x.Processors.Value);

            // Command Binding
            this.BindCommand(ViewModel, vm => vm.BrowseInputFile, v => v.InputFileButton);
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

            this.Processors.Maximum = Environment.ProcessorCount;
            ViewModel.MPI = FindMPI();
            ViewModel.Solver = Properties.Settings.Default["mppPath"].ToString();
            ViewModel.Processors = Environment.ProcessorCount;
            
            // file dialogs 
            var dlg = new OpenFileDialog();

            // The following code wires up the OpenFileDialog boxes for the inputfile, outputfile and solver
            // Subjects for OpenFileDialog Results
            var inputFilePath = new Subject<string>();
            var outputFilePath = new Subject<string>();
            var solverFilePath = new Subject<string>();

            // Input File Dialog Box. This calls the OpenFileDialog to allow the user to select an input file, then sets the MainView.inputfile to
            // selected file to the textbox and if outfile is empty it adds a value to this field
            this.WhenAnyObservable(x => x.ViewModel.BrowseInputFile)
                .Subscribe(_ => 
                  {
                      dlg.Filter = inputFileFilter;
                      dlg.FileName = this.ViewModel.InputFile;
                      var result = dlg.ShowDialog();
                      inputFilePath.OnNext(dlg.FileName);
                      dlg.CheckFileExists = false;
                      outputFilePath.OnNext(Path.GetDirectoryName(dlg.FileName) + "\\d3hsp");                     
                  });
            inputFilePath.Subscribe(x => this.ViewModel.InputFile = x);

            // Output File Dialog Box
            this.WhenAnyObservable(x => x.ViewModel.BrowseOutputFile)
                .Subscribe(_ => 
                  {
                      dlg.CheckFileExists = false;
                      dlg.Filter = allFileFilter;
                      dlg.FileName = this.ViewModel.OutputFile;
                      var result = dlg.ShowDialog();
                      outputFilePath.OnNext(dlg.FileName);
                  });
            outputFilePath.Subscribe(x => this.ViewModel.OutputFile = x);

            // Solver Dialog Box
            this.WhenAnyObservable(x => x.ViewModel.BrowseSolver)
                .Subscribe(_ =>
                {
                    dlg.CheckFileExists = true;
                    dlg.Filter = exeFileFilter;
                    dlg.FileName = this.ViewModel.Solver;
                    var result = dlg.ShowDialog();
                    Properties.Settings.Default["mppPath"] = dlg.FileName;
                    Properties.Settings.Default.Save();
                    solverFilePath.OnNext(dlg.FileName);
                });
            solverFilePath.Subscribe(x => this.ViewModel.Solver = x);

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
            proc.StartProgram(String.Format("/k {0}", this.ViewModel.mppCommand), this.ViewModel.WorkingDir);
        }

        private static void LineOutputter(string line)
        {
            Debug.Print(line);
        }

        private void InsertSenseSwitch(string sw)
        {
            File.WriteAllText(String.Format("{0}\\D3KIL", this.ViewModel.WorkingDir), sw);
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
    }

    public sealed class ProgramHelper
    {
        private readonly Process _program = new Process();
        public IObservable<string> ObservableOutput { get; private set; }

        public ProgramHelper(string programPath)
        {
            _program.StartInfo.FileName = programPath;
            _program.EnableRaisingEvents = true;
            _program.StartInfo.UseShellExecute = true;
            _program.StartInfo.RedirectStandardOutput = false;
        }

        public void StartProgram(string programArgs, string workingDir)
        {
            _program.StartInfo.WorkingDirectory = workingDir;
            _program.StartInfo.Arguments = programArgs;
            _program.Start();

            ObservableOutput = _program.StandardOutputObservable();
        }
    }
}
