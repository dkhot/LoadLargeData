// components/order-list/order-list.component.ts
import { Component, DestroyRef, inject, signal, resource } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { firstValueFrom, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { OrderService } from './service/orderService';

const SEARCH_DEBOUNCE_MS = 400;

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: 'orderlist.html'
})
export class OrderListComponent {
  private orderService = inject(OrderService);
  private destroyRef = inject(DestroyRef);

  // --- State as signals ---
  searchTerm = signal('');
  page = signal(1);
  pageSize = signal(25);

  // Debounced search term, driven by an RxJS pipeline (not the raw searchTerm)
  private debouncedTerm = signal('');

  // Raw keystrokes flow through here; debounceTime + distinctUntilChanged keep
  // large-dataset searches from firing a request per keystroke or on no-op edits
  // (e.g. typing then deleting back to the same value).
  private searchInput$ = new Subject<string>();

  constructor() {
    this.searchInput$
      .pipe(
        debounceTime(SEARCH_DEBOUNCE_MS),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((term) => {
        this.debouncedTerm.set(term);
        this.page.set(1); // reset to page 1 only once the search actually changes
      });
  }

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.searchInput$.next(value);
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
  totalPages = () => this.ordersResource.value()?.totalPages ?? 1;
  isLoading = () => this.ordersResource.isLoading();
  hasError = () => this.ordersResource.error() !== undefined;

  onPageChange(newPage: number): void {
    this.page.set(newPage);
  }

  onFirstPage(): void {
    this.page.set(1);
  }

  onLastPage(): void {
    this.page.set(this.totalPages());
  }
}