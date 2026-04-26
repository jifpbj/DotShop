// ============================================================
// cart.component.ts — Shopping Cart and Checkout
// ============================================================
// Reads cart state from CartService and sends it to the API
// as a POST /api/orders request when the user checks out.
// ============================================================

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { CartItem } from '../../models';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.component.html',
  styles: [`
    table { width: 100%; border-collapse: collapse; }
    th, td { padding: 12px 16px; text-align: left; border-bottom: 1px solid #eee; }
    th { background: #f5f5f5; font-weight: 600; }
    .remove-btn {
      background: none; border: none; color: #e94560;
      cursor: pointer; font-size: 1.1rem;
    }
    .grade-A { color: #2e7d32; font-weight: 700; }
    .grade-B { color: #e65100; font-weight: 700; }
    .grade-C { color: #c62828; font-weight: 700; }
    .summary { text-align: right; margin-top: 1.5rem; }
    .total { font-size: 1.5rem; font-weight: 700; margin-bottom: 1rem; }
    .btn-checkout {
      padding: 12px 32px; background: #1a1a2e;
      color: white; border: none; border-radius: 4px;
      font-size: 1rem; cursor: pointer;
    }
    .btn-checkout:hover { background: #e94560; }
    .btn-checkout:disabled { background: #aaa; cursor: not-allowed; }
    .empty-cart { color: #666; text-align: center; padding: 3rem 0; }
    .success { color: green; font-weight: 500; margin-top: 1rem; text-align: right; }
    .error   { color: #e94560; margin-top: 1rem; text-align: right; }
  `]
})
export class CartComponent {
  // Subscribe to cart items from CartService via | async in the template.
  items$ = this.cartService.items$;

  // Tracks the checkout state to prevent double-submission.
  isPlacingOrder = false;
  successMessage = '';
  errorMessage   = '';

  constructor(
    public  cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  // -------------------------------------------------------
  // removeItem — remove one line from the cart
  // -------------------------------------------------------
  removeItem(item: CartItem): void {
    this.cartService.removeItem(item.productId);
  }

  // -------------------------------------------------------
  // placeOrder — POST cart contents to /api/orders
  // -------------------------------------------------------
  placeOrder(items: CartItem[]): void {
    if (items.length === 0 || this.isPlacingOrder) return;

    this.isPlacingOrder = true;
    this.errorMessage   = '';

    // Map CartItem[] → { productId, quantity }[] as required by CreateOrderRequest
    const request = {
      items: items.map(i => ({ productId: i.productId, quantity: i.quantity }))
    };

    this.orderService.placeOrder(request).subscribe({
      next: order => {
        // Order created — clear the cart and confirm to the user.
        this.cartService.clearCart();
        this.successMessage = `Order #${order.id.slice(-6).toUpperCase()} placed! Redirecting…`;
        // Navigate to order history after 2 seconds so the user sees the confirmation.
        setTimeout(() => this.router.navigate(['/orders']), 2000);
      },
      error: err => {
        this.isPlacingOrder = false;
        this.errorMessage = err.error?.message ?? 'Failed to place order. Please try again.';
      }
    });
  }
}
