using Ecommerce.DbMigrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
    .MinimumLevel.Override("Ecommerce", LogEventLevel.Debug)
#else
    .MinimumLevel.Override("Ecommerce", LogEventLevel.Information)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

await Host.CreateDefaultBuilder(args)
    .AddAppSettingsSecretsJson()
    .ConfigureLogging((_, logging) => logging.ClearProviders())
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<DbMigratorHostedService>();
    }).RunConsoleAsync();
