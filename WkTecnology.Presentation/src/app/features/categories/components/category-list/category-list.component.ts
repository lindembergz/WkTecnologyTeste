import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Category } from '../../../../core/models/category.model';
import { CategoryService } from '../../../../core/services/category.service';
import { ConfirmationService, MessageService } from 'primeng/api';

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
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css'],
  providers: [ConfirmationService, MessageService],
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
    TooltipModule
  ]
})
export class CategoryListComponent implements OnInit, OnDestroy {
  categories: Category[] = [];
  isLoading = false;
  private destroy$ = new Subject<void>();


  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getCategories()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (data: any) => {
        console.log(data)
        this.categories = data.items;
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
    this.router.navigate(['/categories/new']); 
  }

  navigateToEditCategory(categoryId: number): void {
    this.router.navigate(['/categories/edit', categoryId]); 
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
        this.categoryService.deleteCategory(categoryId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Categoria excluída.' });
            this.loadCategories(); 
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

  filterTable(event: Event, dt: any) {
  const input = event.target as HTMLInputElement;
  dt.filterGlobal(input.value, 'contains');
}
}
