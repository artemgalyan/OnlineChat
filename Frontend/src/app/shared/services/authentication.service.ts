import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Constants} from "../../constants";
import {Observable} from "rxjs";
import {StorageService} from './storage.service';
import {Router} from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private readonly _api = Constants.ApiUrl + '/authentication';

  constructor(private httpClient: HttpClient, private storage: StorageService, private router: Router) {
    this.tryAutoLogin();
  }

  public login(username: string, password: string): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(this._api + '/login', {
        'username': username,
        'password': password
      },
      {
        withCredentials: true
      });
  }

  public logout() {
    this.httpClient.post(Constants.ServerUrl + '/api/authentication/logout', {}, {
      withCredentials: true
    }).subscribe({
      error: () => {
        this.storage.setLoggedIn(false);
        this.router.navigate(['/auth/login']);
      },
      complete: () => {
        this.storage.setLoggedIn(false);
        this.router.navigate(['/auth/login']);
      }
    });
  }

  public sendAutoLogin(): Observable<TokenLoginResponse> {
    return this.httpClient.get<TokenLoginResponse>(this._api + '/auto-login', {
      withCredentials: true
    });
  }

  public tryAutoLogin(): Promise<boolean> {
    return new Promise((resolve, reject) => {
      this.sendAutoLogin()
        .subscribe(result => {
          if (result.responseCode == TokenLoginResponseCode.Success) {
            this.storage.setLoggedIn(true);
            this.storage.set(Constants.NicknameStorageField, result.username);
            resolve(true)
          } else {
            resolve(false);
          }
        })
    })
  }
}

export enum LoginResponse {
  Success = 0,
  WrongPassword,
  WrongUsername
}

export interface TokenLoginResponse {
  username: string;
  responseCode: TokenLoginResponseCode;
}

export enum TokenLoginResponseCode {
  Success = 0,
  TokenExpired,
  NotLoggedIn,
  BadRequest
}
