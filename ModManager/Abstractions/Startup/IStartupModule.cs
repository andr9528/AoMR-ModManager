namespace ModManager.Abstractions.Startup;

public interface IStartupModule
{
    void ConfigureServices(IServiceCollection services);

    void ConfigureApplication(IApplicationBuilder builder);
}
