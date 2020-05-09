import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FillRoasterKeyDialogComponent } from './fill-roaster-key-dialog.component';

describe('FillRoasterKeyDialogComponent', () => {
  let component: FillRoasterKeyDialogComponent;
  let fixture: ComponentFixture<FillRoasterKeyDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FillRoasterKeyDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FillRoasterKeyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
