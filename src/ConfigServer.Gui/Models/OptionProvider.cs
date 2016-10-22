using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.mvc.Models
{
    public interface IOptionProvider
    {
        ICollection<Option> GetOptions();
    }



    public class OptionProvider : IOptionProvider
    {
        public ICollection<Option> GetOptions()
        {
            return new List<Option>
            {
                new Option { Id = 1, Description = "Option One" },
                new Option { Id = 2, Description = "Option Two" },
                new Option { Id = 3, Description = "Option Three" },
                new Option { Id = 4, Description = "Option Four" }

            };
        }
    }
}
