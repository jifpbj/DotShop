// ============================================================
// login.component.ts — Login and Registration Page
// ============================================================
// A single page that toggles between login and register modes.
// On success, AuthService stores the JWT and the router
// navigates the user to /products automatically.
// ============================================================

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // enables [(ngModel)] two-way binding
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styles: [`
    .login-card {
      max-width: 420px; margin: 4rem auto; padding: 2rem;
      border: 1px solid #ddd; border-radius: 8px;
      box-shadow: 0 2px 12px rgba(0,0,0,0.08);
    }
    h2 { margin-bottom: 1.5rem; text-align: center; }
    .form-group { margin-bottom: 1rem; }
    label { display: block; margin-bottom: 4px; font-weight: 500; }
    input {
      width: 100%; padding: 8px 12px; border: 1px solid #ccc;
      border-radius: 4px; font-size: 1rem; box-sizing: border-box;
    }
    .btn-primary {
      width: 100%; padding: 10px; background: #1a1a2e;
      color: white; border: none; border-radius: 4px;
      font-size: 1rem; cursor: pointer; margin-top: 0.5rem;
    }
    .btn-primary:hover { background: #e94560; }
    .toggle-link { text-align: center; margin-top: 1rem; }
    .toggle-link a { color: #1a1a2e; cursor: pointer; text-decoration: underline; }
    .error { color: #e94560; margin-top: 0.5rem; text-align: center; }
  `]
})
export class LoginComponent {
  // Controls which form is visible.
  isRegisterMode = false;

  // Two-way bound to the form inputs via [(ngModel)].
  email    = '';
  password = '';
  firstName = '';
  lastName  = '';

  // Displayed below the form if the API returns an error.
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  // -------------------------------------------------------
  // extractError — parse the error message from any API shape
  // -------------------------------------------------------
  // The API can return two different error shapes:
  //
  //   1. Our ExceptionHandlingMiddleware:
  //        { "error": "An account with this email already exists." }
  //
  //   2. ASP.NET [ApiController] model validation (e.g. [MinLength], [Required]):
  //        { "title": "One or more validation errors occurred.",
  //          "errors": { "Password": ["minimum length is 8"], "Email": ["..."] } }
  //
  // This helper handles both so the user always sees a meaningful message.
  private extractError(err: any, fallback: string): string {
    const body = err.error;
    if (!body) return fallback;

    // Shape 1 — our middleware: { error: "..." }
    if (typeof body.error === 'string') return body.error;

    // Shape 2 — ASP.NET validation: { errors: { Field: ["msg", ...] } }
    if (body.errors) {
      const messages = Object.values(body.errors as Record<string, string[]>)
        .flat(); // flatten [["msg1"], ["msg2"]] → ["msg1", "msg2"]
      if (messages.length > 0) return messages.join(' ');
    }

    // Shape 2 title fallback
    if (typeof body.title === 'string') return body.title;

    return fallback;
  }

  // -------------------------------------------------------
  // submit — called when the form button is clicked
  // -------------------------------------------------------
  submit(): void {
    this.errorMessage = ''; // clear any previous error

    if (this.isRegisterMode) {
      // Registration flow
      this.authService.register({
        email: this.email,
        password: this.password,
        firstName: this.firstName,
        lastName: this.lastName
      }).subscribe({
        // 'next' fires when the API responds with a 2xx status.
        next: () => this.router.navigate(['/products']),
        // 'error' fires when the API responds with 4xx or 5xx.
        error: err => this.errorMessage = this.extractError(err, 'Registration failed.')
      });
    } else {
      // Login flow
      this.authService.login({ email: this.email, password: this.password })
        .subscribe({
          next: () => this.router.navigate(['/products']),
          error: err => this.errorMessage = this.extractError(err, 'Invalid email or password.')
        });
    }
  }

  // Toggle between login and register mode, clearing any stale error.
  toggleMode(): void {
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = '';
  }
}
