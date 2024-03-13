using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Manufacturers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Catalog.Manufacturers;

public class ManufacturersAppService(IRepository<Manufacturer, Guid> repository) : ReadOnlyAppService
<Manufacturer,
    ManufacturerDto,
    Guid,
    PagedResultRequestDto>(repository), IManufacturersAppService
{
    public async Task<List<ManufacturerInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(data);
    }

    public async Task<PagedResultDto<ManufacturerInListDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ManufacturerInListDto>(totalCount,
            ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(data));
    }
}
