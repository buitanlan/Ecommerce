import { Component, inject } from '@angular/core';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { AuthService } from '@abp/ng.core';
import { OAuthService } from 'angular-oauth2-oidc';

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
      <p-table #pnl [value]="items">
        <ng-template pTemplate="header">
          <tr>
            <th>Vin</th>
            <th>Year</th>
            <th>Brand</th>
            <th>Color</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-car>
          <tr>
            <td>{{ car.vin }}</td>
            <td>{{ car.year }}</td>
            <td>{{ car.brand }}</td>
            <td>{{ car.color }}</td>
          </tr>
        </ng-template>
      </p-table>
      <!--Paginator-->
      <p-paginator [rows]="10" [totalRecords]="120" [rowsPerPageOptions]="[10, 20, 30]"></p-paginator>
      <!--Block UI-->
      <p-blockUI [blocked]="blockedPanel" [target]="pnl"></p-blockUI>
    </p-panel>
  `,
  imports: [PanelModule, TableModule, PaginatorModule, BlockUIModule],
  standalone: true,
})
export class ProductComponent {
  blockedPanel: boolean = false;
  items = [];

  readonly #authService = inject(AuthService);
  readonly #oAuthService = inject(OAuthService);
  get hasLoggedIn(): boolean {
    return this.#oAuthService.hasValidAccessToken();
  }

  login() {
    this.#authService.navigateToLogin();
  }
}
