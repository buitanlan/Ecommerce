import type { EntityDto } from '@abp/ng.core';
import type { ProductType } from '../ecommerce/products/product-type.enum';

export interface CreateUpdateProductCategoryDto {
  name?: string;
  code?: string;
  slug?: string;
  sortOrder: number;
  coverPicture?: string;
  visibility: boolean;
  isActive: boolean;
  parentId?: string;
  seoMetaDescription?: string;
}

export interface ProductCategoryDto {
  name?: string;
  code?: string;
  slug?: string;
  sortOrder: number;
  coverPicture?: string;
  visibility: boolean;
  isActive: boolean;
  parentId?: string;
  seoMetaDescription?: string;
  id?: string;
}

export interface ProductCategoryInListDto extends EntityDto<string> {
  name?: string;
  code?: string;
  sortOrder: number;
  coverPicture?: string;
  visibility: boolean;
  isActive: boolean;
}

export interface ProductInListDto extends EntityDto<string> {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  categoryId?: string;
  thumbnailPicture?: string;
  categoryName?: string;
  categorySlug?: string;
}
