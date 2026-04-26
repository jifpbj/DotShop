// ============================================================
// order.service.ts — Place and Retrieve Orders
// ============================================================
// Wraps POST /api/orders and GET /api/orders.
// These endpoints require authentication — the auth interceptor
// automatically adds the Bearer token to every request.
// ============================================================

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateOrderRequest, Order } from '../models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'http://localhost:5047/api/orders';

  constructor(private http: HttpClient) {}

  // -------------------------------------------------------
  // getMyOrders — GET /api/orders
  // -------------------------------------------------------
  // The API reads the customer ID from the JWT claims, so we
  // don't need to pass it in the URL — just send the token.
  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl);
  }

  // -------------------------------------------------------
  // placeOrder — POST /api/orders
  // -------------------------------------------------------
  // Sends the cart contents to the API.
  // The API validates stock, snapshots prices, decrements inventory,
  // and returns the created Order document with its generated ID.
  placeOrder(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.apiUrl, request);
  }
}
