
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import axios from 'axios';

const ApiKey = "85af23cd452647d89546605190d5ccd1";
const baseURL = "https://api.opencagedata.com/geocode/v1/json"

@Pipe({ name: 'GoogleService' })
@Injectable()
export class GoogleService {

  constructor(private httpClient: HttpClient) { }

  async getGeoCoding(address: string) {
    let q = address ? encodeURIComponent(address) : encodeURIComponent("");
    const openCase = `${baseURL}?q=${q}&key=${ApiKey}&language=vi&pretty=1`;
    const response = await axios.get(openCase);
    return response;
  }
}
