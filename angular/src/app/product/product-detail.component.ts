import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductDto, ProductsService } from '../proxy/products';
import { ProductCategoriesService, ProductCategoryInListDto } from '../proxy/product-categories';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { EditorModule } from 'primeng/editor';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ValidationMessageComponent } from '../shared/validation-message/validatetion-message.component';
import { UtilityService } from '../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin } from 'rxjs';
import { ManufacturerInListDto, ManufacturersService } from '../proxy/manufacturers';
import { productTypeOptions } from '../proxy/ecommerce/products';

@Component({
  selector: 'app-product-detail',
  template: `
    <form [formGroup]="form" (ngSubmit)="saveChange()">
      <p-panel #pnl header="Danh sách sản phẩm">
        <!--form grid-->
        <div class="formgrid grid">
          <div class="field col-12">
            <label for="name" class="block">Tên<span class="required">*</span></label>
            <input
              id="name"
              pInputText
              (keyup)="generateSlug()"
              type="text"
              class="w-full"
              formControlName="name"
            />type="text" class="w-full" formControlName="name" />
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="code" class="block">Code<span class="required">*</span></label>
            <input id="code" pInputText type="text" formControlName="code" />
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Slug<span class="required">*</span></label>
            <input id="slug" pInputText type="text" class="w-full" formControlName="slug" />
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="sku" class="block">SKU<span class="required">*</span></label>
            <input id="sku" pInputText type="text" class="w-full" formControlName="sku" />
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="manufacturerId" class="block">Nhà sản xuất<span class="required">*</span></label>
            <p-dropdown
              [options]="manufacturers"
              formControlName="manufacturerId"
              placeholder="Chọn nhà sản xuất"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="categoryId" class="block">Danh mục<span class="required">*</span></label>
            <p-dropdown
              [options]="productCategories"
              formControlName="categoryId"
              placeholder="Chọn danh mục"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="productType" class="block">Loại sản phẩm<span class="required">*</span></label>
            <p-dropdown
              [options]="productTypes"
              formControlName="productType"
              placeholder="Chọn loại"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Thứ tự<span class="required">*</span></label>
            <p-inputNumber formControlName="sortOrder"></p-inputNumber>
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="sellPrice" class="block">Giá bán<span class="required">*</span></label>
            <p-inputNumber formControlName="sellPrice"></p-inputNumber>
            <app-validation-message
              [entityForm]="form"
              fieldName="name"
              [validationMessages]="validationMessages"
            ></app-validation-message>
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
          <button
            type="submit"
            [disabled]="!form.valid || btnDisabled"
            pButton
            icon="fa fa-save"
            iconPos="left"
            label="Lưu lại"
            class="cursor-pointer"
          ></button>
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
    ValidationMessageComponent,
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
  btnDisabled = false;
  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    name: [
      { type: 'required', message: 'Bạn phải nhập tên' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    sku: [{ type: 'required', message: 'Bạn phải mã SKU sản phẩm' }],
    manufacturerId: [{ type: 'required', message: 'Bạn phải chọn nhà cung cấp' }],
    categoryId: [{ type: 'required', message: 'Bạn phải chọn danh mục' }],
    productType: [{ type: 'required', message: 'Bạn phải chọn loại sản phẩm' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
    sellPrice: [{ type: 'required', message: 'Bạn phải nhập giá bán' }],
  };

  readonly #destroyRef = inject(DestroyRef);
  readonly #productService = inject(ProductsService);
  readonly #productCategoryService = inject(ProductCategoriesService);
  readonly #manufacturersService = inject(ManufacturersService);
  readonly #fb = inject(FormBuilder);
  readonly #utilService = inject(UtilityService);
  readonly #config = inject(DynamicDialogConfig);
  readonly #ref = inject(DynamicDialogRef);

  ngOnInit(): void {
    this.buildForm();
    this.loadProductTypes();
    //Load data to form
    const productCategories = this.#productCategoryService.getListAll();
    const manufacturers = this.#manufacturersService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      productCategories,
      manufacturers,
    })
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response: any) => {
          //Push data to dropdown
          const productCategories = response.productCategories as ProductCategoryInListDto[];
          const manufacturers = response.manufacturers as ManufacturerInListDto[];
          productCategories.forEach((element) => {
            this.productCategories.push({
              value: element.id,
              label: element.name,
            });
          });

          manufacturers.forEach((element) => {
            this.manufacturers.push({
              value: element.id,
              label: element.name,
            });
          });
          //Load edit data to form
          if (this.#utilService.isEmpty(this.#config.data?.id)) {
            this.toggleBlockUI(false);
          } else {
            this.loadFormDetails(this.#config.data?.id);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  generateSlug() {
    this.form.controls['slug'].setValue(this.#utilService.MakeSeoTitle(this.form.get('name')?.value));
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
  loadProductTypes() {
    productTypeOptions.forEach((element) => {
      this.productTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  private buildForm() {
    this.form = this.#fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(250)]),
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(this.selectedEntity.manufacturerId || null, Validators.required),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = false;
      }, 1000);
    }
  }
}
