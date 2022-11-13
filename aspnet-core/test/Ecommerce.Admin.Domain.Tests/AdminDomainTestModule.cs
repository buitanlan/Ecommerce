using Ecommerce.Admin.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ecommerce.Admin;

[DependsOn(
    typeof(AdminEntityFrameworkCoreTestModule)
    )]
public class AdminDomainTestModule : AbpModule
{

}
