using Ecommerce.Localization;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin;

/* Inherit your application services from this class.
 */
public abstract class AdminAppService : ApplicationService
{
    protected AdminAppService()
    {
        LocalizationResource = typeof(EcommerceResource);
    }
}
