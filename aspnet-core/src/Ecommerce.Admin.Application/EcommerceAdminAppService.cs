using Ecommerce.Localization;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin;

/* Inherit your application services from this class.
 */
public abstract class EcommerceAdminAppService : ApplicationService
{
    protected EcommerceAdminAppService()
    {
        LocalizationResource = typeof(EcommerceResource);
    }
}
