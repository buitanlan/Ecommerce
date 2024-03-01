using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Ecommerce.Admin.Permissions;
using Ecommerce.Manufacturers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Manufacturers;

[Authorize(EcommercePermissions.Manufacturer.Default, Policy = "AdminOnly")]
public class ManufacturersAppService : CrudAppService<
    Manufacturer,
    ManufacturerDto,
    Guid,
    PagedResultRequestDto,
    CreateUpdateManufacturerDto,
    CreateUpdateManufacturerDto>, IManufacturersAppService
{

    public ManufacturersAppService(IRepository<Manufacturer, Guid> repository) : base(repository)
    {
        GetPolicyName = EcommercePermissions.Manufacturer.Default;
        GetListPolicyName = EcommercePermissions.Manufacturer.Default;
        CreatePolicyName = EcommercePermissions.Manufacturer.Create;
        UpdatePolicyName = EcommercePermissions.Manufacturer.Update;
        DeletePolicyName = EcommercePermissions.Manufacturer.Delete;
    }

    [Authorize(EcommercePermissions.Manufacturer.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    [Authorize(EcommercePermissions.Manufacturer.Default)]
    public async Task<List<ManufacturerInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x=>x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Manufacturer>, List<ManufacturerInListDto>>(data);

    }

    [Authorize(EcommercePermissions.Manufacturer.Default)]
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
