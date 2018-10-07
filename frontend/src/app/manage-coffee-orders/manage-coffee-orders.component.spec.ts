import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageCoffeeOrdersComponent } from './manage-coffee-orders.component';

describe('ManageCoffeeOrdersComponent', () => {
  let component: ManageCoffeeOrdersComponent;
  let fixture: ComponentFixture<ManageCoffeeOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageCoffeeOrdersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageCoffeeOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
