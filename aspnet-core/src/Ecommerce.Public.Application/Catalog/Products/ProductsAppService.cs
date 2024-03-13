﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Catalog.Products;
using Ecommerce.Catalog.Products.Attributes;
using Ecommerce.ProductAttributes;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Catalog.Products;

public class ProductsAppService : ReadOnlyAppService<
    Product,
    ProductDto,
    Guid,
    PagedResultRequestDto>, IProductsAppService
{
    private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
    private readonly IRepository<ProductAttribute> _productAttributeRepository;
    private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
    private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
    private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
    private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
    private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;


    public ProductsAppService(IRepository<Product, Guid> repository,
        IRepository<ProductCategory> productCategoryRepository,
        IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
        IRepository<ProductAttribute> productAttributeRepository,
        IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
        IRepository<ProductAttributeInt> productAttributeIntRepository,
        IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
        IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
        IRepository<ProductAttributeText> productAttributeTextRepository
    )
        : base(repository)
    {
        _fileContainer = fileContainer;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
        _productAttributeIntRepository = productAttributeIntRepository;
        _productAttributeDecimalRepository = productAttributeDecimalRepository;
        _productAttributeVarcharRepository = productAttributeVarcharRepository;
        _productAttributeTextRepository = productAttributeTextRepository;
    }

    public async Task<List<ProductInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
    }


    public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
        query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId);

        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<ProductInListDto>(totalCount,
            ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
    }


    public async Task<string> GetThumbnailImageAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

        if (thumbnailContent is null)
        {
            return null;
        }

        var result = Convert.ToBase64String(thumbnailContent);
        return result;
    }

    public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

        var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
        var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
        var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
        var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
        var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

        var query = from a in attributeQuery
            join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
            from adate in aDateTimeTable.DefaultIfEmpty()
            join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
            from adecimal in aDecimalTable.DefaultIfEmpty()
            join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
            from aint in aIntTable.DefaultIfEmpty()
            join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
            from aVarchar in aVarcharTable.DefaultIfEmpty()
            join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
            from aText in aTextTable.DefaultIfEmpty()
            where (adate == null || adate.ProductId == productId)
                  && (adecimal == null || adecimal.ProductId == productId)
                  && (aint == null || aint.ProductId == productId)
                  && (aVarchar == null || aVarchar.ProductId == productId)
                  && (aText == null || aText.ProductId == productId)
            select new ProductAttributeValueDto()
            {
                Label = a.Label,
                AttributeId = a.Id,
                DataType = a.DataType,
                Code = a.Code,
                ProductId = productId,
                DateTimeValue = adate != null ? adate.Value : null,
                DecimalValue = adecimal != null ? adecimal.Value : null,
                IntValue = aint != null ? aint.Value : null,
                TextValue = aText != null ? aText.Value : null,
                VarcharValue = aVarchar != null ? aVarchar.Value : null,
                DateTimeId = adate != null ? adate.Id : null,
                DecimalId = adecimal != null ? adecimal.Id : null,
                IntId = aint != null ? aint.Id : null,
                TextId = aText != null ? aText.Id : null,
                VarcharId = aVarchar != null ? aVarchar.Id : null,
            };
        query = query.Where(x => x.DateTimeId != null
                                 || x.DecimalId != null
                                 || x.IntValue != null
                                 || x.TextId != null
                                 || x.VarcharId != null);
        return await AsyncExecuter.ToListAsync(query);
    }


    public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(
        ProductAttributeListFilterDto input)
    {
        var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

        var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
        var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
        var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
        var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
        var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

        var query = from a in attributeQuery
            join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
            from adate in aDateTimeTable.DefaultIfEmpty()
            join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
            from adecimal in aDecimalTable.DefaultIfEmpty()
            join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
            from aint in aIntTable.DefaultIfEmpty()
            join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
            from aVarchar in aVarcharTable.DefaultIfEmpty()
            join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
            from aText in aTextTable.DefaultIfEmpty()
            where (adate == null || adate.ProductId == input.ProductId)
                  && (adecimal == null || adecimal.ProductId == input.ProductId)
                  && (aint == null || aint.ProductId == input.ProductId)
                  && (aVarchar == null || aVarchar.ProductId == input.ProductId)
                  && (aText == null || aText.ProductId == input.ProductId)
            select new ProductAttributeValueDto()
            {
                Label = a.Label,
                AttributeId = a.Id,
                DataType = a.DataType,
                Code = a.Code,
                ProductId = input.ProductId,
                DateTimeValue = adate != null ? adate.Value : null,
                DecimalValue = adecimal != null ? adecimal.Value : null,
                IntValue = aint != null ? aint.Value : null,
                TextValue = aText != null ? aText.Value : null,
                VarcharValue = aVarchar != null ? aVarchar.Value : null,
                DateTimeId = adate != null ? adate.Id : null,
                DecimalId = adecimal != null ? adecimal.Id : null,
                IntId = aint != null ? aint.Id : null,
                TextId = aText != null ? aText.Id : null,
                VarcharId = aVarchar != null ? aVarchar.Id : null,
            };
        query = query.Where(x => x.DateTimeId != null
                                 || x.DecimalId != null
                                 || x.IntValue != null
                                 || x.TextId != null
                                 || x.VarcharId != null);
        var totalCount = await AsyncExecuter.LongCountAsync(query);
        var data = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.Label)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );
        return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
    }
}
