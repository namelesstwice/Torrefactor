import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import { CoffeeKindsComponent } from './coffee-kinds/coffee-kinds.component';
import { AppRoutingModule } from './/app-routing.module';


@NgModule({
  declarations: [
    AppComponent,
    CoffeeKindsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
