﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Ecommerce.Manufacturers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Manufacturers;

[Authorize]
public class ManufacturersAppService(IRepository<Manufacturer, Guid> repository) : CrudAppService<
    Manufacturer,
    ManufacturerDto,
    Guid,
    PagedResultRequestDto,
    CreateUpdateManufacturerDto,
    CreateUpdateManufacturerDto>(repository), IManufacturersAppService
{
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    public async Task<List<ManufacturerInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x=>x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(data);

    }

    public async Task<PagedResultDto<ManufacturerInListDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ManufacturerInListDto>(totalCount,ObjectMapper.Map<List<Manufacturer>,List<ManufacturerInListDto>>(data));
    }
}
