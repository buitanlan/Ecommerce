using Ecommerce.Admin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ecommerce.Admin.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AdminEntityFrameworkCoreModule),
    typeof(AdminApplicationContractsModule)
    )]
public class AdminDbMigratorModule : AbpModule
{

}
