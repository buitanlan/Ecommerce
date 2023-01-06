import { importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.route';


bootstrapApplication(AppComponent, {
  providers: [ importProvidersFrom(RouterModule.forRoot(routes))]
})