using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Public.Web.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}
