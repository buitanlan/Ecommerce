import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { RouterModule } from '@angular/router';
import { routes } from './app.routes';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideOAuthClient } from 'angular-oauth2-oidc';

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom([RouterModule.forRoot(routes), BrowserAnimationsModule]),
    provideHttpClient(withInterceptors([])),
    provideOAuthClient(),
  ],
};