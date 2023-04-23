import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {AuthenticationService} from '../services/authentication.service';
import {StorageService} from '../services/storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private storage: StorageService, private router: Router, private auth: AuthenticationService) {
  }

  private readonly redirect: UrlTree = this.router.parseUrl('/conversations');

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    if (this.storage.loggedIn()) {
      return this.redirect;
    } else {
      return new Promise(
        (resolve, reject) =>
          this.auth.tryAutoLogin()
            .then(
              (value: boolean) => value ? resolve(this.redirect) : resolve(true)))
    }

  }
}
