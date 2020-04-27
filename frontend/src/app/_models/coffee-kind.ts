import { CoffeePack } from './coffee-pack';

export class CoffeeKind {
  public name: string;
  public packs: CoffeePack[];
  public isAvailable: boolean;
  public smallPack: CoffeePack;
  public bigPack: CoffeePack;
}
