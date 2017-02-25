using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Gui.Models
{
    public class OptionFromExternalConfigSet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
    }

    public class OptionFromConfigSet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
    }
}
