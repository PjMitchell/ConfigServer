using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.mvc.Models
{
    public class SampleConfig
    {
        public int LlamaCapacity { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Decimal { get; set; }
        public bool IsLlamaFarmer { get; set; }
        public Choice Choice { get; set;}
    }
}

public enum Choice
{
    OptionOne =0,
    OptionTwo =1,
    OptionThree =3
}
