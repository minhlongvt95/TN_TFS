import * as $ from 'jquery';
import { Component, OnInit, ViewChild, ElementRef, HostListener, Renderer2 } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { AccountingService } from "../../../services/accounting.service";
import { AuthenticationService } from "../../../../shared/services/authentication.service";
import { GetPermission } from '../../../../shared/permission/get-permission';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { SortEvent } from 'primeng/api';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';
import 'moment/locale/pt-br';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { OrganizationDialogComponent } from '../../../../shared/components/organization-dialog/organization-dialog.component';


interface Cost {
  costId: string,
  costCode: string,
  costName: string,
  organizationId: string,
  organizationName: string,
  statusId: string,
  statusName: string,
  createdById: string,
  createdByName: string,
  createdDate: Date,
}

interface Category {
  categoryId: string,
  categoryName: string,
  categoryCode: string,
}

@Component({
  selector: 'app-cost-create',
  templateUrl: './cost-create.component.html',
  styleUrls: ['./cost-create.component.css'],
  providers: [
    DatePipe,
  ]
})
export class CostCreateComponent implements OnInit {
  fixed: boolean = false;
  withFiexd: string = "";
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
    }
  }
  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionAdd: boolean = true;
  auth: any = JSON.parse(localStorage.getItem('auth'));
  //get system parameter
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  loading: boolean = false;
  awaitResult: boolean = false;// khóa nút lưu
  cols: any[];
  selectedColumns: any[];
  listCost: Array<Cost> = [];
  listStatus: Array<Category> = [];
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  organzationId: string = '';
  actionDelete = true;
  costId: string;
  isUpdate : boolean = false;

  /*START : FORM COST*/
  costForm: FormGroup;
  costNameControl: FormControl;
  costCodeControl: FormControl;
  costOrganizationControl: FormControl;
  costStatusControl: FormControl;
  /*END : FORM COST*/


  // costUpdate: Cost = {
  //   costId: this.emptyGuid,
  //   costCode: '',
  //   costName: '',
  //   organizationId: this.emptyGuid,
  //   organizantionName: '',
  //   statusId: this.emptyGuid,
  //   statusName: '',
  //   createdById: this.emptyGuid,
  //   createdByName: '',
  //   createdDate: new Date(),
  // };
  isUpdateAI: boolean = false;

  costCode: string;
  costName: string;
  statusId: string;
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('save') save: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private translate: TranslateService,
    private getPermission: GetPermission,
    private accountingService: AccountingService,
    private el: ElementRef,
    private renderer: Renderer2,
    private messageService: MessageService,
    public dialogService: DialogService,
    private router: Router,
  ) {

    this.renderer.listen('window', 'click', (e: Event) => {
      /**
       * Only run when toggleButton is not clicked
       * If we don't check this, all clicks (even on the toggle button) gets into this
       * section which in the result we might never see the menu open!
       * And the menu itself is checked here, and it's where we check just outside of
       * the menu and button the condition abbove must close the menu
       */
      if (this.toggleButton && this.notifi) {
        if (!this.toggleButton.nativeElement.contains(e.target) &&
          !this.notifi.nativeElement.contains(e.target) &&
          !this.save.nativeElement.contains(e.target)) {
          this.isOpenNotifiError = false;
        }
      }
    });
  }

  async ngOnInit() {
    let resource = "acc/accounting/cost-create/";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    } else {
      this.setForm();
      this.setTable();
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      await this.getMasterData();
    }
  }

  createCost() {
    if (!this.costForm.valid) {
      Object.keys(this.costForm.controls).forEach(key => {
        if (this.costForm.controls[key].valid == false) {
          this.costForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.costForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {
      let status = this.costForm.controls['costStatusControl'].value;
      // Lấy giá trị để truyền xuống db
      this.costCode = this.costForm.controls['costCodeControl'].value.trim();
      this.costName = this.costForm.controls['costNameControl'].value.trim();
      this.statusId = status ? status.categoryId : null;

      let check = this.listCost.find(x => x.costCode == this.costCode);
      if (check) {
        //Nếu tồn tại rồi thì không cho thêm và hiển thị cảnh báo
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chi phí này đã tồn tại' };
        this.showMessage(msg);
      } else {
        this.saveCost();
      }
    }
  }

  setDefaultStatus() {
    const toSelectOrderStatus = this.listStatus.find(stt => stt.categoryCode === "DSD");
    this.costForm.controls['costStatusControl'].setValue(toSelectOrderStatus);
  }

  saveCost() {
    this.awaitResult = true;
    this.loading = true;
    this.accountingService.createCost(this.costCode, this.costName, this.organzationId, this.statusId, this.auth.UserId).subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCost = result.listCost;
        let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm chi phí thành công' };
        this.showMessage(mgs);
        this.awaitResult = false;
        this.resetForm();

        if (this.emitStatusChangeForm) {
          this.emitStatusChangeForm.unsubscribe();
          this.isInvalidForm = false; //Ẩn icon-warning-active
        }
      } else {
        let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(mgs);
        this.awaitResult = false;
      }

    }, error => { });
  }

  updateCost() {
    if (!this.costForm.valid) {
      Object.keys(this.costForm.controls).forEach(key => {
        if (this.costForm.controls[key].valid == false) {
          this.costForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;
      this.isOpenNotifiError = true;
      this.emitStatusChangeForm = this.costForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else {
      let status = this.costForm.controls['costStatusControl'].value;
      // Lấy giá trị để truyền xuống db
      this.costCode = this.costForm.controls['costCodeControl'].value.trim();
      this.costName = this.costForm.controls['costNameControl'].value.trim();
      this.statusId = status ? status.categoryId : null;

      let check = this.listCost.find(x => x.costCode == this.costCode && x.costId != this.costId);
      if (check) {
        //Nếu tồn tại rồi thì không cho thêm và hiển thị cảnh báo
        let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chi phí này đã tồn tại' };
        this.showMessage(msg);
      } else {
        this.awaitResult = true;
        this.loading = true;
        this.accountingService.updateCost(this.costId, this.costCode, this.costName, this.organzationId, this.statusId, this.auth.UserId).subscribe(response => {
          var result = <any>response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.listCost = result.listCost;
            let mgs = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhật chi phí thành công' };
            this.showMessage(mgs);
            this.awaitResult = false;
            if (this.emitStatusChangeForm) {
              this.emitStatusChangeForm.unsubscribe();
              this.isInvalidForm = false; //Ẩn icon-warning-active
            }
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
            this.showMessage(mgs);
            this.awaitResult = false;
          }

        }, error => { });
      }
    }
  }

  cancel(){
    this.isUpdate = false;
    this.costId = '';
    this.resetForm();
  }

  /*Hiển thị lại thông tin bổ sung*/
  reShowNote(event: any) {
    let rowData: Cost = event.data;
    this.costId = rowData.costId;
    this.isUpdate = true;
    this.costForm.controls['costCodeControl'].setValue(rowData.costCode);
    this.costForm.controls['costNameControl'].setValue(rowData.costName);
    this.costForm.controls['costOrganizationControl'].setValue(rowData.organizationName);
    let status = this.listStatus.find(c => c.categoryId == rowData.statusId);
    this.costForm.controls['costStatusControl'].setValue(status);

    this.costForm.controls['costCodeControl'].updateValueAndValidity();
    this.costForm.controls['costNameControl'].updateValueAndValidity();
    this.costForm.controls['costOrganizationControl'].updateValueAndValidity();
    this.costForm.controls['costStatusControl'].updateValueAndValidity();
  }

  async getMasterData() {
    this.loading = true;
    this.accountingService.GetMasterDataCreateCost().subscribe(response => {
      var result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listCost = result.listCost;
        this.listStatus = result.listStatus;
        this.setDefaultStatus();
      }
    });
  }

  resetForm() {
    this.costForm.controls['costCodeControl'].reset();
    this.costForm.controls['costNameControl'].reset();
    this.costForm.controls['costOrganizationControl'].reset();
    this.costForm.controls['costStatusControl'].reset();
  }

  setForm() {
    this.costNameControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.costCodeControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.costOrganizationControl = new FormControl(null);
    this.costStatusControl = new FormControl(null);

    this.costForm = new FormGroup({
      costNameControl: this.costNameControl,
      costCodeControl: this.costCodeControl,
      costOrganizationControl: this.costOrganizationControl,
      costStatusControl: this.costStatusControl
    });
  }

  setTable() {
    this.cols = [
      { field: 'costCode', header: 'Mã phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'costName', header: 'Tên phí', width: '150px', textAlign: 'left', color: '#f44336' },
      { field: 'organizationName', header: 'Bộ phận', width: '50px', textAlign: 'left', color: '#f44336' },
      { field: 'statusName', header: 'Trạng thái', width: '50px', textAlign: 'center', color: '#f44336' },
    ];

    this.selectedColumns = this.cols;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView();
  }

  openOrgPopup() {
    let ref = this.dialogService.open(OrganizationDialogComponent, {
      data: {
        chooseFinancialIndependence: false //Nếu chỉ chọn đơn vị độc lập tài chính
      },
      header: 'Chọn đơn vị',
      width: '65%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          this.organzationId = result.selectedOrgId;
          this.costForm.controls['costOrganizationControl'].setValue(result.selectedOrgName);
        }
      }
    });
  }
}
function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

