using AutoMapper;
using Ecommerce.Catalog.Products;
using Ecommerce.Manufacturers;
using Ecommerce.ProductAttributes;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Ecommerce.Public.Catalog.Manufacturers;
using Ecommerce.Public.Catalog.ProductAttributes;
using Ecommerce.Public.Catalog.Products;
using Ecommerce.Public.ProductCategories;

namespace Ecommerce.Public;

public class EcommercePublicApplicationAutoMapperProfile : Profile
{
    public EcommercePublicApplicationAutoMapperProfile()
    {
        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();

        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
    }
}
