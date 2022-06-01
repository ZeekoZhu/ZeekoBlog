import { Component, OnInit } from '@angular/core';
import { ApiService, LoginModel } from '../../api/api.service';
import { createStore } from '@ngneat/elf';
import {
  createRequestsStatusOperator,
  selectIsRequestPending, selectRequestStatus,
  updateRequestsStatus,
  withRequestsStatus,
} from '@ngneat/elf-requests';
import { catchError, debounceTime, EMPTY, map, tap } from 'rxjs';
import { get } from 'lodash-es';
import {Router} from "@angular/router";

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: [ './login-page.component.css' ],
})
export class LoginPageComponent implements OnInit {
  model: LoginModel = {
    userName: '',
    password: '',
  };

  store = createStore({ name: 'login' }, withRequestsStatus<'login'>());
  private trackLoginApiStatus = createRequestsStatusOperator(this.store);
  pending$ = this.store.pipe(selectIsRequestPending('login'), debounceTime(500));
  error$ = this.store.pipe(
    selectRequestStatus('login'),
    map(status => get(status, 'error') as string | undefined),
  );

  login() {
    this.api.login(this.model).pipe(
      this.trackLoginApiStatus('login'),
      tap(() => {
        this.store.update(updateRequestsStatus(['login'], 'success'));
        void this.router.navigate(['/']);
      }),
      catchError((e) => {
        console.log(e);
        this.store.update(updateRequestsStatus([ 'login' ], 'error', get(e, [ 'error', 'UserName' ], 'Login failed')));
        return EMPTY;
      }),
    ).subscribe();
  }

  constructor(private api: ApiService, private router: Router) {
  }

  ngOnInit(): void {
  }

  handleLogin() {
    this.login();
  }
}
