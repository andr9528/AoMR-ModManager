using System.Diagnostics;
using System.Text.Json;
using ModManager.Abstractions.Services;
using ModManager.Abstractions.Startup;
using ModManager.Services;
using ModManager.Strings;
using Uno.Extensions.Localization;
using System.Reflection;
using Serilog;
using Uno.Extensions.Configuration;
using Uno.Resizetizer;
using Path = System.IO.Path;

namespace ModManager;

internal class Startup
{
    private ICollection<IStartupModule> modules;

    private IServiceCollection Services { get; set; }
    public IServiceProvider ServiceProvider { get; private set; }
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
        string logDirectory = Path.Combine(Environment.CurrentDirectory, FileService.MOD_MANAGER_FOLDER, "Logs");
        Directory.CreateDirectory(logDirectory);

        string logFilePath = Path.Combine(logDirectory, "log-.log");

        var loggerConfig = new LoggerConfiguration();
        if (builderContext.HostingEnvironment.IsDevelopment())
        {
            loggerConfig.MinimumLevel.Debug();
        }
        else
        {
            loggerConfig.MinimumLevel.Information();
        }

        logBuilder.AddSerilog(loggerConfig.Enrich.FromLogContext().WriteTo.Console().WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
                outputTemplate:
                "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{SourceContext}]: {Message:lj}{NewLine}{Exception}")
            .CreateLogger());
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

        var logger = ServiceProvider.GetRequiredService<ILogger<Startup>>();
        logger.LogInformation("");
        logger.LogInformation("======== Application started at {Timestamp} ========",
            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
        logger.LogInformation("Current Directory: {CurrentDirectory}", Environment.CurrentDirectory);
        logger.LogInformation("Base Directory: {BaseDirectory}", AppContext.BaseDirectory);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();

        services.AddSingleton<IStateService, StateService>();
        services.AddSingleton<ITranslationService, TranslationService>();
    }
}
