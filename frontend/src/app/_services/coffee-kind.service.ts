import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CoffeeKind } from '../_models/coffee-kind';
import { CoffeeOrder } from '../_models/coffee-order';
import { GroupCoffeeOrder } from "../_models/group-coffee-order";
import { CoffeeKindPage } from '../_models/coffee-kind-page';
import { CoffeeRoaster } from '../_models/coffee-roaster';

@Injectable({
  providedIn: 'root'
})
export class CoffeeKindService {
  constructor(private http: HttpClient) { }

  public getCoffeeKinds() {
    return this.http.get<CoffeeKindPage>('/api/coffee-kinds');
  }

  public getAllOrders() {
    return this.http.get<GroupCoffeeOrder>('/api/coffee-orders')
  }

  public getRoasters() {
    return this.http.get<CoffeeRoaster[]>('/api/coffee-kinds/roasters');
  }

  public createNewGroupOrder(providerId: string) {
    return this.http.post<any>(`/api/coffee-orders?providerId=${providerId}`, {});
  }

  public add(coffeeName: string, weight: number) {
    return this.http.post(`/api/coffee-orders/current-user/${coffeeName}/${weight}`, {});
  }

  public remove(coffeeName: string, weight: number) {
    return this.http.post(`/api/coffee-orders/current-user/${coffeeName}/${weight}`, {});
  }

  public reloadKinds() {
    return this.http.post('/api/coffee-kinds/reload', {});
  }

  public sendOrder() {
    return this.http.post('/api/coffee-orders/send', {});
  }

  public cancelGroupOrder() {
    return this.http.delete('/api/coffee-orders');
  }
}
