import { Component, OnInit } from '@angular/core';
import { CoffeeKind } from "./coffee-kind";

@Component({
  selector: 'app-coffee-kinds',
  templateUrl: './coffee-kinds.component.html',
  styleUrls: ['./coffee-kinds.component.css']
})
export class CoffeeKindsComponent implements OnInit {

  constructor() { }

  coffeeKinds: CoffeeKind[] = [
    {
      id: "1",
      price: 1,
      name: "Indonesia"
    },
    {
      id: "2",
      price: 2,
      name: "Sumathra"
    }];

  ngOnInit() {
  }

}
