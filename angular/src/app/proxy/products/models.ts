import type { EntityDto } from '@abp/ng.core';
import type { ProductType } from '../ecommerce/products/product-type.enum';
import type { BaseListFilterDto } from '../models';
import type { AttributeType } from '../ecommerce/product-attributes/attribute-type.enum';

export interface AddUpdateProductAttributeDto {
  productId?: string;
  attributeId?: string;
  dateTimeValue?: string;
  decimalValue?: number;
  intValue?: number;
  varcharValue?: string;
  textValue?: string;
}

export interface CreateUpdateProductDto extends EntityDto<string> {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  sellPrice: number;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  thumbnailPictureName?: string;
  thumbnailPictureContent?: string;
}

export interface ProductAttributeListFilterDto extends BaseListFilterDto {
  productId?: string;
}

export interface ProductAttributeValueDto {
  productId?: string;
  attributeId?: string;
  code?: string;
  dataType: AttributeType;
  label?: string;
  dateTimeValue?: string;
  decimalValue?: number;
  intValue?: number;
  textValue?: string;
  varcharValue?: string;
  dateTimeId?: string;
  decimalId?: string;
  intId?: string;
  textId?: string;
  varcharId?: string;
  id?: string;
}

export interface ProductDto {
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  thumbnailPicture?: string;
  sellPrice: number;
  id?: string;
  categoryName?: string;
  categorySlug?: string;
}

export interface ProductListFilterDto extends BaseListFilterDto {
  categoryId?: string;
}
