import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { OrderListItem, OrderSearchRequest, PagedResult } from "../models/models";
import { Observable } from "rxjs/internal/Observable";

@Injectable({providedIn: 'root'})
export class OrderService {
    private readonly baseUrl = 'http://localhost:5002/api/orders';
  constructor(private http: HttpClient ) {

  }  

  search(request: OrderSearchRequest) : Observable<PagedResult<OrderListItem>> {
    let params = new HttpParams().set('page', request.page.toString())
                                 .set('pageSize', request.pageSize.toString())
                                 .set('sortDesc', request.sortDesc ?? true);
     if (request.searchTerm) {
       params = params.set('searchTerm', request.searchTerm);
     }
     if (request.sortBy) {
       params = params.set('sortBy', request.sortBy);
     }    

    return this.http.get<PagedResult<OrderListItem>>(`${this.baseUrl}`, { params });
  }
}