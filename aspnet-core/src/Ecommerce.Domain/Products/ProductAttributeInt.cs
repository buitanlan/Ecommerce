using System;
using Volo.Abp.Domain.Entities;

namespace Ecommerce.Products;

public class ProductAttributeInt : Entity<Guid>
{
    public ProductAttributeInt(Guid id, Guid attributeId, Guid productId, int value)
    {
        Id = id;
        AttributeId = attributeId;
        ProductId = productId;
        Value = value;
    }
    public Guid AttributeId { get; set; }
    public Guid ProductId { get; set; }
    public int? Value { get; set; }
}
