using System;

namespace Ecommerce.Admin.Products;

public class ProductListFilterDto: BaseListFilterDto
{
    public Guid? CategoryId { get; set; }
    
}