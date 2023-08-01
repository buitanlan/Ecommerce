using System;

namespace Ecommerce.Admin.Products;

public class ProductAttributeListFilterDto : BaseListFilterDto
{
    public Guid ProductId { get; set; }
}
