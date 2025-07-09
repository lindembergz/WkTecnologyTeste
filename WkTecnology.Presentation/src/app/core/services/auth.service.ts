import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  twoFactorRequired: boolean;
}

interface RegisterResponse {
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
    private baseApiUrl = environment.apiUrl;

  constructor(private apiService: ApiService, private router: Router) { }

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  login(credentials: any): Observable<LoginResponse> {
    return this.apiService.post<LoginResponse>(`${this.baseApiUrl}/Auth/login`, credentials).pipe(
      tap(response => {
        if (response.accessToken && response.refreshToken) {
          localStorage.setItem('accessToken', response.accessToken);
          localStorage.setItem('refreshToken', response.refreshToken);
          this.loggedIn.next(true);
        }
      }),
      catchError(error => {
        this.loggedIn.next(false);
        return throwError(() => error);
      })
    );
  }

  register(userData: any): Observable<RegisterResponse> {
    return this.apiService.post<RegisterResponse>(`${this.baseApiUrl}/Auth/register`, userData).pipe(
      catchError(error => {
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.loggedIn.next(false);
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }
}
