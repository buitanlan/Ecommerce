using System.Collections.Generic;
using Ecommerce.Public.ProductCategories;

namespace Ecommerce.Public.Web.Models;

public class HeaderCacheItem
{
    public List<ProductCategoryInListDto> Categories { set; get; }
}
