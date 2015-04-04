using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using Predictive.Lsdyna.Mpp.Models;

namespace Predictive.Lsdyna.Mpp.ViewModels
{
    class AdvancedOptionsViewModel : ReactiveObject
    {
        public ReactiveList<LsmppOption> Options { get; set; }
        public AdvancedOptionsViewModel(){
            Options = new ReactiveList<LsmppOption>();
        }
    }
}
