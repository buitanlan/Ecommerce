using System;
using System.Collections.Generic;
using System.Text;
using Ecommerce.Admin.Localization;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin;

/* Inherit your application services from this class.
 */
public abstract class AdminAppService : ApplicationService
{
    protected AdminAppService()
    {
        LocalizationResource = typeof(AdminResource);
    }
}
