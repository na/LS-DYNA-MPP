using System;

namespace Predictive.Lsdyna.Mpp
{
    public interface ISenseSwitchService
    {
        IObservable<MainViewModel> WriteSenseSwitch(string sw);
    }

    public interface IOption
    {
        bool IsActive { get; set; }
    }
}
