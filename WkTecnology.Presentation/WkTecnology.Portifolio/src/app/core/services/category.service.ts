import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Category } from '../models/category.model';
import { UpdateCategoryPayload } from '../models/category-payloads.model'; 
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private baseApiUrl = environment.apiUrl; 
  private categoriesEndpoint = '/categories'; 

  constructor(private http: HttpClient) { }

  private get fullCategoriesUrl(): string {
    return `${this.baseApiUrl}${this.categoriesEndpoint}`;
  }

  private handleError(error: HttpErrorResponse) {

    console.error('Ocorreu um erro na API:', error);
    return throwError(() => new Error('Algo deu errado; por favor, tente novamente mais tarde.'));
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.fullCategoriesUrl)
      .pipe(catchError(this.handleError));
  }

  getCategoryById(id: number): Observable<Category> {
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.http.get<Category>(url)
      .pipe(catchError(this.handleError));
  }

  createCategory(categoryData: Partial<Category>): Observable<Category> {
    return this.http.post<Category>(this.fullCategoriesUrl, categoryData)
      .pipe(catchError(this.handleError));
  }

  updateCategory(id: number, payload: UpdateCategoryPayload): Observable<Category> { 
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.http.put<Category>(url, payload)
      .pipe(catchError(this.handleError));
  }

  deleteCategory(id: number): Observable<void> {
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.http.delete<void>(url)
      .pipe(catchError(this.handleError));
  }
}
