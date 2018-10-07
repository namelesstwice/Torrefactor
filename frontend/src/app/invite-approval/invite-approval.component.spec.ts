import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteApprovalComponent } from './invite-approval.component';

describe('InviteApprovalComponent', () => {
  let component: InviteApprovalComponent;
  let fixture: ComponentFixture<InviteApprovalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InviteApprovalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InviteApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
