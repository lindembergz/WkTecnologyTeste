import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../../../../core/models/product.model';
import { ProductService, ProductQueryParameters, PagedResult } from '../../../../core/services/product.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table'; 


import { CommonModule } from '@angular/common';


import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToolbarModule } from 'primeng/toolbar';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { TooltipModule } from 'primeng/tooltip';
import { PaginatorModule } from 'primeng/paginator';
import { Subject, takeUntil } from 'rxjs';


@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    ToastModule,
    ConfirmDialogModule,
    ToolbarModule,
    TableModule,
    TagModule,
    InputTextModule,
    ButtonModule,
    RippleModule,
    TooltipModule,
    PaginatorModule
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
  providers: [ConfirmationService, MessageService] 
})
export class ProductListComponent implements OnInit, OnDestroy {
  @ViewChild('dt') dt!: Table;

  products: Product[] = [];
  totalRecords: number = 0;
  isLoading = false;
  private destroy$ = new Subject<void>();


  queryParams: ProductQueryParameters = {
    page: 1,
    pageSize: 10,
    searchTerm: '',

  };

  constructor(
    private productService: ProductService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProducts(event?: any): void {
    this.isLoading = true;
    if (event) { 
      this.queryParams.page = event.first / event.rows + 1;
      this.queryParams.pageSize = event.rows;
    }

    this.productService.getProducts(this.queryParams)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (pagedResult: PagedResult<Product>) => {
        this.products = pagedResult.items;
        this.totalRecords = pagedResult.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar produtos.' });
        console.error(err);
      }
    });
  }

  filterTable(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    this.queryParams.searchTerm = inputElement.value;
    this.queryParams.page = 1; 
    this.loadProducts();
  }

  clearFilter(): void {
    this.queryParams.searchTerm = '';
    this.queryParams.page = 1;
    if (this.dt) {
      this.dt.clear(); 
      this.dt.first = 0; 
    }
    this.loadProducts();
  }


  navigateToCreateProduct(): void {
    this.router.navigate(['/products/new']);
  }

  navigateToEditProduct(productId: number): void {
    this.router.navigate(['/products/edit', productId]);
  }

  confirmDeleteProduct(productId: number): void {
    this.confirmationService.confirm({
      message: 'Tem certeza que deseja excluir este produto? Esta ação o desativará.',
      header: 'Confirmação de Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, excluir',
      rejectLabel: 'Não',
      accept: () => {
        this.deleteProduct(productId);
      }
    });
  }

  private deleteProduct(productId: number): void {
    this.isLoading = true;

    this.productService.deleteProduct(productId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto desativado.' });
        this.loadProducts(); 
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao desativar produto.' });
        console.error(err);
      }
    });
  }

  toggleProductStatus(product: Product): void {
    const action = product.isActive ? this.productService.deactivateProduct(product.id) : this.productService.activateProduct(product.id);
    const actionMessage = product.isActive ? 'desativado' : 'ativado';

    this.confirmationService.confirm({
      message: `Tem certeza que deseja ${actionMessage.replace('ado', 'ar')} este produto?`,
      header: `Confirmar ${actionMessage.charAt(0).toUpperCase() + actionMessage.slice(1)}`,
      icon: 'pi pi-info-circle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.isLoading = true;
        action
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: `Produto ${actionMessage} com sucesso!` });

             const index = this.products.findIndex(p => p.id === product.id);
             if (index !== -1) {
               this.products[index].isActive = !this.products[index].isActive;
             }
            this.isLoading = false;
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Erro', detail: `Falha ao ${actionMessage.replace('ado', 'ar')} o produto.` });
            this.isLoading = false;
            console.error(err);
          }
        });
      }
    });
  }
}
