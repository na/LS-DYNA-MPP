using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_DYNA_MPP
{
    class MPPDYNA
    {
        #region Members
        string _inputFile;
        string _outputFile;
        int _CPUS;
        string _extraCommands;
        int _memory;
        int _memory2;
        string _solver;
        #endregion

        #region Properties
        /// <summary>
        /// The input file path
        /// </summary>
        public string InputFile
        {
            get { return _inputFile; }
            set { _inputFile = value; }
        }

        /// <summary>
        /// The output file path
        /// </summary>
        public string OutputFile
        {
            get { return _outputFile; }
            set { _outputFile = value; }
        }

        /// <summary>
        /// number of CPUs for MPP
        /// </summary>
        public int CPUS
        {
            get { return _CPUS; }
            set { _CPUS = value; }
        }

        /// <summary>
        /// Memory 1
        /// </summary>
        public int Memory
        {
            get { return _memory; }
            set { _memory = value; }
        }

        /// <summary>
        /// Memory 1
        /// </summary>
        public int Memory2
        {
            get { return _memory2; }
            set { _memory2 = value; }
        }

        /// <summary>
        /// Extra Commands
        /// </summary>
        public string ExtraCommands
        {
            get { return _extraCommands; }
            set { _extraCommands = value; }
        }

        /// <summary>
        /// Path to the solver
        /// </summary>
        public string Solver
        {
            get { return _solver; }
            set { _solver = value; }
        }
        #endregion
    }
}
