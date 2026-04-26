// ============================================================
// product.service.ts — Product Listing and Detail API Calls
// ============================================================
// Wraps GET /api/products and GET /api/products/{id}.
// The auth interceptor automatically attaches the JWT if present,
// but product listing is a public endpoint — no token required.
// ============================================================

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'http://localhost:5047/api/products';

  constructor(private http: HttpClient) {}

  // -------------------------------------------------------
  // getProducts — GET /api/products?category=X&conditionGrade=Y
  // -------------------------------------------------------
  // HttpParams builds the query string safely — no manual string
  // concatenation that could break with special characters.
  // Passing undefined for a param simply omits it from the URL.
  getProducts(category?: string, conditionGrade?: string): Observable<Product[]> {
    let params = new HttpParams();

    // Only append the parameter if a value was actually provided.
    if (category)       params = params.set('category', category);
    if (conditionGrade) params = params.set('conditionGrade', conditionGrade);

    // HttpClient.get<T>() sends the request and returns an Observable.
    // The component subscribes (usually via | async pipe in the template)
    // to receive the Product[] array when the response arrives.
    return this.http.get<Product[]>(this.apiUrl, { params });
  }

  // -------------------------------------------------------
  // getProductById — GET /api/products/{id}
  // -------------------------------------------------------
  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }
}
