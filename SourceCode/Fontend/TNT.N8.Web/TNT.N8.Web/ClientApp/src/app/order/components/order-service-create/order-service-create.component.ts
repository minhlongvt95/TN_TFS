import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CustomerOrderService } from '../../services/customer-order.service';
import { CustomerOrder } from '../../models/customer-order.model';
import { CustomerOrderDetail } from '../../models/customer-order-detail.model';
import { OrderCostDetail } from '../../models/customer-order-cost-detail.model';
import { ContactModel } from '../../../shared/models/contact.model';

class OrderStus {
  orderStatusId: string;
  orderStatusCode: string;
}

class Category {
  categoryId: string;
  categoryCode: string;
}

class ProductCategory {
  productCategoryId: string;
  productCategoryCode: string;
  productCategoryName: string;
  productCategoryLevel: number;
  active: boolean;
}

class Product {
  productId: string;
  productCode: string;
  productName: string;
  productCategoryId: string;
  price1: number;
  productUnitName: string;
  currentQuantity: number;
  productUnitId: string;
}

class LocalAddress {
  localAddressId: string;
  localAddressCode: string;
  localAddressName: string;
  address: string;
  description: string;
  branchId: string;

  listLocalPoint: Array<LocalPoint>;
}

class LocalPoint {
  localPointId: string;
  localPointCode: string;
  localPointName: string;
  address: string;
  description: string;
  statusId: number;
  localAddressId: string;
  statusName: string;
}

@Component({
  selector: 'app-order-service-create',
  templateUrl: './order-service-create.component.html',
  styleUrls: ['./order-service-create.component.css']
})
export class OrderServiceCreateComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  awaitResult: boolean = false;

  isShowArea: boolean = true; //ẩn hiện khu vực
  isShowListProduct: boolean = false; //ẩn hiện danh sách sản phẩm
  isShowSelectedProduct: boolean = false; //ẩn hiện danh sách sản phẩm đã chọn

  dialogQuantity: boolean = false; //ẩn hiện dialog thay đổi số lượng sản phẩm
  currentProductName: string = null;  //Tên sản phẩm đang được chọn
  currentQuantity: string = '1'; //Số lượng sản phẩm đang được chọn

  dialogEditProduct: boolean = false;
  currentEditProductName: string = null;
  currentQuantityEdit: string = '1';

  listLocalAddress: Array<LocalAddress> = []; //list Khu vực
  listSelectedLocalPoint: Array<LocalPoint> = []; //list Điểm đang được chọn
  listOrderStatus: Array<OrderStus> = []; //list trạng thái đơn hàng
  listMoneyUnit: Array<Category> = [];  //list đơn vị tiền
  listProductCategory: Array<ProductCategory> = []; //list nhóm sản phẩm
  listProduct: Array<Product> = []; //list sản phẩm
  listCurrentProduct: Array<Product> = []; //list sản phẩm theo nhóm sản phẩm đang được chọn
  currentProduct: Product = null;
  listChoiceProduct: Array<Product> = []; //list sản phẩm đã gọi
  totalPrice: number = 0; //giá tạm tính
  orderId: string = null;

  constructor(
    private confirmationService: ConfirmationService,
    private customerOrderService: CustomerOrderService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    this.getDataDefault();
  }

  getDataDefault() {
    this.loading = true;
    this.customerOrderService.getMasterDataOrderServiceCreate().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listLocalAddress = result.listLocalAddress;
        this.listOrderStatus = result.listOrderStatus;
        this.listMoneyUnit = result.listMoneyUnit;
        this.listProductCategory = result.listProductCategory;
        this.listProduct = result.listProduct;

        this.setDefaultValue();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    this.listProductCategory.forEach(item => {
      item.active = false;
    });
    //Thêm Tất cả vào nhóm danh mục sản phẩm
    let item: ProductCategory = {
      productCategoryId: this.emptyGuid,
      productCategoryCode: 'all',
      productCategoryName: 'Tất cả',
      productCategoryLevel: 0,
      active: true
    }
    this.listProductCategory.unshift(item);
    
    this.listProduct.forEach(item => {
      item.currentQuantity = 0;
    });
    this.listCurrentProduct = this.listProduct;
  }

  /*Load lại trạng thái của Khu vực và các Điểm*/
  refreshLocalPoint() {
    this.listSelectedLocalPoint = [];
    this.loading = true;
    this.customerOrderService.refreshLocalPoint().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        //Load lại tất cả các điểm
        this.listLocalAddress = result.listLocalAddress;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Hoàn thành chọn các điểm đã được chọn*/
  choiceLocalPoint() {
    if (this.listSelectedLocalPoint?.length > 0) {
      //Chuyển sang màn danh sách sản phẩm
      this.isShowArea = false;
      this.isShowListProduct = true;
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn bàn' };
      this.showMessage(msg);
    }
  }

  /*Thay đổi Nhóm sản phẩm*/
  changeProductCategory(productCategory: ProductCategory) {
    this.listProductCategory.forEach(item => {
      item.active = false;
      if (item.productCategoryId == productCategory.productCategoryId) {
        item.active = true;
      }
    });

    //List sản phẩm sẽ lấy theo nhóm sản phẩm
    if (productCategory.productCategoryId == this.emptyGuid) {
      this.listCurrentProduct = this.listProduct;
    }
    else {
      this.listCurrentProduct = this.listProduct.filter(x => x.productCategoryId == productCategory.productCategoryId);
    }
  }

  /*Chọn các Điểm*/
  selectedLocalPoint(localPoint: LocalPoint) {
    this.orderId = null;
    //Nếu trạng thái của Điểm là Sẵn sàng hoặc Đang được chọn
    if (localPoint.statusId == 1 || localPoint.statusId == 2) {
      //Nếu trạng thái là sẵn sàng
      if (localPoint.statusId == 1) {
        //Thêm vào list
        this.listSelectedLocalPoint.push(localPoint);

        //Đổi trạng thái của Điểm đó sang Đang chọn
        localPoint.statusId = 2;
        localPoint.statusName = 'Đang chọn';
      }
      else {
        //Xóa khỏi list
        this.listSelectedLocalPoint = this.listSelectedLocalPoint.filter(x => x.localPointId != localPoint.localPointId);

        //Đổi trạng thái của Điểm đó sang Sẵn sàng
        localPoint.statusId = 1;
        localPoint.statusName = 'Sẵn sàng';
      }
    }
    //Nếu trạng thái của Điểm là Đang dùng thì hiển thị cảnh báo
    else if (localPoint.statusId == 0) {
      //Hiển thị lại các món đã đặt
      this.confirmationService.confirm({
        message: 'Bạn có chắc chắn muốn thêm món cho bàn ' + localPoint.localPointName + '?',
        header: 'Xác nhận',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          //reset các điểm đang chọn về trạng thái sẵn sàng
          this.listSelectedLocalPoint.forEach(item => {
            if (item.statusId = 2) {
              item.statusId = 1;
              item.statusName = 'Sẵn sàng';
            }
          });

          this.listSelectedLocalPoint = [];

          //reset các món đã chọn
          this.listChoiceProduct = [];
          
          //tính lại giá tạm tính
          this.caculTotalPrice();

          //Lấy lại số lượng các món đã đặt
          this.customerOrderService.getListProductWasOrder(localPoint.localPointId).subscribe(response => {
            let result: any = response;

            if (result.statusCode == 200) {
              this.orderId = result.orderId;
              let listProductWasOrder: Array<any> = result.listProductWasOrder;
              
              this.listProduct.forEach(item => {
                let existProduct = listProductWasOrder.find(x => x.productId == item.productId);

                if (existProduct) {
                  item.currentQuantity = existProduct.quantity;
                }
              });

              this.isShowArea = false;
              this.isShowListProduct = true;
            }
            else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        }
      });
    }
  }

  /*Quay lại chọn khu vực*/
  goToArea() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn quay lại đặt bàn?',
      header: 'Xác nhận',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        //reset số lượng các món đã chọn
        this.listProduct.forEach(item => {
          item.currentQuantity = 0;
        });

        //reset các điểm đang chọn về trạng thái sẵn sàng
        this.listSelectedLocalPoint.forEach(item => {
          if (item.statusId = 2) {
            item.statusId = 1;
            item.statusName = 'Sẵn sàng';
          }
        });

        this.listSelectedLocalPoint = [];

        this.isShowArea = true;
        this.isShowListProduct = false;
        this.isShowSelectedProduct = false;
      }
    });
  }

  /*Thay đổi số lượng của sản phẩm*/
  changeQuantityProduct(product: Product) {
    this.currentProductName = product.productName;
    //bật dialog thay đổi số lượng
    this.dialogQuantity = true;

    this.currentProduct = {
      productId: product.productId,
      productCode: product.productCode,
      productName: product.productName,
      productCategoryId: product.productCategoryId,
      price1: product.price1,
      productUnitName: product.productUnitName,
      currentQuantity: product.currentQuantity,
      productUnitId: product.productUnitId
    };

    this.currentQuantity = this.currentProduct.currentQuantity == 0 ? '1' : this.currentProduct.currentQuantity.toString();
  }

  /*Giảm số lượng sản phẩm đang được chọn*/
  minusQuantity() {
    let quantity = ParseStringToFloat(this.currentQuantity.toString());
    if (quantity > 0) {
      quantity -= 1;
      this.currentQuantity = quantity.toString();
    }

    this.currentProduct.currentQuantity = ParseStringToFloat(this.currentQuantity);
  }

  /*Tăng số lượng sản phẩm đang được chọn*/
  plusQuantity() {
    let quantity = ParseStringToFloat(this.currentQuantity.toString());
    quantity += 1;
    this.currentQuantity = quantity.toString();

    this.currentProduct.currentQuantity = ParseStringToFloat(this.currentQuantity);
  }

  /*Đồng ý thay đổi số lượng sản phẩm đang được chọn*/
  completeQuantity() {
    //thay đổi số lượng của sản phẩm
    let item = this.listCurrentProduct.find(x => x.productId == this.currentProduct.productId);
    item.currentQuantity = ParseStringToFloat(this.currentQuantity);
    this.currentQuantity = '1';

    this.dialogQuantity = false;
  }

  /*Hủy thay đổi số lượng sản phẩm đang được chọn*/
  cancelQuantity() {
    //không thay đổi số lượng của sản phẩm
    this.currentProduct = null;
    this.currentQuantity = '1';

    this.dialogQuantity = false;
  }

  /*Hoàn thành chọn sản phẩm*/
  completeOrder() {
    //Lấy những sản phẩm có số lượng lớn hơn 0
    this.listChoiceProduct = this.listProduct.filter(x => x.currentQuantity > 0);

    if (this.listChoiceProduct.length > 0) {
      this.isShowListProduct = false;
      this.isShowSelectedProduct = true;
  
      //Tính lại giá Tạm tính
      this.caculTotalPrice();
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn chưa chọn món' };
      this.showMessage(msg);
    }
  }

  /*Quay lại chọn món*/
  goToListProduct() {
    this.isShowSelectedProduct = false;
    this.isShowListProduct = true;
  }

  /*Xóa 1 sản phẩm đã gọi*/
  removeProduct(product: Product) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      header: 'Xác nhận',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.listChoiceProduct = this.listChoiceProduct.filter(x => x.productId != product.productId);

        //Tính lại giá Tạm tính
        this.caculTotalPrice();

        //Nếu đã xóa hết sản phẩm đã gọi thì quay lại màn hình chọn món
        if (this.listChoiceProduct.length == 0) {
          //reset list sản phẩm
          this.listProduct.forEach(item => {
            item.currentQuantity = 0;
          });

          //chọn lại tab tất cả sản phẩm
          let productCategory = this.listProductCategory.find(x => x.productCategoryId == this.emptyGuid);
          this.changeProductCategory(productCategory);

          //Quay lại Chọn món
          this.isShowSelectedProduct = false;
          this.isShowListProduct = true;
        }
      }
    });
  }

  /*Sửa một sản phẩm đã gọi*/
  editProduct(product: Product) {
    this.currentEditProductName = product.productName;

    this.currentProduct = {
      productId: product.productId,
      productCode: product.productCode,
      productName: product.productName,
      productCategoryId: product.productCategoryId,
      price1: product.price1,
      productUnitName: product.productUnitName,
      currentQuantity: product.currentQuantity,
      productUnitId: product.productUnitId
    };

    this.currentQuantityEdit = this.currentProduct.currentQuantity == 0 ? '1' : this.currentProduct.currentQuantity.toString();

    this.dialogEditProduct = true;
  }

  /*Giảm số lượng sản phẩm đã gọi*/
  minusQuantityEdit() {
    let quantity = ParseStringToFloat(this.currentQuantityEdit.toString());
    if (quantity > 0) {
      quantity -= 1;
      this.currentQuantityEdit = quantity.toString();
    }

    this.currentProduct.currentQuantity = ParseStringToFloat(this.currentQuantityEdit);
  }

  /*Tăng số lượng sản phẩm đã gọi*/
  plusQuantityEdit() {
    let quantity = ParseStringToFloat(this.currentQuantityEdit.toString());
    quantity += 1;
    this.currentQuantityEdit = quantity.toString();

    this.currentProduct.currentQuantity = ParseStringToFloat(this.currentQuantityEdit);
  }

  /*Đồng ý thay đổi số lượng sản phẩm đã gọi*/
  completeQuantityEdit() {
    //thay đổi số lượng của sản phẩm
    let item = this.listChoiceProduct.find(x => x.productId == this.currentProduct.productId);
    item.currentQuantity = ParseStringToFloat(this.currentQuantityEdit);
    this.currentQuantityEdit = '1';

    //Tính lại giá Tạm tính
    this.caculTotalPrice();
        
    this.dialogEditProduct = false;
  }

  /*Hủy thay đổi số lượng sản phẩm đã gọi*/
  cancelQuantityEdit() {
    //không thay đổi số lượng của sản phẩm
    this.currentProduct = null;
    this.currentQuantityEdit = '1';

    this.dialogEditProduct = false;
  }

  /*Gửi đơn hàng*/
  complete() {
    //Nếu có sản phẩm đã gọi thì mới tạo đơn hàng
    if (this.listChoiceProduct?.length > 0) {
      //Nếu là gọi thêm món 
      if (this.orderId) {
        let listCustomerOrderDetail: Array<CustomerOrderDetail> = [];
        let zeroQuantity = true;

        this.listChoiceProduct.forEach((item, index) => {
          let customerOrderDetail = new CustomerOrderDetail();

          customerOrderDetail.OrderDetailId = this.emptyGuid;
          customerOrderDetail.OrderId = this.orderId;
          customerOrderDetail.CurrencyUnit = this.listMoneyUnit.find(x => x.categoryCode == 'VND').categoryId; //Đơn vị tiền lấy mặc định là VND
          customerOrderDetail.IncurredUnit = 'CCCCC';
          customerOrderDetail.OrderDetailType = 0;
          customerOrderDetail.OrderNumber = index + 1;
          customerOrderDetail.PriceInitial = 0;
          customerOrderDetail.ProductId = item.productId; //id sản phẩm
          customerOrderDetail.ProductName = item.productName; //tên sản phẩm
          customerOrderDetail.Quantity = item.currentQuantity; //số lượng sản phẩm
          customerOrderDetail.UnitId = item.productUnitId; //đơn vị tính của sản phẩm
          customerOrderDetail.UnitPrice = item.price1; //đơn giá của sản phẩm

          listCustomerOrderDetail = [...listCustomerOrderDetail, customerOrderDetail];

          if (item.currentQuantity == 0) {
            zeroQuantity = false;
          }
        });

        //Nếu có ít nhất một sản phẩm có số lượng = 0 thì cảnh báo và không cho tạo đơn hàng
        if (!zeroQuantity) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Có món có số lượng bằng 0, vui lòng kiểm tra lại' };
          this.showMessage(msg);
        }
        else {
          this.loading = true;
          this.awaitResult = true;
          this.customerOrderService.updateCustomerService(this.orderId, listCustomerOrderDetail).subscribe(response => {
            let result: any = response;

            if (result.statusCode == 200) {
              //reset lại
              this.orderId = null;
              this.listChoiceProduct = [];
              this.listSelectedLocalPoint = [];
              this.getDataDefault();
              //trở về chọn khu vực
              this.isShowArea = true;
              this.isShowListProduct = false;
              this.isShowSelectedProduct = false;

              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
              this.showMessage(msg);
            }
            else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        }
      }
      //Nếu tạo đơn hàng mới
      else {
        let customerOrder = new CustomerOrder();

        customerOrder.OrderDate = convertToUTCTime(new Date());
        customerOrder.Seller = this.auth.EmployeeId;
        customerOrder.CustomerId = this.emptyGuid;
        customerOrder.PaymentMethod = this.emptyGuid;
        customerOrder.DaysAreOwed = 0;
        customerOrder.MaxDebt = 0;
        customerOrder.ReceivedDate = convertToUTCTime(new Date());
        customerOrder.RecipientName = '';
        customerOrder.Amount = this.totalPrice; //tiền tạm tính
        customerOrder.StatusId = this.listOrderStatus.find(x => x.orderStatusCode == 'DRA').orderStatusId; //trạng thái nháp
        customerOrder.CustomerName = 'Khách lẻ';
        customerOrder.CustomerAddress = '';

        let listCustomerOrderDetail: Array<CustomerOrderDetail> = [];
        let zeroQuantity = true;

        this.listChoiceProduct.forEach((item, index) => {
          let customerOrderDetail = new CustomerOrderDetail();

          customerOrderDetail.OrderDetailId = this.emptyGuid;
          customerOrderDetail.OrderId = this.emptyGuid;
          customerOrderDetail.CurrencyUnit = this.listMoneyUnit.find(x => x.categoryCode == 'VND').categoryId; //Đơn vị tiền lấy mặc định là VND
          customerOrderDetail.IncurredUnit = 'CCCCC';
          customerOrderDetail.OrderDetailType = 0;
          customerOrderDetail.OrderNumber = index + 1;
          customerOrderDetail.PriceInitial = 0;
          customerOrderDetail.ProductId = item.productId; //id sản phẩm
          customerOrderDetail.ProductName = item.productName; //tên sản phẩm
          customerOrderDetail.Quantity = item.currentQuantity; //số lượng sản phẩm
          customerOrderDetail.UnitId = item.productUnitId; //đơn vị tính của sản phẩm
          customerOrderDetail.UnitPrice = item.price1; //đơn giá của sản phẩm

          listCustomerOrderDetail = [...listCustomerOrderDetail, customerOrderDetail];

          if (item.currentQuantity == 0) {
            zeroQuantity = false;
          }
        });

        let listLocalPointId = this.listSelectedLocalPoint.map(x => x.localPointId);

        //Nếu có ít nhất một sản phẩm có số lượng = 0 thì cảnh báo và không cho tạo đơn hàng
        if (!zeroQuantity) {
          let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Có món có số lượng bằng 0, vui lòng kiểm tra lại' };
          this.showMessage(msg);
        } 
        else {
          this.loading = true;
          this.awaitResult = true;
          this.customerOrderService.createOrderService(customerOrder, listCustomerOrderDetail, listLocalPointId).subscribe(response => {
            let result: any = response;
            this.loading = false;
            this.awaitResult = false;

            if (result.statusCode == 200) {
              //reset lại
              this.orderId = null;
              this.listChoiceProduct = [];
              this.listSelectedLocalPoint = [];
              this.getDataDefault();
              //trở về chọn khu vực
              this.isShowArea = true;
              this.isShowListProduct = false;
              this.isShowSelectedProduct = false;

              let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo đơn hàng thành công' };
              this.showMessage(msg);
            }
            else {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
              this.showMessage(msg);
            }
          });
        }
      }
    }
    else {
      let msg = { severity: 'warn', summary: 'Thông báo:', detail: 'Thực đơn trống, không thể gửi thực đơn' };
      this.showMessage(msg);
    }
  }

  /*Tính giá tạm tính*/
  caculTotalPrice() {
    this.totalPrice = 0;
    this.listChoiceProduct.forEach(item => {
      this.totalPrice += item.currentQuantity * item.price1;
    });
  }

  /*Xóa toàn bộ item đã chọn*/
  removeAllProduct() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa toàn bộ thực đơn?',
      header: 'Xác nhận',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        //xóa hết sản phẩm đã gọi
        this.listChoiceProduct = [];

        //reset list sản phẩm
        this.listProduct.forEach(item => {
          item.currentQuantity = 0;
        });

        //chọn lại tab tất cả sản phẩm
        let productCategory = this.listProductCategory.find(x => x.productCategoryId == this.emptyGuid);
        this.changeProductCategory(productCategory);

        //Tính lại giá Tạm tính
        this.caculTotalPrice();

        //Quay lại Chọn món
        this.isShowSelectedProduct = false;
        this.isShowListProduct = true;
      }
    });
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};