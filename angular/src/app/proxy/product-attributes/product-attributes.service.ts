import type { CreateUpdateProductAttributeDto, ProductAttributeDto, ProductAttributeInListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { BaseListFilterDto } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ProductAttributesService {
  apiName = 'Default';

  create = (input: CreateUpdateProductAttributeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeDto>(
      {
        method: 'POST',
        url: '/api/app/product-attributes',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/product-attributes/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  deleteMultiple = (ids: string[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: '/api/app/product-attributes/multiple',
        params: { ids },
      },
      { apiName: this.apiName, ...config },
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeDto>(
      {
        method: 'GET',
        url: `/api/app/product-attributes/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  getList = (input: PagedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductAttributeDto>>(
      {
        method: 'GET',
        url: '/api/app/product-attributes',
        params: { skipCount: input.skipCount, maxResultCount: input.maxResultCount },
      },
      { apiName: this.apiName, ...config },
    );

  getListAll = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeInListDto[]>(
      {
        method: 'GET',
        url: '/api/app/product-attributes/all',
      },
      { apiName: this.apiName, ...config },
    );

  getListFilter = (input: BaseListFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductAttributeInListDto>>(
      {
        method: 'GET',
        url: '/api/app/product-attributes/filter',
        params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
      },
      { apiName: this.apiName, ...config },
    );

  update = (id: string, input: CreateUpdateProductAttributeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeDto>(
      {
        method: 'PUT',
        url: `/api/app/product-attributes/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  constructor(private restService: RestService) {}
}
