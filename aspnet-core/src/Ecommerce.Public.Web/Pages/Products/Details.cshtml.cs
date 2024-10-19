using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Ecommerce.Public.Catalog.Products;
using Ecommerce.Public.ProductCategories;

namespace Ecommerce.Public.Web.Pages.Products
{
    public class DetailsModel(
        IProductsAppService productsAppService,
        IProductCategoriesAppService productCategoriesAppService)
        : PageModel
    {
        public ProductCategoryDto Category { get; set; }
        public ProductDto Product { get; set; }
        public async Task OnGetAsync(string categorySlug, string slug)
        {
            Category = await productCategoriesAppService.GetBySlugAsync(categorySlug);
            Product = await productsAppService.GetBySlugAsync(slug);
        }
    }
}
