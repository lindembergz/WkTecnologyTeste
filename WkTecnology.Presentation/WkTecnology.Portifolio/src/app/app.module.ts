import { importProvidersFrom, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // Importar BrowserAnimationsModule
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule, // Adicionar BrowserAnimationsModule
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [    
      providePrimeNG({
        theme: {
          preset: Aura
        }
      })
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
