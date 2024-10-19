using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Public.Application.Contracts;
using Ecommerce.ProductAttributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Catalog.ProductAttributes;

public class ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository) : ReadOnlyAppService<
    ProductAttribute,
    ProductAttributeDto,
    Guid,
    PagedResultRequestDto>(repository), IProductAttributesAppService
{
    public async Task<List<ProductAttributeInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
    }

    public async Task<PagedResult<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Label.Contains(input.Keyword));

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize)
            .Take(input.PageSize));
        return new PagedResult<ProductAttributeInListDto>(
            ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data), totalCount, input.CurrentPage, input.PageSize);
    }
}
