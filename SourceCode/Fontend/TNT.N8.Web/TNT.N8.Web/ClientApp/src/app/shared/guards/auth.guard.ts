import { Pipe, Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Pipe({ name: 'AuthGuard' })
export class AuthGuard implements CanActivate {

  constructor(private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (localStorage.getItem('auth')) {
      // logged in so return true

      //Lưu lại url hiện tại vào sessionStorage
      // let currentUrl = state.url;
      // sessionStorage.setItem('currentUrl', currentUrl);

      return true;
    }
    // not logged in so redirect to login page with the return url
    this.router.navigate(['login', { returnUrl: state.url }]);
    return false;
  }
}
