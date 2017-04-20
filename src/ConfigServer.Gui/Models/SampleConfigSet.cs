using ConfigServer.Core;
using ConfigServer.Gui.Models;
using ConfigServer.Server;
using System;

namespace ConfigServer.Gui.Models
{
    public class SampleConfigSet : ConfigurationSet<SampleConfigSet>
    {
        public SampleConfigSet() : base("Core Configuration Set", "Only Configuration Set in the app") {}

        public OptionSet<OptionFromConfigSet> Options { get; set; }
        public OptionSet<Option> OptionFromProvider { get; set; }


        public Config<SampleConfig> SampleConfig { get; set; }

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
            configBuilder.PropertyWithOption(p => p.Option, (SampleConfigSet set) => set.OptionFromProvider)
                .WithDescription("Is a selected option");

            configBuilder.PropertyWithMultipleOptions(p => p.MoarOptions, (SampleConfigSet set) => set.OptionFromProvider)
                .WithDescription("Is a multi select option");

            configBuilder.Collection(p=> p.ListOfConfigs)
                .WithUniqueKey(x=>x.Name);

            configBuilder.PropertyWithOption(p => p.OptionFromConfigSet, (SampleConfigSet set) => set.Options)
                .WithDescription("Options from the option set");

            configBuilder.PropertyWithMultipleOptions(p => p.MoarOptionFromConfigSet, (SampleConfigSet set) => set.Options)
                .WithDescription("More options from the option set");
            configBuilder.PropertyWithOptionValue(p => p.OptionId, (SampleConfigSet set) => set.OptionFromProvider, option => option.Id);
            configBuilder.PropertyWithMultipleOptionValues(p => p.MoarOptionValues, (SampleConfigSet set) => set.OptionFromProvider, option => option.Id);


            modelBuilder.Options(s => s.Options, o=> o.Id, o=> o.Description, "Options", "Options for sample config");
            modelBuilder.Options(s => s.OptionFromProvider, o => o.Id, o => o.Description , (IOptionProvider optionProvider) => optionProvider.GetOptions(), "Options", "Options for sample config");

        }
    }
}
