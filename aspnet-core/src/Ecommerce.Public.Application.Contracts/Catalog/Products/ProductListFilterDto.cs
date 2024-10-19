using System;

namespace Ecommerce.Catalog.Products;

 public class ProductListFilterDto : BaseListFilterDto
{
    public Guid? CategoryId { get; set; }
}
