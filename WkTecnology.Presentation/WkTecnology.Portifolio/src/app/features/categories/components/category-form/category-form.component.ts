import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../core/models/category.model';
import { UpdateCategoryPayload } from '../../../../core/models/category-payloads.model';
import { MessageService } from 'primeng/api';

// PrimeNG Modules - Experimental direct import for non-standalone component
import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
// RippleModule might be needed for pRipple on button, usually comes with ButtonModule or imported separately
import { RippleModule } from 'primeng/ripple';


@Component({
  selector: 'app-category-form',
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.css'],
  providers: [MessageService],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ToastModule,
    CardModule,
    InputTextModule,
    InputTextareaModule,
    InputSwitchModule,
    ButtonModule,
    RippleModule // Added for pRipple
  ] // This is for standalone components. Angular might error here.
})
export class CategoryFormComponent implements OnInit {
  categoryForm!: FormGroup;
  isEditMode = false;
  categoryId?: number;
  isLoading = false;
  pageTitle = 'Nova Categoria';

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.categoryId = this.route.snapshot.params['id'];
    this.isEditMode = !!this.categoryId;
    this.pageTitle = this.isEditMode ? 'Editar Categoria' : 'Nova Categoria';

    this.initForm();

    if (this.isEditMode && this.categoryId) {
      this.loadCategoryData(this.categoryId);
    }
  }

  private initForm(): void {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(255)]],
      isActive: [true, Validators.required]
      // parentCategoryId: [null] // Adicionar se a lógica de subcategorias for implementada no form
    });
  }

  private loadCategoryData(id: number): void {
    this.isLoading = true;
    this.categoryService.getCategoryById(id).subscribe({
      next: (category) => {
        this.categoryForm.patchValue({
          name: category.name,
          description: category.description,
          isActive: category.isActive
          // parentCategoryId: category.parentCategoryId
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar dados da categoria.' });
        console.error(err);
        this.isLoading = false;
        this.router.navigate(['/categories']); // Volta para a lista em caso de erro
      }
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'Atenção', detail: 'Formulário inválido. Verifique os campos.' });
      // Marcar todos os campos como tocados para exibir mensagens de erro
      Object.values(this.categoryForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }

    this.isLoading = true;
    const formData = this.categoryForm.value;

    if (this.isEditMode && this.categoryId) {
      const updatePayload: UpdateCategoryPayload = {
        name: formData.name,
        description: formData.description,
        isActive: formData.isActive,
        // parentCategoryId: formData.parentCategoryId // Adicionar se o campo existir no form
      };
      this.categoryService.updateCategory(this.categoryId, updatePayload).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Categoria atualizada!' });
          this.isLoading = false;
          this.router.navigate(['/categories']);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao atualizar categoria.' });
          console.error(err);
          this.isLoading = false;
        }
      });
    } else {
      // Use formData, which is this.categoryForm.value
      // Ensure the structure of formData matches what createCategory expects.
      // If CreateCategoryPayload is defined and different, map formData to it.
      // Assuming createCategory can take an object like { name, description, isActive }
      this.categoryService.createCategory(formData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Categoria criada!' });
          this.isLoading = false;
          this.router.navigate(['/categories']);
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao criar categoria.' });
          console.error(err);
          this.isLoading = false;
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/categories']);
  }

  // Helper para acesso fácil aos controles do formulário no template
  get f() { return this.categoryForm.controls; }
}
