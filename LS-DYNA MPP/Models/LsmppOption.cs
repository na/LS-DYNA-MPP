using Predictive.StringExtensions;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

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

        private string _value;
        public string Value
        {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
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

        public LsmppOption(string name, string flag, string value ="", string helpText="", bool isActive=false)
        {
            Flag = flag;
            Name = name;
            HelpText = helpText;
            Value = value;
            IsActive = isActive;

            // If the Option gets set to empty or whitespace set IsActive to false
            this.WhenAnyValue(x => x.Value).Where(x => String.IsNullOrWhiteSpace(x)).Subscribe(_ => this.IsActive=false);
        }

        public override string ToString()
        {
            // restart files need special handling b/c with MPP the file is split up into multiple files 
            // and the solver just wants the base file name, but GetShortPathName doesn't work b/c the file
            // does not exist. 
            if ((Flag.Equals("R=") || Flag.Equals("O=")) && !String.IsNullOrEmpty(Value))
            {
                var dir = Path.GetDirectoryName(Value);
                var file = Path.GetFileNameWithoutExtension(Value);
                return String.Format("{0}{1}\\{2}", Flag, dir.GetShortPathName(), file);
            }
            else if ((Flag.Equals("Memory=") || Flag.Equals("Memory2=")) && !String.IsNullOrEmpty(Value))
            {
                return String.Format("{0}{1}", Flag, Value.ToWords());
            }
            return string.IsNullOrEmpty(Value) ? "" : String.Format("{0}{1}", Flag, Value.GetShortPathName());
        }
    }
}
 