using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.TestModels
{
    public interface IOptionProvider
    {
        ICollection<Option> GetOptions();
    }



    public class OptionProvider : IOptionProvider
    {
        public static readonly Option OptionOne = new Option { Id = 1, Description = "Option One" };
        public static readonly Option OptionTwo = new Option { Id = 2, Description = "Option Two" };
        public static readonly Option OptionThree = new Option { Id = 3, Description = "Option Three" };
        public static readonly Option OptionFour = new Option { Id = 4, Description = "Option Four" };

        public ICollection<Option> GetOptions()
        {
            return Options.ToList();
        }

        public static IEnumerable<Option> Options => new List<Option>
            {
                OptionOne,OptionTwo, OptionThree, OptionFour
            };
    }
}
