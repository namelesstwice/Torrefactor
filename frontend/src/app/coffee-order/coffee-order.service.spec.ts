import { TestBed } from '@angular/core/testing';

import { CoffeeOrderService } from './coffee-order.service';

describe('CoffeeOrderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CoffeeOrderService = TestBed.get(CoffeeOrderService);
    expect(service).toBeTruthy();
  });
});
