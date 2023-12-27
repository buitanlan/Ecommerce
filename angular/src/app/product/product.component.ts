import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProductCategoriesService, ProductCategoryInListDto, ProductInListDto } from '../proxy/product-categories';
import { PagedResultDto } from '@abp/ng.core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProductDto, ProductsService } from '../proxy/products';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogService } from 'primeng/dynamicdialog';
import { NotificationService } from '../shared/services/notification.service';
import { ProductDetailComponent } from './product-detail.component';
import { BadgeModule } from 'primeng/badge';
import { ProductType } from '../proxy/ecommerce/products';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-product',
  template: `
    <p-panel header="Danh sách sản phẩm">
      <!--Filter (search panel)-->
      <div class="grid">
        <div class="col-4">
          <button pButton type="button" (click)="showAddModal()" icon="fa fa-plus" iconPos="left" label="Thêm"></button>
          @if (selectedItems.length == 1) {
            <button
              pButton
              type="button"
              (click)="showEditModal()"
              class="ml-1 p-button-help"
              icon="fa fa-minus"
              iconPos="left"
              label="Sửa"
            ></button>
          }
        </div>
        @if (selectedItems.length > 0) {
          <button
            pButton
            type="button"
            (click)="deleteItems()"
            class="ml-1 p-button-danger"
            icon="fa fa-minus"
            iconPos="left"
            label="xóa"
          ></button>
        }
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
      <p-table #pnl [value]="items!" [(selection)]="selectedItems" selectionMode="multiple">
        <ng-template pTemplate="header">
          <tr [pSelectableRow]="'row'">
            <th style="width: 10px">
              <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
            </th>
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
            <td style="width: 10px">
              <span class="ui-column-title"></span>
              <p-tableCheckbox [value]="row"></p-tableCheckbox>
            </td>
            <td>{{ row.code }}</td>
            <td>{{ row.sku }}</td>
            <td>{{ row.name }}</td>
            <td>{{ getProductTypeName(row.productType) }}</td>
            <td>{{ row.categoryName }}</td>
            <td>{{ row.sortOrder }}</td>
            <td>
              @if (row.visibility == 1) {
                <p-badge severity="success" value="Hiển thị"></p-badge>
              }
              @if (row.visibility == 0) {
                <p-badge severity="danger" value="Ẩn"></p-badge>
              }
            </td>
            <td>
              @if (row.isActive == 1) {
                <p-badge value="Kích hoạt" severity="success"></p-badge>
              }
              @if (row.isActive == 0) {
                <p-badge value="Khoá" severity="danger"></p-badge>
              }
            </td>
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
    BadgeModule,
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
  public selectedItems: ProductInListDto[] = [];

  readonly #productsService = inject(ProductsService);
  readonly #productCategoriesService = inject(ProductCategoriesService);
  readonly #dialogService = inject(DialogService);
  readonly #notificationService = inject(NotificationService);
  readonly #confirmationService = inject(ConfirmationService);
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
          label: element.name,
        });
      });
    });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }
  showAddModal() {
    const ref = this.#dialogService.open(ProductDetailComponent, {
      header: 'Thêm mới sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.#notificationService.showSuccess('Thêm sản phẩm thành công');
        this.selectedItems = [];
      }
    });
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.#notificationService.showError('Bạn phải chọn một bản ghi');
      return;
    }
    const id = this.selectedItems[0].id;
    const ref = this.#dialogService.open(ProductDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.#notificationService.showSuccess('Cập nhật sản phẩm thành công');
      }
    });
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

  getProductTypeName(value: number) {
    return ProductType[value];
  }

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.#notificationService.showError('Phải chọn ít nhất một bản ghi');
      return;
    }
    const ids: string[] = [];
    this.selectedItems.forEach((element) => {
      if (element.id != null) {
        ids.push(element.id);
      }
    });
    this.#confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa bản ghi này?',
      accept: () => {
        this.deleteItemsConfirmed(ids);
      },
    });
  }

  deleteItemsConfirmed(ids: string[]) {
    this.toggleBlockUI(true);
    this.#productsService
      .deleteMultiple(ids)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: () => {
          this.#notificationService.showSuccess('Xóa thành công');
          this.loadData();
          this.selectedItems = [];
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
}
