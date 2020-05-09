import { Component, OnInit } from '@angular/core';
import { CoffeeKindService } from '../_services/coffee-kind.service';
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
  activeGroupOrderExists = false;
  isLoading = true;

  constructor(private coffeeKindSvc: CoffeeKindService) { }

  ngOnInit(): void {
    this.coffeeKindSvc.getCoffeeKinds().subscribe(p => {
      this.activeGroupOrderExists = p.activeGroupOrderExists;
      this.coffeeKinds = p.coffeeKinds.filter(_ => _.isAvailable);
      this.isLoading = false;
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
