using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Public.Application.Contracts;
using Ecommerce.Catalog.Products;
using Ecommerce.Public.Catalog.Products;
using Ecommerce.Public.ProductCategories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Public.Web.Pages.Products;

 public class CategoryModel(
     IProductCategoriesAppService productCategoriesAppService,
     IProductsAppService productsAppService)
     : PageModel
 {
        public ProductCategoryDto Category { set; get; }

        public List<ProductCategoryInListDto> Categories { set; get; }
        public PagedResult<ProductInListDto> ProductData { set; get; }

        public async Task OnGetAsync(string code,int page = 1)
        {
            Category = await productCategoriesAppService.GetByCodeAsync(code);
            Categories = await productCategoriesAppService.GetListAllAsync();
            ProductData = await productsAppService.GetListFilterAsync(new ProductListFilterDto()
            {
                CurrentPage = page
            });
        }
    }
