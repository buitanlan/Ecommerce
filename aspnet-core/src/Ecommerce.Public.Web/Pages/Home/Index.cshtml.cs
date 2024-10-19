using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Catalog.Products;
using Ecommerce.Public.Catalog.Products;
using Ecommerce.Public.ProductCategories;
using Ecommerce.Public.Web.Models;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace Ecommerce.Public.Web.Pages;

public class IndexModel(
    IProductCategoriesAppService productCategoriesAppService,
    IProductsAppService productsAppService,
    IDistributedCache<HomeCacheItem> distributedCache)
    : EcommercePublicPageModel
{
    public List<ProductCategoryInListDto> Categories { set; get; }
    public List<ProductInListDto> TopSellerProducts { set; get; }

    public async Task OnGetAsync()
    {
        var cacheItem = await distributedCache.GetOrAddAsync(EcommercePublicConsts.CacheKeys.HomeData, async () =>
            {
                var allCategories = await productCategoriesAppService.GetListAllAsync();
                var rootCategories = allCategories.Where(x => x.ParentId == null).ToList();
                foreach (var category in rootCategories)
                {
                    category.Children = rootCategories.Where(x => x.ParentId == category.Id).ToList();
                }

                var topSellerProducts = await productsAppService.GetListTopSellerAsync(10);
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
