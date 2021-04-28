import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { map } from 'rxjs/operators';
import 'rxjs/add/operator/map';
import { WarehouseModel } from '../models/warehouse.model';
import { ProductQuantityInWarehouseModel, Serial } from '../../product/models/product.model';
import { InventoryReceivingVoucherModel } from '../models/InventoryReceivingVoucher.model';
import { InventoryReceivingVoucherMapping } from '../models/inventoryReceivingVoucherMapping.model';
import { SanPhamPhieuNhapKhoModel } from '../models/sanPhamPhieuNhapKhoModel.model';

@Injectable()
export class WarehouseService {

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  constructor(private httpClient: HttpClient) { }

  createWareHouse(wareHouse: WarehouseModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateWareHouse';
    return this.httpClient.post(url, {
      WareHouse: wareHouse
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  createWareHouseAsync(wareHouse: WarehouseModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateWareHouse';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        WareHouse: wareHouse
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateWareHouse(wareHouse: WarehouseModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateWareHouse';
    return this.httpClient.post(url, { WareHouse: wareHouse })
      .map((response: Response) => {
        return <any>response;
      });
  }

  searchWareHouse() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/searchWareHouse';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  downloadTemplateSerial() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/downloadTemplateSerial';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  searchWareHouseAsync() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/searchWareHouse';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getWareHouseCha() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getWareHouseCha';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  getVendorOrderDetailByVenderOrderId(listId: any[], typeWarehouseVocher: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getVendorOrderDetailByVenderOrderId';
    return this.httpClient.post(url, { TypeWarehouseVocher: typeWarehouseVocher, ListVendorOrderId: listId })
      .map((response: Response) => {
        return <any>response;
      });
  }
  
  createOrUpdateInventoryVoucher(inventoryReceivingVoucher: any, inventoryReceivingVoucherMapping: Array<any>, fileList: File[], noteContent: string,userId:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createOrUpdateInventoryVoucher';

    let formData: FormData = new FormData();
    if (fileList !== null) {
      for (var i = 0; i < fileList.length; i++) {
        formData.append("fileList", fileList[i]);
      }
    }

    formData.append("inventoryReceivingVoucher", JSON.stringify(inventoryReceivingVoucher));

    formData.append("inventoryReceivingVoucherMapping", JSON.stringify(inventoryReceivingVoucherMapping));

    formData.append('noteContent', noteContent);

    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  removeWareHouse(wareHouseId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/removeWareHouse';
    return this.httpClient.post(url, {
      WareHouseId: wareHouseId
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getInventoryReceivingVoucherById(id: string, userId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getInventoryReceivingVoucherById';


    return this.httpClient.post(url, { Id: id })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getListInventoryReceivingVoucher(voucherCode: string, listStatusSelectedId: Array<any>, listWarehouseSelectedId: Array<any>,
    listCreateVoucherSelectedId: Array<any>, listStorekeeperSelectedId: Array<any>, listVendorSelectedId: Array<any>,
    listProductSelectedId: Array<any>, listCreateDate: Array<Date>, listInventoryReceivingDate: Array<Date>, serial:string,userId:any
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getListInventoryReceivingVoucher';

    return this.httpClient.post(url, {
      VoucherCode: voucherCode, listStatusSelectedId: listStatusSelectedId, listWarehouseSelectedId: listWarehouseSelectedId,
      listCreateVoucherSelectedId: listCreateVoucherSelectedId, listStorekeeperSelectedId: listStorekeeperSelectedId,
      listVendorSelectedId: listVendorSelectedId, listProductSelectedId: listProductSelectedId, listCreateDate: listCreateDate,
      listInventoryReceivingDate: listInventoryReceivingDate, serial: serial, UserId: userId
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getListCustomerOrderByIdCustomerId(customerId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getListCustomerOrderByIdCustomerId';

    return this.httpClient.post(url, { CustomerId: customerId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getCustomerOrderDetailByCustomerOrderId(listCustomerOrderId: Array<any>, typeWarehouseVocher:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getCustomerOrderDetailByCustomerOrderId';

    return this.httpClient.post(url, { TypeWarehouseVocher: typeWarehouseVocher, ListCustomerOrderId: listCustomerOrderId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  checkQuantityActualReceivingVoucher(objectId: any, type: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/checkQuantityActualReceivingVoucher';

    return this.httpClient.post(url, { ObjectId: objectId, Type: type})
      .map((response: Response) => {
        return <any>response;
      });
  }

  filterVendor() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/filterVendor';

    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  filterCustomer() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/filterCustomer';

    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  changeStatusInventoryReceivingVoucher(inventoryReceivingVoucherId:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/changeStatusInventoryReceivingVoucher';

    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  deleteInventoryReceivingVoucher(inventoryReceivingVoucherId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/deleteInventoryReceivingVoucher';

    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  inventoryDeliveryVoucherFilterVendorOrder() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/inventoryDeliveryVoucherFilterVendorOrder';

    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  inventoryDeliveryVoucherFilterVendorOrderAsyc() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/warehouse/inventoryDeliveryVoucherFilterVendorOrder";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  inventoryDeliveryVoucherFilterCustomerOrder() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/inventoryDeliveryVoucherFilterCustomerOrder';

    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  inventoryDeliveryVoucherFilterCustomerOrderAsyc() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/warehouse/inventoryDeliveryVoucherFilterCustomerOrder";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTop10WarehouseFromReceivingVoucher() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getTop10WarehouseFromReceivingVoucher';

    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  getTop10WarehouseFromReceivingVoucherAsync() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getTop10WarehouseFromReceivingVoucher';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getSerial(warehouseId: any, productId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getSerial';

    return this.httpClient.post(url, { WarehouseId: warehouseId, ProductId: productId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getSerialAsync(warehouseId: any, productId: any) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getSerial';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { WarehouseId: warehouseId, ProductId: productId}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createUpdateInventoryDeliveryVoucher(inventoryReceivingVoucher: any, inventoryReceivingVoucherMapping: Array<any>, fileList: File[], noteContent: string, userId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateInventoryDeliveryVoucher';

    let formData: FormData = new FormData();
    if (fileList !== null) {
      for (var i = 0; i < fileList.length; i++) {
        formData.append("fileList", fileList[i]);
      }
    }

    formData.append("inventoryDeliveryVoucher", JSON.stringify(inventoryReceivingVoucher));

    formData.append("inventoryyDeliveryVoucherMapping", JSON.stringify(inventoryReceivingVoucherMapping));

    formData.append('noteContent', noteContent);

    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getInventoryDeliveryVoucherById(id: string, userId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getInventoryDeliveryVoucherById';
    return this.httpClient.post(url, { Id: id })
      .map((response: Response) => {
        return <any>response;
      });
  }

  deleteInventoryDeliveryVoucher(inventoryDeliveryVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/deleteInventoryDeliveryVoucher';
    return this.httpClient.post(url, { InventoryDeliveryVoucherId: inventoryDeliveryVoucherId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  changeStatusInventoryDeliveryVoucher(inventoryDeliveryVoucherId: string,userId:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/changeStatusInventoryDeliveryVoucher';
    return this.httpClient.post(url, { InventoryDeliveryVoucherId: inventoryDeliveryVoucherId, UserId: userId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  filterCustomerInInventoryDeliveryVoucher() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/filterCustomerInInventoryDeliveryVoucher';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  searchInventoryDeliveryVoucher(voucherCode: string, listStatusSelectedId: Array<any>, listWarehouseSelectedId: Array<any>,
    listCreateVoucherSelectedId: Array<any>, listStorekeeperSelectedId: Array<any>, listCustomerSelectedId: Array<any>,
    listProductSelectedId: Array<any>, listCreateDate: Array<Date>, listInventoryReceivingDate: Array<Date>, serial: string,userId:any
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/searchInventoryDeliveryVoucher';

    return this.httpClient.post(url, {
      VoucherCode: voucherCode, listStatusSelectedId: listStatusSelectedId, listWarehouseSelectedId: listWarehouseSelectedId,
      listCreateVoucherSelectedId: listCreateVoucherSelectedId, listStorekeeperSelectedId: listStorekeeperSelectedId,
      listCustomerSelectedId: listCustomerSelectedId, listProductSelectedId: listProductSelectedId, listCreateDate: listCreateDate,
      listInventoryReceivingDate: listInventoryReceivingDate, serial: serial, UserId: userId
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  filterProduct(listProductCategory: Array<any>, listProductId: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/filterProduct';
    return this.httpClient.post(url, { ListProductCategory: listProductCategory, ListProductId: listProductId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getProductNameAndProductCode(query:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getProductNameAndProductCode';
    return this.httpClient.post(url, { Query: query })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getVendorInvenoryReceiving() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getVendorInvenoryReceiving';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  getCustomerDelivery() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getCustomerDelivery';
    return this.httpClient.post(url, {})
      .map((response: Response) => {
        return <any>response;
      });
  }

  inStockReport(lstProduct: Array<any>, lstWarehouse: Array<any>, fromQuantity: any, toQuantity: any, serialCode:any,userId:any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/inStockReport';
    return this.httpClient.post(url, { lstProduct: lstProduct, lstWarehouse: lstWarehouse, FromQuantity: fromQuantity, ToQuantity: toQuantity, SerialCode: serialCode, UserId: userId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  createUpdateWarehouseMasterdata(warehouseId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateWarehouseMasterdata';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        WarehouseId: warehouseId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  ////////////////////////////////////////////////////////////////////////////////////////

  getMasterDataSearchInStockReport() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getMasterDataSearchInStockReport';
    return this.httpClient.post(url, { })
      .map((response: Response) => {
        return <any>response;
      });
  }

  searchInStockReport(FromDate: Date, ProductNameCode: string, ProductCategoryId: string, WarehouseId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/searchInStockReport';
    return this.httpClient.post(url, { 
      FromDate: FromDate,
      ProductNameCode: ProductNameCode,
      ProductCategoryId: ProductCategoryId,
      WarehouseId: WarehouseId
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getMasterDataPhieuNhapKho() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getMasterDataPhieuNhapKho';
    return this.httpClient.post(url, { })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getVendorOrderByVendorId(vendorId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getVendorOrderByVendorId';
    return this.httpClient.post(url, { VendorId: vendorId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getDanhSachSanPhamCuaPhieu(listObjectId: Array<string>, objectType: number, warehouseId: string, inventoryReceivingVoucherId?: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getDanhSachSanPhamCuaPhieu';
    return this.httpClient.post(url, { 
      ListObjectId: listObjectId, 
      ObjectType: objectType,
      WarehouseId: warehouseId,
      InventoryReceivingVoucherId: inventoryReceivingVoucherId
    })
      .map((response: Response) => {
        return <any>response;
      });
  }

  getDanhSachKhoCon(warehouseId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getDanhSachKhoCon';
    return this.httpClient.post(url, { WarehouseId: warehouseId })
      .map((response: Response) => {
        return <any>response;
      });
  }

  createItemInventoryReport(inventoryReport : ProductQuantityInWarehouseModel){
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createItemInventoryReport';
    return this.httpClient.post(url, { InventoryReport : inventoryReport }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  updateItemInventoryReport(inventoryReport : ProductQuantityInWarehouseModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/updateItemInventoryReport';
    return this.httpClient.post(url, { InventoryReport : inventoryReport }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  createUpdateSerial(listSerial: Array<Serial>, warehouseId: string, productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createUpdateSerial';
    return this.httpClient.post(url, 
      { ListSerial : listSerial, WarehouseId: warehouseId, ProductId: productId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  deleteItemInventoryReport(inventoryReportId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/deleteItemInventoryReport';
    return this.httpClient.post(url, { InventoryReportId : inventoryReportId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  /*Lấy Số giữ trước của sản phẩm theo kho*/
  getSoGTCuaSanPhamTheoKho(productId: string, warehouseId: string, quantityRequest: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getSoGTCuaSanPhamTheoKho';
    return this.httpClient.post(url, { 
      ProductId : productId, 
      WarehouseId: warehouseId, 
      QuantityRequest: quantityRequest }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  createPhieuNhapKho(inventoryReceivingVoucher: InventoryReceivingVoucherModel, 
    listInventoryReceivingVoucherMapping: Array<InventoryReceivingVoucherMapping>, listFile: Array<any>
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/createPhieuNhapKho';

    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);

    formData.append("InventoryReceivingVoucher.inventoryReceivingVoucherId", inventoryReceivingVoucher.inventoryReceivingVoucherId);
    formData.append("InventoryReceivingVoucher.inventoryReceivingVoucherCode", inventoryReceivingVoucher.inventoryReceivingVoucherCode ?? "");
    formData.append("InventoryReceivingVoucher.statusId", inventoryReceivingVoucher.statusId);
    formData.append("InventoryReceivingVoucher.inventoryReceivingVoucherType", inventoryReceivingVoucher.inventoryReceivingVoucherType.toString());
    formData.append("InventoryReceivingVoucher.warehouseId", inventoryReceivingVoucher.warehouseId);
    formData.append("InventoryReceivingVoucher.shiperName", inventoryReceivingVoucher.shiperName ?? "");
    formData.append("InventoryReceivingVoucher.storekeeper", inventoryReceivingVoucher.storekeeper);
    formData.append("InventoryReceivingVoucher.inventoryReceivingVoucherDate", 
      inventoryReceivingVoucher.inventoryReceivingVoucherDate == null ? null : inventoryReceivingVoucher.inventoryReceivingVoucherDate.toUTCString());
    formData.append("InventoryReceivingVoucher.inventoryReceivingVoucherTime", inventoryReceivingVoucher.storekeeper);
    formData.append("InventoryReceivingVoucher.licenseNumber", inventoryReceivingVoucher.licenseNumber.toString());
    formData.append("InventoryReceivingVoucher.expectedDate", 
      inventoryReceivingVoucher.expectedDate == null ? null : inventoryReceivingVoucher.expectedDate.toUTCString());
    formData.append("InventoryReceivingVoucher.description", inventoryReceivingVoucher.description ?? "");
    formData.append("InventoryReceivingVoucher.note", inventoryReceivingVoucher.note ?? "");
    formData.append("InventoryReceivingVoucher.partnersId", inventoryReceivingVoucher.partnersId);
    formData.append("InventoryReceivingVoucher.createdDate", inventoryReceivingVoucher.createdDate.toUTCString());
    formData.append("InventoryReceivingVoucher.createdById", inventoryReceivingVoucher.createdById);
    formData.append("InventoryReceivingVoucher.active", inventoryReceivingVoucher.active ? 'true' : 'false');

    listInventoryReceivingVoucherMapping.forEach((item, index) => {
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].inventoryReceivingVoucherMappingId", item.inventoryReceivingVoucherMappingId);
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].inventoryReceivingVoucherId", item.inventoryReceivingVoucherId); 
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].objectId", item.objectId);
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].objectDetailId", item.objectDetailId);
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].productId", item.productId);
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].quantityRequest", item.quantityRequest.toString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].quantityActual", item.quantityActual.toString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].quantityReservation", item.quantityReservation.toString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].quantitySerial", item.quantitySerial == null ? "0" : item.quantitySerial.toString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].priceProduct", item.priceProduct.toString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].priceAverage", item.priceAverage ? 'true' : 'false');
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].warehouseId", item.warehouseId);
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].description", item.description ?? '');
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].active", item.active ? 'true' : 'false');
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].createdDate", item.createdDate.toUTCString());
      formData.append("ListInventoryReceivingVoucherMapping[" + index + "].createdById", item.createdById);
    });

    for (var i = 0; i < listFile.length; i++) {
      formData.append("ListFile", listFile[i]);
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDetailPhieuNhapKho(inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getDetailPhieuNhapKho';
    return this.httpClient.post(url, { 
      InventoryReceivingVoucherId : inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  suaPhieuNhapKho(inventoryReceivingVoucher: InventoryReceivingVoucherModel, listInventoryReceivingVoucherMapping: Array<InventoryReceivingVoucherMapping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/suaPhieuNhapKho';
    return this.httpClient.post(url, { 
      InventoryReceivingVoucher : inventoryReceivingVoucher, 
      ListInventoryReceivingVoucherMapping: listInventoryReceivingVoucherMapping }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  kiemTraKhaDungPhieuNhapKho(listSanPhamPhieuNhapKho: Array<SanPhamPhieuNhapKhoModel>, inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/kiemTraKhaDungPhieuNhapKho';
    return this.httpClient.post(url, { 
      ListSanPhamPhieuNhapKho: listSanPhamPhieuNhapKho,
      InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  danhDauCanLamPhieuNhapKho(inventoryReceivingVoucherId: string, check: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/danhDauCanLamPhieuNhapKho';
    return this.httpClient.post(url, { 
      InventoryReceivingVoucherId: inventoryReceivingVoucherId, 
      Check: check }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  nhanBanPhieuNhapKho(inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/nhanBanPhieuNhapKho';
    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  xoaPhieuNhapKho(inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/xoaPhieuNhapKho';
    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  huyPhieuNhapKho(inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/huyPhieuNhapKho';
    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  khongGiuPhanPhieuNhapKho(listSanPhamPhieuNhapKho: Array<SanPhamPhieuNhapKhoModel>, inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/khongGiuPhanPhieuNhapKho';
    return this.httpClient.post(url, { 
      ListSanPhamPhieuNhapKho: listSanPhamPhieuNhapKho,
      InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  kiemTraNhapKho(listSanPhamPhieuNhapKho: Array<SanPhamPhieuNhapKhoModel>, inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/kiemTraNhapKho';
    return this.httpClient.post(url, { 
      ListSanPhamPhieuNhapKho: listSanPhamPhieuNhapKho,
      InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  datVeNhapPhieuNhapKho(inventoryReceivingVoucherId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/datVeNhapPhieuNhapKho';
    return this.httpClient.post(url, { InventoryReceivingVoucherId: inventoryReceivingVoucherId }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getListProductPhieuNhapKho() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getListProductPhieuNhapKho';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  getMasterDataListPhieuNhapKho() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/getMasterDataListPhieuNhapKho';
    return this.httpClient.post(url, { }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }

  searchListPhieuNhapKho(maPhieu: string, 
    fromNgayLapPhieu: Date, toNgayLapPhieu: Date,
    fromNgayNhapKho: Date, toNgayNhapKho: Date,
    listStatusId: Array<string>, 
    listWarehouseId: Array<string>, listEmployeeId: Array<string>, listProductId: Array<string>, serialCode: string) {
    
    const url = localStorage.getItem('ApiEndPoint') + '/api/warehouse/searchListPhieuNhapKho';
    return this.httpClient.post(url, { 
      MaPhieu: maPhieu,
      FromNgayLapPhieu: fromNgayLapPhieu,
      ToNgayLapPhieu: toNgayLapPhieu,
      FromNgayNhapKho: fromNgayNhapKho,
      ToNgayNhapKho: toNgayNhapKho,
      ListStatusId: listStatusId,
      ListWarehouseId: listWarehouseId,
      ListEmployeeId: listEmployeeId,
      ListProductId: listProductId,
      SerialCode: serialCode
    }).pipe(
      map((response: Response) => {
        return <any>response;
    }));
  }
}
