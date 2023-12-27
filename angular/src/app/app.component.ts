import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AuthService } from './shared/services/auth.service';
import { LOGIN_URL } from './shared/constants/urls.const';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-root',
  template: `
    <router-outlet></router-outlet>
    <p-toast position="top-right"></p-toast>
    <p-confirmDialog
      header="Xác nhận"
      acceptLabel="Có"
      rejectLabel="Không"
      icon="pi pi-exclamation-triangle"
    ></p-confirmDialog>
  `,
  imports: [RouterOutlet, ToastModule, ConfirmDialogModule],
  standalone: true,
})
export class AppComponent implements OnInit {
  menuMode = 'static';
  readonly #primengConfig = inject(PrimeNGConfig);
  readonly #authService = inject(AuthService);
  readonly #router = inject(Router);

  ngOnInit() {
    this.#primengConfig.ripple = true;
    document.documentElement.style.fontSize = '14px';
    if (!this.#authService.isAuthenticated()) {
      void this.#router.navigate([LOGIN_URL]);
    }
  }
}
