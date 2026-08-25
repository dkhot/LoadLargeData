import { Component, signal } from '@angular/core';
import { OrderListComponent } from "../order/OrderListComponent";

@Component({
  selector: 'app-root',
  imports: [OrderListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('reportui');
}
