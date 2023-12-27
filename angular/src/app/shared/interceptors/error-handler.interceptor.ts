import { HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { NotificationService } from '../services/notification.service';
import { catchError } from 'rxjs';

export const errorHandlerInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((ex) => {
      if (ex.status == HttpStatusCode.InternalServerError) {
        notificationService.showError('Hệ thống có lôi xảy ra. Vui lòng liên hệ admin');
      }
      throw ex;
    }),
  );
};
