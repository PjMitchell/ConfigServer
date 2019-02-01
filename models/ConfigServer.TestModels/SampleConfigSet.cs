using ConfigServer.Server;
using System;

namespace ConfigServer.TestModels
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
            configBuilder.PropertyWithOptionValue(p => p.OptionId, (SampleConfigSet set) => set.OptionFromProvider, option => option.Id);

            configBuilder.Collection(p => p.ListOfInts).HasUniqueValues();
            configBuilder.Collection(p => p.ListOfStrings).HasUniqueValues();



            modelBuilder.Options(s => s.Options, o=> o.Id, o=> o.Description, "Options", "Options for sample config");
            modelBuilder.Options(s => s.OptionFromProvider, o => o.Id, o => o.Description , (IOptionProvider optionProvider) => optionProvider.GetOptions(), "Options", "Options for sample config");

        }
    }
}
