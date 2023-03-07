using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Admin.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin.Products;

public interface IProductsAppService : ICrudAppService<ProductDto,
Guid, 
PagedResultRequestDto,
CreateUpdateProductDto, 
CreateUpdateProductDto>
{
    Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input);
    Task<List<ProductInListDto>> GetListAllAsync();
    Task DeleteMultipleAsync(IEnumerable<Guid> ids);
    Task<string> GetThumbnailImageAsync(string fileName);
}