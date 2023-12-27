import { Component, DestroyRef, inject } from '@angular/core';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { CheckboxModule } from 'primeng/checkbox';
import { ValidationMessageComponent } from '../shared/validation-message/validatetion-message.component';
import { PanelModule } from 'primeng/panel';
import { ProductAttributeDto, ProductAttributesService } from '../proxy/product-attributes';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { NotificationService } from '../shared/services/notification.service';
import { ConfirmationService } from 'primeng/api';
import { UtilityService } from '../shared/services/utility.service';
import { attributeTypeOptions } from '../proxy/ecommerce/product-attributes';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-attribute-detail',
  template: `
    <form [formGroup]="form" skipValidation (ngSubmit)="saveChange()">
      <p-panel #pnl header="Chi tiết thuộc tính">
        <!--form grid-->
        <div class="formgrid grid">
          <div class="field col-12">
            <label for="label" class="block">Nhãn <span class="required">*</span></label>
            <input id="label" pInputText type="text" class="w-full" formControlName="label" />
            <app-validation-message
              [entityForm]="form"
              fieldName="label"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="code" class="block">Code <span class="required">*</span></label>
            <input id="code" pInputText type="text" formControlName="code" />
            <app-validation-message
              [entityForm]="form"
              fieldName="code"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>

          <div class="field col-12">
            <label for="productType" class="block">Kiểu dữ liệu <span class="required">*</span></label>
            <p-dropdown
              [options]="dataTypes"
              formControlName="dataType"
              placeholder="Chọn kiểu dữ liệu"
              [showClear]="true"
              autoWidth="false"
              [style]="{ width: '100%' }"
            ></p-dropdown>
            <app-validation-message
              [entityForm]="form"
              fieldName="dataType"
              [validationMessages]="validationMessages"
            ></app-validation-message>
          </div>
          <div class="field col-12">
            <label for="slug" class="block">Thứ tự <span class="required">*</span></label>
            <p-inputNumber formControlName="sortOrder"></p-inputNumber>
            <app-validation-message
              [entityForm]="form"
              fieldName="sortOrder"
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
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="isRequired" [binary]="true" id="isRequired"></p-checkbox>
            <label for="isRequired">Bắt buộc nhập</label>
          </div>
          <div class="field-checkbox col-12 md:col-3">
            <p-checkbox formControlName="isUnique" [binary]="true" id="isUnique"></p-checkbox>
            <label for="isUnique">Duy nhất</label>
          </div>

          <div class="field col-12">
            <label for="note" class="block">Ghi chú</label>
            <textarea id="note" pInputTextarea class="w-full" formControlName="note"></textarea>
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
    ProgressSpinnerModule,
    BlockUIModule,
    CheckboxModule,
    ValidationMessageComponent,
    PanelModule,
    DropdownModule,
    InputNumberModule,
    ReactiveFormsModule,
    InputTextModule,
    InputTextareaModule,
    ButtonModule,
  ],
})
export class AttributeDetailComponent {
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form!: FormGroup;

  //Dropdown
  dataTypes: any[] = [];
  selectedEntity = {} as ProductAttributeDto;
  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    label: [
      { type: 'required', message: 'Bạn phải nhập nhãn hiển thị' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    dataType: [{ type: 'required', message: 'Bạn phải chọn kiểu dữ liệu' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
  };

  readonly #attributeService = inject(ProductAttributesService);
  readonly #dialogService = inject(DialogService);
  readonly #notificationService = inject(NotificationService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #fb = inject(FormBuilder);
  readonly #utilService = inject(UtilityService);
  readonly #config = inject(DynamicDialogConfig);
  readonly #ref = inject(DynamicDialogRef);
  readonly #destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.buildForm();
    this.loadAttributeTypes();
    this.initFormData();
  }

  initFormData() {
    //Load edit data to form
    if (this.#utilService.isEmpty(this.#config.data?.id)) {
      this.toggleBlockUI(false);
    } else {
      this.loadFormDetails(this.#config.data?.id);
    }
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.#attributeService
      .get(id)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (response: ProductAttributeDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChange() {
    this.toggleBlockUI(true);

    if (this.#utilService.isEmpty(this.#config.data?.id)) {
      this.#attributeService
        .create(this.form.value)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);

            this.#ref.close(this.form.value);
          },
          error: (err) => {
            this.#notificationService.showError(err.error.error.message);

            this.toggleBlockUI(false);
          },
        });
    } else {
      this.#attributeService
        .update(this.#config.data?.id, this.form.value)
        .pipe(takeUntilDestroyed(this.#destroyRef))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);
            this.#ref.close(this.form.value);
          },
          error: (err: any) => {
            this.#notificationService.showError(err.error.error.message);
            this.toggleBlockUI(false);
          },
        });
    }
  }

  loadAttributeTypes() {
    attributeTypeOptions.forEach((element) => {
      this.dataTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  private buildForm() {
    this.form = this.#fb.group({
      label: new FormControl(
        this.selectedEntity.label || null,
        Validators.compose([Validators.required, Validators.maxLength(250)]),
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      dataType: new FormControl(this.selectedEntity.dataType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      note: new FormControl(this.selectedEntity.note || null),
      isRequired: new FormControl(this.selectedEntity.isRequired || true),
      isUnique: new FormControl(this.selectedEntity.isUnique || false),
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
