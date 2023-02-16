import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'app-root',
  template: ` <router-outlet></router-outlet> `,
  imports: [RouterOutlet],
  standalone: true,
})
export class AppComponent {
  menuMode = 'static';
  private readonly primengConfig = inject(PrimeNGConfig);
  private readonly authService = inject(AuthService);

  ngOnInit() {
    this.primengConfig.ripple = true;
    document.documentElement.style.fontSize = '14px';
  }
}
