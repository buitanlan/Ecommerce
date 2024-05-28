﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Ecommerce.EntityFrameworkCore;
using Ecommerce.BackgroundWorkers.MailCampaigns;
using TeduEcommerce.BackgroundWorkers;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace Ecommerce.BackgroundWorkers
{
    [DependsOn(
      typeof(AbpAutofacModule),
      typeof(AbpBackgroundWorkersModule),
      typeof(EcommerceEntityFrameworkCoreModule),
      typeof(AbpCachingStackExchangeRedisModule)
  )]
    public class EcommerceBackgroundWorkersModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            context.Services.AddHostedService<EcommerceBackgroundWorkersHostedService>();

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "TEDU:";
            });
            var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("TEDU");
            if (!hostEnvironment.IsDevelopment())
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "TEDU-Protection-Keys");
            }

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorkerAsync<EmailMarketingWorker>();
        }
    }
}
