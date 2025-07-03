import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private _xsrfToken: string | null = null;
  private baseApiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {
    this.loadXsrfToken();
  }

  private loadXsrfToken() {
    this.http.get<{ token: string }>(`${this.baseApiUrl}/antiforgery/token`, { withCredentials: true })
      .subscribe({

        next: res => this._xsrfToken = res.token,
        error: () => this._xsrfToken = null
      });
  }

  private getHeaders(extraHeaders?: { [key: string]: string }): HttpHeaders {
   
   
    let headers = new HttpHeaders(extraHeaders || {});
    if (this._xsrfToken) {
      headers = headers.set('X-XSRF-TOKEN', this._xsrfToken);
    }
    return headers;
  }

  get<T>(url: string, params?: HttpParams, extraHeaders?: { [key: string]: string }): Observable<T> {
    return this.http.get<T>(url, {
      params,
      headers: this.getHeaders(extraHeaders),
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  post<T>(url: string, body: any, extraHeaders?: { [key: string]: string }): Observable<T> {
    return this.http.post<T>(url, body, {
      headers: this.getHeaders(extraHeaders),
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  put<T>(url: string, body: any, extraHeaders?: { [key: string]: string }): Observable<T> {

    return this.http.put<T>(url, body, {
      headers: this.getHeaders(extraHeaders),
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  delete<T>(url: string, extraHeaders?: { [key: string]: string }): Observable<T> {
    return this.http.delete<T>(url, {
      headers: this.getHeaders(extraHeaders),
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  patch<T>(url: string, body: any, extraHeaders?: { [key: string]: string }): Observable<T> {
    return this.http.patch<T>(url, body, {
      headers: this.getHeaders(extraHeaders),
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    // Centralize seu tratamento de erro aqui
    return throwError(() => error);
  }
}