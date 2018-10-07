import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {Invite} from '../auth/invite';
import {AuthService} from '../auth/auth.service';

@Component({
  selector: 'app-invite-approval',
  templateUrl: './invite-approval.component.html',
  styleUrls: ['./invite-approval.component.css']
})
export class InviteApprovalComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) { }

  public invites: Invite[];

  ngOnInit() {
    this.route.data.subscribe((data: { invites: Invite[] }) => {
      this.invites = data.invites;
    });
  }

  async accept(invite: Invite) {
    await this.authService.acceptOrDecline(invite, true);
    const inviteIx = this.invites.indexOf(invite);
    this.invites.splice(inviteIx, 1);
  }

  // noinspection TsLint
  async decline(invite: Invite) {
    await this.authService.acceptOrDecline(invite, false);
    const inviteIx = this.invites.indexOf(invite);
    this.invites.splice(inviteIx, 1);
  }
}
