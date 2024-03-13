using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Public.Web.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}
