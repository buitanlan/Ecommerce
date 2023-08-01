using System;
using Volo.Abp.Domain.Entities;

namespace Ecommerce.Products;

public class ProductAttributeVarchar : Entity<Guid>
{
    public ProductAttributeVarchar(Guid id, Guid attributeId, Guid productId, string value)
    {
        Id = id;
        AttributeId = attributeId;
        ProductId = productId;
        Value = value;
    }
    public Guid AttributeId { get; set; }
    public Guid ProductId { get; set; }
    public string Value { get; set; }
}
