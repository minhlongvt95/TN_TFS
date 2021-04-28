
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductModel, ProductModel2, ProductQuantityInWarehouseModel, ProductAttributeCategory, ProductImageModel, InventoryReportModel, ProductVendorMappingModel, PriceProductModel } from '../models/product.model';
import { VendorModel } from '../../vendor/models/vendor.model';
import { ProductAttributeCategoryModel } from '../models/productAttributeCategory.model';
@Injectable()
export class ProductService {

  constructor(private httpClient: HttpClient) { }

  searchProduct(productName: string, productCode: string, listProductCategory: Array<string>, listVendor: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/searchProduct';
    return this.httpClient.post(url, {
      ProductName: productName,
      ProductCode: productCode,
      ListProductCategory: listProductCategory,
      ListVendor: listVendor
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProduct(product: ProductModel, LstVendor: Array<string>, LstProductAttributeCategory: Array<ProductAttributeCategoryModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProduct';
    return this.httpClient.post(url, {
      Product: product,
      lstVendor: LstVendor,
      lstProductAttributeCategory: LstProductAttributeCategory,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProductAsync(
    product: ProductModel,
    ListProductVendorMapping: Array<ProductVendorMappingModel>,
    ListProductAttributeCategory: Array<ProductAttributeCategory>,
    ListProductBillOfMaterials: Array<any>,
    ListInventoryReport: Array<ProductQuantityInWarehouseModel>,
    ListProductImage: Array<ProductImageModel>,
    userId: string
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Product: product,
        ListProductVendorMapping: ListProductVendorMapping,
        ListProductAttributeCategory: ListProductAttributeCategory,
        ListProductBillOfMaterials: ListProductBillOfMaterials,
        ListInventoryReport: ListInventoryReport,
        ListProductImage: ListProductImage,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductByID(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/GetProductByID';
    return this.httpClient.post(url, {
      ProductId: productId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProduct(product: ProductModel, LstVendor: Array<VendorModel>, LstProductAttributeCategory: Array<ProductAttributeCategoryModel>) {
    //api/Product/UpdateProduct
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/UpdateProduct';
    return this.httpClient.post(url, {
      Product: product,
      lstVendor: LstVendor,
      lstProductAttributeCategory: LstProductAttributeCategory,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductAsync(product: ProductModel2,
    listProductVendorMapping: Array<ProductVendorMappingModel>,
    LstProductAttributeCategory: Array<ProductAttributeCategory>,
    ListProductBillOfMaterials: Array<any>,
    listMinimunQuantity: Array<InventoryReportModel>,
    listStartQuantity: Array<InventoryReportModel>,
    userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/UpdateProduct';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Product: product,
        ListProductVendorMapping: listProductVendorMapping,
        lstProductAttributeCategory: LstProductAttributeCategory,
        ListProductBillOfMaterials: ListProductBillOfMaterials,
        ListMinimunQuantity: listMinimunQuantity,
        ListStartQuantity: listStartQuantity
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateActiveProduct(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/updateActiveProduct';
    return this.httpClient.post(url, {
      ProductId: productId
    }).pipe(
      map((response: Response) => {
        return response;
      }));

  }

  getProductAttributeByProductID(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getProductAttributeByProductID';
    return this.httpClient.post(url, {
      ProductId: productId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getProductByVendorID(vendorId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getProductByVendorID';
    return this.httpClient.post(url, {
      VendorId: vendorId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getProductByVendorIDAsync(vendorId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getProductByVendorID";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { VendorId: vendorId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  getListCodeAsync() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getAllProductCode";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchProductAsync(productName: string, productCode: string, listProductCategory: Array<string>, listVendor: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/searchProduct";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductName: productName,
        ProductCode: productCode,
        ListProductCategory: listProductCategory,
        ListVendor: listVendor
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListProduct() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getListProduct";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterdataCreateProduct() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getMasterdataCreateProduct";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  addSerialNumber(productId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/dddSerialNumber";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { ProductId: productId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getProductByIDAsync(productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/GetProductByID';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductId: productId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadProductImage(listImageUrl: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/file/downloadProductImage';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListFileUrl: listImageUrl
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataVendorDialog() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getMasterDataVendorDialog";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateProduct() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/downloadTemplateProductService';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  importProduct(listProduct: Array<ProductModel2>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/importProduct';
    return this.httpClient.post(url, { ListProduct: listProduct }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataPriceList() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/getMasterDataPriceList';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  createOrUpdatePriceProduct(priceProduct: PriceProductModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/createOrUpdatePriceProduct';
    return this.httpClient.post(url, { PriceProduct: priceProduct }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  deletePriceProduct(priceProductId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/deletePriceProduct';
    return this.httpClient.post(url, { PriceProductId: priceProductId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataCreateUpdateBOM() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/Product/getDataCreateUpdateBOM";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadPriceProductTemplate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/downloadPriceProductTemplate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  importPriceProduct(list: Array<PriceProductModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/Product/importPriceProduct';
    return this.httpClient.post(url, { ListPriceProduct: list }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }


}
