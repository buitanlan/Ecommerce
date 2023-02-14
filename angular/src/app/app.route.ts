import { Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app.layout.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => AppLayoutComponent
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.route').then(m => m.routes)
  }
];
