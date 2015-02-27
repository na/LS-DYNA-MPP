using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Predictive.Lsdyna.Mpp
{
    public static class RxProcessUtilities
    {
        /// <summary>
        /// Creates a connectable observable for a process.
        /// </summary>
        /// <remarks>Must be a connectable observable in order to hinder multiple subscriptions to call the process multiple times.</remarks>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public static IConnectableObservable<string> CreateConnectableObservableProcess(string filename, string arguments, IObservable<string> input = null)
        {
            var observable = Observable.Using(() =>
            {
                Process process = new Process();

                // process configuration
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                process.EnableRaisingEvents = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                if (null != input)
                {
                    process.StartInfo.RedirectStandardInput = true;

                    input.Subscribe(s =>
                    {
                        if (!process.HasExited)
                        {
                            process.StandardInput.Write(s);
                        }
                    });
                }

                return process;
            },
                process =>
                {
                    return Observable.Create<string>(
                        (IObserver<string> observer) =>
                        {
                            // listen to stdout and stderr
                            var stdOut = RxProcessUtilities.CreateStandardOutputObservable(process);
                            var stdErr = RxProcessUtilities.CreateStandardErrorObservable(process);

                            var stdOutSubscription = stdOut.Subscribe(observer);
                            var stdErrSubscription = stdErr.Subscribe(observer);

                            var processExited = Observable.FromEventPattern(h => process.Exited += h, h => process.Exited -= h);

                            var processError = processExited.Subscribe(args =>
                            {
                                process.WaitForExit();

                                try
                                {
                                    if (process.ExitCode != 0)
                                    {
                                        observer.OnError(new Exception(String.Format("Process '{0}' terminated with error code {1}",
                                            process.StartInfo.FileName, process.ExitCode)));
                                    }
                                    else
                                    {
                                        observer.OnCompleted();
                                    }
                                }
                                finally
                                {
                                    process.Close();
                                }
                            });

                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            return new CompositeDisposable(stdOutSubscription,
                                                           stdErrSubscription,
                                                           processError);
                        });
                });

            return observable.Publish();
        }

        /// <summary>
        /// Creates an IObservable&lt;string&gt; for the standard error of a process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public static IObservable<string> CreateStandardErrorObservable(Process process)
        {
            // var processExited = Observable.FromEventPattern
            //    (h => process.Exited += h, h => process.Exited -= h);

            var receivedStdErr =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                    (h => process.ErrorDataReceived += h,
                     h => process.ErrorDataReceived -= h)
                //.TakeUntil(processExited) 
                // cannot be used here, since process exited event might be raised 
                // before all stderr and stdout events occurred.
                .Select(e => e.EventArgs.Data);

            return Observable.Create<string>(observer =>
            {
                var cancel = Disposable.Create(process.CancelErrorRead);

                return new CompositeDisposable(cancel, receivedStdErr.Subscribe(observer));
            });
        }

        /// <summary>
        /// Creates an IObservable&lt;string&gt; for the standard output of a process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public static IObservable<string> CreateStandardOutputObservable(Process process)
        {
            var receivedStdOut =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                (h => process.OutputDataReceived += h,
                 h => process.OutputDataReceived -= h)
                .Select(e => e.EventArgs.Data);

            return Observable.Create<string>(observer =>
            {
                var cancel = Disposable.Create(process.CancelOutputRead);

                return new CompositeDisposable(cancel, receivedStdOut.Subscribe(observer));
            });
        }
    }
}
