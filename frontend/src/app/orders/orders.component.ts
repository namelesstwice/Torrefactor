import { Component, OnInit } from '@angular/core';
import { CoffeeKindService } from '../coffee-kind.service';
import { CoffeeOrder } from '../_models/coffee-order';
import { CoffeeKind } from '../_models/coffee-kind';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.less']
})
export class OrdersComponent implements OnInit {

  public isLoading = false;
  public orders: CoffeeOrder[] = [];

  public get totalPrice() {
    return this.orders.reduce((prev, cur) => prev + cur.price, 0);
  }

  constructor(private coffeeService: CoffeeKindService) { }

  ngOnInit(): void {
    this.coffeeService.getAllOrders().subscribe(orders => {
      this.orders = orders;
    })
  }

  public async reloadKinds() {
    await this.load(() => 
      this.coffeeService.reloadKinds().toPromise());
  }

  public async removeAll() {
    await this.load(async () => {
      await this.coffeeService.removeAll().toPromise();
      this.orders = [];
    });
  }

  public async push() {
    await this.load(() => 
      this.coffeeService.pushOrders().toPromise());
  }

  public format(orders: CoffeeKind[]) {
    return orders
      .map(o => `${o.name} (${o.packs.map(p => `${p.weight}g x ${p.count}`)})`)
      .join(', ');
  }

  private async load(func: () => Promise<any>) {
    try {
      this.isLoading = true;
      await func();
    } finally {
      this.isLoading = false;
    }
  }
}
