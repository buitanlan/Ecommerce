using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Ecommerce.Public.Web;

[Dependency(ReplaceServices = true)]
public class EcommercePublicBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Public";
}
