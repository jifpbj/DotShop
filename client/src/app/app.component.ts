// ============================================================
// app.component.ts — Root Component (Shell)
// ============================================================
// AppComponent is the outermost component — it wraps the entire app.
// Its template contains the NavBar and a <router-outlet> placeholder.
//
// <router-outlet> is where Angular swaps in the active page component
// based on the current URL. Think of it as a content slot:
//   /products → renders ProductListComponent here
//   /cart     → renders CartComponent here
//   /login    → renders LoginComponent here
// ============================================================

import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';

@Component({
  selector: 'app-root',  // used as <app-root> in index.html
  standalone: true,
  // Standalone components declare their own imports instead of relying on NgModule.
  // RouterOutlet  → enables <router-outlet> in the template
  // RouterLink    → enables [routerLink] navigation directives
  // CommonModule  → enables *ngIf, *ngFor, | async etc.
  imports: [RouterOutlet, RouterLink, CommonModule],
  template: `
    <!-- ── Navigation Bar ─────────────────────────────── -->
    <nav class="navbar">
      <a class="brand" [routerLink]="['/products']">
        🔌 DotShop <span class="tagline">Refurbished Electronics</span>
      </a>

      <!-- Only show nav links when the user is logged in -->
      <div class="nav-links" *ngIf="authService.isLoggedIn()">
        <a [routerLink]="['/products']">Browse</a>
        <a [routerLink]="['/cart']">
          Cart
          <!-- Show item count badge if the cart has items -->
          <span class="badge" *ngIf="cartService.itemCount > 0">
            {{ cartService.itemCount }}
          </span>
        </a>
        <a [routerLink]="['/orders']">My Orders</a>
        <!-- logout() clears localStorage and redirects to /login -->
        <button class="logout-btn" (click)="authService.logout()">Log out</button>
      </div>
    </nav>

    <!-- ── Page Content ──────────────────────────────── -->
    <!-- The active route's component renders here -->
    <main class="main-content">
      <router-outlet />
    </main>
  `,
  styles: [`
    /* Sticky top navigation bar */
    .navbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0.75rem 2rem;
      background: #1a1a2e;
      color: white;
      position: sticky;
      top: 0;
      z-index: 100;
    }
    .brand { color: white; text-decoration: none; font-size: 1.2rem; font-weight: 700; }
    .tagline { font-size: 0.75rem; color: #aaa; margin-left: 0.5rem; }
    .nav-links { display: flex; align-items: center; gap: 1.5rem; }
    .nav-links a { color: #ccc; text-decoration: none; }
    .nav-links a:hover { color: white; }
    /* Red badge showing number of cart items */
    .badge {
      background: #e94560; color: white;
      border-radius: 999px; padding: 1px 7px;
      font-size: 0.75rem; margin-left: 4px;
    }
    .logout-btn {
      background: transparent; border: 1px solid #ccc;
      color: #ccc; cursor: pointer; padding: 4px 12px;
      border-radius: 4px;
    }
    .logout-btn:hover { background: #e94560; border-color: #e94560; color: white; }
    .main-content { padding: 2rem; max-width: 1200px; margin: 0 auto; }
  `]
})
export class AppComponent {
  // Services are injected via the constructor — Angular's DI provides them.
  constructor(
    public authService: AuthService,  // public so the template can access it
    public cartService: CartService
  ) {}
}
