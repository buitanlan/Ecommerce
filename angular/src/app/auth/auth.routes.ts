import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: 'error', loadChildren: () => import('./error/error.routes').then((m) => m.routes) },
  { path: 'access', loadChildren: () => import('./access/access.routes').then((m) => m.routes) },
  { path: 'login', loadChildren: () => import('./login/login.routes ').then((m) => m.routes) },
  { path: '**', redirectTo: '/notfound' },
];
