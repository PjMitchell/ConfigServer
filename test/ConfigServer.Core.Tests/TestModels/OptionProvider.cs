using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.Models
{
    public interface IOptionProvider
    {
        ICollection<Option> GetOptions();
    }



    public class OptionProvider : IOptionProvider
    {
        public ICollection<Option> GetOptions()
        {
            return new List<Option>(Options);
        }

        public static readonly Option OptionOne   = new Option { Id = 1, Description = "Option One"  };
        public static readonly Option OptionTwo   = new Option { Id = 2, Description = "Option Two"  };
        public static readonly Option OptionThree = new Option { Id = 3, Description = "Option Three"};
        public static readonly Option OptionFour  = new Option { Id = 4, Description = "Option Four" };
        public static IEnumerable<Option> Options => new[] { OptionOne, OptionTwo, OptionThree, OptionFour };
    }
}
