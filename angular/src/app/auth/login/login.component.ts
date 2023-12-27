import { Component, DestroyRef, inject } from '@angular/core';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { PasswordModule } from 'primeng/password';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { Router, RouterLink } from '@angular/router';
import { ChipsModule } from 'primeng/chips';
import { LoginRequestDto } from '../../shared/models/login-request.dto';
import { AuthService } from '../../shared/services/auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TokenStorageService } from '../../shared/services/token.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-login',
  template: `
    <div class="surface-0 flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden">
      <div class="grid justify-content-center p-2 lg:p-0" style="min-width: 80%">
        <div class="col-12 mt-5 xl:mt-0 text-center">
          <img
            src="assets/layout/images/{{
              layoutService.config.colorScheme === 'light' ? 'logo-dark' : 'logo-white'
            }}.svg"
            alt="Sakai logo"
            class="mb-5"
            style="width: 81px; height: 60px"
          />
        </div>
        <div
          class="col-12 xl:col-6"
          style="
        border-radius: 56px;
        padding: 0.3rem;
        background: linear-gradient(180deg, var(--primary-color) 10%, rgba(33, 150, 243, 0) 30%);
      "
        >
          <div
            class="h-full w-full m-0 py-7 px-4"
            style="
          border-radius: 53px;
          background: linear-gradient(180deg, var(--surface-50) 38.9%, var(--surface-0));
        "
          >
            <div class="text-center mb-5">
              <div class="text-900 text-3xl font-medium mb-3">Xin chào quản trị!</div>
              <span class="text-600 font-medium">Đăng nhập</span>
            </div>

            <form class="w-full md:w-10 mx-auto" [formGroup]="loginForm">
              <label for="email1" class="block text-900 text-xl font-medium mb-2">Email</label>
              <input
                id="email1"
                formControlName="username"
                type="text"
                placeholder="Địa chỉ email"
                pInputText
                class="w-full mb-3"
                style="padding: 1rem"
              />

              <label for="password1" class="block text-900 font-medium text-xl mb-2">Mật khẩu</label>
              <p-password
                id="password1"
                formControlName="password"
                placeholder="Mật khẩu"
                [toggleMask]="true"
                styleClass="w-full mb-3"
              ></p-password>

              <button pButton pRipple (click)="login()" label="Đăng nhập" class="w-full p-3 text-xl"></button>
            </form>
            <p-blockUI [blocked]="blockedPanel">
              <p-progressSpinner></p-progressSpinner>
            </p-blockUI>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      :host ::ng-deep .p-password input {
        width: 100%;
        padding: 1rem;
      }

      :host ::ng-deep .pi-eye {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }

      :host ::ng-deep .pi-eye-slash {
        transform: scale(1.6);
        margin-right: 1rem;
        color: var(--primary-color) !important;
      }
    `,
  ],
  imports: [
    PasswordModule,
    FormsModule,
    CheckboxModule,
    ButtonModule,
    RippleModule,
    RouterLink,
    ChipsModule,
    ReactiveFormsModule,
    ProgressSpinnerModule,
    BlockUIModule,
  ],
  standalone: true,
})
export class LoginComponent {
  valCheck: string[] = ['remember'];
  blockedPanel = false;
  password!: string;

  readonly layoutService = inject(LayoutService);
  readonly fb = inject(FormBuilder);
  readonly router = inject(Router);
  readonly authService = inject(AuthService);
  readonly #destroyRef = inject(DestroyRef);
  readonly #tokenService = inject(TokenStorageService);
  readonly #notificationService = inject(NotificationService);

  loginForm = this.fb.group({
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });
  login() {
    const request: LoginRequestDto = {
      username: this.loginForm.controls.username.value!,
      password: this.loginForm.controls.password.value!,
    };

    this.authService
      .login(request)
      .pipe(takeUntilDestroyed(this.#destroyRef))
      .subscribe({
        next: (res: any) => {
          this.#tokenService.saveToken(res.access_token);
          this.#tokenService.saveRefreshToken(res.refresh_token);
          this.toggleBlockUI(false);
          void this.router.navigate(['']);
        },
        error: () => {
          this.toggleBlockUI(false);
          this.#notificationService.showError('Đăng nhập không đúng.');
        },
      });
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
