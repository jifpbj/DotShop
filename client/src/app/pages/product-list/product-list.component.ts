// ============================================================
// product-list.component.ts — Browse Refurbished Electronics
// ============================================================
// Fetches products from the API and renders them as cards.
// Supports filtering by category and condition grade.
//
// The products$ Observable is passed directly to the template
// via the | async pipe, which subscribes/unsubscribes automatically
// so we never need to manage the subscription lifecycle manually.
// ============================================================

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { Product } from '../../models';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './product-list.component.html',
  styles: [`
    .filters { display: flex; gap: 1rem; margin-bottom: 1.5rem; flex-wrap: wrap; }
    select { padding: 8px 12px; border-radius: 4px; border: 1px solid #ccc; font-size: 1rem; }
    .product-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
      gap: 1.5rem;
    }
    .product-card {
      border: 1px solid #e0e0e0; border-radius: 8px;
      padding: 1rem; display: flex; flex-direction: column;
    }
    .product-card img { width: 100%; height: 180px; object-fit: cover; border-radius: 4px; }
    .product-name { font-weight: 600; margin: 0.75rem 0 0.25rem; }
    .product-meta { color: #666; font-size: 0.875rem; margin-bottom: 0.5rem; }
    .price { font-size: 1.25rem; font-weight: 700; color: #1a1a2e; margin-bottom: 0.75rem; }
    /* Condition grade badge colours: green=A, orange=B, red=C */
    .grade-A { color: #2e7d32; font-weight: 700; }
    .grade-B { color: #e65100; font-weight: 700; }
    .grade-C { color: #c62828; font-weight: 700; }
    .btn-add {
      margin-top: auto; padding: 8px; background: #1a1a2e;
      color: white; border: none; border-radius: 4px; cursor: pointer;
    }
    .btn-add:hover { background: #e94560; }
    .out-of-stock { color: #999; font-size: 0.875rem; margin-top: auto; text-align: center; }
  `]
})
export class ProductListComponent implements OnInit {
  // Observable of products — the template subscribes via | async.
  // Type assertion '!' tells TypeScript it will be assigned before use.
  products$!: Observable<Product[]>;

  // Filter values bound to <select> dropdowns in the template via [(ngModel)].
  selectedCategory    = '';
  selectedGrade       = '';

  // Fixed lists for the filter dropdowns — matches the seed data categories and grades.
  categories   = ['', 'Laptops', 'Phones', 'Tablets', 'Accessories'];
  grades       = ['', 'A', 'B', 'C'];

  // Toast notification shown briefly after adding an item to cart.
  addedToCartMessage = '';

  constructor(
    private productService: ProductService,
    public  cartService: CartService   // public so template can read itemCount
  ) {}

  // ngOnInit runs once after Angular has initialised the component.
  // Good place for initial data fetching.
  ngOnInit(): void {
    this.loadProducts();
  }

  // -------------------------------------------------------
  // loadProducts — re-fetch from API with current filters
  // -------------------------------------------------------
  loadProducts(): void {
    // Pass filter values — ProductService converts them to query params.
    // Empty string is treated as "no filter" by the service.
    this.products$ = this.productService.getProducts(
      this.selectedCategory || undefined,
      this.selectedGrade    || undefined
    );
  }

  // -------------------------------------------------------
  // addToCart — add a product and show a brief confirmation
  // -------------------------------------------------------
  addToCart(product: Product): void {
    this.cartService.addItem({
      productId:      product.id,
      productName:    product.name,
      conditionGrade: product.conditionGrade,
      price:          product.price,
      quantity:       1
    });

    // Show a brief "Added!" message, then clear it after 1.5 seconds.
    this.addedToCartMessage = `"${product.name}" added to cart!`;
    setTimeout(() => (this.addedToCartMessage = ''), 1500);
  }
}
