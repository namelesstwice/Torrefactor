import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CoffeeOrderComponent } from './coffee-order.component';

describe('CoffeeOrderComponent', () => {
  let component: CoffeeOrderComponent;
  let fixture: ComponentFixture<CoffeeOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CoffeeOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoffeeOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
