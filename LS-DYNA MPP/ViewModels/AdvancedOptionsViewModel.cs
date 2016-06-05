using Predictive.Lsdyna.Mpp.Models;
using ReactiveUI;

namespace Predictive.Lsdyna.Mpp.ViewModels
{
    public class AdvancedOptionsViewModel : ReactiveObject
    {
        
        public AdvancedOptionsViewModel()
        {
            AcousticOuput = new LsmppOption("Acoustic output", "BEM=");
            InterfaceSegment = new LsmppOption("Interface segment", "L=");
            VdaGeometry = new LsmppOption("Vda geometry", "V=");
            Cal3dInput = new LsmppOption("CAL3D input", "Y=");
            Topaz3dfile = new LsmppOption("TOPAZ3D file", "T=");
            StressInitialization = new LsmppOption("Stress initialization", "M=");
            MappingInputFile = new LsmppOption("Mapping input file", "MAP=");
            Graphics = new LsmppOption("Graphics", "G=");
            TimeHistories = new LsmppOption("Time histories", "F=");
            InterfaceForce = new LsmppOption("Interface force", "S=");
            FsiInterfaceForce = new LsmppOption("FSI interface force", "H=");
            DynamicRelaxation = new LsmppOption("Dynamic relaxation", "B=");
            DemInterfaceForce = new LsmppOption("DEM interface force", "DEM=");
            InputEcho = new LsmppOption("Input echo", "E=");
            RestartDump = new LsmppOption("Restart dump", "D=");
            InterfaceSegmentSave = new LsmppOption("Interface segment save", "Z=");
            RemapCrackDatabase = new LsmppOption("Remap Crack database", "Q=");
            RunningRestartDump = new LsmppOption("Running restart dump", "A=");
            PropertyOutput = new LsmppOption("Property output", "D3PROP=");
            MappingOutputFile = new LsmppOption("Mapping output file", "MAP=");
        }


        private LsmppOption _interfaceSegment;
        public LsmppOption InterfaceSegment
        {
            get { return _interfaceSegment; }
            set { this.RaiseAndSetIfChanged(ref _interfaceSegment, value); }
        }
        private LsmppOption _vdaGeometry;
        public LsmppOption VdaGeometry
        {
            get { return _vdaGeometry; }
            set { this.RaiseAndSetIfChanged(ref _vdaGeometry, value); }
        }
        private LsmppOption _cal3dInput;
        public LsmppOption Cal3dInput
        {
            get { return _cal3dInput; }
            set { this.RaiseAndSetIfChanged(ref _cal3dInput, value); }
        }
        private LsmppOption _topaz3dfile;
        public LsmppOption Topaz3dfile
        {
            get { return _topaz3dfile; }
            set { this.RaiseAndSetIfChanged(ref _topaz3dfile, value); }
        }
        private LsmppOption _stressInitialization;
        public LsmppOption StressInitialization
        {
            get { return _stressInitialization; }
            set { this.RaiseAndSetIfChanged(ref _stressInitialization, value); }
        }
        private LsmppOption _mappingInputFile;
        public LsmppOption MappingInputFile
        {
            get { return _mappingInputFile; }
            set { this.RaiseAndSetIfChanged(ref _mappingInputFile, value); }
        }
        private LsmppOption _graphics;
        public LsmppOption Graphics
        {
            get { return _graphics; }
            set { this.RaiseAndSetIfChanged(ref _graphics, value); }
        }
        private LsmppOption _timeHistories;
        public LsmppOption TimeHistories
        {
            get { return _timeHistories; }
            set { this.RaiseAndSetIfChanged(ref _timeHistories, value); }
        }
        private LsmppOption _interfaceForce;
        public LsmppOption InterfaceForce
        {
            get { return _interfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _interfaceForce, value); }
        }
        private LsmppOption _fsiInterfaceForce;
        public LsmppOption FsiInterfaceForce
        {
            get { return _fsiInterfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _fsiInterfaceForce, value); }
        }
        private LsmppOption _dynamicRelaxation;
        public LsmppOption DynamicRelaxation
        {
            get { return _dynamicRelaxation; }
            set { this.RaiseAndSetIfChanged(ref _dynamicRelaxation, value); }
        }
        private LsmppOption _acousticOuput;
        public LsmppOption AcousticOuput
        {
            get { return _acousticOuput; }
            set { this.RaiseAndSetIfChanged(ref _acousticOuput, value); }
        }
        private LsmppOption _demInterfaceForce;
        public LsmppOption DemInterfaceForce
        {
            get { return _demInterfaceForce; }
            set { this.RaiseAndSetIfChanged(ref _demInterfaceForce, value); }
        }
        private LsmppOption _inputEcho;
        public LsmppOption InputEcho
        {
            get { return _inputEcho; }
            set { this.RaiseAndSetIfChanged(ref _inputEcho, value); }
        }
        private LsmppOption _restartDump;
        public LsmppOption RestartDump
        {
            get { return _restartDump; }
            set { this.RaiseAndSetIfChanged(ref _restartDump, value); }
        }
        private LsmppOption _interfaceSegmentSave;
        public LsmppOption InterfaceSegmentSave
        {
            get { return _interfaceSegmentSave; }
            set { this.RaiseAndSetIfChanged(ref _interfaceSegmentSave, value); }
        }
        private LsmppOption _remapCrackDatabase;
        public LsmppOption RemapCrackDatabase
        {
            get { return _remapCrackDatabase; }
            set { this.RaiseAndSetIfChanged(ref _remapCrackDatabase, value); }
        }
        private LsmppOption _runningRestartDump;
        public LsmppOption RunningRestartDump
        {
            get { return _runningRestartDump; }
            set { this.RaiseAndSetIfChanged(ref _runningRestartDump, value); }
        }
        private LsmppOption _propertyOutput;
        public LsmppOption PropertyOutput
        {
            get { return _propertyOutput; }
            set { this.RaiseAndSetIfChanged(ref _propertyOutput, value); }
        }
        private LsmppOption _mappingOutputFile;
        public LsmppOption MappingOutputFile
        {
            get { return _mappingOutputFile; }
            set { this.RaiseAndSetIfChanged(ref _mappingOutputFile, value); }
        }
    }
}
