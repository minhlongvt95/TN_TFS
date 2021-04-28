import { Component, OnInit, ViewChild, ElementRef, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { VendorService } from "../../services/vendor.service";
import { CategoryService } from "../../../shared/services/category.service";
import { SuggestedSupplierQuotesDetailModel, SuggestedSupplierQuotesModel, VendorModel } from "../../models/vendor.model";
import { ContactModel } from "../../../shared/models/contact.model";

import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MenuItem } from 'primeng/api';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';

class vendorGroupModel {
  categoryId: string;
  categoryName: string;
  categoryCode: string;
}
/*Back-end sử dụng 2 model khác nhau do 2 người code*/
/*Get dữ liệu lên sự dụng VendorQuoteModel - đẩy dữ liệu xuống sử dụng SuggestedSupplierQuoteRequestModel model*/
class suggestedSupplierQuote {
  vendorId: string;
  vendorName: string
  vendorCode: string;
  vendorGroupId: string;
  vendorGroupName: string;
  active: boolean;
  contactId: string;
  countVendorInformation: number;
  suggestedSupplierQuote: string;
  suggestedSupplierQuoteId: string;
  statusId: string;
  statusName: string;
  quoteTermDate: string;
  recommendedDate: string;
  personInChargeId: string;
  canDelete: boolean;

  listVendorQuoteDetail: Array<suggestedSupplierQuotesDetail>;
}

class suggestedSupplierQuotesDetail {
  suggestedSupplierQuoteDetailId: string;
  suggestedSupplierQuoteId: string;
  productId: string;
  productCode: string;
  productName: string;
  quantity: string;
  note: string;
  active: boolean;
  quoteName: string;
  saleBiddingName: string;
  procurenmentRequest: string;
  vendorId: string;
}

interface ResultDialog {
  status: boolean,  // True là lưu thành công, false là lưu thất bại
  message: string;
}

@Component({
  selector: 'app-list-vendor-quote',
  templateUrl: './list-vendor-quote.component.html',
  styleUrls: ['./list-vendor-quote.component.css'],
  providers: [ConfirmationService, MessageService, DialogService]
})
export class ListVendorQuoteComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;
  //master data
  listVendorGroup: Array<vendorGroupModel> = [];
  listVendorQuote: Array<suggestedSupplierQuote> = [];
  listVendorQuoteDetail: Array<suggestedSupplierQuotesDetail> = [];
  //form
  searchVendorForm: FormGroup;
  //responsive
  innerWidth: number = 0; //number window size first
  isShowFilterTop: boolean = false;
  isShowFilterLeft: boolean = false;
  leftColNumber: number = 12;
  rightColNumber: number = 2;
  @ViewChild('myTable') myTable: Table;
  filterGlobal: string = '';
  first: number = 0;
  colsListProduct: any;
  selectedColumns: any[];
  rows: number = 10;

  actionAdd: boolean = true;
  actionDelete: boolean = true;
  actionEdit: boolean = true;
  actions: MenuItem[] = [];
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  userPermission: any = localStorage.getItem("UserPermission").split(',');
  createPermission: string = "vendor/create";
  viewPermission: string = "vendor/view";
  editPermission: string = 'vendor/edit';
  vendorOrderPermission: string = 'vendor-order';
  auth: any = JSON.parse(localStorage.getItem("auth"));

  constructor(private translate: TranslateService,
    private vendorService: VendorService,
    private confirmationService: ConfirmationService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private router: Router,
    private categoryService: CategoryService) {
    translate.setDefaultLang('vi');
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    let resource = "buy/vendor/list-vendor-quote/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      if (listCurrentActionResource.indexOf("delete") == -1) {
        this.actionDelete = false;
      }
      this.initForm();
      this.initTable();
      this.getMasterData();
    }
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  initForm() {
    this.searchVendorForm = new FormGroup({
      "VendorName": new FormControl(''),
      "VendorCode": new FormControl(''),
      "VendorGroup": new FormControl([])
    });
  }

  patchValueForm() {
    this.searchVendorForm.reset();
    this.searchVendorForm.patchValue({
      "VendorName": '',
      "VendorCode": '',
      "VendorGroup": []
    });
  }

  initTable() {
    this.colsListProduct = [
      { field: 'recommendedDate', header: 'Ngày đề nghị', textAlign: 'right', display: 'table-cell' },
      { field: 'suggestedSupplierQuote', header: 'Số đề nghị', textAlign: 'left', display: 'table-cell' },
      { field: 'vendorCode', header: 'Mã NCC', textAlign: 'left', display: 'table-cell' },
      { field: 'vendorName', header: 'Tên NCC', textAlign: 'left', display: 'table-cell' },
      { field: 'note', header: 'Diễn giải', textAlign: 'left', display: 'table-cell' },
      { field: 'statusName', header: 'Trạng thái', textAlign: 'center', display: 'table-cell' },
    ];
    this.selectedColumns = this.colsListProduct;
  }

  refreshFilter() {
    this.searchVendorForm.reset();
    this.patchValueForm();
    this.resetTable();
    this.searchVendor();
  }

  showFilter() {
    if (this.innerWidth < 1024) {
      this.isShowFilterTop = !this.isShowFilterTop;
    } else {
      this.isShowFilterLeft = !this.isShowFilterLeft;
      if (this.isShowFilterLeft) {
        this.leftColNumber = 8;
        this.rightColNumber = 4;
      } else {
        this.leftColNumber = 12;
        this.rightColNumber = 0;
      }
    }
  }

  resetTable() {
    this.listVendorQuote = [];
    this.filterGlobal = '';
    this.first = 0;
    // this.myTable.reset();
  }

  async getMasterData() {
    this.loading = true;
    let [masterdataResponse, initSearchResponse]: any = await Promise.all([
      this.vendorService.getDataSearchVendor(this.auth.UserId),
      this.vendorService.searchVendorQuoteAsync('', '', [], this.auth.UserId)
    ]);
    this.loading = false;
    if (masterdataResponse.statusCode === 200 && initSearchResponse.statusCode === 200) {
      
      this.listVendorGroup = masterdataResponse.listVendorGroup;
      this.listVendorQuote = initSearchResponse.listVendorQuote;

    } else if (masterdataResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', masterdataResponse.messageCode);
    } else if (initSearchResponse.statusCode !== 200) {
      this.clearToast();
      this.showToast('error', 'Thông báo', initSearchResponse.messageCode);
    }
  }

  goToCreateVendorQuote(){
    this.router.navigate(['/vendor/vendor-quote-create']);
  }

  goToDetailVendorQuote(rowData: any){
    this.router.navigate(['/vendor/vendor-quote-detail', { SuggestedSupplierQuoteId: rowData.suggestedSupplierQuoteId }]);
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }


  goToDetail(rowData: suggestedSupplierQuote) {
    // this.router.navigate(['/vendor/detail', { vendorId: rowData.vendorId, contactId: rowData.contactId }]);
  }

  deleteSuggestedSupplierQuoteRequest(rowData: suggestedSupplierQuote) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this.vendorService.deleteSuggestedSupplierQuoteRequest(rowData.suggestedSupplierQuoteId, this.auth.UserId).subscribe(response => {
          let result = <any>response;
          if (result.statusCode === 200) {
            this.clearToast();
            this.showToast('success', 'Thông báo', result.messageCode);
            this.searchVendor();
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        }, error => { });
      }
    });
  }

  checkEnterPress(event: any) {
    if (event.code === "Enter") {
      this.searchVendor();
    }
  }

  async searchVendor() {
    let name = this.searchVendorForm.get('VendorName').value;
    let code = this.searchVendorForm.get('VendorCode').value;
    let listVendorGroupId = this.searchVendorForm.get('VendorGroup').value.map(e => e.categoryId);
    this.loading = true;
    let result: any = await this.vendorService.searchVendorQuoteAsync(name, code, listVendorGroupId, this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.resetTable(); //reset state of table
      this.listVendorQuote = result.listVendorQuote;
      if (this.listVendorQuote.length == 0) {
        this.showToast('warn', 'Thông báo', 'Không tìm thấy nhà cung cấp nào')
      }
    } else {
      this.clearToast();
      this.showToast('error', 'Thông báo', result.messageCode);
    }
  }

  // onChangeAction(rowData: suggestedSupplierQuote) {
  //   this.actions = [];
  //   let editVendorProductPrice: MenuItem = {
  //     id: '1', label: 'Sửa DNBG NCC', icon: 'pi pi-comment', command: () => {
  //       // this.editCreateSuggestedSupplierQuote(rowData);
  //     }
  //   }
  //   let deleteVendorProductPrice: MenuItem = {
  //     id: '2', label: 'Xóa DNBG NCC', icon: 'pi pi-trash', command: () => {
  //       this.deleteSuggestedSupplierQuoteRequest(rowData);
  //     }
  //   }

  //   if (this.actionEdit === true) {
  //     this.actions.push(editVendorProductPrice);
  //   }
  //   if (this.actionDelete === true && rowData.canDelete == true) {
  //     this.actions.push(deleteVendorProductPrice);
  //   }
  // }

}
