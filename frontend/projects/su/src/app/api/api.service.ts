import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface LoginModel {
  userName: string;
  password: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApiService {

  constructor(private http: HttpClient) {
  }

  login(model: LoginModel) {
    return this.http.post('api/Token', model);
  }
}
