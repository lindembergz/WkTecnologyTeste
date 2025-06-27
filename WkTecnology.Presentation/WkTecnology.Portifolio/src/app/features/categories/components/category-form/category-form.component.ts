import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../core/models/category.model';
import { UpdateCategoryPayload } from '../../../../core/models/category-payloads.model';
import { MessageService } from 'primeng/api';

import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextarea } from 'primeng/inputtextarea';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';

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
    InputTextarea,
    InputSwitchModule,
    ButtonModule,
    RippleModule,
    ToastModule, 
  ] 

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
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar dados da categoria.' });
        console.error(err);
        this.isLoading = false;
        this.router.navigate(['/categories']); 
      }
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'Atenção', detail: 'Formulário inválido. Verifique os campos.' });
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
  get f() { return this.categoryForm.controls; }
}
