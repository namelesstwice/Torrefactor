import { CoffeeOrder } from './coffee-order';

export interface GroupCoffeeOrder {
  hasActiveOrder: boolean;
  personalOrders: CoffeeOrder[];
}
