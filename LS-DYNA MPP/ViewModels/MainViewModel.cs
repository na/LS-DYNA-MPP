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
using Predictive.ProcessExtensions;

namespace Predictive.Lsdyna.Mpp
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var mppcommandArgs = this.WhenAnyValue(
                                    x => x.Processors,
                                    x => x.Solver,
                                    x => x.InputFile,
                                    x => x.OutputFile,
                                    x => x.ExtraCommands,
                                    (Processors, Solver, InputFile, OutputFile, ExtraCommands) => String.Format("-np {0} {1} i={2} o={3} {4}", Processors, Solver, InputFile, OutputFile, ExtraCommands))
                                    .ToProperty(this, x => x.mppCommandArgs, out _mppCommandArgs);

            var mppcommand = this.WhenAnyValue(
                                x => x.MPI,
                                x => x.mppCommandArgs,
                                (MPI, Args) => String.Format("{0} {1}", MPI, Args))
                                .ToProperty(this, x => x.mppCommand, out _mppCommand);

            this.WhenAnyValue(x => x.InputFile)
                .Select(x => Path.GetDirectoryName(x))
                .ToProperty(this, x => x.WorkingDir, out _workingDir);

            BrowseInputFile = ReactiveCommand.Create();
            BrowseOutputFile = ReactiveCommand.Create();
            BrowseSolver = ReactiveCommand.Create();


            var canRunMPP = this.WhenAny(
                                x => x.InputFile,
                                x => x.OutputFile,
                                x => x.Solver,
                                x => x.Processors,
                                (i, o, s, p) => !String.IsNullOrWhiteSpace(i.Value) && !String.IsNullOrWhiteSpace(o.Value) && !String.IsNullOrWhiteSpace(s.Value) && p.Value > 0);

            Run = ReactiveCommand.Create(canRunMPP);

            SWRestartStop = ReactiveCommand.Create(canRunMPP);
            SWRestartContinue = ReactiveCommand.Create(canRunMPP);
            SWRezonerToggle = ReactiveCommand.Create(canRunMPP);
            SWTimeAndCycle = ReactiveCommand.Create(canRunMPP);
            SWVisToggle = ReactiveCommand.Create(canRunMPP);
            SWPlotState = ReactiveCommand.Create(canRunMPP);
            SWASCIIFlush = ReactiveCommand.Create(canRunMPP);

        }

        public ReactiveCommand<object> BrowseInputFile { get; protected set; }
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

        private int _memory;
        public int Memory
        {
            get { return _memory; }
            set { this.RaiseAndSetIfChanged(ref _memory, value); }
        }

        private int _memory2;
        public int Memory2
        {
            get { return _memory2; }
            set { this.RaiseAndSetIfChanged(ref _memory2, value); }
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

        ObservableAsPropertyHelper<string> _mppCommandArgs;
        public string mppCommandArgs
        {
            get { return _mppCommandArgs.Value; }
        }

        ObservableAsPropertyHelper<string> _workingDir;
        public string WorkingDir
        {
            get { return _workingDir.Value; }
        }
    }
}
