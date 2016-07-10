# ConfigServer

Handling of application configuration storage and management across multiple instances.

## Core concepts

#### ConfigServer
Servers the configurations to the various clients

#### Configuration set
Represents a collection of associated configuration. The configuration set contains meta data and information on validating the configuration, all of which can be declared via a fluent api.
Currently support configuration classes with primative properties.

#### ConfigRepository
Store for the configurations.
Currently support:
* In Memory store
* File store

#### ConfigServer Configurator
Provides configurators a way of configuring associated client configuration through a simple web interface with meta data and validation.

#### ConfigServer Client 
Client used by application to retrieve configuration. Local and HttpClients available.
Configuration is placed in service collection or can be retrieved by injecting IConfigServerClient.
Currently will have issues if multiple config of the same type are expected.

## Setup

#### Setting up ConfigServer and Configurator

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
        
        //Setup configurator
        app.UseConfigServerConfigurator(new ConfigServerConfiguratorOptions());
        //...
```

#### Setting up a Configuration Set

```csharp
    public class SampleConfigSet : ConfigurationSet
    {
        Config<SampleConfig> SampleConfig { get; set; }

        protected override void OnModelCreation(ConfigurationSetModelBuilder modelBuilder)
        {
            var configBuilder = modelBuilder.Config<SampleConfig>();

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
        .WithConfig<SampleConfig>();
```

## Samples

#### ConfigServer.Sample.mvc

Includes self hosted ConfigServer and configurator along with local client to consume the configuration.

#### ConfigServer.Sample.mvc

Includes httpClient that gets configuration from ConfigServer.Sample.mvc
