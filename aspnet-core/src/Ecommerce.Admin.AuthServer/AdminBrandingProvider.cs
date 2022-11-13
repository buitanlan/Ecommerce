using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Ecommerce.Admin;

[Dependency(ReplaceServices = true)]
public class AdminBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Admin";
}
