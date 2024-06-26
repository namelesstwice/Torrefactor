import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button'
import { MatTableModule } from '@angular/material/table'
import { MatDividerModule } from '@angular/material/divider'
import { MatDialogModule } from '@angular/material/dialog'

import { LoginFormComponent } from './login-form/login-form.component';
import { RegistrationFormComponent } from './registration-form/registration-form.component';
import { RegistrationCompletedComponent } from './registration-completed/registration-completed.component';
import { CoffeeKindsComponent } from './coffee-kinds/coffee-kinds.component';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { InvitesComponent } from './invites/invites.component';
import { OrdersComponent } from './orders/orders.component';
import { CoffeeRoasterSelectDialogComponent } from './coffee-roaster-select-dialog/coffee-roaster-select-dialog.component';
import { FillRoasterKeyDialogComponent } from './fill-roaster-key-dialog/fill-roaster-key-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginFormComponent,
    RegistrationFormComponent,
    RegistrationCompletedComponent,
    CoffeeKindsComponent,
    InvitesComponent,
    OrdersComponent,
    CoffeeRoasterSelectDialogComponent,
    FillRoasterKeyDialogComponent,
    FillRoasterKeyDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatDividerModule,
    FormsModule,
    HttpClientModule,
    FlexLayoutModule,
    MatDialogModule,
    MatSelectModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

