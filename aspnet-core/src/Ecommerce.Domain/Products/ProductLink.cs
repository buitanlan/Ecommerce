using System;
using Volo.Abp.Domain.Entities;

namespace Ecommerce.Products;

public class ProductLink : Entity
{
    public Guid ProductId { get; set; }
    public Guid LinkedProductId { get; set; }
    public override object[] GetKeys()
    {
        return [ProductId, LinkedProductId];
    }
}
