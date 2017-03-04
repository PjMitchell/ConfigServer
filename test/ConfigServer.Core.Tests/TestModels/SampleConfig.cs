using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.Models
{
    public class SampleConfig
    {
        public int LlamaCapacity { get; set; }
        public int? SpareLlamaCapacity { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Decimal { get; set; }
        [Display(Name = "Is Llama farmer?", Description = "Is this a Llama farmer")]
        public bool IsLlamaFarmer { get; set; }
        public Choice Choice { get; set;}
        public Option Option { get; set; }
        public List<Option> MoarOptions { get; set; }
        public List<ListConfig> ListOfConfigs { get; set; }
        public ExternalOption ExternalOption { get; set; }        
    }


    public enum Choice
    {
        OptionOne =0,
        OptionTwo =1,
        OptionThree =3
    }

    public class Option
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class ExternalOption
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class ListConfig
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
