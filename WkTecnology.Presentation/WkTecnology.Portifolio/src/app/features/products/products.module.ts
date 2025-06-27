import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProductsRoutingModule } from './products-routing.module';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';

// PrimeNG Modules
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { RippleModule } from 'primeng/ripple';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { DropdownModule } from 'primeng/dropdown'; // Para selecionar Categoria
import { InputNumberModule } from 'primeng/inputnumber'; // Para campos numéricos como Preço, Ano, Estoque

@NgModule({
  declarations: [
    // Os componentes serão declarados como standalone ou aqui se não forem
    // ProductListComponent, // Removido se standalone
    // ProductFormComponent  // Removido se standalone
  ],
  imports: [
    CommonModule,
    ProductsRoutingModule,
    FormsModule,
    ReactiveFormsModule,

    // PrimeNG
    TableModule,
    ToastModule,
    ToolbarModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    InputSwitchModule,
    CardModule,
    DialogModule,
    ConfirmDialogModule,
    RippleModule,
    TagModule,
    TooltipModule,
    DropdownModule,
    InputNumberModule,

    // Importar os componentes se eles forem standalone e usados dentro deste módulo (ex: em um componente container)
    // Ou se eles não forem standalone, eles já fazem parte do módulo através das declarations.
    // Se os componentes forem declarados como standalone e roteados diretamente,
    // eles não precisam ser importados aqui, mas seus próprios imports (CommonModule, PrimeNG etc) devem estar corretos.
    // Dado o padrão do CategoriesModule, os componentes são importados diretamente.
    ProductListComponent,
    ProductFormComponent
  ],
  providers: [
    // MessageService e ConfirmationService são geralmente providos nos componentes que os utilizam
    // ou podem ser providos aqui se forem usados por múltiplos componentes no módulo.
  ]
})
export class ProductsModule { }
