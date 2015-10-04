using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Predictive.ProcessExtensions
{
    /// <summary>
    /// Extensions for System.Diagnostics.Process to turn StandardOutput and StandardError into observables.
    /// </summary>
    public static class ProcessExtension
    {
        public static IObservable<string> StandardOutputObservable(this System.Diagnostics.Process process)
        {
            var receivedStdOut =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                (h => process.OutputDataReceived += h,
                 h => process.OutputDataReceived -= h)
                .Select(e => e.EventArgs.Data);

            process.BeginOutputReadLine();

            return receivedStdOut;
        }

        public static IObservable<string> StandardErrorObservable(this System.Diagnostics.Process process)
        {
            var receivedStdErr =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                (h => process.ErrorDataReceived += h,
                 h => process.ErrorDataReceived -= h)
                .Select(e => e.EventArgs.Data);
            
            process.BeginErrorReadLine();

            return receivedStdErr;
        }
    }
}
