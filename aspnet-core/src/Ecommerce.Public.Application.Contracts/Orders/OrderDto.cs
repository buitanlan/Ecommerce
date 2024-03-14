﻿using System;
using Ecommerce.Orders;
using Volo.Abp.Application.Dtos;

namespace Ecommerce.Public.Orders;

public class OrderDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public double ShippingFee { get; set; }
    public double Tax { get; set; }
    public double Total { get; set; }
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double GrandTotal { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhoneNumber { get; set; }
    public string CustomerAddress { get; set; }
    public Guid? CustomerUserId { get; set; }
}
