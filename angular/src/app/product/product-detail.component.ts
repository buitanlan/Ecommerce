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

@Component({
  selector: 'app-product-detail',
  template: `
    <form [formGroup]="form" (ngSubmit)="saveChange()">
      <p-panel #pnl header="Danh sách sản phẩm">
        <!--form grid-->
        <div class="formgrid grid">
          <div class="field col-12">
            <label for="name" class="block">Tên</label>
            <input id="name" pInputText type="text" formControlName="name" ð />
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
  imports: [BlockUIModule, ProgressSpinnerModule, ReactiveFormsModule, PanelModule, ButtonModule, InputTextModule],
})
export class ProductDetailComponent implements OnInit {
  blockedPanel: boolean = false;
  public form!: FormGroup;

  //Dropdown
  productCategories: any[] = [];
  selectedEntity = {} as ProductDto;
  #destroyRef = inject(DestroyRef);
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
