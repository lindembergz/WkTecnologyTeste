import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'products',
    loadChildren: () => import('./features/products/products.module').then(m => m.ProductsModule)
  },
  {
    path: 'categories',
    loadChildren: () => import('./features/categories/categories.module').then(m => m.CategoriesModule)
  },
  {
    path: '',
    redirectTo: 'products', // Definir /products como a rota padrão
    pathMatch: 'full'
  },
  // Adicionar uma rota wildcard para página não encontrada, se desejado
  // { path: '**', component: PageNotFoundComponent }, // Exemplo: criar um PageNotFoundComponent
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
