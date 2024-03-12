using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Admin.Permissions;
using Ecommerce.ProductAttributes;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.ProductAttributes;

[Authorize(EcommerceAdminPermissions.Attribute.Default, Policy = "AdminOnly")]
public class ProductAttributesAppService : CrudAppService<
    ProductAttribute,
    ProductAttributeDto,
    Guid,
    PagedResultRequestDto,
    CreateUpdateProductAttributeDto,
    CreateUpdateProductAttributeDto>, IProductAttributesAppService
{
    public ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
    {
        GetPolicyName = EcommerceAdminPermissions.Attribute.Default;
        GetListPolicyName = EcommerceAdminPermissions.Attribute.Default;
        CreatePolicyName = EcommerceAdminPermissions.Attribute.Create;
        UpdatePolicyName = EcommerceAdminPermissions.Attribute.Update;
        DeletePolicyName = EcommerceAdminPermissions.Attribute.Delete;

    }

    [Authorize(EcommerceAdminPermissions.Attribute.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    [Authorize(EcommerceAdminPermissions.Attribute.Default)]
    public async Task<List<ProductAttributeInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);

    }

    [Authorize(EcommerceAdminPermissions.Attribute.Default)]
    public async Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Label.Contains(input.Keyword));

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductAttributeInListDto>(totalCount,
            ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data));
    }
}
