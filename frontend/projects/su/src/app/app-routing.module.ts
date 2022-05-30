import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EntryPageComponent } from './pages/entry-page/entry-page.component';

const routes: Routes = [
  {
    path: 'entry',
    component: EntryPageComponent,
  },
  {
    path: '**',
    redirectTo: '/entry',
  },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ],
})
export class AppRoutingModule {
}
