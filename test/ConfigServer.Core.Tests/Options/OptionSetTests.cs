using ConfigServer.Sample.Models;
using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Options
{
    public class OptionSetTests
    {

        private OptionSet<Option> target;

        public OptionSetTests()
        {
            target = new OptionSet<Option>(OptionProvider.Options, o => o.Id.ToString(), o => o.Description);
        }

        [Fact]
        public void TryGetValue_ReturnOptionByKey_IfExists()
        {
            Option output;
            var result = target.TryGetValue(2.ToString(), out output);

            Assert.True(result);
            Assert.NotNull(output);
            Assert.Equal(OptionProvider.OptionTwo.Description, output.Description);

        }

        [Fact]
        public void IOptionSet_TryGetValue_ReturnOptionByKey_IfExists()
        {
            object output;
            var result = ((IOptionSet)target).TryGetValue(2.ToString(), out output);
            var castOption = output as Option;
            Assert.True(result);
            Assert.NotNull(castOption);
            Assert.Equal(OptionProvider.OptionTwo.Description, castOption.Description);

        }
    }
}
