import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service'; // Importar CategoryService
import { Product } from '../../../../core/models/product.model';
import { Category } from '../../../../core/models/category.model'; // Importar Category model
import { CreateProductPayload, UpdateProductPayload } from '../../../../core/models/product-payloads.model';
import { MessageService, SelectItem } from 'primeng/api'; // SelectItem para o dropdown

// PrimeNG Modules
import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextarea } from 'primeng/inputtextarea';
import { InputSwitchModule } from 'primeng/inputswitch'; // Para o campo isActive (se for editável no form)
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { DropdownModule } from 'primeng/dropdown'; // Para o campo Categoria
import { InputNumberModule } from 'primeng/inputnumber'; // Para Ano e Quilometragem

@Component({
  selector: 'app-product-form',
  standalone: true, // Marcando como standalone
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
    DropdownModule,
    InputNumberModule
  ],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css'],
  providers: [MessageService] // Provendo MessageService aqui
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  isEditMode = false;
  productId?: number;
  isLoading = false;
  pageTitle = 'Novo Produto';
  categories: SelectItem[] = []; // Para o dropdown de categorias

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService, // Injetar CategoryService
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.productId = this.route.snapshot.params['id'];
    this.isEditMode = !!this.productId;
    this.pageTitle = this.isEditMode ? 'Editar Produto' : 'Novo Produto';

    this.initForm();
    this.loadCategories(); // Carregar categorias para o dropdown

    if (this.isEditMode && this.productId) {
      this.loadProductData(this.productId);
    }
  }

  private initForm(): void {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
      description: ['', [Validators.maxLength(500)]],
      brand: ['', [Validators.required, Validators.maxLength(50)]],
      model: ['', [Validators.required, Validators.maxLength(50)]],
      year: [null, [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      color: ['', [Validators.required, Validators.maxLength(30)]],
      mileage: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null, [Validators.required]],
      isActive: [true] // Valor padrão para isActive. Ajustar se necessário.
                       // Este campo não está no CreateProductPayload nem no UpdateProductPayload
                       // Se for editável, precisa ser adicionado aos payloads ou gerenciado separadamente.
    });
  }

  private loadCategories(): void {
    this.categoryService.getCategories().subscribe({ // Assumindo que getCategories retorna um array de Category
      next: (categoriesPagedResult: any) => { // Ajustar para PagedResult<Category> se for o caso
        const activeCategories = categoriesPagedResult.items.filter((cat: Category) => cat.isActive);
        this.categories = activeCategories.map((cat: Category) => ({ label: cat.name, value: cat.id }));
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar categorias.' });
        console.error(err);
      }
    });
  }

  private loadProductData(id: number): void {
    this.isLoading = true;
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.productForm.patchValue({
          name: product.name,
          description: product.description,
          brand: product.brand,
          model: product.model,
          year: product.year,
          color: product.color,
          mileage: product.mileage,
          categoryId: product.categoryId,
          isActive: product.isActive 
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar dados do produto.' });
        console.error(err);
        this.isLoading = false;
        this.router.navigate(['/products']); 
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'Atenção', detail: 'Formulário inválido. Verifique os campos.' });
      Object.values(this.productForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }

    this.isLoading = true;
    const formValues = this.productForm.value;

    if (this.isEditMode && this.productId) {
      const updatePayload: UpdateProductPayload = {
        name: formValues.name,
        description: formValues.description,
        brand: formValues.brand,
        model: formValues.model,
        year: formValues.year,
        color: formValues.color,
        mileage: formValues.mileage

      };
      this.productService.updateProduct(this.productId, updatePayload).subscribe({
        next: (updatedProduct) => {
          if (formValues.isActive !== updatedProduct.isActive) {
            const statusUpdateAction = formValues.isActive
              ? this.productService.activateProduct(updatedProduct.id)
              : this.productService.deactivateProduct(updatedProduct.id);

            statusUpdateAction.subscribe({
              next: () => {
                this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto atualizado e status modificado!' });
                this.isLoading = false;
                this.router.navigate(['/products']);
              },
              error: (err) => {
                 this.messageService.add({ severity: 'warn', summary: 'Parcialmente Concluído', detail: 'Produto atualizado, mas falha ao modificar status.' });
                 this.isLoading = false;
                 this.router.navigate(['/products']);
              }
            });
          } else {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto atualizado!' });
            this.isLoading = false;
            this.router.navigate(['/products']);
          }
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao atualizar produto.' });
          console.error(err);
          this.isLoading = false;
        }
      });
    } else { 
      const createPayload: CreateProductPayload = {
        name: formValues.name,
        description: formValues.description,
        brand: formValues.brand,
        model: formValues.model,
        year: formValues.year,
        color: formValues.color,
        mileage: formValues.mileage,
        categoryId: formValues.categoryId

      };
      this.productService.createProduct(createPayload).subscribe({
        next: (createdProduct) => {
          if (formValues.isActive === false && createdProduct.isActive === true) {
             this.productService.deactivateProduct(createdProduct.id).subscribe({
               next: () => {
                  this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto criado e desativado conforme solicitado!' });
                  this.isLoading = false;
                  this.router.navigate(['/products']);
               },
               error: (err) => {
                  this.messageService.add({ severity: 'warn', summary: 'Parcialmente Concluído', detail: 'Produto criado, mas falha ao desativar.' });
                  this.isLoading = false;
                  this.router.navigate(['/products']);
               }
             });
          } else {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Produto criado!' });
            this.isLoading = false;
            this.router.navigate(['/products']);
          }
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao criar produto.' });
          console.error(err);
          this.isLoading = false;
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/products']);
  }

  get f() { return this.productForm.controls; }
}
