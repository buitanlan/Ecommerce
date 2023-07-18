﻿using AutoMapper;
using Ecommerce.Admin.Manufacturers;
using Ecommerce.Admin.ProductAttributes;
using Ecommerce.Admin.ProductCategories;
using Ecommerce.Admin.Products;
using Ecommerce.Manufacturers;
using Ecommerce.ProductAttributes;
using Ecommerce.ProductCategories;
using Ecommerce.Products;

namespace Ecommerce.Admin;

public class AdminApplicationAutoMapperProfile : Profile
{
    public AdminApplicationAutoMapperProfile()
    {
        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        CreateMap<CreateUpdateProductCategoryDto, ProductCategory>();
        
        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();
        CreateMap<CreateUpdateProductDto, Product>();
        
        //Manufacturer
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
        CreateMap<CreateUpdateProductAttributeDto, ProductAttribute>();
    }
}
