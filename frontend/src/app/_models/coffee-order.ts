import { CoffeePack } from './coffee-pack';

enum PackState
{
  Available,
  PriceChanged,
  Unavailable
}

export interface CoffeeOrder {
  name: string;
  coffeePacks: CoffeePack[];
  totalCost: number;
  overallState: PackState;
}
