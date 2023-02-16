using System;
using Ecommerce.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.ProductCategories;

public class ProductCategoriesAppService: CrudAppService<
    ProductCategory,
    ProductCategoryDto,
    Guid,
    PagedResultRequestDto,
    CreateUpdateProductCategoryDto,
    CreateUpdateProductCategoryDto>, IProductCategoriesAppService
{
    public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository)
        : base(repository)
    {
    }
}