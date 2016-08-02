using ConfigServer.Core;
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

            configBuilder.Property(p => p.IsLlamaFarmer).WithDisplayName("Is Llama farmer?").WithDescription("Is this a Llama farmer");
            configBuilder.Property(p => p.Decimal).WithDisplayName("Value").WithDescription("Is a value in decimal");
            configBuilder.Property(p => p.LlamaCapacity).WithDisplayName("Llama capacity")
                .WithDescription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.StartDate).WithDisplayName("Start date")
                .WithMinValue(new DateTime(2013, 10, 10));
            configBuilder.Property(p => p.Name).WithMaxLength(250);

        }
    }
}
