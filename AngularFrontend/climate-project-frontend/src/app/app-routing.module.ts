import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DataloggerListComponent } from './datalogger-list/datalogger-list.component';
import { PlantListComponent } from './plant-list/plant-list.component';

const routes: Routes = [
  {path: '',redirectTo:'/plants',pathMatch:'full'},
  {path:'plants', component:PlantListComponent},
  {path:'dataloggers', component:DataloggerListComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
