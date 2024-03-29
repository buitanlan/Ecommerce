import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { RouterModule } from '@angular/router';
import { routes } from './app.routes';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { registerLocale } from '@abp/ng.core/locale';
import { environment } from '../environments/environment';
import { CoreModule } from '@abp/ng.core';
import { tokenInterceptor } from './shared/interceptors/token.interceptor';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { NotificationService } from './shared/services/notification.service';
import { UtilityService } from './shared/services/utility.service';
import { errorHandlerInterceptor } from './shared/interceptors/error-handler.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(
      RouterModule.forRoot(routes),
      BrowserAnimationsModule,
      CoreModule.forRoot({
        environment,
        registerLocaleFn: registerLocale(),
      }),
    ),
    provideHttpClient(withInterceptors([tokenInterceptor, errorHandlerInterceptor])),
    DialogService,
    MessageService,
    NotificationService,
    UtilityService,
    ConfirmationService,
  ],
};
