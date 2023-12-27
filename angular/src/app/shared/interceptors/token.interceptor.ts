import { inject } from '@angular/core';
import { HttpRequest, HttpHandler, HttpErrorResponse, HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { TokenStorageService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { LoginResponseDto } from '../models/login-response.dto';
const TOKEN_HEADER_KEY = 'Authorization'; // for Spring Boot back-end

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  let isRefreshing = false;
  let refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  const tokenService = inject(TokenStorageService);
  const authService = inject(AuthService);

  let authReq = req;
  const token = tokenService.getToken();
  if (token != null) {
    authReq = addTokenHeader(req, token);
  }

  return next(authReq).pipe(
    catchError((error) => {
      if (
        error instanceof HttpErrorResponse &&
        !authReq.url.includes('auth/login') &&
        error.status === HttpStatusCode.Unauthorized
      ) {
        if (!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(null);

          const token = tokenService.getRefreshToken();

          if (token)
            return authService.refreshToken(token).pipe(
              switchMap((res: LoginResponseDto) => {
                isRefreshing = false;

                tokenService.saveToken(res.access_token);
                refreshTokenSubject.next(res.access_token);

                return next(addTokenHeader(authReq, res.access_token));
              }),
              catchError((err) => {
                isRefreshing = false;

                tokenService.signOut();
                return throwError(err);
              }),
            );
        }
        return refreshTokenSubject.pipe(
          filter((token) => token !== null),
          take(1),
          switchMap((token) => next(addTokenHeader(authReq, token))),
        );
      }

      return throwError(error);
    }),
  );
};

function addTokenHeader(request: HttpRequest<any>, token: string) {
  return request.clone({ headers: request.headers.set(TOKEN_HEADER_KEY, `Bearer ${token}`) });
}
