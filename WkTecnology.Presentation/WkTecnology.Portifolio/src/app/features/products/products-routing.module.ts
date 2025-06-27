import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';

const routes: Routes = [
  {
    path: '', // Rota raiz do módulo de produtos (ex: /products)
    component: ProductListComponent
  },
  {
    path: 'new', // Rota para criar novo produto (ex: /products/new)
    component: ProductFormComponent
  },
  {
    path: 'edit/:id', // Rota para editar produto (ex: /products/edit/1)
    component: ProductFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
