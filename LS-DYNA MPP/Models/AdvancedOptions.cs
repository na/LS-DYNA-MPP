using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictive.Lsdyna.Mpp.Models
{
    public class AdvancedOptions
    {
        private LsmppOption _interfaceSegment;
        private LsmppOption _vdaGeometry;
        private LsmppOption _cal3dInput;
        private LsmppOption _topaz3dfile;
        private LsmppOption _stressInitialization;
        private LsmppOption _mappingInputFile;
        private LsmppOption _graphics;
        private LsmppOption _timeHistories;
        private LsmppOption _interfaceForce;
        private LsmppOption _fsiInterfaceForce;
        private LsmppOption _dynamicRelaxation;
        private LsmppOption _acousticOuput;
        private LsmppOption _demInterfaceForce;
        private LsmppOption _inputEcho;
        private LsmppOption _restartDump;
        private LsmppOption _interfaceSegmentSave;
        private LsmppOption _remapCrackDatabase;
        private LsmppOption _runningRestartDump;
        private LsmppOption _propertyOutput;
        private LsmppOption _mappingOutputFile;

        public AdvancedOptions()
        {
            _acousticOuput = new LsmppOption("Acoustic output", "BEM");
            _interfaceSegment = new LsmppOption("Interface segment", "L");     
            _vdaGeometry = new LsmppOption("Vda geometry","V");
            _cal3dInput  = new LsmppOption("CAL3D input","Y");
            _topaz3dfile = new LsmppOption("TOPAZ3D file", "T");
            _stressInitialization = new LsmppOption("Stress initialization", "M");
            _mappingInputFile = new LsmppOption("Mapping input file", "MAP");
            _graphics = new LsmppOption("Graphics", "G", "d3plot");
            _timeHistories = new LsmppOption("Time histories", "F","d3thdt");
            _interfaceForce = new LsmppOption("Interface force", "S");
            _fsiInterfaceForce = new LsmppOption("FSI interface force", "H");
            _dynamicRelaxation = new LsmppOption("Dynamic relaxation", "B", "d3drfl");
            _acousticOuput = new LsmppOption("Acoustic Output", "BEM");
            _demInterfaceForce = new LsmppOption("DEM interface force", "DEM");
            _inputEcho = new LsmppOption("Input echo","E");
            _restartDump = new LsmppOption("Restart dump", "D", "d3dump");
            _interfaceSegmentSave = new LsmppOption("Interface segment save", "Z");
            _remapCrackDatabase = new LsmppOption("Remap, Crack database", "Q", "remap");
            _runningRestartDump = new LsmppOption("Running restart dump", "A", "runrsf");
            _propertyOutput = new LsmppOption("Property output", "D3PROP", "d3prop");
            _mappingOutputFile = new LsmppOption("Mapping output file", "MAP");
        }

        public LsmppOption InterfaceSegment
        {
            get { return _interfaceSegment;}
        }
    }
}
