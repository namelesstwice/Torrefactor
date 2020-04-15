import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestInviteComponent } from './request-invite.component';

describe('RequestInviteComponent', () => {
  let component: RequestInviteComponent;
  let fixture: ComponentFixture<RequestInviteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestInviteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestInviteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
