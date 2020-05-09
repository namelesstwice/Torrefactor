import { CoffeeKind } from './coffee-kind';

export interface CoffeeKindPage {
    activeGroupOrderExists: boolean;
    coffeeKinds: CoffeeKind[];
}