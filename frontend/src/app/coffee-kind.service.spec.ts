import { TestBed } from '@angular/core/testing';

import { CoffeeKindService } from './coffee-kind.service';

describe('CoffeeKindService', () => {
  let service: CoffeeKindService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CoffeeKindService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
