using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictive.Lsdyna.Mpp.Models
{
    public class LsmppOption
    {
        public string Flag { get; private set; }
        public string Name { get; private set; }
        public string Option { get; set; }
        public string HelpText { get; set; }

        public LsmppOption(string name, string flag, string option = "", string helpText="")
        {
            Flag = flag;
            Name = name;
            HelpText = helpText;
            Option = option;
        }

        public override string ToString()
        {
            // this is ugly but it is time to get beta out.
            return string.IsNullOrEmpty(Option) ? "" : String.Format(" {0}={1}", Flag, Option);
        }
    }
}
