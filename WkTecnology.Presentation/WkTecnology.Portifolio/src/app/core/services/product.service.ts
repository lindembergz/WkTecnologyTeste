import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Product } from '../models/product.model';
import { CreateProductPayload, UpdateProductPayload } from '../models/product-payloads.model';
import { environment } from '../../../environments/environment';

// Interface para representar a resposta paginada da API (similar ao PagedResult<T> do backend)
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  // Adicione outras propriedades de paginação se necessário (hasNextPage, hasPreviousPage, etc.)
}

// Interface para os parâmetros de consulta de produtos (similar ao ProductQuery do backend)
export interface ProductQueryParameters {
  searchTerm?: string;
  categoryId?: number;
  minYear?: number;
  maxYear?: number;
  brand?: string;
  color?: string;
  minMileage?: number;
  maxMileage?: number;
  isActive?: boolean;
  sortBy?: string; // Ex: 'name', 'year', 'mileage'
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private baseApiUrl = environment.apiUrl;
  private productsEndpoint = '/products'; // Conforme ProductsController.cs

  constructor(private http: HttpClient) { }

  private get fullProductsUrl(): string {
    return `${this.baseApiUrl}${this.productsEndpoint}`;
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Ocorreu um erro na API:', error);

    let errorMessage = 'Algo deu errado; por favor, tente novamente mais tarde.';
    if (error.error instanceof ErrorEvent) {

      errorMessage = `Erro: ${error.error.message}`;
    } else if (error.status === 400) {
      errorMessage = error.error?.message || error.error || 'Requisição inválida.';
      if (typeof error.error === 'object' && error.error.errors) {
        //errorMessage = JSON.stringify(error.error.errors);
      }
    } else if (error.status === 404) {
      errorMessage = 'Recurso não encontrado.';
    } else if (error.status === 422) {
        errorMessage = error.error?.message || error.error || 'Entidade não processável.';
    }

    return throwError(() => new Error(errorMessage));
  }

  getProducts(queryParams?: ProductQueryParameters): Observable<PagedResult<Product>> {
    let params = new HttpParams();
    if (queryParams) {
      Object.entries(queryParams).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          if (key === 'isActive' && typeof value === 'boolean') {
            params = params.append(key, value.toString());
          } else if (value) {
             params = params.append(key, value as string | number);
          }
        }
      });
    }

    return this.http.get<PagedResult<Product>>(this.fullProductsUrl, { params })
      .pipe(catchError(this.handleError));
  }

  getProductById(id: number): Observable<Product> {
    const url = `${this.fullProductsUrl}/${id}`;
    return this.http.get<Product>(url)
      .pipe(catchError(this.handleError));
  }

  createProduct(productData: CreateProductPayload): Observable<Product> {
    return this.http.post<Product>(this.fullProductsUrl, productData)
      .pipe(catchError(this.handleError));
  }

  updateProduct(id: number, payload: UpdateProductPayload): Observable<Product> {
    const url = `${this.fullProductsUrl}/${id}`;
    return this.http.put<Product>(url, payload)
      .pipe(catchError(this.handleError));
  }


  deleteProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}`;
    return this.http.delete<void>(url)
      .pipe(catchError(this.handleError));
  }

  activateProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}/activate`;
    return this.http.patch<void>(url, null) 
      .pipe(catchError(this.handleError));
  }

  deactivateProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}/deactivate`;
    return this.http.patch<void>(url, null) 
      .pipe(catchError(this.handleError));
  }
}
