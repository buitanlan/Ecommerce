import { Component } from '@angular/core';
import { AppMenuitemComponent } from './app.menuitem.component';

@Component({
  selector: 'app-menu',
  template: `
    <ul class="layout-menu">
      @for (item of model; track item; let i = $index) {
        @if (!item.separator) {
          <li app-menuitem [item]="item" [index]="i" [root]="true"></li>
        }
        @if (item.separator) {
          <li class="menu-separator"></li>
        }
      }
    </ul>
  `,
  imports: [AppMenuitemComponent],
  standalone: true,
})
export class AppMenuComponent {
  model: any[] = [
    {
      label: 'Trang chủ',
      items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }],
    },
    {
      label: 'Sản phẩm',
      items: [
        { label: 'Danh sách sản phẩm', icon: 'pi pi-fw pi-circle', routerLink: ['/product'] },
        { label: 'Danh sách thuộc tính', icon: 'pi pi-fw pi-circle', routerLink: ['/attribute'] },
      ],
    },
  ];
}
