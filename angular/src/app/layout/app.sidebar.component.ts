import { Component, ElementRef, inject } from '@angular/core';
import { LayoutService } from './service/app.layout.service';
import { AppMenuComponent } from './app.menu.component';

@Component({
  selector: 'app-sidebar',
  template: ` <app-menu></app-menu> `,
  imports: [AppMenuComponent],
  standalone: true,
})
export class AppSidebarComponent {
  public layoutService = inject(LayoutService);
  public el = inject(ElementRef);
}
