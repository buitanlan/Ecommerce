using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Admin.Permissions;
using Ecommerce.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.ProductCategories;

[Authorize(EcommercePermissions.ProductCategory.Default, Policy = "AdminOnly")]
public class ProductCategoriesAppService: CrudAppService<
    ProductCategory,
    ProductCategoryDto,
    Guid,
    PagedResultRequestDto,
    CreateUpdateProductCategoryDto,
    CreateUpdateProductCategoryDto>, IProductCategoriesAppService
{
    public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
    {
        GetPolicyName = EcommercePermissions.ProductCategory.Default;
        GetListPolicyName = EcommercePermissions.ProductCategory.Default;
        CreatePolicyName = EcommercePermissions.ProductCategory.Create;
        UpdatePolicyName = EcommercePermissions.ProductCategory.Update;
        DeletePolicyName = EcommercePermissions.ProductCategory.Delete;
    }
    
    [Authorize(EcommercePermissions.ProductCategory.Default)]
    public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
        var totalCount = await AsyncExecuter.CountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));
        return new PagedResultDto<ProductCategoryInListDto>(totalCount,
            ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data));

    }

    [Authorize(EcommercePermissions.ProductCategory.Default)]
    public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);
        return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
    }

    [Authorize(EcommercePermissions.ProductCategory.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }
}
