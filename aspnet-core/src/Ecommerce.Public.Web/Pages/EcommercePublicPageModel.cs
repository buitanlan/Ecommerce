using Ecommerce.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Ecommerce.Public.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class EcommercePublicPageModel : AbpPageModel
{
    protected EcommercePublicPageModel()
    {
        LocalizationResourceType = typeof(EcommerceResource);
    }
}
