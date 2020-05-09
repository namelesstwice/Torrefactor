import { CoffeePack } from './coffee-pack';

export interface CoffeeKind {
  name: string;
  packs: CoffeePack[];
  isAvailable: boolean;
  smallPack: CoffeePack;
  bigPack: CoffeePack;
}
