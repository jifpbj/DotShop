// ============================================================
// auth.service.ts — Login, Register, and Session Management
// ============================================================
// Handles all communication with the /api/auth endpoints and
// manages the JWT token stored in the browser's localStorage.
//
// localStorage persists across page refreshes and browser tabs.
// When the user closes and reopens the browser, they stay logged in
// until the token expires (60 minutes, set in appsettings.json).
// ============================================================

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models';

// @Injectable({ providedIn: 'root' }) makes this service a singleton
// available everywhere in the app without needing to add it to any module.
// "providedIn: 'root'" is the Angular 17 way to register application-wide services.
@Injectable({ providedIn: 'root' })
export class AuthService {
  // The base URL of our ASP.NET Core API (matching launchSettings.json).
  private apiUrl = 'http://localhost:5047/api/auth';

  // Angular injects HttpClient (for HTTP calls) and Router (for navigation).
  constructor(private http: HttpClient, private router: Router) {}

  // -------------------------------------------------------
  // login — POST /api/auth/login
  // -------------------------------------------------------
  // Returns an Observable<AuthResponse>. The component subscribes to it.
  // tap() is a "side-effect" operator — it runs code on the emitted value
  // without transforming it (the response still flows through unchanged).
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => this.storeSession(response))
    );
  }

  // -------------------------------------------------------
  // register — POST /api/auth/register
  // -------------------------------------------------------
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request).pipe(
      tap(response => this.storeSession(response))
    );
  }

  // -------------------------------------------------------
  // logout — clear session and redirect
  // -------------------------------------------------------
  logout(): void {
    // Remove all session data from localStorage.
    localStorage.removeItem('token');
    localStorage.removeItem('customerId');
    localStorage.removeItem('role');
    // Send the user back to the login page.
    this.router.navigate(['/login']);
  }

  // -------------------------------------------------------
  // isLoggedIn — quick check used by the NavBar
  // -------------------------------------------------------
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token'); // !! converts string | null → boolean
  }

  // -------------------------------------------------------
  // getCustomerId — used by OrdersService to scope requests
  // -------------------------------------------------------
  getCustomerId(): string | null {
    return localStorage.getItem('customerId');
  }

  // -------------------------------------------------------
  // storeSession (private) — persist JWT and user info
  // -------------------------------------------------------
  // Called after both login and register succeed.
  private storeSession(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('customerId', response.customerId);
    localStorage.setItem('role', response.role);
  }
}
