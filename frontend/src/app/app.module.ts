import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';

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
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
