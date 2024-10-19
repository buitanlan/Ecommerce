using System;

namespace Ecommerce.Catalog.Products.Attributes;

    public class ProductAttributeListFilterDto : BaseListFilterDto
    {
        public Guid ProductId { get; set; }
    }
