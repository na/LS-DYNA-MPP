﻿using System;
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
            this.Bind(ViewModel, x => x.InputFile, x => x.InputFile.Text);
            this.Bind(ViewModel, x => x.OutputFile, x => x.OutputFile.Text);
            this.Bind(ViewModel, x => x.Solver, x => x.Solver.Text);
            
            //this.OneWayBind(ViewModel, x => x.Processors, x => x.Processors.Value);

            // Command Binding
            this.BindCommand(ViewModel, x => x.BrowseInputFile, x => x.InputFileButton);
            this.BindCommand(ViewModel, x => x.BrowseOutputFile, x => x.OutputFileButton);
            this.BindCommand(ViewModel, x => x.BrowseSolver, x => x.SolverButton);
            this.BindCommand(ViewModel, x => x.Run, x => x.RunButton);

            this.Processors.Maximum = Environment.ProcessorCount;
            ViewModel.MPI = FindMPI();
            ViewModel.Solver = Properties.Settings.Default["mppPath"].ToString();
            ViewModel.Processors = Environment.ProcessorCount;
            ViewModel.IsRunning = false;

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
                      if (string.IsNullOrEmpty(this.ViewModel.OutputFile)) 
                      {            
                          dlg.CheckFileExists = false;
                          outputFilePath.OnNext(Path.GetDirectoryName(dlg.FileName) + "\\d3hsp");                     
                      }
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
                                   
            var proc = new ProgramHelper(this.ViewModel.MPI);
            
            this.WhenAnyObservable(x => x.ViewModel.Run)
                .Subscribe(_ => StartCommand(proc));

           //proc.ObservableOutput.Subscribe(LineOutputter);
           //this.Bind(ViewModel, _ => proc.ObservableOutput, x => x.CommandOutput.Text);
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

        private void StartCommand(ProgramHelper progHelper)
        {
            this.ViewModel.IsRunning = true;
            progHelper.StartProgram(this.ViewModel.mppCommandArgs, this.ViewModel.WorkingDir);
        }

        private static void LineOutputter(string line)
        {
            Debug.Print(line);
        }

        private static IEnumerable<string> GetLineReader(StreamReader reader)
        {
            while (reader.BaseStream.CanRead)
            {
                var l = reader.ReadLine();
                if (l == null)
                {
                    break;
                }
                yield return l;
            }
        }
    }

    public static class ProcessExtension
    {
        public static IObservable<string> StandardOutputObservable(this System.Diagnostics.Process @this)
        {
            return Observable
              .FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(h => @this.OutputDataReceived += h, h => @this.OutputDataReceived -= h)
              .Select(x => x.EventArgs.Data);
        }
    }

    public sealed class ProgramHelper
    {
        private readonly Process _program = new Process();
        public IObservable<string> ObservableOutput { get; private set; }

        public ProgramHelper(string programPath)
        {
            _program.StartInfo.FileName = programPath;
            //_program.StartInfo.WorkingDirectory = "C:\\scratch\\mppdyna";
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
