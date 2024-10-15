import { ReplaySubject, map } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { IUser } from '../model/user.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'Account/login', model).pipe(
      map((res: IUser) => {
        const user = res;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: IUser) {
    if (user) {
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role; //copy token to jwt.io see .role
      Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);
      localStorage.setItem('user', JSON.stringify(user));
      this.currentUserSource.next(user);
    }
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'Account/register', model).pipe(
      map((res: IUser) => {
        if (res) {
          this.setCurrentUser(res);
        }
        return res;
      })
    );
  }

  private getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }

  checkUsernameExists(username: string) {
    return this.http.get(
      this.baseUrl + 'account/username_exists?username=' + username
    );
  }

  getUsers(search?: string) {
    return this.http.get<IUser[]>(
      this.baseUrl + 'Account/get-users?username=' + search
    );
  }
}
