import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CoffeeKind } from './_models/coffee-kind';
import { CoffeeOrder } from './_models/coffee-order';

@Injectable({
  providedIn: 'root'
})
export class CoffeeKindService {
  constructor(private http: HttpClient) { }

  public getCoffeeKinds() {
    return this.http.get<CoffeeKind[]>('/api/coffee');
  }

  public getAllOrders() {
    return this.http.get<CoffeeOrder[]>('/api/coffee/orders')
  }

  public add(coffeeName: string, weight: number) {
    return this.http.post(`/api/coffee/add?coffeeName=${coffeeName}&weight=${weight}`, {});
  }

  public remove(coffeeName: string, weight: number) {
    return this.http.post(`/api/coffee/remove?coffeeName=${coffeeName}&weight=${weight}`, {});
  }

  public reloadKinds() {
    return this.http.post('/api/coffee/reload', {});
  }

  public pushOrders() {
    return this.http.post('/api/coffee/send', {});
  }

  public removeAll() {
    return this.http.post('/api/coffee/clear', {});
  }
}
