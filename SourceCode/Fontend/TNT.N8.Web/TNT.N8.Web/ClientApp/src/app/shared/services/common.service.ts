
import {map} from 'rxjs/operators';
import { Pipe, Injectable, EventEmitter, Output, Directive } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


// tslint:disable-next-line:use-pipe-transform-interface
// @Directive()
@Pipe({ name: 'CommonService' })
@Injectable()
export class CommonService {
  constructor(private http: HttpClient) { }

  testToken(): Observable<any> {
    let currentUser = <any>localStorage.getItem('auth');
    let headers = new HttpHeaders({
      'Authorization': 'Bearer ' + JSON.parse(currentUser).token
    });
    
    return this.http.post('http://localhost:5001/api/auth/testToken', { UserId: JSON.parse(currentUser).UserId }, { headers: headers }).pipe(
      map((response: Response) => {
        var result = <any>response.json();
        return result;
      }));
  }

  getApiEndPoint(): any {
    let url = '/api/shared/getkey';
    return this.http.get(url + '?key=API_ENDPOINT').pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVersion(): any {
    let url = '/api/shared/getkey';
    return this.http.get(url + '?key=VERSION').pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // tslint:disable-next-line:member-ordering
  @Output() change: EventEmitter<string> = new EventEmitter();
  click(value: string) {
    // this.value = this.value;
    this.change.emit(value);
  }

}
