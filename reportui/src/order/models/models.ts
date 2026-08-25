// models/order.model.ts
export interface OrderListItem {
  orderId: number;
  customerName: string;
  orderDate: string;
  total: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface OrderSearchRequest {
  searchTerm?: string;
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDesc?: boolean;
}