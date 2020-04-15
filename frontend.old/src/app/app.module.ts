import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { APP_INITIALIZER, NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CoffeeOrderComponent } from './coffee-order/coffee-order.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { RequestInviteComponent } from './request-invite/request-invite.component';
import { HttpClientModule } from '@angular/common/http';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { InviteApprovalComponent } from './invite-approval/invite-approval.component';
import { CompleteRegistrationComponent } from './complete-registration/complete-registration.component';
import { ManageCoffeeOrdersComponent } from './manage-coffee-orders/manage-coffee-orders.component';
import { UserService } from './auth/user.service';

function loadCurrentUser(authService: UserService) {
  return () => authService.loadCurrentUserIfNeeded();
}

@NgModule({
  declarations: [
    AppComponent,
    CoffeeOrderComponent,
    SignInComponent,
    PageNotFoundComponent,
    RequestInviteComponent,
    TopMenuComponent,
    InviteApprovalComponent,
    CompleteRegistrationComponent,
    ManageCoffeeOrdersComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    UserService,
    { provide: APP_INITIALIZER, useFactory: loadCurrentUser, deps: [UserService], multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
