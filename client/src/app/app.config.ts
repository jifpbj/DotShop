// ============================================================
// app.config.ts — Application-Level Providers
// ============================================================
// In Angular 17 standalone mode there is no AppModule.
// This file replaces it — it declares the global providers
// (router, HTTP client, interceptors) that the whole app shares.
//
// Angular equivalent of Program.cs in .NET: the wiring closet.
// ============================================================

import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    // Register the router with our route definitions from app.routes.ts.
    // provideRouter() replaces the old RouterModule.forRoot() approach.
    provideRouter(routes),

    // Register Angular's HttpClient so any service can inject it.
    // withInterceptors() wires our JWT interceptor in — it runs on
    // every outgoing HTTP request automatically.
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
