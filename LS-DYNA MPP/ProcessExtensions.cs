using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Diagnostics;

namespace Predictive.ProcessExtensions
{
    public static class ProcessExtension
    {
        public static IObservable<string> StandardOutputObservable(this System.Diagnostics.Process @this)
        {
            var receivedStdOut =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                (h => @this.OutputDataReceived += h,
                 h => @this.OutputDataReceived -= h)
                .Select(e => e.EventArgs.Data);

            return Observable.Create<string>(observer =>
            {
                var cancel = Disposable.Create(@this.CancelOutputRead);

                return new CompositeDisposable(cancel, receivedStdOut.Subscribe(observer));
            });
        }

        public static IObservable<string> StandardErrorObservable(this System.Diagnostics.Process @this)
        {
            var receivedStdErr =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>
                (h => @this.ErrorDataReceived += h,
                 h => @this.ErrorDataReceived -= h)
                .Select(e => e.EventArgs.Data);

            return Observable.Create<string>(observer =>
            {
                var cancel = Disposable.Create(@this.CancelErrorRead);

                return new CompositeDisposable(cancel, receivedStdErr.Subscribe(observer));
            });
        }
    }
}
