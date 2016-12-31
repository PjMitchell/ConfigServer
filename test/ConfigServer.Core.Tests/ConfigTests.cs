using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace ConfigServer.Core.Tests
{
    public class ConfigTests
    {

        private const string clientId = "7aa7d5f0-90fb-420b-a906-d482428a0c44";
        [Fact]
        public void ConfigDefaultName_IsGenericClassName()
        {
            var config = new ConfigInstance<SimpleConfig>(clientId);
            Assert.Equal("SimpleConfig", config.Name);
        }

    }
}
