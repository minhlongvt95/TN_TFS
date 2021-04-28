
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { Pipe, Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';


import { finalize, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Pipe({ name: 'AppInterceptor' })
export class AppInterceptor implements HttpInterceptor {
  constructor(private _router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const currentUser = <any>localStorage.getItem('auth');
    if (currentUser != null) {
      // Clone the request to add the new header.
      let body = req.body != null ? req.body : {};
      let headers = req.headers.set("Authorization", `Bearer ${JSON.parse(currentUser).Token}`)
      body.UserId = JSON.parse(currentUser).UserId;

      const authReq = req.clone({ headers: headers, body: body });      
      //send the newly created request
      return next.handle(authReq).pipe(catchError((error, caught) => {
        //intercept the respons error and displace it to the console        
        console.log(error);
        //return the error to the method that called it
        return observableThrowError(error);
      })).pipe(catchError(err => {
        console.log(err);
        if (err instanceof HttpErrorResponse) {          
          console.log(err);
          if (err.status === 401) {
            localStorage.removeItem('auth');
            this._router.navigate(['/login']);
          }
        }
        return new Observable<any>(err);
      }), finalize(() => { })) as any;
    } else {
      return next.handle(req).pipe(
        catchError((error, caught) => {
          //intercept the respons error and displace it to the console
          console.log("Error Occurred");
          console.log(error);
          //return the error to the method that called it
          return observableThrowError(error);
        })) as any;
    }
  }
}
