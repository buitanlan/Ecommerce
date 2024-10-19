using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Public.Application.Contracts;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Public.Catalog.ProductAttributes;

public interface IProductAttributesAppService : IReadOnlyAppService<ProductAttributeDto, Guid, PagedResultRequestDto>
{
    Task<PagedResult<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ProductAttributeInListDto>> GetListAllAsync();
}
