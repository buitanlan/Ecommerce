import { Component, OnDestroy, Renderer2, viewChild, inject } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { LayoutService } from './service/app.layout.service';
import { AppSidebarComponent } from './app.sidebar.component';
import { AppTopBarComponent } from './app.topbar.component';
import { NgClass } from '@angular/common';
import { AppFooterComponent } from './app.footer.component';
import { AppConfigComponent } from './config/app.config.component';

@Component({
  selector: 'app-layout',
  template: `
    <div class="layout-wrapper" [ngClass]="containerClass">
      <app-topbar></app-topbar>
      <div class="layout-sidebar">
        <app-sidebar></app-sidebar>
      </div>
      <div class="layout-main-container">
        <div class="layout-main">
          <router-outlet></router-outlet>
        </div>
        <app-footer></app-footer>
      </div>
      <app-config></app-config>
      <div class="layout-mask"></div>
    </div>
  `,
  imports: [NgClass, RouterOutlet, AppFooterComponent, AppConfigComponent, AppSidebarComponent, AppTopBarComponent],
  standalone: true,
})
export class AppLayoutComponent implements OnDestroy {
  public layoutService = inject(LayoutService);
  public renderer = inject(Renderer2);
  public router = inject(Router);
  overlayMenuOpenSubscription: Subscription;

  menuOutsideClickListener: any;

  profileMenuOutsideClickListener: any;

  appSidebar = viewChild(AppSidebarComponent);

  appTopbar = viewChild(AppTopBarComponent);

  constructor() {
    this.overlayMenuOpenSubscription = this.layoutService.overlayOpen$.subscribe(() => {
      if (!this.menuOutsideClickListener) {
        this.menuOutsideClickListener = this.renderer.listen('document', 'click', (event) => {
          const isOutsideClicked = !(
            this.appSidebar()?.el.nativeElement.isSameNode(event.target) ||
            this.appSidebar()?.el.nativeElement.contains(event.target) ||
            this.appTopbar()?.menuButton()?.nativeElement.isSameNode(event.target) ||
            this.appTopbar()?.menuButton()?.nativeElement.contains(event.target)
          );

          if (isOutsideClicked) {
            this.hideMenu();
          }
        });
      }

      if (!this.profileMenuOutsideClickListener) {
        this.profileMenuOutsideClickListener = this.renderer.listen('document', 'click', (event) => {
          const isOutsideClicked = !(
            this.appTopbar()?.menu()?.nativeElement.isSameNode(event.target) ||
            this.appTopbar()?.menu()?.nativeElement.contains(event.target) ||
            this.appTopbar()?.topbarMenuButton()?.nativeElement.isSameNode(event.target) ||
            this.appTopbar()?.topbarMenuButton()?.nativeElement.contains(event.target)
          );

          if (isOutsideClicked) {
            this.hideProfileMenu();
          }
        });
      }

      if (this.layoutService.state.staticMenuMobileActive) {
        this.blockBodyScroll();
      }
    });

    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.hideMenu();
      this.hideProfileMenu();
    });
  }

  hideMenu() {
    this.layoutService.state.overlayMenuActive = false;
    this.layoutService.state.staticMenuMobileActive = false;
    this.layoutService.state.menuHoverActive = false;
    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
      this.menuOutsideClickListener = null;
    }
    this.unblockBodyScroll();
  }

  hideProfileMenu() {
    this.layoutService.state.profileSidebarVisible = false;
    if (this.profileMenuOutsideClickListener) {
      this.profileMenuOutsideClickListener();
      this.profileMenuOutsideClickListener = null;
    }
  }

  blockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.add('blocked-scroll');
    } else {
      document.body.className += ' blocked-scroll';
    }
  }

  unblockBodyScroll(): void {
    if (document.body.classList) {
      document.body.classList.remove('blocked-scroll');
    } else {
      document.body.className = document.body.className.replace(
        new RegExp('(^|\\b)' + 'blocked-scroll'.split(' ').join('|') + '(\\b|$)', 'gi'),
        ' ',
      );
    }
  }

  get containerClass() {
    return {
      'layout-theme-light': this.layoutService.config.colorScheme === 'light',
      'layout-theme-dark': this.layoutService.config.colorScheme === 'dark',
      'layout-overlay': this.layoutService.config.menuMode === 'overlay',
      'layout-static': this.layoutService.config.menuMode === 'static',
      'layout-static-inactive':
        this.layoutService.state.staticMenuDesktopInactive && this.layoutService.config.menuMode === 'static',
      'layout-overlay-active': this.layoutService.state.overlayMenuActive,
      'layout-mobile-active': this.layoutService.state.staticMenuMobileActive,
      'p-input-filled': this.layoutService.config.inputStyle === 'filled',
      'p-ripple-disabled': !this.layoutService.config.ripple,
    };
  }

  ngOnDestroy() {
    if (this.overlayMenuOpenSubscription) {
      this.overlayMenuOpenSubscription.unsubscribe();
    }

    if (this.menuOutsideClickListener) {
      this.menuOutsideClickListener();
    }
  }
}
