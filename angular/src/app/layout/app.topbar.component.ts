import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { LayoutService } from './service/app.layout.service';
import { NgClass } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { LOGIN_URL } from '../../shared/constants/urls.const';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-topbar',
  template: `
    <div class="layout-topbar">
      <a class="layout-topbar-logo" routerLink="">
        <img
          src="assets/layout/images/{{ layoutService.config.colorScheme === 'light' ? 'logo-dark' : 'logo-white' }}.svg"
          alt="logo"
        />
        <span>Ecommerce</span>
      </a>

      <button #menubutton class="p-link layout-menu-button layout-topbar-button" (click)="layoutService.onMenuToggle()">
        <i class="pi pi-bars"></i>
      </button>

      <button
        #topbarmenubutton
        class="p-link layout-topbar-menu-button layout-topbar-button"
        (click)="layoutService.showProfileSidebar()"
      >
        <i class="pi pi-ellipsis-v"></i>
      </button>

      <div
        #topbarmenu
        class="layout-topbar-menu"
        [ngClass]="{ 'layout-topbar-menu-mobile-active': layoutService.state.profileSidebarVisible }"
      >
        <button class="p-link layout-topbar-button">
          <i class="pi pi-calendar"></i>
          <span>Calendar</span>
        </button>
        <button class="p-link layout-topbar-button" (click)="userMenu.toggle($event)">
          <i class="pi pi-user"></i>
          <span>Tài khoản</span>
        </button>
        <button class="p-link layout-topbar-button" [routerLink]="'/documentation'">
          <i class="pi pi-cog"></i>
          <span>Cài đặt</span>
        </button>
      </div>
    </div>
    <p-tieredMenu #userMenu [model]="userMenuItems" [popup]="true"></p-tieredMenu>
  `,
  imports: [NgClass, RouterLink, TieredMenuModule],
  standalone: true,
})
export class AppTopBarComponent {
  items!: MenuItem[];
  userMenuItems: MenuItem[] = [
    {
      label: 'Xem thông tin cá nhân',
      icon: 'pi pi-id-card',
      routerLink: ['/profile'],
    },
    {
      label: 'Đổi mật khẩu',
      icon: 'pi pi-key',
      routerLink: ['/change-password'],
    },
    {
      label: 'Đăng xuất',
      icon: 'pi pi-sign-out',
      command: (event) => {
        this.#authService.logout();
        this.#router.navigate([LOGIN_URL]);
      },
    },
  ];

  @ViewChild('menubutton') menuButton!: ElementRef;
  @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;
  @ViewChild('topbarmenu') menu!: ElementRef;

  readonly layoutService = inject(LayoutService);
  readonly #authService = inject(AuthService);
  readonly #router = inject(Router);
}
