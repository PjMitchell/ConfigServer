using ConfigServer.Core;
using ConfigServer.Server;
using System;

namespace ConfigServer.Sample.mvc.Models
{
    public class SampleConfigSet : ConfigurationSet
    {
        public SampleConfigSet() : base("Core Configuration Set", "Only Configuration Set in the app") {}

        Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetModelBuilder modelBuilder)
        {
            var configBuilder = modelBuilder.Config<SampleConfig>("Sample Config", "Basic Configuration");

            configBuilder.Property(p => p.IsLlamaFarmer);
            configBuilder.Property(p => p.Decimal).WithDisplayName("Value").WithDescription("Is a value in decimal");
            configBuilder.Property(p => p.LlamaCapacity)
                .WithDescription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.StartDate)
                .WithMinValue(new DateTime(2013, 10, 10));
            configBuilder.Property(p => p.Name).WithMaxLength(250);
            configBuilder.PropertyWithOptions(p => p.Option, (IOptionProvider provider) => provider.GetOptions(), op => op.Id, op => op.Description)
                .WithDescription("Is a selected option");
            configBuilder.PropertyWithMulitpleOptions(p => p.MoarOptions, (IOptionProvider provider) => provider.GetOptions(), op => op.Id, op => op.Description)
                .WithDescription("Is a multi select option");
            configBuilder.Collection(p=> p.ListOfConfigs);


        }
    }
}
