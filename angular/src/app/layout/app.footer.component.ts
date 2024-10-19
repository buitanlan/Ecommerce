import { Component, inject } from '@angular/core';
import { LayoutService } from './service/app.layout.service';

@Component({
  selector: 'app-footer',
  template: `
    <div class="layout-footer">
      <img
        src="assets/layout/images/{{ layoutService.config.colorScheme === 'light' ? 'logo-dark' : 'logo-white' }}.svg"
        alt="Logo"
        height="20"
        class="mr-2"
      />
      by
      <span class="font-medium ml-2">PrimeNG</span>
    </div>
  `,
  standalone: true,
})
export class AppFooterComponent {
  public layoutService = inject(LayoutService);
}
