import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Pipe({ name: 'ProvinceService' })

@Injectable()
export class ProductService {

  constructor(private httpClient: HttpClient) { }

  getDataQuickCreateProduct() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/Product/getDataQuickCreateProduct';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createProductAsync(product: any,
    ListProductVendorMapping: Array<any>,
     ListProductAttributeCategory: Array<any>,
     ListInventoryReport: Array<any>,
     ListProductImage: Array<any>,
     userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProduct';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        Product: product,
        ListProductVendorMapping: ListProductVendorMapping,
        ListProductAttributeCategory: ListProductAttributeCategory,
        ListInventoryReport: ListInventoryReport,
        ListProductImage: ListProductImage,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

}
