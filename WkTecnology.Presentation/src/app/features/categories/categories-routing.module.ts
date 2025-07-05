import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { CategoryFormComponent } from './components/category-form/category-form.component'; // Importar CategoryFormComponent

const routes: Routes = [
  {
    path: '', // Rota raiz do módulo de categorias (ex: /categories)
    component: CategoryListComponent
  },
  {
    path: 'new', // Rota para criar nova categoria (ex: /categories/new)
    component: CategoryFormComponent
  },
  {
    path: 'edit/:id', // Rota para editar categoria (ex: /categories/edit/1)
    component: CategoryFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CategoriesRoutingModule { }
