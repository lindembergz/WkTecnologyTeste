import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'categories',
    loadChildren: () => import('./features/categories/categories.module').then(m => m.CategoriesModule)
  },
  // Adicionar uma rota padrão, por exemplo, redirecionar para categories ou um dashboard
  {
    path: '',
    redirectTo: 'categories', // Ou qualquer outra rota padrão desejada
    pathMatch: 'full'
  },
  // Adicionar uma rota wildcard para página não encontrada, se desejado
  // { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
