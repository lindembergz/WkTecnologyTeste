import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../../../../core/models/product.model';
import { ProductService, ProductQueryParameters, PagedResult } from '../../../../core/services/product.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table'; // Para interagir com a p-table

// Angular Common Module
import { CommonModule } from '@angular/common';

// PrimeNG Modules
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


@Component({
  selector: 'app-product-list',
  standalone: true, // Marcando como standalone
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
  providers: [ConfirmationService, MessageService] // Provendo serviços aqui
})
export class ProductListComponent implements OnInit {
  @ViewChild('dt') dt!: Table; // Referência à p-table

  products: Product[] = [];
  totalRecords: number = 0;
  isLoading = false;

  // Parâmetros para a consulta da API, incluindo paginação e filtro
  queryParams: ProductQueryParameters = {
    page: 1,
    pageSize: 10,
    searchTerm: '',
    // Outros filtros podem ser adicionados aqui e ligados a inputs no template
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

  loadProducts(event?: any): void {
    this.isLoading = true;
    if (event) { // Evento do paginador ou sort da tabela
      this.queryParams.page = event.first / event.rows + 1;
      this.queryParams.pageSize = event.rows;
      // Para ordenação (se a tabela PrimeNG estiver configurada para custom sort)
      // if (event.sortField) {
      //   this.queryParams.sortBy = event.sortField;
      //   this.queryParams.sortDirection = event.sortOrder === 1 ? 'asc' : 'desc';
      // }
    }

    this.productService.getProducts(this.queryParams).subscribe({
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
    this.queryParams.page = 1; // Resetar para a primeira página ao filtrar
    // this.dt.first = 0; // Resetar o paginador da tabela (se estiver usando o paginador interno da tabela)
    this.loadProducts();
  }

  clearFilter(): void {
    this.queryParams.searchTerm = '';
    this.queryParams.page = 1;
    if (this.dt) {
      this.dt.clear(); // Limpa filtros globais e de coluna da tabela
      this.dt.first = 0; // Reseta o paginador da tabela para a primeira página
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
    // O método deleteProduct no serviço já faz o soft delete (desativação)
    this.productService.deleteProduct(productId).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto desativado.' });
        this.loadProducts(); // Recarrega a lista
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
      accept: ()_=> {
        this.isLoading = true;
        action.subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: `Produto ${actionMessage} com sucesso!` });
            // Atualizar o status localmente ou recarregar
             const index = this.products.findIndex(p => p.id === product.id);
             if (index !== -1) {
               this.products[index].isActive = !this.products[index].isActive;
             }
            // this.loadProducts(); // Ou apenas atualizar o item na lista para evitar recarregar tudo
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
