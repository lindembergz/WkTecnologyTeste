import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Category } from '../../../../core/models/category.model';
import { CategoryService } from '../../../../core/services/category.service';
import { ConfirmationService, MessageService } from 'primeng/api'; // Para diálogos de confirmação e mensagens

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css'],
  providers: [ConfirmationService, MessageService] // Adicionar seletor de confirmação e mensagens
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];
  isLoading = false;
  // TODO: Adicionar propriedade para o termo de pesquisa e colunas da tabela

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar categorias.' });
        console.error(err);
      }
    });
  }

  navigateToCreateCategory(): void {
    this.router.navigate(['/categories/new']); // TODO: Ajustar a rota conforme necessário
  }

  navigateToEditCategory(categoryId: number): void {
    this.router.navigate(['/categories/edit', categoryId]); // TODO: Ajustar a rota conforme necessário
  }

  deleteCategory(categoryId: number): void {
    this.confirmationService.confirm({
      message: 'Tem certeza que deseja excluir esta categoria?',
      header: 'Confirmação de Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.isLoading = true;
        this.categoryService.deleteCategory(categoryId).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Categoria excluída.' });
            this.loadCategories(); // Recarrega a lista
            this.isLoading = false;
          },
          error: (err) => {
            this.isLoading = false;
            this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao excluir categoria.' });
            console.error(err);
          }
        });
      }
    });
  }

  // TODO: Implementar lógica de pesquisa/filtro
}
