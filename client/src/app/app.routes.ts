// ============================================================
// app.routes.ts — Client-Side Route Definitions
// ============================================================
// Each entry maps a URL path to a component.
// Angular reads this table when the URL changes and renders
// the matching component inside <router-outlet> in app.component.html.
//
// canActivate: [authGuard] means the guard runs BEFORE the component
// is rendered. If the user has no JWT, they're redirected to /login.
// Public routes (login) deliberately have no guard.
// ============================================================

import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

// Components are lazy-imported with () => import(...)
// This is "lazy loading" — the browser only downloads a component's
// code when the user actually navigates to that route.
// Reduces initial page load time significantly.
export const routes: Routes = [
  {
    path: 'login',
    // Loaded immediately on app start — users need this without any bundle delay.
    loadComponent: () =>
      import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'products',
    canActivate: [authGuard], // redirect to /login if no JWT
    loadComponent: () =>
      import('./pages/product-list/product-list.component').then(m => m.ProductListComponent)
  },
  {
    path: 'cart',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/cart/cart.component').then(m => m.CartComponent)
  },
  {
    path: 'orders',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/order-history/order-history.component').then(m => m.OrderHistoryComponent)
  },
  {
    // Default route — redirect the bare "/" URL to /products
    path: '',
    redirectTo: 'products',
    pathMatch: 'full' // only match if the ENTIRE path is empty
  }
];
