import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Product } from '../models/product.model';
import { CreateProductPayload, UpdateProductPayload } from '../models/product-payloads.model';
import { environment } from '../../../environments/environment';
import { ApiService } from './api.service';

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
export class ProductService implements OnInit{
  private baseApiUrl = environment.apiUrl;
  private productsEndpoint = '/products'; // Conforme ProductsController.cs

  private _xsrfToken: string | null = null;

  constructor( private api: ApiService
  ) {
    
   }

    ngOnInit(): void {

            // No serviço Angular, chame este método ao iniciar a aplicação ou antes de requisições protegidas
     


    }

  private get fullProductsUrl(): string {
    return `${this.baseApiUrl}${this.productsEndpoint}`;
  }
/*
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
  }*/

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

    return this.api.get<PagedResult<Product>>(this.fullProductsUrl);
  }

  getProductById(id: number): Observable<Product> {
    const url = `${this.fullProductsUrl}/${id}`;
    return this.api.get<Product>(url);
  }

  createProduct(productData: CreateProductPayload): Observable<Product> {
    return this.api.post<Product>(this.fullProductsUrl, productData)
      
  }

  updateProduct(id: number, payload: UpdateProductPayload): Observable<Product> {
     return this.api.put<Product>(`${this.fullProductsUrl}/${id}`, payload );
  }

  deleteProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}`;
    return this.api.delete<void>(url);
  }

  activateProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}/activate`;
    return this.api.patch<void>(url, null);
  }

  deactivateProduct(id: number): Observable<void> {
    const url = `${this.fullProductsUrl}/${id}/deactivate`;
    return this.api.patch<void>(url, null);
  }
}
