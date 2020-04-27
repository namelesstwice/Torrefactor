import { CoffeeKind } from './coffee-kind';

enum PackState
{
  Available,
  PriceChanged,
  Unavailable
}

export class CoffeeOrder {
  name: string;
  orders: CoffeeKind[];
  price: number;
  overallState: PackState;
}
