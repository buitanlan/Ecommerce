using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Orders;
using Ecommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Orders;

public class OrdersAppService(
    IRepository<Order, Guid> repository,
    OrderCodeGenerator orderCodeGenerator,
    IRepository<OrderItem> orderItemRepository,
    IRepository<Product, Guid> productRepository)
    : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto, CreateOrderDto, CreateOrderDto>(repository), IOrdersAppService
{
    public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var subTotal = input.Items.Sum(x => x.Quantity * x.Price);
        var orderId = Guid.NewGuid();
        var order = new Order(orderId)
        {
            Code = await orderCodeGenerator.GenerateAsync(),
            CustomerAddress = input.CustomerAddress,
            CustomerName = input.CustomerName,
            CustomerPhoneNumber = input.CustomerPhoneNumber,
            ShippingFee = 0,
            CustomerUserId = input.CustomerUserId,
            Tax = 0,
            Subtotal = subTotal,
            GrandTotal = subTotal,
            Discount = 0,
            PaymentMethod = PaymentMethod.COD,
            Total = subTotal,
            Status = OrderStatus.New
        };
        var items = new List<OrderItem>();
        foreach (var item in input.Items)
        {
            var product = await productRepository.GetAsync(item.ProductId);
            items.Add(new OrderItem()
            {
                OrderId = orderId,
                Price = item.Price,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                SKU = product.SKU
            });
        }

        await orderItemRepository.InsertManyAsync(items);
        var result = await Repository.InsertAsync(order);
        return ObjectMapper.Map<Order, OrderDto>(result);
    }
}
