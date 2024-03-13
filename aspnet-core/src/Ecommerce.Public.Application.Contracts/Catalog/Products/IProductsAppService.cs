using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Catalog.Products.Attributes;
using Ecommerce.Public.Application.Contracts;
using Ecommerce.Public.Catalog.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Catalog.Products;

public interface IProductsAppService : IReadOnlyAppService<ProductDto, Guid, PagedResultRequestDto>
{
    Task<PagedResult<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input);
    Task<List<ProductInListDto>> GetListAllAsync();
    Task<string> GetThumbnailImageAsync(string fileName);
    Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId);
    Task<PagedResult<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input);
    Task<List<ProductInListDto>> GetListTopSellerAsync(int numberOfRecords);
}
