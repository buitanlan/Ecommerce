using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin.ProductCategories;

public interface IProductCategoriesAppService : ICrudAppService
    <ProductCategoryDto,
        Guid, 
        PagedResultRequestDto,
        CreateUpdateProductCategoryDto, 
        CreateUpdateProductCategoryDto>
{
    Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input);
}

