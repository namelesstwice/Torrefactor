import { Injectable } from '@angular/core';
import { CoffeeKind } from './coffee-kind';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CoffeeOrder } from './coffee-order';

@Injectable({
  providedIn: 'root'
})
export class CoffeeOrderService implements Resolve<CoffeeKind[]> {

  constructor(
    private http: HttpClient
  ) { }

  public async getCoffeeKinds() {
    return await this.http.get<CoffeeKind[]>('/api/coffee').toPromise();
  }

  public async getCoffeeOrders() {
    return await this.http.get<CoffeeOrder[]>('/api/coffee/orders').toPromise();
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.getCoffeeKinds();
  }
}
