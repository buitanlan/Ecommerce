using System;
using Volo.Abp.Domain.Entities;

namespace Ecommerce.Products;

public class ProductAttributeDateTime : Entity<Guid>
{
    public ProductAttributeDateTime(Guid id, Guid attributeId, Guid productId, DateTime? value)
    {
        Id = id;
        AttributeId = attributeId;
        ProductId = productId;
        Value = value;
    }
    public Guid AttributeId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime? Value { get; set; }
}
