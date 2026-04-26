// ============================================================
// auth.guard.ts — Route Protection
// ============================================================
// A route guard is a function Angular calls BEFORE it renders a
// component. If the guard returns false (or a redirect URL),
// Angular stops navigation and sends the user somewhere else.
//
// Our guard checks: "Does the user have a JWT in localStorage?"
//   Yes → allow navigation (return true)
//   No  → redirect to /login
//
// It is attached to protected routes in app.routes.ts like this:
//   { path: 'products', component: ProductListComponent, canActivate: [authGuard] }
//
// This prevents a logged-out user from visiting /products or /cart
// by typing the URL directly in the address bar.
// ============================================================

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

// CanActivateFn is Angular 17's functional guard signature.
export const authGuard: CanActivateFn = () => {
  // inject() retrieves a service from the DI container inside a functional guard.
  // (Class-based guards would use constructor injection instead.)
  const router = inject(Router);

  const token = localStorage.getItem('token');

  if (token) {
    // Token exists — let the navigation proceed.
    return true;
  }

  // No token — redirect to the login page.
  // router.parseUrl('/login') creates a UrlTree, which Angular treats as
  // a redirect instruction rather than a plain boolean false.
  return router.parseUrl('/login');
};
