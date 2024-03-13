using System;
using System.Threading.Tasks;
using Ecommerce.Public.ProductCategories;
using Ecommerce.Public.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Caching;

namespace Ecommerce.Public.Web.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly IProductCategoriesAppService _productCategoriesAppService;
    private readonly IDistributedCache<HeaderCacheItem> _distributedCache;

    public HeaderViewComponent(IProductCategoriesAppService productCategoriesAppService,
        IDistributedCache<HeaderCacheItem> distributedCache)
    {
        _productCategoriesAppService = productCategoriesAppService;
        _distributedCache = distributedCache;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cacheItem = await _distributedCache.GetOrAddAsync(
            EcommercePublicConsts.CacheKeys.HeaderData, async () =>
            {
                var model = await _productCategoriesAppService.GetListAllAsync();
                return new HeaderCacheItem()
                {
                    Categories = model
                };
            },
            () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            });
        return View(cacheItem.Categories);
    }
}
