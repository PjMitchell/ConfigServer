using ConfigServer.Core;
using ConfigServer.Gui.Models;
using ConfigServer.Server;
using System;

namespace ConfigServer.Sample.mvc.Models
{
    public class SampleConfigSet : ConfigurationSet<SampleConfigSet>
    {
        public SampleConfigSet() : base("Core Configuration Set", "Only Configuration Set in the app") {}

        OptionSet<OptionFromConfigSet> Options { get; set; }

        Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetModelBuilder<SampleConfigSet> modelBuilder)
        {
            var configBuilder = modelBuilder.Config(s=>s.SampleConfig, "Sample Config", "Basic Configuration");

            configBuilder.Property(p => p.IsLlamaFarmer);
            configBuilder.Property(p => p.Decimal)
                .WithDisplayName("Value")
                .WithDescription("Is a value in decimal");

            configBuilder.Property(p => p.LlamaCapacity)
                .WithDescription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.SpareLlamaCapacity)
                .WithDescription("Some spare capacity for LLamas")
                .WithMaxValue(50);
            
            configBuilder.Property(p => p.StartDate)
                .WithMinValue(new DateTime(2013, 10, 10));

            configBuilder.Property(p => p.Name).WithMaxLength(250);
            configBuilder.PropertyWithOptions(p => p.Option, (IOptionProvider provider) => provider.GetOptions(), op => op.Id, op => op.Description)
                .WithDescription("Is a selected option");

            configBuilder.PropertyWithMulitpleOptions(p => p.MoarOptions, (IOptionProvider provider) => provider.GetOptions(), op => op.Id, op => op.Description)
                .WithDescription("Is a multi select option");

            configBuilder.Collection(p=> p.ListOfConfigs)
                .WithUniqueKey(x=>x.Name);

            configBuilder.PropertyWithConfigurationSetOptions(p => p.OptionFromConfigSet, (SampleConfigSet set) => set.Options)
                .WithDescription("Options from the option set");

            configBuilder.PropertyWithMultipleConfigurationSetOptions(p => p.MoarOptionFromConfigSet, (SampleConfigSet set) => set.Options)
                .WithDescription("More options from the option set");

            var optionBuilder = modelBuilder.Options(s => s.Options, o=> o.Id, o=> o.Description, "Options", "Options for sample config");
        }
    }
}
