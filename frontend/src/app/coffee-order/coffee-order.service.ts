import { Injectable } from '@angular/core';
import { CoffeeKind } from './coffee-kind';

@Injectable({
  providedIn: 'root'
})
export class CoffeeOrderService {

  constructor() { }

  public getCoffeeKinds(): CoffeeKind[] {
    return [
      {
        name: 'asd',
        isAvailable: true,
        packs: [
          {
            weight: 450,
            count: 0,
            price: 100
          }, {
            weight: 150,
            count: 1,
            price: 100
          }
        ]
      }, {
        name: 'assdfsdfsdfd',
        isAvailable: true,
        packs: [
          {
            weight: 450,
            count: 0,
            price: 100
          }, {
            weight: 150,
            count: 0,
            price: 100
          }
        ]
      }, {
        name: 'asdfsd',
        isAvailable: false,
        packs: [
          {
            weight: 450,
            count: 0,
            price: 100
          }, {
            weight: 150,
            count: 1,
            price: 100
          }
        ]
      }
    ];
  }
}
