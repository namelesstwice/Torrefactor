import { Component, OnInit } from '@angular/core';
import { CoffeeOrderService } from '../coffee-order/coffee-order.service';
import {CoffeeOrder} from '../coffee-order/coffee-order';

@Component({
  selector: 'app-manage-coffee-orders',
  templateUrl: './manage-coffee-orders.component.html',
  styleUrls: ['./manage-coffee-orders.component.css']
})
export class ManageCoffeeOrdersComponent implements OnInit {

  constructor(
    private coffeeOrderService: CoffeeOrderService
  ) { }

  public orders: CoffeeOrder[];
  public isLoading: boolean;

  async ngOnInit() {
    this.orders = await this.coffeeOrderService.getCoffeeOrders();
  }

  async reload() {
    try {
      this.isLoading = true;
      await this.coffeeOrderService.syncCoffeeKinds();
    } finally {
      this.isLoading = false;
    }
  }

  async upload() {
    try {
      this.isLoading = true;
      await this.coffeeOrderService.sendToTorrefacto();
    } finally {
      this.isLoading = false;
    }
  }

  async clear() {
    try {
      this.isLoading = true;
      await this.coffeeOrderService.deleteAllOrders();
    } finally {
      this.isLoading = false;
    }
  }
}
