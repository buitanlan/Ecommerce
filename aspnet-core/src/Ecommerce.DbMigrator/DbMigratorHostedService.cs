﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ecommerce.Data;
using Ecommerce.Seeding;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace Ecommerce.DbMigrator;

public class DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var application = await AbpApplicationFactory.CreateAsync<EcommerceDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        });
        await application.InitializeAsync();

        await application
            .ServiceProvider
            .GetRequiredService<EcommerceDbMigrationService>()
            .MigrateAsync();
        
        await application
            .ServiceProvider
            .GetRequiredService<IdentityDataSeeder>()
            .SeedAsync("admin@gmail.com","Pa$$w0rd");

        await application.ShutdownAsync();

        hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
