import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginRequestDto } from '../models/login-request.dto';
import { LoginResponseDto } from '../models/login-response.dto';
import { environment } from '../../../environments/environment';
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../constants/keys.constant';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  readonly #httpClient = inject(HttpClient);
  public login(input: LoginRequestDto): Observable<LoginResponseDto> {
    const body = {
      username: input.username,
      password: input.password,
      client_id: environment.oAuthConfig.clientId,
      client_secret: environment.oAuthConfig.dummyClientSecret,
      grant_type: 'password',
      scope: environment.oAuthConfig.scope,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent((body as { [K: string]: string })[key])}`)
      .join('&');
    return this.#httpClient.post<LoginResponseDto>(environment.oAuthConfig.issuer + 'connect/token', data, {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    });
  }

  public refreshToken(refreshToken: string): Observable<LoginResponseDto> {
    const body = {
      client_id: environment.oAuthConfig.clientId,
      client_secret: environment.oAuthConfig.dummyClientSecret,
      grant_type: 'refresh_token',
      refresh_token: refreshToken,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent((body as any)[key])}`)
      .join('&');
    return this.#httpClient.post<LoginResponseDto>(environment.oAuthConfig.issuer + 'connect/token', data, {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    });
  }

  isAuthenticated = () => !!localStorage.getItem(ACCESS_TOKEN);

  logout() {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(REFRESH_TOKEN);
  }
}
