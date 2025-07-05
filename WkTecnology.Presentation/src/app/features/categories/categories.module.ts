import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CategoriesRoutingModule } from './categories-routing.module';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { CategoryFormComponent } from './components/category-form/category-form.component'; // Importar CategoryFormComponent

// PrimeNG Modules
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextarea } from 'primeng/inputtextarea'; // Adicionar InputTextareaModule
import { InputSwitchModule } from 'primeng/inputswitch';   // Adicionar InputSwitchModule
import { CardModule } from 'primeng/card';                 // Adicionar CardModule
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { RippleModule } from 'primeng/ripple';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
// import { DropdownModule } from 'primeng/dropdown'; // Para p-dropdown de Parent Category, se necessário

@NgModule({
  declarations: [
// Adicionar CategoryFormComponent às declarações
  ],
  imports: [
    CommonModule,
    CategoriesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    // PrimeNG
    TableModule,
    ToastModule,
    ToolbarModule,
    ButtonModule,
    InputTextModule,
    InputTextarea, // Adicionar
    InputSwitchModule,   // Adicionar
    CardModule,          // Adicionar
    DialogModule,
    ConfirmDialogModule,
    RippleModule,
    TagModule,
    TooltipModule,
    // DropdownModule // Adicionar se usar p-dropdown

        CategoryListComponent,
    CategoryFormComponent 
  ],
  providers: [
    // MessageService e ConfirmationService são providos nos próprios componentes ou podem ser providos aqui se forem usados por mais componentes no módulo.
  ]
})
export class CategoriesModule { }
