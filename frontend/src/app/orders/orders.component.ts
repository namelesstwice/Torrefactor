import { Component, OnInit } from '@angular/core';
import { CoffeeKindService } from '../_services/coffee-kind.service';
import { CoffeeOrder } from '../_models/coffee-order';
import { CoffeePack } from '../_models/coffee-pack';
import { MatDialog } from '@angular/material/dialog';
import { CoffeeRoasterSelectDialogComponent } from '../coffee-roaster-select-dialog/coffee-roaster-select-dialog.component';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.less']
})
export class OrdersComponent implements OnInit {

  public isLoading = false;
  public hasActiveGroupOrder = false;
  public orders: CoffeeOrder[] = [];

  public get totalPrice() {
    return this.orders.reduce((prev, cur) => prev + cur.totalCost, 0);
  }

  constructor(
    private coffeeService: CoffeeKindService,
    private dialog: MatDialog) { }

  ngOnInit(): void {
    this.coffeeService.getAllOrders().subscribe(groupOrder => {
      this.orders = groupOrder.personalOrders;
      this.hasActiveGroupOrder = groupOrder.hasActiveOrder;
    })
  }

  public async createNewGroupOrder() {
    const roasters = await this.coffeeService.getRoasters().toPromise();

    const dialogRef = this.dialog.open(CoffeeRoasterSelectDialogComponent, {
      data: roasters
    });

    dialogRef.afterClosed().subscribe(async selectedRoasterId => {
      if (!selectedRoasterId)
        return;

      await this.coffeeService.createNewGroupOrder(selectedRoasterId).toPromise();
      this.hasActiveGroupOrder = true;
    });
  }

  public async reloadKinds() {
    await this.load(() => 
      this.coffeeService.reloadKinds().toPromise());
  }

  public async cancel() {
    await this.load(async () => {
      await this.coffeeService.cancelGroupOrder().toPromise();
      this.orders = [];
      this.hasActiveGroupOrder = false;
    });
  }

  public async push() {
    await this.load(() => 
      this.coffeeService.sendOrder().toPromise());
  }

  public format(packs: CoffeePack[]) {
    return packs
      .map(p => `${p.coffeeKindName} ${p.weight}g x ${p.count}`)
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
