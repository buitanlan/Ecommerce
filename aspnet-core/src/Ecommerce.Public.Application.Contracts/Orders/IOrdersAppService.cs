using System;
using Ecommerce.Public.Orders;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Public.Orders;

public interface IOrdersAppService : ICrudAppService<OrderDto, Guid, PagedResultRequestDto, CreateOrderDto, CreateOrderDto>;
