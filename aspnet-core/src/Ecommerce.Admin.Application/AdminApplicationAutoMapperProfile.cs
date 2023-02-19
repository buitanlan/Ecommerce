using AutoMapper;
using Ecommerce.Admin.ProductCategories;
using Ecommerce.ProductCategories;

namespace Ecommerce.Admin;

public class AdminApplicationAutoMapperProfile : Profile
{
    public AdminApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        CreateMap<CreateUpdateProductCategoryDto, ProductCategory>();
    }
}
