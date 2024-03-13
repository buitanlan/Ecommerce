using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Catalog.Products;
using Ecommerce.Public.ProductCategories;
using Ecommerce.Public.Web.Models;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace Ecommerce.Public.Web.Pages;

public class IndexModel : EcommercePublicPageModel
{
    private readonly IDistributedCache<HomeCacheItem> _distributedCache;
    private readonly IProductCategoriesAppService _productCategoriesAppService;
    private readonly IProductsAppService _productsAppService;

    public List<ProductCategoryInListDto> Categories { set; get; }
    public List<ProductInListDto> TopSellerProducts { set; get; }

    public IndexModel(IProductCategoriesAppService productCategoriesAppService,
        IProductsAppService productsAppService, IDistributedCache<HomeCacheItem> distributedCache)
    {
        _productCategoriesAppService = productCategoriesAppService;
        _productsAppService = productsAppService;
        _distributedCache = distributedCache;
    }

    public async Task OnGetAsync()
    {
        var cacheItem = await _distributedCache.GetOrAddAsync(EcommercePublicConsts.CacheKeys.HomeData, async () =>
            {
                var allCategories = await _productCategoriesAppService.GetListAllAsync();
                var rootCategories = allCategories.Where(x => x.ParentId == null).ToList();
                foreach (var category in rootCategories)
                {
                    category.Children = rootCategories.Where(x => x.ParentId == category.Id).ToList();
                }

                var topSellerProducts = await _productsAppService.GetListTopSellerAsync(10);
                return new HomeCacheItem()
                {
                    TopSellerProducts = topSellerProducts,
                    Categories = rootCategories
                };
            },
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            });

        TopSellerProducts = cacheItem.TopSellerProducts;
        Categories = cacheItem.Categories;
    }
}
