# ConfigServer
Handling of application configuration storage and management across multiple instances.

[![Build status](https://ci.appveyor.com/api/projects/status/qmqipgjgyg70r3su/branch/master?svg=true)](https://ci.appveyor.com/project/PjMitchell/configserver/branch/master)

## Core concepts

#### ConfigServer
Servers the configurations to the various clients

#### Configuration set
Represents a collection of associated configuration. The configuration set contains meta data and information on validating the configuration, all of which can be declared via a fluent api.
Currently support configuration classes with primative properties.

#### Option set
Represents a collection of config items that can be used on their own or referenced by other configs.
Option set may be internal to config server or from an external provider

#### ConfigRepository
Store for the configurations.
Currently support:
* In Memory store
* File store
* Azure Blob storage
* Azure Table storage

#### ConfigServer Manager
Provides configurators a way of configuring associated client configuration through a simple web interface with meta data and validation.
Access through configserverpath/Manager

#### ConfigServer Client 
Client used by application to retrieve configuration. Local and HttpClients available.
Configuration can be retrieved by injecting IConfigServer.
Currently will have issues if multiple config of the same type are expected.

## Setup

#### Setting up ConfigServer

Adding ConfigServer and configuration sets to service collection:

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigServer()
            .UseConfigSet<SampleConfigSet>()
            .UseInMemoryProvider();
        //...
```
Adding ConfigServer to OWIN pipeline:

```csharp
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {          
        app.Map("/Config", configSrv => configSrv.UseConfigServer(new ConfigServerOptions()));
```

#### Setting up a Configuration Set
```csharp
    public class SampleConfigSet : ConfigurationSet<SampleConfigSet>
    {
        public OptionSet<Option> OptionFromProvider { get; set; }
        public OptionSet<OptionFromConfigSet> Options { get; set; } 
        public Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetModelBuilder<SampleConfigSet> modelBuilder)
        {
            modelBuilder.Options(s => s.Options, o => o.Id, o => o.Description, "Options", "Options for sample config");
            modelBuilder.Options(s => s.OptionFromProvider, o => o.Id, o => o.Description, (IOptionProvider provider) => provider.GetOptions()); 
            
            var configBuilder = modelBuilder.Config(x=> x.SampleConfig);

            configBuilder.Property(p => p.IsLlamaFarmer).WithDisplayName("Is Llama farmer?").WithDescription("Is this a Llama farmer");
            configBuilder.Property(p => p.Decimal).WithDisplayName("Value").WithDescription("Is a value in decimal");
            configBuilder.Property(p => p.LlamaCapacity).WithDisplayName("Llama capacity")
                .WithDescription("Is the capacity of llama")
                .WithMinValue(0)
                .WithMaxValue(50);
            configBuilder.Property(p => p.StartDate).WithDisplayName("Start date")
                .WithMinValue(new DateTime(2013, 10, 10));
            configBuilder.Property(p => p.Name).WithMaxLength(250);
            configBuilder.PropertyWithOption(p => p.Option, (SampleConfigSet set) => set.OptionFromProvider)
                .WithDescription("Is a selected option");
        }
    }
```


#### Setting up ConfigServer Client

Adding local client to ConfigServer:

```csharp
        services.AddConfigServer()
        .UseConfigSet<SampleConfigSet>()
        .UseInMemoryProvider()
        .UseLocalConfigServerClient(clientId)
        .WithConfig<SampleConfig>();
```

Adding remote client to application
```csharp
        services.AddConfigServerClient(new ConfigServerClientOptions
        {
            ClientId = "6A302E7D-05E9-4188-9612-4A2920E5C1AE",
            ConfigServer = "http://localhost:58201/Config"
        })
        .WithConfig<SampleConfig>()
        .WithCollectionConfig<OptionFromConfigSet>();;
```

```csharp
        services.AddConfigServerClient(new ConfigServerClientOptions
        {
            ClientId = "6A302E7D-05E9-4188-9612-4A2920E5C1AE",
            ConfigServer = "http://localhost:58201/Config"
        })
        .WithConfig<SampleConfig>()
        .WithCollectionConfig<OptionFromConfigSet>();
```

Consuming configuration

```csharp
        public HomeController(IConfigServer configServer)
        {
            config = configServer.GetConfig<SampleConfig>();
            options = configServer.GetCollectionConfig<OptionFromConfigSet>();
        }
```

```csharp
        public async Task<IActionResult> Index()
        {
            var config = await configProvider.GetConfigAsync<SampleConfig>();
            var options = await configProvider.GetCollectionConfigAsync<OptionFromConfigSet>();

            return View(new ConfigViewModel { Config = config, Options = options });
        }
```


## Samples

#### ConfigServer.Sample.mvc

Includes self hosted ConfigServer and configurator along with local client to consume the configuration.

#### ConfigServer.Sample.mvc

Includes httpClient that gets configuration from ConfigServer.Sample.mvc
