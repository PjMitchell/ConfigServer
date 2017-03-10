using ConfigServer.Server;
using System;

namespace ConfigServer.Sample.Models
{
    public class SampleConfigSet : ConfigurationSet<SampleConfigSet>
    {
        public SampleConfigSet() : base("Core Configuration Set", "Only Configuration Set in the app") {}
        public OptionSet<Option> Options { get; set; }
        public Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetModelBuilder<SampleConfigSet> modelBuilder)
        {
            var configBuilder = modelBuilder.Config(s=> s.SampleConfig,"Sample Config", "Basic Configuration");

            configBuilder.Property(p => p.IsLlamaFarmer);
            configBuilder.Property(p => p.Decimal).WithDisplayName("Value").WithDescription("Is a value in decimal");
            configBuilder.Property(p => p.LlamaCapacity)
                .WithDescription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.StartDate)
                .WithMinValue(new DateTime(2013, 10, 10));
            configBuilder.Property(p => p.Name).WithMaxLength(250);
            configBuilder.PropertyWithConfigurationSetOptions(p => p.Option, (SampleConfigSet set) => set.Options)
                .WithDescription("Is a selected option");
            configBuilder.PropertyWithMultipleConfigurationSetOptions(p => p.MoarOptions, (SampleConfigSet set) => set.Options)
                .WithDescription("Is a multi select option");
            configBuilder.Collection(p=> p.ListOfConfigs);

            modelBuilder.Options(p => p.Options, op => op.Id, op => op.Description, (IOptionProvider provider) => provider.GetOptions());
        }
    }
}
