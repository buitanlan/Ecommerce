import { inject } from '@angular/core';
import { HttpRequest, HttpHandler, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { TokenStorageService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
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
      if (error instanceof HttpErrorResponse && !authReq.url.includes('auth/login') && error.status === 401) {
        // return this.handle401Error(authReq, next);
      }

      return throwError(error);
    }),
  );
};

// private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
//   if (!this.isRefreshing) {
//     this.isRefreshing = true;
//     this.refreshTokenSubject.next(null);
//
//     const token = this.tokenService.getRefreshToken();
//
//     if (token)
//       return this.authService.refreshToken(token).pipe(
//         switchMap((res: LoginResponseDto) => {
//           this.isRefreshing = false;
//
//           this.tokenService.saveToken(res.access_token);
//           this.refreshTokenSubject.next(res.access_token);
//
//           return next.handle(this.addTokenHeader(request, res.access_token));
//         }),
//         catchError(err => {
//           this.isRefreshing = false;
//
//           this.tokenService.signOut();
//           return throwError(err);
//         })
//       );
//   }

//   return this.refreshTokenSubject.pipe(
//     filter(token => token !== null),
//     take(1),
//     switchMap(token => next.handle(this.addTokenHeader(request, token)))
//   );
// }

function addTokenHeader(request: HttpRequest<any>, token: string) {
  return request.clone({ headers: request.headers.set(TOKEN_HEADER_KEY, `Bearer ${token}`) });
}
