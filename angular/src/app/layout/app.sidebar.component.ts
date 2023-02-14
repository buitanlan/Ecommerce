import { Component, ElementRef } from '@angular/core';
import { LayoutService } from './service/app.layout.service';
import { AppMenuComponent } from './app.menu.component';

@Component({
  selector: 'app-sidebar',
  template: ` <app-menu></app-menu> `,
  imports: [AppMenuComponent],
  standalone: true,
})
export class AppSidebarComponent {
  constructor(public layoutService: LayoutService, public el: ElementRef) {}
}
