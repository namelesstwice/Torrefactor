import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CoffeeRoasterSelectDialogComponent } from './coffee-roaster-select-dialog.component';

describe('CoffeeRoasterSelectDialogComponent', () => {
  let component: CoffeeRoasterSelectDialogComponent;
  let fixture: ComponentFixture<CoffeeRoasterSelectDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CoffeeRoasterSelectDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoffeeRoasterSelectDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
