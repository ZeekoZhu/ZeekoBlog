import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GreetingComponent } from './components/greeting/greeting.component';
import { EntryPageComponent } from './pages/entry-page/entry-page.component';
import { ButtonComponent } from './components/button/button.component';
import { EditPageComponent } from './pages/edit-page/edit-page.component';

@NgModule({
  declarations: [
    AppComponent,
    GreetingComponent,
    EntryPageComponent,
    ButtonComponent,
    EditPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
