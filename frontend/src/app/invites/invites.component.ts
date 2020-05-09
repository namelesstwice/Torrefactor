import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { InvitesService } from '../_services/invites.service';

@Component({
  selector: 'app-invites',
  templateUrl: './invites.component.html',
  styleUrls: ['./invites.component.less']
})
export class InvitesComponent implements OnInit {

  public invites: User[];

  constructor(private invitesSvc: InvitesService) { }

  ngOnInit(): void {
    this.invitesSvc.getInvites().subscribe(invites => {
      this.invites = invites;
    })
  }

  public async accept(user: User) {
    await this.invitesSvc.accept(user).toPromise();
    this.invites = this.invites.filter(_ => _ != user);
  }

  public async decline(user: User) {
    await this.invitesSvc.decline(user).toPromise();
    this.invites = this.invites.filter(_ => _ != user);
  }
}
