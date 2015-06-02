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
            Affinity = new LsmppOption("Affinity", "", "-Affinity");
            Options.Add(Affinity);
            LicenseType = new LsmppOption("License Type", "-ENV lstc_license ");
            Options.Add(LicenseType);
            LicenseFile = new LsmppOption("Local License File", "-ENV lstc_file ");
            Options.Add(LicenseFile);
            LicenseServer = new LsmppOption("License Server", "-ENV lstc_license_server ");
            Options.Add(LicenseServer);
            LicensePort = new LsmppOption("Network License Port", "-ENV lstc_license_port ");
            Options.Add(LicensePort);
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
            AcousticOuput = new LsmppOption("Acoustic output", "BEM=");
            Options.Add(AcousticOuput);
            InterfaceSegment = new LsmppOption("Interface segment", "L=");
            Options.Add(InterfaceSegment);
            VdaGeometry = new LsmppOption("Vda geometry", "V=");
            Options.Add(VdaGeometry);
            Cal3dInput = new LsmppOption("CAL3D input", "Y=");
            Options.Add(Cal3dInput);
            Topaz3dfile = new LsmppOption("TOPAZ3D file", "T=");
            Options.Add(Topaz3dfile);
            StressInitialization = new LsmppOption("Stress initialization", "M=");
            Options.Add(StressInitialization);
            MappingInputFile = new LsmppOption("Mapping input file", "MAP=");
            Options.Add(MappingInputFile);
            Graphics = new LsmppOption("Graphics", "G=", "d3plot");
            Options.Add(Graphics);
            TimeHistories = new LsmppOption("Time histories", "F=", "d3thdt");
            Options.Add(TimeHistories);
            InterfaceForce = new LsmppOption("Interface force", "S=");
            Options.Add(InterfaceForce);
            FsiInterfaceForce = new LsmppOption("FSI interface force", "H=");
            Options.Add(FsiInterfaceForce);
            DynamicRelaxation = new LsmppOption("Dynamic relaxation", "B=", "d3drfl");
            Options.Add(DynamicRelaxation);
            AcousticOuput = new LsmppOption("Acoustic Output", "BEM=");
            Options.Add(AcousticOuput);
            DemInterfaceForce = new LsmppOption("DEM interface force", "DEM=");
            Options.Add(DemInterfaceForce);
            InputEcho = new LsmppOption("Input echo", "E=");
            Options.Add(InputEcho);
            RestartDump = new LsmppOption("Restart dump", "D=", "d3dump");
            Options.Add(RestartDump);
            InterfaceSegmentSave = new LsmppOption("Interface segment save", "Z=");
            Options.Add(InterfaceSegmentSave);
            RemapCrackDatabase = new LsmppOption("Remap, Crack database", "Q=", "remap");
            Options.Add(RemapCrackDatabase);
            RunningRestartDump = new LsmppOption("Running restart dump", "A=", "runrsf");
            Options.Add(RunningRestartDump);
            PropertyOutput = new LsmppOption("Property output", "D3PROP=", "d3prop");
            Options.Add(PropertyOutput);
            MappingOutputFile = new LsmppOption("Mapping output file", "MAP=");
            Options.Add(MappingOutputFile);

            this.WhenAny(
                x => x.InputFile.Value,
                x => x.RestartFile.Value,
                (input, restart) => ToWorkingDir(input.Value, restart.Value)).ToProperty(this, x => x.WorkingDir, out _workingDir);

            this.WhenAnyValue(x => x.OutputFile.Value).Where(x => !String.IsNullOrWhiteSpace(x)).Subscribe(x => File.Create(x));

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

        private LsmppOption _interfaceSegment; public LsmppOption InterfaceSegment
        {
            get { return _interfaceSegment; }
            set { this.RaiseAndSetIfChanged(ref _interfaceSegment, value); }
        }
        private LsmppOption _vdaGeometry; public LsmppOption VdaGeometry
        {
            get { return _vdaGeometry; }
            set { this.RaiseAndSetIfChanged(ref _vdaGeometry, value); }
        }
        private LsmppOption _cal3dInput; public LsmppOption Cal3dInput
        {
            get { return _cal3dInput; }
            set { this.RaiseAndSetIfChanged(ref _cal3dInput, value); }
        }
        private LsmppOption _topaz3dfile; public LsmppOption Topaz3dfile
        {
            get { return _topaz3dfile; }
            set { this.RaiseAndSetIfChanged(ref _topaz3dfile, value); }
        }
        private LsmppOption _stressInitialization; public LsmppOption StressInitialization
        {
            get { return _stressInitialization; }
            set { this.RaiseAndSetIfChanged(ref _stressInitialization, value); }
        }
        private LsmppOption _mappingInputFile; public LsmppOption MappingInputFile
        {
            get { return _mappingInputFile; }
            set { this.RaiseAndSetIfChanged(ref _mappingInputFile, value); }
        }
        private LsmppOption _graphics; public LsmppOption Graphics
        {
            get { return _graphics; }
            set { this.RaiseAndSetIfChanged(ref _graphics, value); }
        }
        private LsmppOption _timeHistories; public LsmppOption TimeHistories
        {
            get { return _timeHistories; }
            set { this.RaiseAndSetIfChanged(ref _timeHistories, value); }
        }
        private LsmppOption _interfaceForce; public LsmppOption InterfaceForce
        {
            get { return _interfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _interfaceForce, value); }
        }
        private LsmppOption _fsiInterfaceForce; public LsmppOption FsiInterfaceForce
        {
            get { return _fsiInterfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _fsiInterfaceForce, value); }
        }
        private LsmppOption _dynamicRelaxation; public LsmppOption DynamicRelaxation
        {
            get { return _dynamicRelaxation; }
            set { this.RaiseAndSetIfChanged(ref _dynamicRelaxation, value); }
        }
        private LsmppOption _acousticOuput; public LsmppOption AcousticOuput
        {
            get { return _acousticOuput; }
            set { this.RaiseAndSetIfChanged(ref _acousticOuput, value); }
        }
        private LsmppOption _demInterfaceForce; public LsmppOption DemInterfaceForce
        {
            get { return _demInterfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _demInterfaceForce, value); }
        }
        private LsmppOption _inputEcho; public LsmppOption InputEcho
        {
            get { return _inputEcho; }
            set { this.RaiseAndSetIfChanged(ref _inputEcho, value); }
        }
        private LsmppOption _restartDump; public LsmppOption RestartDump
        {
            get { return _restartDump; }
            set { this.RaiseAndSetIfChanged(ref _restartDump, value); }
        }
        private LsmppOption _interfaceSegmentSave; public LsmppOption InterfaceSegmentSave
        {
            get { return _interfaceSegmentSave; }
            set { this.RaiseAndSetIfChanged(ref _interfaceSegmentSave, value); }
        }
        private LsmppOption _remapCrackDatabase; public LsmppOption RemapCrackDatabase
        {
            get { return _remapCrackDatabase; }
            set { this.RaiseAndSetIfChanged(ref _remapCrackDatabase, value); }
        }
        private LsmppOption _runningRestartDump; public LsmppOption RunningRestartDump
        {
            get { return _runningRestartDump; }
            set { this.RaiseAndSetIfChanged(ref _runningRestartDump, value); }
        }
        private LsmppOption _propertyOutput; public LsmppOption PropertyOutput
        {
            get { return _propertyOutput; }
            set { this.RaiseAndSetIfChanged(ref _propertyOutput, value); }
        }
        private LsmppOption _mappingOutputFile; public LsmppOption MappingOutputFile
        {
            get { return _mappingOutputFile; }
            set { this.RaiseAndSetIfChanged(ref _mappingOutputFile, value); }
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

        private LsmppOption _licenseServer; public LsmppOption LicenseServer
        {
            get { return _licenseServer; }
            set { this.RaiseAndSetIfChanged(ref _licenseServer, value); }
        }
        private LsmppOption _licensePort; public LsmppOption LicensePort
        {
            get { return _licensePort; }
            set { this.RaiseAndSetIfChanged(ref _licensePort, value); }
        }
        private LsmppOption _LicenseFile; public LsmppOption LicenseFile
        {
            get { return _LicenseFile; }
            set { this.RaiseAndSetIfChanged(ref _LicenseFile, value); }
        }
        private LsmppOption _licenseType; public LsmppOption LicenseType
        {
            get { return _licenseType; }
            set { this.RaiseAndSetIfChanged(ref _licenseType, value); }
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

        //private string MemoryToOption(string memory)
        //{
        //    return String.IsNullOrWhiteSpace(memory) ? null : String.Format(" memory={0}", memory);           
        //}

        //private string ToOption(string input, string flag)
        //{
        //    return String.IsNullOrWhiteSpace(input) ? null : String.Format(" '{0}={1}'", flag, input);
        //}

        private SettingsViewModel _settings;
        public SettingsViewModel Settings
        {
            get { return _settings; }
            set { this.RaiseAndSetIfChanged(ref _settings, value); }
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

        public override string ToString()
        {
            var result = Options.Where(x => x.IsActive).Select(x => x.ToString()).ToArray();
            return String.Join(" ", result);
        }
    }
}
