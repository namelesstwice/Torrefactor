import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginFormComponent } from './login-form/login-form.component';
import { RegistrationFormComponent } from './registration-form/registration-form.component';
import { RegistrationCompletedComponent } from './registration-completed/registration-completed.component';
import { CoffeeKindsComponent } from './coffee-kinds/coffee-kinds.component';
import { AuthGuard } from './_helpers/auth.guard';
import { LoginGuard } from './_helpers/login.guard';
import { InvitesComponent } from './invites/invites.component';
import { OrdersComponent } from './orders/orders.component';

const routes: Routes = [
  { path: 'sign-in', component: LoginFormComponent, canActivate: [LoginGuard] },
  { path: 'register', component: RegistrationFormComponent, canActivate: [LoginGuard] },
  { path: 'registration-completed', component: RegistrationCompletedComponent, canActivate: [LoginGuard] },
  { path: 'coffee-kinds', component: CoffeeKindsComponent, canActivate: [AuthGuard] },
  { path: 'admin/invites', component: InvitesComponent, canActivate: [AuthGuard] },
  { path: 'admin/orders', component: OrdersComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/sign-in', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
