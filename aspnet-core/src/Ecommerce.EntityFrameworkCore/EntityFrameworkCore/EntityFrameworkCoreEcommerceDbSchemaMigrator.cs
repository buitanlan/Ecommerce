using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Data;
using Volo.Abp.DependencyInjection;

namespace Ecommerce.EntityFrameworkCore;

public class EntityFrameworkCoreEcommerceDbSchemaMigrator
    (IServiceProvider serviceProvider) : IEcommerceDbSchemaMigrator, ITransientDependency
{
    public async Task MigrateAsync()
    {
        /* We intentionally resolving the EcommerceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await serviceProvider
            .GetRequiredService<EcommerceDbContext>()
            .Database
            .MigrateAsync();
    }
}
