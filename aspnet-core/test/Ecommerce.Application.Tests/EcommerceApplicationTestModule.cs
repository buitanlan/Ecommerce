using Ecommerce.Public;
using Volo.Abp.Modularity;

namespace Ecommerce;

[DependsOn(
    typeof(EcommercePublicApplicationModule),
    typeof(EcommerceDomainTestModule)
    )]
public class EcommerceApplicationTestModule : AbpModule;
