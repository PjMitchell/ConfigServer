using System;

namespace ConfigServer.Core
{
    public abstract class Config
    {
        protected Config(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string ConfigSetId { get; set; }
        public abstract object GetConfiguration();
        public abstract void SetConfiguration(object value);
        public abstract Type ConfigType { get; }

    }

    public class Config<TConfig> : Config where TConfig : class, new()
    {
        public Config() : base(typeof(TConfig).Name)
        {
            Configuration = new TConfig();
        }

        public Config(TConfig config) : base(typeof(TConfig).Name)
        {
            Configuration = config;
        }

        public override Type ConfigType => typeof(TConfig);
        
        public TConfig Configuration { get; set; }

        public override object GetConfiguration() => Configuration;

        public override void SetConfiguration(object value) => Configuration = (TConfig)value;

    }
}
