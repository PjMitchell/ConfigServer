using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace ConfigServer.Core.Tests
{
    public class ConfigTests
    {

        [Fact]
        public void ConfigDefaultName_IsGenericClassName()
        {
            var config = new ConfigInstance<SimpleConfig>();
            Assert.Equal("SimpleConfig", config.Name);
        }

    }
}
