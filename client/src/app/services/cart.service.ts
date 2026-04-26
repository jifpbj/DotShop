// ============================================================
// cart.service.ts — In-Memory Shopping Cart State
// ============================================================
// The cart is NOT stored in the database. It lives entirely in
// the browser's memory while the user shops.
//
// STATE MANAGEMENT WITH BehaviorSubject:
//   RxJS BehaviorSubject is like a variable that notifies all
//   "subscribers" whenever its value changes.
//
//   Think of it like a shared whiteboard:
//     - CartService writes to it (addItem, removeItem, clear)
//     - Components read from it (subscribe via | async pipe)
//     - When the whiteboard changes, every component auto-updates
//
//   BehaviorSubject vs plain Subject:
//     BehaviorSubject holds the LAST value and gives it immediately
//     to any new subscriber. So if you navigate to the Cart page,
//     you see the current items right away without waiting for a new event.
// ============================================================

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartItem } from '../models';

@Injectable({ providedIn: 'root' })
export class CartService {
  // Private BehaviorSubject — only this service can push new values.
  // Starts as an empty array (no items in cart).
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);

  // Public Observable — components subscribe to this to read cart state.
  // Exposed as Observable (not BehaviorSubject) to prevent external code
  // from calling .next() and mutating state outside this service.
  items$ = this.cartItemsSubject.asObservable();

  // -------------------------------------------------------
  // addItem — add a product to the cart (or increase quantity)
  // -------------------------------------------------------
  addItem(item: CartItem): void {
    const current = this.cartItemsSubject.getValue(); // read the current array
    const existing = current.find(i => i.productId === item.productId);

    if (existing) {
      // Product is already in the cart — just bump the quantity.
      // We spread [...current] to create a NEW array rather than mutating
      // the existing one. Angular's change detection works better with
      // new references (it can tell something changed).
      existing.quantity++;
      this.cartItemsSubject.next([...current]);
    } else {
      // New product — append it to the cart.
      this.cartItemsSubject.next([...current, { ...item, quantity: 1 }]);
    }
  }

  // -------------------------------------------------------
  // removeItem — remove a product line entirely from the cart
  // -------------------------------------------------------
  removeItem(productId: string): void {
    const updated = this.cartItemsSubject
      .getValue()
      .filter(i => i.productId !== productId); // keep everything EXCEPT this item
    this.cartItemsSubject.next(updated);
  }

  // -------------------------------------------------------
  // clearCart — empty the cart after a successful order
  // -------------------------------------------------------
  clearCart(): void {
    this.cartItemsSubject.next([]); // replace the array with an empty one
  }

  // -------------------------------------------------------
  // total — computed order total (not stored, always fresh)
  // -------------------------------------------------------
  // A getter (not a stored value) so it's always calculated from
  // the current items. No risk of it going stale.
  get total(): number {
    return this.cartItemsSubject
      .getValue()
      .reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  // -------------------------------------------------------
  // itemCount — number of distinct product lines in the cart
  // -------------------------------------------------------
  // Used by the NavBar to show a badge: "Cart (3)"
  get itemCount(): number {
    return this.cartItemsSubject.getValue().length;
  }
}
