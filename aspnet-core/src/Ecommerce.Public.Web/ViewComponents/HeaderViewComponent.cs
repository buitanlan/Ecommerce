using System.Threading.Tasks;
using Ecommerce.Public.ProductCategories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Public.Web.ViewComponents;

public class HeaderViewComponent(IProductCategoriesAppService productCategoriesAppService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await productCategoriesAppService.GetListAllAsync();
        return View(model);
    }
}
