using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.mvcApp2.Models
{
    public class SampleConfig
    {
        public int LlamaCapacity { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Decimal { get; set; }
        public bool IsLlamaFarmer { get; set; }
        public Choice Choice { get; set;}
        public Option Option { get; set; }
    }


    public enum Choice
    {
        OptionOne = 0,
        OptionTwo = 1,
        OptionThree = 3
    }

    public class Option
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    public class OptionFromConfigSet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
    }
}