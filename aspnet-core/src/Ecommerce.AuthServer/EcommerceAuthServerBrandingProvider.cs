using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Ecommerce;

[Dependency(ReplaceServices = true)]
public class EcommerceAuthServerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "EcommerceAuthServer";
}
