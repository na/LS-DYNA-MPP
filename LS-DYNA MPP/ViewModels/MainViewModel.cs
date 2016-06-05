using Predictive.Lsdyna.Mpp.Models;
using Predictive.Lsdyna.Mpp.ViewModels;
using Predictive.StringExtensions;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
//using Predictive.ProcessExtensions;

namespace Predictive.Lsdyna.Mpp
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel(AdvancedOptionsViewModel AdvancedOptions, SettingsViewModel Settings)
        {
            // Master list of command line options this is a reactive list so we can track changes and build the command
            Options = new ReactiveList<LsmppOption>() { ChangeTrackingEnabled = true };
            
            // Track changes to options and when an option value changes set it to active
            Options.ItemChanged.Where(x => x.PropertyName == "Value").Select(x => x.Sender).Subscribe(x => x.IsActive = true);
            
            // Here we are updating the command for mpp any time an option changes
            Options.ItemChanged.Select(_ => String.Join(" ", Options.Where(x=>x.IsActive).Select(x=>x.ToString()).ToArray())).ToProperty(this,x=>x.mppCommand, out _mppCommand);

            // Add all the various commandline options to Options
            MpiExe = new LsmppOption("MPI EXE", "");
            Options.Add(MpiExe);
            Processors = new LsmppOption("Processors", "-np ");
            Options.Add(Processors);
            Affinity = new LsmppOption("Affinity", "", "-affinity");
            Options.Add(Affinity);
            Options.Add(Settings.LicenseType);
            Options.Add(Settings.LicenseFile);
            Options.Add(Settings.LicenseServer);
            Options.Add(Settings.LicensePort);
            ExtraMPICommands = new LsmppOption("Extra MPI commands", "");
            Options.Add(ExtraMPICommands);
            Solver = new LsmppOption("Solver", "");
            Options.Add(Solver);
            InputFile = new LsmppOption("Input File", "I=");
            Options.Add(InputFile);
            RestartFile = new LsmppOption("Restart File", "R=");
            Options.Add(RestartFile);
            OutputFile = new LsmppOption("OutputFile", "O=");
            Options.Add(OutputFile);
            Memory = new LsmppOption("Memory", "Memory=");
            Options.Add(Memory);
            Memory2 = new LsmppOption("Memory2", "Memory2=");
            Options.Add(Memory2);
            ExtraMPPCommands = new LsmppOption("Extra MPP commands", "");
            Options.Add(ExtraMPPCommands);

            // Advanced Options
            Options.Add(AdvancedOptions.AcousticOuput);
            Options.Add(AdvancedOptions.InterfaceSegment);
            Options.Add(AdvancedOptions.VdaGeometry);
            Options.Add(AdvancedOptions.Cal3dInput);
            Options.Add(AdvancedOptions.Topaz3dfile);
            Options.Add(AdvancedOptions.StressInitialization);
            Options.Add(AdvancedOptions.MappingInputFile);
            Options.Add(AdvancedOptions.Graphics);
            Options.Add(AdvancedOptions.TimeHistories);
            Options.Add(AdvancedOptions.InterfaceForce);
            Options.Add(AdvancedOptions.FsiInterfaceForce);
            Options.Add(AdvancedOptions.DynamicRelaxation);
            Options.Add(AdvancedOptions.AcousticOuput);
            Options.Add(AdvancedOptions.DemInterfaceForce);
            Options.Add(AdvancedOptions.InputEcho);
            Options.Add(AdvancedOptions.RestartDump);
            Options.Add(AdvancedOptions.InterfaceSegmentSave);
            Options.Add(AdvancedOptions.RemapCrackDatabase);
            Options.Add(AdvancedOptions.RunningRestartDump);
            Options.Add(AdvancedOptions.PropertyOutput);
            Options.Add(AdvancedOptions.MappingOutputFile);

            // Set the working directory when the user selects an input or restart file
            this.WhenAny(
                x => x.InputFile.Value,
                x => x.RestartFile.Value,
                (input, restart) => ToWorkingDir(input.Value, restart.Value)).ToProperty(this, x => x.WorkingDir, out _workingDir);

            BrowseInputFile = ReactiveCommand.Create();
            BrowseRestartFile = ReactiveCommand.Create();
            BrowseOutputFile = ReactiveCommand.Create();
            BrowseSolver = ReactiveCommand.Create();

            var canRunMPP = this.WhenAny(
                                x => x.InputFile.Value,
                                x => x.RestartFile.Value,
                                x => x.OutputFile.Value,
                                x => x.Solver.Value,
                                x => x.Processors.Value,
                                (input, restart, output, solver, processors) => (!String.IsNullOrWhiteSpace(input.Value) || !String.IsNullOrWhiteSpace(restart.Value)) && !String.IsNullOrWhiteSpace(output.Value) && !String.IsNullOrWhiteSpace(solver.Value) && processors.Value != "0");

            Run = ReactiveCommand.Create(canRunMPP);
            SWRestartStop = ReactiveCommand.Create(canRunMPP);
            SWRestartContinue = ReactiveCommand.Create(canRunMPP);
            SWRezonerToggle = ReactiveCommand.Create(canRunMPP);
            SWTimeAndCycle = ReactiveCommand.Create(canRunMPP);
            SWVisToggle = ReactiveCommand.Create(canRunMPP);
            SWPlotState = ReactiveCommand.Create(canRunMPP);
            SWASCIIFlush = ReactiveCommand.Create(canRunMPP);
            SWStop = ReactiveCommand.Create(canRunMPP);

            this.WhenAnyValue(x => x.RestartFile.Value).Where(x => x.ToLowerInvariant().Contains("full")).Subscribe(_ => this.RestartFile.Flag = "N=");
            this.WhenAnyValue(x => x.RestartFile.Value).Where(x => x.ToLowerInvariant().Contains("dump")).Subscribe(_ => this.RestartFile.Flag = "R=");
            this.WhenAnyValue(x => x.MemorySize).Subscribe(_ => this.Memory.Value = this.Memory.Value + this.MemorySize);
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

        private void SetRestartFlag(string value)
        {
            var flag = value.ToLowerInvariant().Contains("full") ? "N=" : "R=";
            this.RestartFile.Flag = flag;
        }

        private ReactiveList<LsmppOption> Options;

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
        public ReactiveCommand<object> SWStop { get; protected set; }

        public AdvancedOptionsViewModel AdvancedOptions { get; protected set; }

        private LsmppOption _mpiExe; public LsmppOption MpiExe
        {
            get { return _mpiExe; }
            set { this.RaiseAndSetIfChanged(ref _mpiExe, value); }
        }
        private LsmppOption _inputFile; public LsmppOption InputFile
        {
            get { return _inputFile; }
            set { this.RaiseAndSetIfChanged(ref _inputFile, value); }
        }
        private LsmppOption _restartFile; public LsmppOption RestartFile
        {
            get { return _restartFile; }
            set { this.RaiseAndSetIfChanged(ref _restartFile, value); }
        }
        private LsmppOption _outputFile; public LsmppOption OutputFile
        {
            get { return _outputFile; }
            set { this.RaiseAndSetIfChanged(ref _outputFile, value); }
        }
        private LsmppOption _solver; public LsmppOption Solver
        {
            get { return _solver; }
            set { this.RaiseAndSetIfChanged(ref _solver, value); }
        }
        private LsmppOption _processors; public LsmppOption Processors
        {
            get { return _processors; }
            set { this.RaiseAndSetIfChanged(ref _processors, value); }
        }
        private LsmppOption _memory; public LsmppOption Memory
        {
            get { return _memory; }
            set { this.RaiseAndSetIfChanged(ref _memory, value); }
        }
        private LsmppOption _memory2; public LsmppOption Memory2
        {
            get { return _memory2; }
            set { this.RaiseAndSetIfChanged(ref _memory2, value); }
        }
        private LsmppOption _affinity; public LsmppOption Affinity
        {
            get { return _affinity; }
            set { this.RaiseAndSetIfChanged(ref _affinity, value); }
        }
        private LsmppOption _extraMPPCommands; public LsmppOption ExtraMPPCommands
        {
            get { return _extraMPPCommands; }
            set { this.RaiseAndSetIfChanged(ref _extraMPPCommands, value); }
        }
        private LsmppOption _extraMPICommands; public LsmppOption ExtraMPICommands
        {
            get { return _extraMPICommands; }
            set { this.RaiseAndSetIfChanged(ref _extraMPICommands, value); }
        }
        private LsmppOption _outputText; public LsmppOption OutputText
        {
            get { return _outputText; }
            set { this.RaiseAndSetIfChanged(ref _outputText, value); }
        }

        private bool _isRunning; 
        public bool IsRunning
        {
            get { return _isRunning; }
            set { this.RaiseAndSetIfChanged(ref _isRunning, value); }
        }

        private string _memorySize;
        public string MemorySize
        {
            get { return _memorySize; }
            set { this.RaiseAndSetIfChanged(ref _memorySize, value); }
        }
 
        ObservableAsPropertyHelper<string> _mppCommand;
        public string mppCommand
        {
            get { return _mppCommand.Value; }
            
        }

        ObservableAsPropertyHelper<string> _workingDir;
        public string WorkingDir
        {
            get { return _workingDir.Value; }
        }

        private SettingsViewModel _settings;
        public SettingsViewModel Settings
        {
            get { return _settings; }
            set { this.RaiseAndSetIfChanged(ref _settings, value); }
        }

        public override string ToString()
        {
            var result = Options.Where(x => x.IsActive).Select(x => x.ToString()).ToArray();
            return String.Join(" ", result);
        }
    }
}
