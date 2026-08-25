// components/order-list/order-list.component.ts
import { Component, inject, signal, resource } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { OrderService } from './service/orderService';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: 'orderlist.html'
})
export class OrderListComponent {
  private orderService = inject(OrderService);

  // --- State as signals ---
  searchTerm = signal('');
  page = signal(1);
  pageSize = signal(25);

  // Debounced search term (separate signal, updated via timer below)
  private debouncedTerm = signal('');
  private debounceHandle?: ReturnType<typeof setTimeout>;

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.page.set(1); // reset to page 1 on new search

    clearTimeout(this.debounceHandle);
    this.debounceHandle = setTimeout(() => {
      this.debouncedTerm.set(value);
    }, 400);
  }

  // --- resource() re-runs automatically when any signal read inside `params` changes ---
  ordersResource = resource({
    params: () => ({
      searchTerm: this.debouncedTerm(),
      page: this.page(),
      pageSize: this.pageSize()
    }),
    loader: ({ params }) => firstValueFrom(this.orderService.search(params))
  });

  // --- Derived values ---
  orders = () => this.ordersResource.value()?.items ?? [];
  totalCount = () => this.ordersResource.value()?.totalCount ?? 0;
  isLoading = () => this.ordersResource.isLoading();
  hasError = () => this.ordersResource.error() !== undefined;

  onPageChange(newPage: number): void {
    this.page.set(newPage);
  }
}