import { Component, OnInit } from '@angular/core';
import { CoffeeOrderService } from './coffee-order.service';
import { CoffeeKind } from './coffee-kind';
import { CoffeePack } from './coffee-pack';

@Component({
  selector: 'app-coffee-order',
  templateUrl: './coffee-order.component.html',
  styleUrls: ['./coffee-order.component.css']
})
export class CoffeeOrderComponent implements OnInit {

  constructor(
    private coffeeOrderService: CoffeeOrderService
  ) { }

  public availableCoffeeKinds: CoffeeKind[];
  public unavailableCoffeeKinds: CoffeeKind[];
  public availablePacks: CoffeePack[];

  ngOnInit() {
    const coffeeKinds = this.coffeeOrderService.getCoffeeKinds();
    this.availableCoffeeKinds = coffeeKinds.filter(k => k.isAvailable);
    this.unavailableCoffeeKinds = coffeeKinds.filter(k => !k.isAvailable);
    this.availablePacks = coffeeKinds[0].packs;
  }

  addPack(coffeePack: CoffeePack) {
    ++coffeePack.count;
  }

  removePack(coffeePack: CoffeePack) {
    if (coffeePack.count === 0) {
      return;
    }

    --coffeePack.count;
  }

  isOrdered(coffee: CoffeeKind) {
    return coffee.packs.some(p => p.count > 0);
  }
}
