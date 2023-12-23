import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductDto, ProductsService } from '../proxy/products';
import { ProductCategoriesService, ProductCategoryInListDto } from '../proxy/product-categories';
import { takeUntil } from 'rxjs';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { EditorModule } from 'primeng/editor';
import { InputTextareaModule } from 'primeng/inputtextarea';

@Component({
  selector: 'app-product-detail',
  template: `
    <form [formGroup]="form" (ngSubmit)="saveChange()">
      <p-panel #pnl header="Danh sách sản phẩm">
        <!--form grid-->
        <div class="formgrid grid">
          <div class="field col-12">
            <label for="name" class="block">Tên</label>
            <input id="name" pInputText type="text" class="w-full" formControlName="name" />
          </div>
          <div class="field col-12">
            <label for="code" class="block">Code</label>
            <input id="code" pInputText type="text" formControlName="code" />
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Slug</label>
            <input id="slug" pInputText type="text" class="w-full" formControlName="slug" />
          </div>
          <div class="field col-12">
            <label for="sku" class="block">SKU</label>
            <input id="sku" pInputText type="text" class="w-full" formControlName="sku" />
          </div>
          <div class="field col-12">
            <label for="manufacturerId" class="block">Nhà sản xuất</label>
            <p-dropdown
              [options]="manufacturers"
              formControlName="manufacturerId"
              placeholder="Chọn nhà sản xuất"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>
          <div class="field col-12">
            <label for="categoryId" class="block">Danh mục</label>
            <p-dropdown
              [options]="productCategories"
              formControlName="categoryId"
              placeholder="Chọn danh mục"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>
          <div class="field col-12">
            <label for="productType" class="block">Loại sản phẩm</label>
            <p-dropdown
              [options]="productTypes"
              formControlName="productType"
              placeholder="Chọn loại"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Thứ tự</label>
            <p-inputNumber formControlName="sortOrder"></p-inputNumber>
          </div>
          <div class="field col-12">
            <label for="sellPrice" class="block">Giá bán</label>
            <p-inputNumber formControlName="sellPrice"></p-inputNumber>
          </div>
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="visibility" [binary]="true" id="visibility"></p-checkbox>
            <label for="visibility">Hiển thị</label>
          </div>
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="isActive" [binary]="true" id="isActive"></p-checkbox>
            <label for="isActive">Kích hoạt</label>
          </div>

          <div class="field col-12">
            <label for="seoMetaDescription" class="block">Mô tả SEO</label>
            <textarea
              id="seoMetaDescription"
              pInputTextarea
              class="w-full"
              formControlName="seoMetaDescription"
            ></textarea>
          </div>
          <div class="field col-12">
            <label for="description" class="block">Mô tả</label>
            <p-editor formControlName="description" [style]="{ height: '320px' }"></p-editor>
          </div>
        </div>
        <ng-template pTemplate="footer">
          <button type="submit" pButton icon="fa fa-save" iconPos="left" label="Lưu lại"></button>
        </ng-template>
        <!--Block UI-->
        <p-blockUI [blocked]="blockedPanel" [target]="pnl">
          <p-progressSpinner></p-progressSpinner>
        </p-blockUI>
      </p-panel>
    </form>
  `,
  standalone: true,
  imports: [
    BlockUIModule,
    ProgressSpinnerModule,
    ReactiveFormsModule,
    PanelModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    InputNumberModule,
    DropdownModule,
    EditorModule,
    InputTextareaModule,
  ],
})
export class ProductDetailComponent implements OnInit {
  blockedPanel: boolean = false;
  public form!: FormGroup;

  //Dropdown
  productCategories: any[] = [];
  manufacturers: any[] = [];
  productTypes: any[] = [];
  selectedEntity = {} as ProductDto;

  readonly #destroyRef = inject(DestroyRef);
  readonly #productService = inject(ProductsService);
  readonly #productCategoryService = inject(ProductCategoriesService);
  readonly #fb = inject(FormBuilder);

  ngOnInit(): void {
    this.buildForm();
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.#productService
      .get(id)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response: ProductDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  saveChange() {}
  loadProductCategories() {
    this.#productCategoryService.getListAll().subscribe((response: ProductCategoryInListDto[]) => {
      response.forEach((element) => {
        this.productCategories.push({
          value: element.id,
          name: element.name,
        });
      });
    });
  }

  private buildForm() {
    this.form = this.#fb.group({
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(this.selectedEntity.manufacturerId || null, Validators.required),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || false),
      isActive: new FormControl(this.selectedEntity.isActive || false),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
