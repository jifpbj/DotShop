// ============================================================
// models/index.ts — Shared TypeScript Interfaces
// ============================================================
// These interfaces mirror the C# DTO classes returned by the API.
// TypeScript uses them to give us type-checking and autocomplete
// when working with API responses — if the API changes a field name,
// the compiler will flag every place we used the old name.
//
// These are plain interfaces (not classes) because we never need to
// call methods on them — they are just shapes for data.
// ============================================================

// Matches DTOs/Products/ProductDto.cs
export interface Product {
  id: string;
  name: string;
  description: string;
  category: string;
  conditionGrade: string; // "A" | "B" | "C"
  price: number;
  stockQuantity: number;
  imageUrl: string;
}

// Matches DTOs/Orders/OrderItemDto.cs (used when reading order history)
export interface OrderItemView {
  productId: string;
  productName: string;
  conditionGrade: string;
  unitPrice: number;
  quantity: number;
}

// Matches DTOs/Orders/OrderDto.cs
export interface Order {
  id: string;
  customerId: string;
  items: OrderItemView[];
  totalAmount: number;
  status: string; // "Pending" | "Confirmed" | "Shipped" | "Delivered"
  createdAt: string; // ISO date string from the API
}

// Matches DTOs/Auth/AuthResponse.cs
export interface AuthResponse {
  token: string;
  customerId: string;
  email: string;
  role: string;
}

// Matches DTOs/Customers/CustomerDto.cs
export interface CustomerProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

// ------------------------------------------------------------
// CartItem — Client-side only, never sent to the server as-is
// ------------------------------------------------------------
// The cart lives entirely in the browser (CartService).
// When the user checks out, we map cart items → CreateOrderRequest
// and send that to POST /api/orders.
export interface CartItem {
  productId: string;
  productName: string;
  conditionGrade: string;
  price: number;
  quantity: number;
}

// ------------------------------------------------------------
// Request shapes — what we SEND to the API
// ------------------------------------------------------------

// Body for POST /api/auth/login
export interface LoginRequest {
  email: string;
  password: string;
}

// Body for POST /api/auth/register
export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

// Body for POST /api/orders
export interface CreateOrderRequest {
  items: { productId: string; quantity: number }[];
}
