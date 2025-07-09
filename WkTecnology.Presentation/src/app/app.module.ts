import { importProvidersFrom, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; 
import { HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms'; // Import FormsModule

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './features/auth/login/login.component'; // Import LoginComponent
import { RegisterComponent } from './features/auth/register/register.component'; // Import RegisterComponent

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
    FormsModule, // Add FormsModule here

    ToolbarModule,
    ButtonModule,
    RippleModule,
             HttpClientXsrfModule.withOptions({
           cookieName: 'XSRF-TOKEN',
           headerName: 'X-XSRF-TOKEN'
         })
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
