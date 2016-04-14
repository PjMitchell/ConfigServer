using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class LocalConfigServer : IConfigServer
    {
        private readonly IConfigProvider configProvider;
        private readonly ConfigurationIdentity applicationId;
        public LocalConfigServer(IConfigProvider configProvider, string applicationId)
        {
            this.configProvider = configProvider;
            this.applicationId = new ConfigurationIdentity { ApplicationIdentity = applicationId };
        }

        public TConfig BuildConfig<TConfig>() where TConfig : class, new()
        {
            return configProvider.Get<TConfig>(applicationId).Configuration;
        }

        public object BuildConfig(Type type)
        {
            return configProvider.Get(type, applicationId);
        }
    }
}
