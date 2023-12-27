import { Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app.layout.component';
import { ProductComponent } from './product/product.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => AppLayoutComponent,
  },
  {
    path: 'product',
    loadChildren: () => import('./product/product.routes').then((m) => m.routes),
    loadComponent: () => AppLayoutComponent,
  },
  {
    path: 'attribute',
    loadChildren: () => import('./attribute/attribute.routes').then((m) => m.routes),
    component: AppLayoutComponent,
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes').then((m) => m.routes),
  },
];
