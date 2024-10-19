using System;
using Volo.Abp.Application.Dtos;

namespace Ecommerce.Public.Orders;

public class OrderItemDto : EntityDto
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}
