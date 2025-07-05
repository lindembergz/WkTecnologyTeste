import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Category } from '../models/category.model';
import { UpdateCategoryPayload } from '../models/category-payloads.model'; 
import { environment } from '../../../environments/environment';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private baseApiUrl = environment.apiUrl; 
  private categoriesEndpoint = '/categories'; 

  constructor(private api: ApiService) { }

  private get fullCategoriesUrl(): string {
    return `${this.baseApiUrl}${this.categoriesEndpoint}`;
  }

  getCategories(): Observable<Category[]> {
    return this.api.get<Category[]>(this.fullCategoriesUrl);
  }

  getCategoryById(id: number): Observable<Category> {
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.api.get<Category>(url);
  }

  createCategory(categoryData: Partial<Category>): Observable<Category> {
    return this.api.post<Category>(this.fullCategoriesUrl, categoryData);
  }

  updateCategory(id: number, payload: UpdateCategoryPayload): Observable<Category> { 
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.api.put<Category>(url, payload);
  }

  deleteCategory(id: number): Observable<void> {
    const url = `${this.fullCategoriesUrl}/${id}`;
    return this.api.delete<void>(url);
  }
}
