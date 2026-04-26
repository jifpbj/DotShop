// ============================================================
// auth.interceptor.ts — Automatic JWT Attachment
// ============================================================
// An Angular HTTP interceptor is a function that runs on EVERY
// outgoing HTTP request before it leaves the browser.
//
// Our interceptor's job: check if a JWT token is stored in
// localStorage, and if so, add it as an Authorization header.
//
// WHY DO THIS HERE instead of in each service?
//   Without an interceptor, every service method (getProducts,
//   placeOrder, getMyOrders, etc.) would need to manually fetch
//   the token and set the header — lots of repetition.
//   One interceptor handles it for all requests automatically.
//
// FLOW:
//   Angular sends request
//     → interceptor adds "Authorization: Bearer <token>"
//       → API validates the token
//         → returns the protected resource
// ============================================================

import { HttpInterceptorFn } from '@angular/common/http';

// HttpInterceptorFn is Angular 17's functional interceptor signature.
// It receives the outgoing request and a 'next' handler that forwards it.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Read the JWT that was stored in localStorage when the user logged in.
  // Returns null if the user is not logged in.
  const token = localStorage.getItem('token');

  if (token) {
    // HttpRequest objects are IMMUTABLE in Angular — you can't modify them.
    // .clone() creates a copy with your additions merged in.
    const authReq = req.clone({
      setHeaders: {
        // The "Bearer " prefix is part of the HTTP Authorization standard.
        // The API's JWT middleware expects exactly this format.
        Authorization: `Bearer ${token}`
      }
    });
    // Forward the cloned (authenticated) request down the chain.
    return next(authReq);
  }

  // No token — forward the original request unchanged.
  // Public endpoints (login, register, product listing) don't need a token.
  return next(req);
};
