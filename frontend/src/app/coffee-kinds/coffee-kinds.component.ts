import { Component, OnInit } from '@angular/core';
import { CoffeeKindService } from '../coffee-kind.service';
import { map } from 'rxjs/operators';
import { CoffeeKind } from '../_models/coffee-kind';
import { CoffeePack } from '../_models/coffee-pack';

@Component({
  selector: 'app-coffee-kinds',
  templateUrl: './coffee-kinds.component.html',
  styleUrls: ['./coffee-kinds.component.less']
})
export class CoffeeKindsComponent implements OnInit {
  coffeeKinds: CoffeeKind[] = [];

  constructor(private coffeeKindSvc: CoffeeKindService) { }

  ngOnInit(): void {
    this.coffeeKindSvc.getCoffeeKinds().subscribe(coffee => {
      this.coffeeKinds = coffee.filter(_ => _.isAvailable);
    });
  }

  async addPack(kind: CoffeeKind, pack: CoffeePack) {
    await this.coffeeKindSvc.add(kind.name, pack.weight).toPromise();
    ++pack.count;
  }

  async removePack(kind: CoffeeKind, pack: CoffeePack) {
    await this.coffeeKindSvc.remove(kind.name, pack.weight).toPromise();
    if (pack.count > 0) {
      --pack.count;
    }
  }
}
