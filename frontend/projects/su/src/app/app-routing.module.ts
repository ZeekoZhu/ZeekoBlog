import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EntryPageComponent } from './pages/entry-page/entry-page.component';
import { EditPageComponent } from './pages/edit-page/edit-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';

const routes: Routes = [
  {
    path: 'entry',
    component: EntryPageComponent,
  },
  {
    path: 'edit',
    component: EditPageComponent,
  },
  {
    path: 'login',
    component: LoginPageComponent,
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
