using System.Text.Json;
using ModManager.Abstractions.Services;
using ModManager.Abstractions.Startup;
using ModManager.Services;
using ModManager.Strings;
using Uno.Extensions.Localization;
using System.Reflection;
using Uno.Extensions.Configuration;
using Uno.Resizetizer;

namespace ModManager;

internal class Startup
{
    private ICollection<IStartupModule> modules;

    public IServiceCollection Services { get; protected set; }
    public IServiceProvider ServiceProvider { get; protected set; }
    public IHost? Host { get; private set; }

    public Startup()
    {
        modules = new List<IStartupModule>();
    }

    public IApplicationBuilder SetupApplication(IApplicationBuilder builder)
    {
        ConfigureApplication(builder);
        foreach (IStartupModule module in modules)
        {
            module.ConfigureApplication(builder);
        }

        return builder;
    }

    private void ConfigureApplication(IApplicationBuilder app)
    {
        app.Configure(host => host
#if DEBUG
            // Switch to Development environment when running in DEBUG
            .UseEnvironment(Environments.Development)
#endif
            .UseLogging(ConfigureLogging, true).UseSerilog(true, true)
            .UseConfiguration(configure: ConfigureConfigurationSource).UseLocalization(ConfigureLocalization)
            .UseSerialization(ConfigureSerialization));
    }

    private void ConfigureLocalization(HostBuilderContext builderContext, IServiceCollection serviceCollection)
    {
        // Doesn't appear to be run, when checking the Uno Platform source code.

        // Allows for additional configuration regarding localization
        // See appsettings.json for supported languages.
    }

    private void ConfigureSerialization(HostBuilderContext builderContext, IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(new JsonSerializerOptions {IncludeFields = true,});
    }

    private IHostBuilder ConfigureConfigurationSource(IConfigBuilder configBuilder)
    {
        return configBuilder.EmbeddedSource<App>().Section<AppConfig>();
    }

    private void ConfigureLogging(HostBuilderContext builderContext, ILoggingBuilder logBuilder)
    {
        // Configure log levels for different categories of logging
        logBuilder.SetMinimumLevel(builderContext.HostingEnvironment.IsDevelopment()
                ? LogLevel.Information
                : LogLevel.Warning)

            // Default filters for core Uno Platform namespaces
            .CoreLogLevel(LogLevel.Warning);

        // Uno Platform namespace filter groups
        // Uncomment individual methods to see more detailed logging
        //// Generic Xaml events
        //logBuilder.XamlLogLevel(LogLevel.Debug);
        //// Layout specific messages
        //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
        //// Storage messages
        //logBuilder.StorageLogLevel(LogLevel.Debug);
        //// Binding related messages
        //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
        //// Binder memory references tracking
        //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
        //// DevServer and HotReload related
        //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
    }

    public void SetupServices(IServiceCollection? services = null)
    {
        Services = services ??= new ServiceCollection();

        ConfigureServices(services);
        foreach (IStartupModule module in modules)
        {
            module.ConfigureServices(Services);
        }


        ServiceProvider = Services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();

        services.AddSingleton<IStateService, StateService>();
        services.AddSingleton<ITranslationService, TranslationService>();
    }
}
