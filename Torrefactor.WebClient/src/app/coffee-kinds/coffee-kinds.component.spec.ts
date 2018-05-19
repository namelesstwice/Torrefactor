import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CoffeeKindsComponent } from './coffee-kinds.component';

describe('CoffeeKindsComponent', () => {
  let component: CoffeeKindsComponent;
  let fixture: ComponentFixture<CoffeeKindsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CoffeeKindsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoffeeKindsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
