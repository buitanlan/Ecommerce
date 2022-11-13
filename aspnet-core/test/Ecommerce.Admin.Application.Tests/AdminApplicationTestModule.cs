using Volo.Abp.Modularity;

namespace Ecommerce.Admin;

[DependsOn(
    typeof(AdminApplicationModule),
    typeof(AdminDomainTestModule)
    )]
public class AdminApplicationTestModule : AbpModule
{

}
