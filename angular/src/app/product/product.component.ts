import { Component, inject } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProductInListDto } from '../proxy/product-categories';
import { DestroyService } from '../../shared/services/destroy.service';
import { PagedResultDto } from '@abp/ng.core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProductsService } from '../proxy/products';

@Component({
  selector: 'app-product',
  template: `
    <p-panel header="Danh sách sản phẩm">
      <!--Filter (search panel)-->
      <div class="grid">
        <div class="col-4">4</div>
        <div class="col-8">8</div>
      </div>

      <!--Table-->
      <p-table #pnl [value]="items!">
        <ng-template pTemplate="header">
          <tr>
            <th>Mã</th>
            <th>SKU</th>
            <th>Tên</th>
            <th>Loại</th>
            <th>Tên danh mục</th>
            <th>Thứ tự</th>
            <th>Hiển thị</th>
            <th>Kích hoạt</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-row>
          <tr>
            <td>{{ row.code }}</td>
            <td>{{ row.sku }}</td>
            <td>{{ row.name }}</td>
            <td>{{ row.productType }}</td>
            <td>{{ row.categoryId }}</td>
            <td>{{ row.sortOrder }}</td>
            <td>{{ row.visibility }}</td>
            <td>{{ row.isActive }}</td>
          </tr>
        </ng-template>
        <ng-template pTemplate="summary">
          <div style="text-align: left">Tổng số bản ghi: {{ totalCount! }}</div>
        </ng-template>
      </p-table>
      <!--Paginator-->
      <p-paginator
        [rows]="maxResultCount"
        [totalRecords]="totalCount!"
        (onPageChange)="pageChanged($event)"
        [rowsPerPageOptions]="[10, 20, 30, 50, 100]"
      ></p-paginator>
      <!--Block UI-->
      <p-blockUI [blocked]="blockedPanel" [target]="pnl"></p-blockUI>
    </p-panel>
  `,
  imports: [PanelModule, TableModule, PaginatorModule, BlockUIModule],
  standalone: true,
  providers: [DestroyService],
})
export class ProductComponent {
  blockedPanel: boolean = false;
  items: ProductInListDto[] | undefined = [];

  //Paging variables
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;

  readonly #productsService = inject(ProductsService);
  loadData() {
    this.#productsService
      .getListFilter({
        keyword: '',
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: (response: PagedResultDto<ProductInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
        },
        error: () => {},
      });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }
}
