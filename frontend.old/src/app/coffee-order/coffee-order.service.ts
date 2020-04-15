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

  public async syncCoffeeKinds() {
    return await this.http.post('/api/coffee/reload', {}).toPromise();
  }

  public async sendToTorrefacto() {
    return await this.http.post('/api/coffee/send', {}).toPromise();
  }

  public async deleteAllOrders() {
    return await this.http.post('/api/coffee/clear', {}).toPromise();
  }

  public async addPack(coffee: string, weight: number) {
    return await this.http.post(`/api/coffee/add?coffeeName=${coffee}&weight=${weight}`, {}).toPromise();
  }

  public async removePack(coffee: string, weight: number) {
    return await this.http.post(`/api/coffee/remove?coffeeName=${coffee}&weight=${weight}`, {}).toPromise();
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.getCoffeeKinds();
  }
}
