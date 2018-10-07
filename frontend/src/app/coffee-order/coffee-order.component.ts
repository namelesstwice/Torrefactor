import { Component, OnInit } from '@angular/core';
import { CoffeeOrderService } from './coffee-order.service';
import { CoffeeKind } from './coffee-kind';
import { CoffeePack } from './coffee-pack';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-coffee-order',
  templateUrl: './coffee-order.component.html',
  styleUrls: ['./coffee-order.component.css']
})
export class CoffeeOrderComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private coffeeOrderService: CoffeeOrderService
  ) { }

  public availableCoffeeKinds: CoffeeKind[];
  public unavailableCoffeeKinds: CoffeeKind[];
  public availablePacks: CoffeePack[];

  ngOnInit() {
    this.route.data
      .subscribe((data: { coffeeKinds: CoffeeKind[] }) => {
        this.availableCoffeeKinds = data.coffeeKinds.filter(k => k.isAvailable);
        this.unavailableCoffeeKinds = data.coffeeKinds.filter(k => !k.isAvailable);
        this.availablePacks = data.coffeeKinds[0].packs;
      });
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
