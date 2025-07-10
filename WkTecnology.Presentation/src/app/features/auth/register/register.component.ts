import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule] // Add CommonModule to imports
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';
  successMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit(): void {
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.authService.register({ username: this.username, email: this.email, password: this.password }).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Registration successful. Please confirm your email.';
        this.errorMessage = '';
        // Optionally navigate to login after successful registration
        // this.router.navigate(['/login']);
      },
      error: (error) => {
        this.errorMessage = 'Registration failed: ' + (error.error.message || error.message);
        this.successMessage = '';
        console.error('Registration error:', error);
      }
    });
  }
}
