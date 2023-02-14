import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: 'error', loadChildren: () => import('./error/error.route').then((m) => m.routes) },
  { path: 'access', loadChildren: () => import('./access/access.route').then((m) => m.routes) },
  { path: 'login', loadChildren: () => import('./login/login.route').then((m) => m.routes) },
  { path: '**', redirectTo: '/notfound' },
];
