using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Public.Application.Contracts;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Public.Catalog.Manufacturers;

public interface IManufacturersAppService : IReadOnlyAppService<ManufacturerDto, Guid, PagedResultRequestDto>
{
    Task<PagedResult<ManufacturerInListDto>> GetListFilterAsync(BaseListFilterDto input);
    Task<List<ManufacturerInListDto>> GetListAllAsync();
}
