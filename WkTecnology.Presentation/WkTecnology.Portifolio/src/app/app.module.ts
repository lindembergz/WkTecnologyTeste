import { importProvidersFrom, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple'; 

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule, 
    AppRoutingModule,
    HttpClientModule,

    ToolbarModule,
    ButtonModule,
    RippleModule
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
