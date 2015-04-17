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
//using Predictive.ProcessExtensions;

namespace Predictive.Lsdyna.Mpp
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var mppcommand = this.WhenAnyValue(
                                    x => x.Affinity,
                                    x => x.Processors,
                                    x => x.Solver,
                                    x => x.InputFile,
                                    x => x.RestartFile,
                                    x => x.OutputFile,
                                    x => x.Memory,
                                    x => x.Memory2,
                                    x => x.ExtraCommands,
                                    (affinity, processors, solver, inputFile, restart, outputFile, memory, memory2, extraCommands) => String.Format("mpiexec.exe {0} -np {1} {2} {3} {4} {5} {6} {7} {8}", affinity ? "-affinity" : "", processors, solver, inputFile.GetShortPathName().ToOption("I"), RestartToOption(restart.GetShortPathName()), outputFile.GetShortPathName().ToOption("O"), memory.ToOption("Memory"), memory2.ToOption("Memory2"), extraCommands))
                                    .ToProperty(this, x => x.mppCommand, out _mppCommand);

            this.WhenAny(
                x => x.InputFile,
                x => x.RestartFile,
                (input, restart) => ToWorkingDir(input.Value, restart.Value)).ToProperty(this, x => x.WorkingDir, out _workingDir);

            //this.WhenAnyValue(x => x.OutputFile).Where(x => !String.IsNullOrWhiteSpace(x)).Subscribe(x => File.Create(x));
            //    )
            //    .Where(x => x !="")
            //    .Select(x => x.Directory())
            //    .Log(this, "Working Directory")
            //    .ToProperty(this, x => x.WorkingDir, out _workingDir);

            //this.WhenAnyValue(x => x.RestartFile)
            //    .Where(x => x != "")
            //    .Select(x => x.Directory())
            //    .ToProperty(this, x => x.WorkingDir, out _workingDir);

            BrowseInputFile = ReactiveCommand.Create();
            BrowseRestartFile = ReactiveCommand.Create();
            BrowseOutputFile = ReactiveCommand.Create();
            BrowseSolver = ReactiveCommand.Create();

            var canRunMPP = this.WhenAny(
                                x => x.InputFile,
                                x => x.RestartFile,
                                x => x.OutputFile,
                                x => x.Solver,
                                x => x.Processors,
                                (input, restart, output, solver, processors) => !String.IsNullOrWhiteSpace(input.Value) || !String.IsNullOrWhiteSpace(restart.Value) && !String.IsNullOrWhiteSpace(output.Value) && !String.IsNullOrWhiteSpace(solver.Value) && processors.Value > 0);

            Run = ReactiveCommand.Create(canRunMPP);
            SWRestartStop = ReactiveCommand.Create(canRunMPP);
            SWRestartContinue = ReactiveCommand.Create(canRunMPP);
            SWRezonerToggle = ReactiveCommand.Create(canRunMPP);
            SWTimeAndCycle = ReactiveCommand.Create(canRunMPP);
            SWVisToggle = ReactiveCommand.Create(canRunMPP);
            SWPlotState = ReactiveCommand.Create(canRunMPP);
            SWASCIIFlush = ReactiveCommand.Create(canRunMPP);

            //_inputFile = new LsmppOption("Input File", "I");
            //_outputFile = new LsmppOption("Output File", "O");
            //_restartFile = new LsmppOption("Restart File", "R");
        }

        private string ToWorkingDir(string input, string restart)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                return input.Directory();
            } else if (!String.IsNullOrWhiteSpace(restart)){
                return restart.Directory();
            }
            return "";
        }

        public ReactiveCommand<object> BrowseInputFile { get; protected set; }
        public ReactiveCommand<object> BrowseRestartFile { get; protected set; }
        public ReactiveCommand<object> BrowseOutputFile { get; protected set; }
        public ReactiveCommand<object> BrowseSolver { get; protected set; }
        public ReactiveCommand<object> Run { get; protected set; }
        public ReactiveCommand<object> SWRestartStop { get; protected set; }
        public ReactiveCommand<object> SWRestartContinue { get; protected set; }
        public ReactiveCommand<object> SWTimeAndCycle { get; protected set; }
        public ReactiveCommand<object> SWPlotState { get; protected set; }
        public ReactiveCommand<object> SWVisToggle { get; protected set; }
        public ReactiveCommand<object> SWRezonerToggle { get; protected set; }
        public ReactiveCommand<object> SWASCIIFlush { get; protected set; }

        private string _inputFile;
        public string InputFile
        {
            get { return _inputFile; }
            set { this.RaiseAndSetIfChanged(ref _inputFile, value); }
        }

        private string _restartFile;
        public string RestartFile
        {
            get { return _restartFile; }
            set { this.RaiseAndSetIfChanged(ref _restartFile, value); }
        }

        private string _outputFile;
        public string OutputFile
        {
            get { return _outputFile; }
            set { this.RaiseAndSetIfChanged(ref _outputFile, value); }
        }

        private string _solver;
        public string Solver
        {
            get { return _solver; }
            set { this.RaiseAndSetIfChanged(ref _solver, value); }
        }

        private int _processors;
        public int Processors
        {
            get { return _processors; }
            set { this.RaiseAndSetIfChanged(ref _processors, value); }
        }

        private string _memory;
        public string Memory
        {
            get { return _memory; }
            set { this.RaiseAndSetIfChanged(ref _memory, value); }
        }

        private string _memory2;
        public string Memory2
        {
            get { return _memory2; }
            set { this.RaiseAndSetIfChanged(ref _memory2, value); }
        }

        private bool _affinity;
        public bool Affinity
        {
            get { return _affinity; }
            set { this.RaiseAndSetIfChanged(ref _affinity, value); }
        }

        private string _extraCommands;
        public string ExtraCommands
        {
            get { return _extraCommands; }
            set { this.RaiseAndSetIfChanged(ref _extraCommands, value); }
        }

        private string _outputText;
        public string OutputText
        {
            get { return _outputText; }
            set { this.RaiseAndSetIfChanged(ref _outputText, value); }
        }

        private string _mpi;
        public string MPI
        {
            get { return _mpi; }
            set { this.RaiseAndSetIfChanged(ref _mpi, value); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { this.RaiseAndSetIfChanged(ref _isRunning, value); }
        }

        ObservableAsPropertyHelper<string> _mppCommand;
        public string mppCommand
        {
            get { return _mppCommand.Value; }
        }

        //ObservableAsPropertyHelper<string> _mppCommandArgs;
        //public string mppCommandArgs
        //{
        //    get { return _mppCommandArgs.Value; }
        //}

        ObservableAsPropertyHelper<string> _workingDir;
        public string WorkingDir
        {
            get { return _workingDir.Value; }
        }

        private string MemoryToOption(string memory)
        {
            return String.IsNullOrWhiteSpace(memory) ? null : String.Format(" memory={0}", memory);           
        }

        private string ToOption(string input, string flag)
        {
            return String.IsNullOrWhiteSpace(input) ? null : String.Format(" '{0}={1}'", flag, input);
        }

        private string RestartToOption(string restart)
        {
            if (!string.IsNullOrEmpty(restart))
            {
                var flag = restart.ToLowerInvariant().Contains("full") ? "N" : "R";
                return String.Format(" {0}={1}", flag, string.Format("{0}\\{1}", restart.Directory().GetShortPathName(), restart.FileNameWithoutExtension()));
            } else
            {
                return null;
            }
        }
    }
}
