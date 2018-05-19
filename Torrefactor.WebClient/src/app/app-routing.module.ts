import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CoffeeKindsComponent } from "./coffee-kinds/coffee-kinds.component";

const routes: Routes = [
  { path: 'coffee-kinds', component: CoffeeKindsComponent },
  { path: '', redirectTo: '/coffee-kinds', pathMatch: 'full' },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ],
  declarations: []
})
export class AppRoutingModule { }
