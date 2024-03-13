using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Public.Application.Contracts;
using Ecommerce.ProductCategories;
using Ecommerce.Public.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Catalog.ProductCategories;

public class ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : CrudAppService<
    ProductCategory,
    ProductCategoryDto,
    Guid,
    PagedResultRequestDto>(repository), IProductCategoriesAppService
{
        public async Task<ProductCategoryDto> GetByCodeAsync(string code)
        {
            var category = await repository.GetAsync(x=>x.Code==code);

            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(category);
        }

        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x=>x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);

        }
        public async Task<PagedResult<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize)
                .Take(input.PageSize));
            return new PagedResult<ProductCategoryInListDto>(ObjectMapper.Map<List<ProductCategory>,List<ProductCategoryInListDto>>(data), totalCount, input.CurrentPage, input.PageSize);
        }
    }
