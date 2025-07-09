import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { FormsModule } from '@angular/forms'; // Import FormsModule

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true, // Mark as standalone
  imports: [FormsModule] // Add FormsModule to imports
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit(): void {
    this.authService.login({ username: this.username, password: this.password }).subscribe({
      next: (response) => {
        // Handle 2FA if needed, otherwise navigate
        if (response.twoFactorRequired) {
          this.errorMessage = 'Two-factor authentication is required. Please verify your code.';
          // You might want to navigate to a 2FA verification page here
        } else {
          this.router.navigate(['/products']); // Navigate to products page on successful login
        }
      },
      error: (error) => {
        this.errorMessage = 'Login failed: ' + (error.error.message || error.message);
        console.error('Login error:', error);
      }
    });
  }
}
