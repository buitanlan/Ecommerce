import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { RouterModule } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { registerLocale } from '@abp/ng.core/locale';
import { environment } from '../environments/environment';
import { CoreModule } from '@abp/ng.core';
import { tokenInterceptor } from './shared/interceptors/token.interceptor';
import { NotificationService } from './shared/services/notification.service';
import { UtilityService } from './shared/services/utility.service';
import { errorHandlerInterceptor } from './shared/interceptors/error-handler.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService, MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideAnimationsAsync(),
    importProvidersFrom(
      RouterModule.forRoot(routes),
      CoreModule.forRoot({
        environment,
        registerLocaleFn: registerLocale(),
      }),
    ),
    provideHttpClient(withFetch(), withInterceptors([tokenInterceptor, errorHandlerInterceptor])),
    DialogService,
    MessageService,
    ConfirmationService,
    NotificationService,
    UtilityService,
  ],
};
