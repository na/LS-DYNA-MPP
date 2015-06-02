using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using ReactiveUI;
using Predictive.Lsdyna.Mpp;
using Predictive.StringExtensions;

namespace Predictive.Lsdyna.Mpp.Models
{
    public class LsmppOption : ReactiveObject, IOption
    {
        private string _flag;
        public string Flag
        {
            get { return _flag; }
            set { this.RaiseAndSetIfChanged(ref _flag, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _option;
        public string Value
        {
            get { return _option; }
            set { this.RaiseAndSetIfChanged(ref _option, value); }
        }

        private string _helpText;
        public string HelpText
        {
            get { return _helpText; }
            set { this.RaiseAndSetIfChanged(ref _helpText, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); }
        }

        public LsmppOption(string name, string flag, string option ="", string helpText="", bool isActive=false)
        {
            Flag = flag;
            Name = name;
            HelpText = helpText;
            Value = option;
            IsActive = isActive;

            // If the Option get set to empty or whitespace set IsActive to false
            this.WhenAnyValue(x => x.Value).Where(x => String.IsNullOrWhiteSpace(x)).Subscribe(_ => this.IsActive=false);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Value) ? "" : String.Format("{0}{1}", Flag, Value.GetShortPathName());
        }
    }
}
