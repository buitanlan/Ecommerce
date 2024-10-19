using System.Collections.Generic;
using Ecommerce.Catalog.Products;
using Ecommerce.Public.ProductCategories;

namespace Ecommerce.Public.Web.Models;

public class HomeCacheItem
{
    public List<ProductCategoryInListDto> Categories { set; get; }
    public List<ProductInListDto> TopSellerProducts { set; get; }
}
