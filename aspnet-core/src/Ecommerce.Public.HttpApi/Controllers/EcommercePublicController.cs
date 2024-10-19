using Ecommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class EcommercePublicController : AbpControllerBase
{
    protected EcommercePublicController()
    {
        LocalizationResource = typeof(EcommerceResource);
    }
}
