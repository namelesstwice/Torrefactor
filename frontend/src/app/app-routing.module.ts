import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { AuthService } from './auth/auth.service';
import { CoffeeOrderComponent } from './coffee-order/coffee-order.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { RequestInviteComponent } from './request-invite/request-invite.component';
import { InviteApprovalComponent } from './invite-approval/invite-approval.component';
import { CompleteRegistrationComponent } from './complete-registration/complete-registration.component';

const routes: Routes = [
  { path: 'coffee-order', component: CoffeeOrderComponent, canActivate: [AuthGuard] },
  { path: 'sign-in', component: SignInComponent },
  { path: 'request-invite', component: RequestInviteComponent },
  { path: 'invite-approval', component: InviteApprovalComponent, resolve: { invites: AuthService } },
  { path: 'complete-registration', component: CompleteRegistrationComponent },
  { path: '', redirectTo: '/coffee-order', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ],
})
export class AppRoutingModule { }
