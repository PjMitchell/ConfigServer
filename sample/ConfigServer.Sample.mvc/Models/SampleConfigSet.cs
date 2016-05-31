using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Sample.mvc.Models
{
    public class SampleConfigSet : ConfigurationSet
    {
        Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetBuilder modelBuilder)
        {
            var configBuilder = modelBuilder.Config<SampleConfig>();

            configBuilder.Property(p => p.IsLlamaFarmer).WithDisplayName("Is Llama farmer?").WithDiscription("Is this a Llama farmer");
            configBuilder.Property(p => p.Decimal).WithDisplayName("Value").WithDiscription("Is a value in decimal");
            configBuilder.Property(p => p.LlamaCapacity).WithDisplayName("Llama capacity")
                .WithDiscription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.StartDate).WithDisplayName("Start date")
                .WithMinValue(new DateTime(2013, 10, 10));
            configBuilder.Property(p => p.Name).WithMaxLength(250);

        }
    }
}
