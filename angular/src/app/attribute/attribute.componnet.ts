import { Component, DestroyRef, inject } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { ValidationMessageComponent } from '../shared/validation-message/validatetion-message.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { PanelModule } from 'primeng/panel';
import { ProductAttributeDto, ProductAttributeInListDto, ProductAttributesService } from '../proxy/product-attributes';
import { DialogService } from 'primeng/dynamicdialog';
import { NotificationService } from '../shared/services/notification.service';
import { ConfirmationService } from 'primeng/api';
import { AttributeType } from '../proxy/ecommerce/product-attributes';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PagedResultDto } from '@abp/ng.core';
import { PaginatorModule } from 'primeng/paginator';
import { BadgeModule } from 'primeng/badge';
import { TableModule } from 'primeng/table';
import { AttributeDetailComponent } from './atrribute-detail.componnet';
import { ButtonDirective } from 'primeng/button';

@Component({
  selector: 'app-attribute',
  template: `
    <p-panel header="Danh sách thuộc tính">
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
        </div>
        <div class="col-8">
          <div class="formgroup-inline">
            <div class="field">
              <label for="txt-keyword" class="p-sr-only">Từ khóa</label>
              <input id="txt-keyword" [(ngModel)]="keyword" pInputText type="text" placeholder="Gõ từ khóa" />
            </div>
            <button type="button" pButton (click)="loadData()" icon="fa fa-search" iconPos="left" label="Tìm"></button>
          </div>
        </div>
      </div>

      <!--Table-->
      <p-table #pnl [value]="items" [(selection)]="selectedItems" selectionMode="multiple">
        <ng-template pTemplate="header">
          <tr>
            <th style="width: 10px">
              <p-tableHeaderCheckbox></p-tableHeaderCheckbox>
            </th>
            <th>Mã</th>
            <th>Kiểu dữ liệu</th>
            <th>Nhãn</th>
            <th>Thứ tự</th>
            <th>Hiển thị</th>
            <th>Bắt buộc nhập</th>
            <th>Duy nhất</th>
            <th>Kích hoạt</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-row>
          <tr [pSelectableRow]="row">
            <td style="width: 10px">
              <span class="ui-column-title"></span>
              <p-tableCheckbox [value]="row"></p-tableCheckbox>
            </td>
            <td>{{ row.code }}</td>
            <td>{{ getAttributeTypeName(row.dataType) }}</td>
            <td>{{ row.label }}</td>
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
              @if (row.isRequired == 1) {
                <p-badge severity="success" value="Có"></p-badge>
              }
              @if (row.isRequired == 0) {
                <p-badge severity="danger" value="Không"></p-badge>
              }
            </td>
            <td>
              @if (row.isUnique == 1) {
                <p-badge severity="success" value="Có"></p-badge>
              }
              @if (row.isUnique == 0) {
                <p-badge severity="danger" value="Không"></p-badge>
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
          <div style="text-align: left">Tổng số bản ghi: {{ totalCount }}</div>
        </ng-template>
      </p-table>
      <!--Paginator-->
      <p-paginator
        [rows]="maxResultCount"
        [totalRecords]="totalCount"
        (onPageChange)="pageChanged($event)"
        [rowsPerPageOptions]="[10, 20, 30, 50, 100]"
      ></p-paginator>
      <!--Block UI-->
      <p-blockUI [blocked]="blockedPanel" [target]="pnl">
        <p-progressSpinner></p-progressSpinner>
      </p-blockUI>
    </p-panel>
  `,
  standalone: true,
  imports: [
    BlockUIModule,
    ProgressSpinnerModule,
    CheckboxModule,
    DropdownModule,
    ValidationMessageComponent,
    InputNumberModule,
    PanelModule,
    PaginatorModule,
    BadgeModule,
    TableModule,
    ButtonDirective,
  ],
})
export class AttributeComponent {
  blockedPanel = false;
  items: ProductAttributeDto[] = [];
  selectedItems: ProductAttributeDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number = 0;

  //Filter
  attributeCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  readonly #attributeService = inject(ProductAttributesService);
  readonly #dialogService = inject(DialogService);
  readonly #notificationService = inject(NotificationService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.#attributeService
      .getListFilter({
        keyword: this.keyword,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response: PagedResultDto<ProductAttributeInListDto>) => {
          this.items = response.items!;
          this.totalCount = response.totalCount!;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  pageChanged(event: any): void {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }
  showAddModal() {
    const ref = this.#dialogService.open(AttributeDetailComponent, {
      header: 'Thêm mới sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductAttributeDto) => {
      if (data) {
        this.loadData();
        this.#notificationService.showSuccess('Thêm thuộc tính thành công');
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
    const ref = this.#dialogService.open(AttributeDetailComponent, {
      data: {
        id: id,
      },
      header: 'Cập nhật sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductAttributeDto) => {
      if (data) {
        this.loadData();
        this.selectedItems = [];
        this.#notificationService.showSuccess('Cập nhật thuộc tính thành công');
      }
    });
  }
  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.#notificationService.showError('Phải chọn ít nhất một bản ghi');
      return;
    }
    const ids: any[] = [];
    this.selectedItems.forEach((element) => {
      ids.push(element.id);
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
    this.#attributeService
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

  getAttributeTypeName(value: number) {
    return AttributeType[value];
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
