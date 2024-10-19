using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ecommerce.Admin.Permissions;
using Ecommerce.Admin.ProductCategories;
using Ecommerce.ProductAttributes;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Products;

[Authorize(EcommerceAdminPermissions.Product.Default, Policy = "AdminOnly")]
public partial class ProductsAppService
    : CrudAppService<
            Product,
            ProductDto,
            Guid,
            PagedResultRequestDto,
            CreateUpdateProductDto,
            CreateUpdateProductDto>,
        IProductsAppService
{
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly ProductManager _productManager;
    private readonly IBlobContainer _fileContainer;
    private readonly ProductCodeGenerator _productCodeGenerator;
    private readonly IRepository<ProductAttribute> _productAttributeRepository;
    private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
    private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
    private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
    private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
    private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;

    public ProductsAppService(IRepository<Product, Guid> repository,
        IRepository<ProductCategory> productCategoryRepository,
        ProductManager productManager,
        IBlobContainer fileContainer,
        ProductCodeGenerator productCodeGenerator,
        IRepository<ProductAttribute> productAttributeRepository,
        IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
        IRepository<ProductAttributeInt> productAttributeIntRepository,
        IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
        IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
        IRepository<ProductAttributeText> productAttributeTextRepository) : base(repository)

    {
        _productCategoryRepository = productCategoryRepository;
        _productManager = productManager;
        _fileContainer = fileContainer;
        _productCodeGenerator = productCodeGenerator;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeDecimalRepository = productAttributeDecimalRepository;
        _productAttributeIntRepository = productAttributeIntRepository;
        _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
        _productAttributeVarcharRepository = productAttributeVarcharRepository;
        _productAttributeTextRepository = productAttributeTextRepository;

        GetPolicyName = EcommerceAdminPermissions.Product.Default;
        GetListPolicyName = EcommerceAdminPermissions.Product.Default;
        CreatePolicyName = EcommerceAdminPermissions.Product.Create;
        UpdatePolicyName = EcommerceAdminPermissions.Product.Update;
        DeletePolicyName = EcommerceAdminPermissions.Product.Delete;
    }

    [Authorize(EcommerceAdminPermissions.Product.Default)]
    public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
        query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId);
        var totalCount = await AsyncExecuter.CountAsync(query);
        var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<ProductInListDto>(totalCount,
            ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
    }

    [Authorize(EcommerceAdminPermissions.Product.Default)]
    public async Task<List<ProductInListDto>> GetListAllAsync()
    {
        var query = await Repository.GetQueryableAsync();
        query = query.Where(x => x.IsActive == true);
        var data = await AsyncExecuter.ToListAsync(query);

        return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
    }

    [Authorize(EcommerceAdminPermissions.Product.Update)]
    public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
    {
        var product = await _productManager.CreateAsync(
            input.ManufacturerId,
            input.Name,
            input.Code,
            input.Slug,
            input.ProductType,
            input.SKU,
            input.SortOrder,
            input.Visibility,
            input.IsActive,
            input.CategoryId,
            input.SeoMetaDescription,
            input.Description,
            input.SellPrice);

        if (input.ThumbnailPictureContent is { Length: > 0 })
        {
            await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
            product.ThumbnailPicture = input.ThumbnailPictureName;
        }

        var result = await Repository.InsertAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(result);
    }


    [Authorize(EcommerceAdminPermissions.Product.Update)]
    public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
    {
        var product = await Repository.GetAsync(id);
        if (product is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);
        }

        product.ManufacturerId = input.ManufacturerId;
        product.Name = input.Name;
        product.Code = input.Code;
        product.Slug = input.Slug;
        product.ProductType = input.ProductType;
        product.SKU = input.SKU;
        product.SortOrder = input.SortOrder;
        product.Visibility = input.Visibility;
        product.IsActive = input.IsActive;

        if (product.CategoryId != input.CategoryId)
        {
            product.CategoryId = input.CategoryId;
            var category = await _productCategoryRepository.GetAsync(x => x.Id == input.CategoryId);
            product.CategoryName = category.Name;
            product.CategorySlug = category.Slug;
        }

        product.SeoMetaDescription = input.SeoMetaDescription;
        product.Description = input.Description;
        if (input.ThumbnailPictureContent is { Length: > 0 })
        {
            await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
            product.ThumbnailPicture = input.ThumbnailPictureName;
        }

        product.SellPrice = input.SellPrice;
        await Repository.UpdateAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }


    [Authorize(EcommerceAdminPermissions.Product.Delete)]
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }


    [Authorize(EcommerceAdminPermissions.Product.Default)]
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


    public async Task<string> GetSuggestNewCodeAsync()
    {
        return await _productCodeGenerator.GenerateAsync();
    }


    private async Task SaveThumbnailImageAsync(string fileName, string base64)
    {
        var regex = ThumbnailRegex();
        base64 = regex.Replace(base64, string.Empty);
        var bytes = Convert.FromBase64String(base64);
        await _fileContainer.SaveAsync(fileName, bytes, true);
    }

    [GeneratedRegex(@"^[\w/\:.-]+;base64,")]
    private static partial Regex ThumbnailRegex();


    [Authorize(EcommerceAdminPermissions.Product.Update)]
    public async Task<ProductAttributeValueDto> AddProductAttributeAsync(AddUpdateProductAttributeDto input)
    {
        var product = await Repository.GetAsync(input.ProductId);
        if (product is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);
        }

        var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId);
        if (attribute is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
        }

        var newAttributeId = Guid.NewGuid();
        switch (attribute.DataType)
        {
            case AttributeType.Date:
                if (input.DateTimeValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeDateTime = new ProductAttributeDateTime(newAttributeId, input.AttributeId,
                    input.ProductId, input.DateTimeValue);
                await _productAttributeDateTimeRepository.InsertAsync(productAttributeDateTime);
                break;

            case AttributeType.Int:
                if (input.IntValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeInt = new ProductAttributeInt(newAttributeId, input.AttributeId, input.ProductId,
                    input.IntValue.Value);
                await _productAttributeIntRepository.InsertAsync(productAttributeInt);
                break;

            case AttributeType.Decimal:
                if (input.DecimalValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeDecimal = new ProductAttributeDecimal(newAttributeId, input.AttributeId,
                    input.ProductId, input.DecimalValue.Value);
                await _productAttributeDecimalRepository.InsertAsync(productAttributeDecimal);
                break;

            case AttributeType.Varchar:
                if (input.VarcharValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeVarchar = new ProductAttributeVarchar(newAttributeId, input.AttributeId,
                    input.ProductId, input.VarcharValue);
                await _productAttributeVarcharRepository.InsertAsync(productAttributeVarchar);
                break;

            case AttributeType.Text:
                if (input.TextValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeText =
                    new ProductAttributeText(newAttributeId, input.AttributeId, input.ProductId, input.TextValue);
                await _productAttributeTextRepository.InsertAsync(productAttributeText);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new ProductAttributeValueDto()
        {
            AttributeId = input.AttributeId,
            Code = attribute.Code,
            DataType = attribute.DataType,
            DateTimeValue = input.DateTimeValue,
            DecimalValue = input.DecimalValue,
            Id = newAttributeId,
            IntValue = input.IntValue,
            Label = attribute.Label,
            ProductId = input.ProductId,
            TextValue = input.TextValue
        };
    }


    [Authorize(EcommerceAdminPermissions.Product.Update)]
    public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
    {
        var attribute = await _productAttributeRepository.GetAsync(x => x.Id == attributeId);
        if (attribute is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
        }

        switch (attribute.DataType)
        {
            case AttributeType.Date:
                var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id);
                if (productAttributeDateTime is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                await _productAttributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                break;

            case AttributeType.Int:

                var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id);
                if (productAttributeInt is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                await _productAttributeIntRepository.DeleteAsync(productAttributeInt);
                break;

            case AttributeType.Decimal:
                var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id);
                if (productAttributeDecimal is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                await _productAttributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                break;

            case AttributeType.Varchar:
                var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id);
                if (productAttributeVarchar is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                await _productAttributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                break;

            case AttributeType.Text:
                var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id);
                if (productAttributeText is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                await _productAttributeTextRepository.DeleteAsync(productAttributeText);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        await UnitOfWorkManager.Current.SaveChangesAsync();
    }


    [Authorize(EcommerceAdminPermissions.Product.Default)]
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
                DateTimeValue = adate.Value,
                DecimalValue = adecimal.Value,
                IntValue = aint.Value,
                TextValue = aText.Value,
                VarcharValue = aVarchar.Value,
                DecimalId = adecimal.Id,
                IntId = aint.Id,
                TextId = aText.Id,
                VarcharId = aVarchar.Id,
            };

        query = query.Where(x => x.DateTimeId != null
                                 || x.DecimalId != null
                                 || x.IntValue != null
                                 || x.TextId != null
                                 || x.VarcharId != null);

        return await AsyncExecuter.ToListAsync(query);
    }


    [Authorize(EcommerceAdminPermissions.Product.Default)]
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
                TextId = aText != null ? aText.Id : Guid.Empty,
                VarcharId = aVarchar != null ? aVarchar.Id : Guid.Empty,
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


    [Authorize(EcommerceAdminPermissions.Product.Update)]
    public async Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
    {
        var product = await Repository.GetAsync(input.ProductId);
        if (product is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);
        }

        var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId);
        if (attribute is null)
        {
            throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
        }

        switch (attribute.DataType)
        {
            case AttributeType.Date:
                if (input.DateTimeValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id);
                if (productAttributeDateTime is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                productAttributeDateTime.Value = input.DateTimeValue.Value;
                await _productAttributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                break;

            case AttributeType.Int:
                if (input.IntValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id);
                if (productAttributeInt is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                productAttributeInt.Value = input.IntValue.Value;
                await _productAttributeIntRepository.UpdateAsync(productAttributeInt);
                break;

            case AttributeType.Decimal:
                if (input.DecimalValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id);
                if (productAttributeDecimal is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                productAttributeDecimal.Value = input.DecimalValue.Value;
                await _productAttributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                break;

            case AttributeType.Varchar:
                if (input.VarcharValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id);
                if (productAttributeVarchar is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                productAttributeVarchar.Value = input.VarcharValue;
                await _productAttributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                break;

            case AttributeType.Text:
                if (input.TextValue is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                }

                var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id);
                if (productAttributeText is null)
                {
                    throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                }

                productAttributeText.Value = input.TextValue;
                await _productAttributeTextRepository.UpdateAsync(productAttributeText);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        await UnitOfWorkManager.Current.SaveChangesAsync();
        return new ProductAttributeValueDto()
        {
            AttributeId = input.AttributeId,
            Code = attribute.Code,
            DataType = attribute.DataType,
            DateTimeValue = input.DateTimeValue,
            DecimalValue = input.DecimalValue,
            Id = id,
            IntValue = input.IntValue,
            Label = attribute.Label,
            ProductId = input.ProductId,
            TextValue = input.TextValue
        };
    }
}
