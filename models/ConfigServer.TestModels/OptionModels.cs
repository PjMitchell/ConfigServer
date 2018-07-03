using System.Collections.Generic;

namespace ConfigServer.TestModels
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

        public static readonly OptionFromConfigSet OptionOne = new OptionFromConfigSet { Id = 1, Description = "Option One", Value = 1 };
        public static readonly OptionFromConfigSet OptionTwo = new OptionFromConfigSet { Id = 2, Description = "Option Two", Value = 2 };
        public static readonly OptionFromConfigSet OptionThree = new OptionFromConfigSet { Id = 3, Description = "Option Three", Value = 3 };
        public static readonly OptionFromConfigSet OptionFour = new OptionFromConfigSet { Id = 4, Description = "Option Four", Value = 4 };

        

        public static IEnumerable<OptionFromConfigSet> Options => new List<OptionFromConfigSet>
            {
                OptionOne,OptionTwo, OptionThree, OptionFour
            };
    }
}
