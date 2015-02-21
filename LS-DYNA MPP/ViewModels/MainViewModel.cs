using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Reactive.Subjects;
using ReactiveUI;

namespace LS_DYNA_MPP.ViewModels
{
    class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var mppcommand = this.WhenAnyValue(x => x.Processors,
                                            X => X.Solver,
                                            x => x.InputFile, 
                                            x => x.OutputFile,
                                            x => x.ExtraCommands,
                                            (Processors,  Solver, InputFile, OutputFile, ExtraCommands) => String.Format("mpiexec -np {0} {1} i={2} o={3} {4}", Processors, Solver, InputFile, OutputFile, ExtraCommands))
                                            .ToProperty(this, x => x.mppCommand, out _mppCommand);

            var dlg = new OpenFileDialog();

            // Input file browse dialog
            var inputFileDone = new Subject<string>();
            BrowseInputFile = ReactiveCommand.Create();
            BrowseInputFile.Subscribe(_ =>
              {
                  var result = dlg.ShowDialog();
                  inputFileDone.OnNext(dlg.FileName);
              });
            inputFileDone.Subscribe(x => InputFile = x);

            // Output file browse dialog
            BrowseOutputFile = ReactiveCommand.Create();
            var outputFileDone = new Subject<string>();
            BrowseOutputFile.Subscribe(_ => 
                {
                    var result = dlg.ShowDialog();
                    outputFileDone.OnNext(dlg.FileName);
                });
            outputFileDone.Subscribe(x => OutputFile = x);

            // Solver browse dialog
            BrowseSolver = ReactiveCommand.Create();
            var solverDone = new Subject<string>();
            BrowseSolver.Subscribe(_ =>
            {
                var result = dlg.ShowDialog();
                solverDone.OnNext(dlg.FileName);
            });
            solverDone.Subscribe(x => Solver = x);
        }

        public ReactiveCommand<object> BrowseInputFile { get; protected set; }
        public ReactiveCommand<object> BrowseOutputFile { get; protected set; }
        public ReactiveCommand<object> BrowseSolver { get; protected set; }

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

        ObservableAsPropertyHelper<string> _mppCommand;
        public string mppCommand
        {
            get { return _mppCommand.Value; }
        }

    }
}
