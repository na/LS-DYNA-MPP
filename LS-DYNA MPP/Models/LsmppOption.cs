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
        public string Value { get; set; }
        public string HelpText { get; set; }

        public LsmppOption(string name, string flag, string value = "", string helpText="")
        {
            Flag = flag;
            Name = name;
            HelpText = helpText;
            Value = value;
        }

        public override string ToString()
        {
            return String.Format("{0}={1}", Flag, Value);
        }


    }
}
