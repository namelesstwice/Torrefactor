import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from './auth/auth.guard';
import { AuthService } from './auth/auth.service';
import { CoffeeOrderService } from './coffee-order/coffee-order.service';

import { CoffeeOrderComponent } from './coffee-order/coffee-order.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { RequestInviteComponent } from './request-invite/request-invite.component';
import { InviteApprovalComponent } from './invite-approval/invite-approval.component';
import { CompleteRegistrationComponent } from './complete-registration/complete-registration.component';
import { ManageCoffeeOrdersComponent } from './manage-coffee-orders/manage-coffee-orders.component';

const routes: Routes = [
  { path: '', children: [
    { path: 'sign-in', component: SignInComponent },
    { path: 'request-invite', component: RequestInviteComponent },
    { path: 'complete-registration', component: CompleteRegistrationComponent },
    { path: 'coffee-order', canActivate: [AuthGuard], children: [
        { path: '', component: CoffeeOrderComponent, resolve: { coffeeKinds: CoffeeOrderService } },
      ] },
    { path: 'admin', canActivate: [AuthGuard], children: [
        { path: 'manage-coffee-orders', component: ManageCoffeeOrdersComponent, canActivate: [AuthGuard]},
        { path: 'invite-approval', component: InviteApprovalComponent, canActivate: [AuthGuard], resolve: { invites: AuthService } },
      ]
    },
    { path: '', redirectTo: '/coffee-order', pathMatch: 'full' },
    { path: '**', component: PageNotFoundComponent },
  ]},
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ],
})
export class AppRoutingModule { }
