import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProductCategoriesService, ProductCategoryInListDto, ProductInListDto } from '../proxy/product-categories';
import { PagedResultDto } from '@abp/ng.core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProductsService } from '../proxy/products';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-product',
  template: `
    <p-panel header="Danh sách sản phẩm">
      <!--Filter (search panel)-->
      <div class="grid">
        <div class="col-4">
          <button pButton type="button" icon="fa fa-plus" iconPos="left" label="Thêm mới"></button>
        </div>
        <div class="col-8">
          <div class="formgroup-inline">
            <div class="field">
              <label for="txt-keyword" class="p-sr-only">Từ khóa</label>
              <input id="txt-keyword" pInputText type="text" placeholder="Gõ từ khóa" />
            </div>
            <div class="field">
              <p-dropdown
                [options]="productCategories"
                [(ngModel)]="categoryId"
                placeholder="Chọn danh mục"
              ></p-dropdown>
            </div>
            <button type="button" pButton (click)="loadData()" icon="fa fa-search" iconPos="left" label="Tìm"></button>
          </div>
        </div>
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
      <p-blockUI [blocked]="blockedPanel" [target]="pnl">
        <p-progressSpinner></p-progressSpinner>
      </p-blockUI>
    </p-panel>
  `,
  imports: [
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    InputTextModule,
    ProgressSpinnerModule,
  ],
  standalone: true,
})
export class ProductComponent implements OnInit {
  ngOnInit(): void {
    this.loadProductCategories();
    this.loadData();
  }

  blockedPanel: boolean = false;
  items: ProductInListDto[] | undefined = [];

  //Paging variables
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number | undefined = 0;
  //Filter
  productCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  readonly #productsService = inject(ProductsService);
  readonly #productCategoriesService = inject(ProductCategoriesService);
  readonly #destroyRef = inject(DestroyRef);

  loadData() {
    this.#productsService
      .getListFilter({
        keyword: this.keyword,
        categoryId: this.categoryId,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response: PagedResultDto<ProductInListDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
        },
        error: () => this.toggleBlockUI(false),
      });
  }

  loadProductCategories() {
    this.#productCategoriesService.getListAll().subscribe((response: ProductCategoryInListDto[]) => {
      response.forEach((element) => {
        this.productCategories.push({
          value: element.id,
          name: element.name,
        });
      });
    });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
