// ============================================================
// order-history.component.ts — My Orders Page
// ============================================================
// Fetches the logged-in customer's past orders from GET /api/orders.
// The JWT in the Authorization header tells the API whose orders to return.
// ============================================================

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-history.component.html',
  styles: [`
    .order-card {
      border: 1px solid #e0e0e0; border-radius: 8px;
      padding: 1.25rem; margin-bottom: 1.5rem;
    }
    .order-header {
      display: flex; justify-content: space-between;
      align-items: center; margin-bottom: 1rem;
    }
    .order-id { font-weight: 700; font-size: 1rem; }
    .order-date { color: #666; font-size: 0.875rem; }
    /* Status badge colours */
    .status-badge {
      padding: 3px 10px; border-radius: 999px;
      font-size: 0.8rem; font-weight: 600;
    }
    .status-Pending   { background: #fff3e0; color: #e65100; }
    .status-Confirmed { background: #e3f2fd; color: #1565c0; }
    .status-Shipped   { background: #ede7f6; color: #4527a0; }
    .status-Delivered { background: #e8f5e9; color: #2e7d32; }
    .status-Cancelled { background: #fce4ec; color: #b71c1c; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 0.75rem; }
    th, td { padding: 8px 12px; text-align: left; border-bottom: 1px solid #f0f0f0; }
    th { font-size: 0.8rem; text-transform: uppercase; color: #999; }
    .order-total { text-align: right; font-weight: 700; font-size: 1.1rem; }
    .empty { color: #666; text-align: center; padding: 3rem 0; }
  `]
})
export class OrderHistoryComponent implements OnInit {
  orders$!: Observable<Order[]>;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    // Trigger the GET /api/orders request and assign the Observable.
    // The template's | async pipe subscribes and renders when data arrives.
    this.orders$ = this.orderService.getMyOrders();
  }

  // -------------------------------------------------------
  // formatDate — convert ISO string to a readable date
  // -------------------------------------------------------
  // The API returns dates as ISO 8601 strings: "2025-04-24T14:30:00Z"
  // We format them as "Apr 24, 2025" for display.
  formatDate(isoString: string): string {
    return new Date(isoString).toLocaleDateString('en-CA', {
      year: 'numeric', month: 'short', day: 'numeric'
    });
  }
}
