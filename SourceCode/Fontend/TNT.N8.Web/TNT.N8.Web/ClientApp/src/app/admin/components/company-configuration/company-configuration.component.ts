import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { SelectionModel } from '@angular/cdk/collections';
import { ConfirmationService } from 'primeng/api';
import { CompanyService } from '../../../shared/services/company.service';
import { EmployeeService } from '../../../employee/services/employee.service';
import { BankService } from '../../../shared/services/bank.service';
import { CompanyConfigModel } from '../../../shared/models/companyConfig.model';
import { BankpopupComponent } from '../../../shared/components/bankpopup/bankpopup.component';
import * as $ from 'jquery';
import { GetPermission } from '../../../shared/permission/get-permission';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

interface BankAccount {
  bankAccountId: string,
  objectId: string,
  objectType: string,
  bankName: string, //Tên ngân hàng
  accountNumber: string,  //Số tài khoản
  accountName: string,  //Chủ tài khoản
  branchName: string, //Chi nhánh
  bankDetail: string,  //Diễn giải
  createdById: string,
  createdDate: Date
}

@Component({
  selector: 'app-company-configuration',
  templateUrl: './company-configuration.component.html',
  styleUrls: ['./company-configuration.component.css']
})
export class CompanyConfigurationComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listBankAccount: Array<BankAccount> = [];
  colHeader: any[];
  isViewMode = true;
  submitted = false;
  companyConfigModel = new CompanyConfigModel();

  configFormGroup: FormGroup;
  companyName: FormControl;
  phoneNumber: FormControl;
  email: FormControl;
  taxCode: FormControl;
  address: FormControl;
  representative: FormControl;
  position: FormControl;

  displayedColumns = ['bankname', 'bankdetail', 'accountname', 'accountnumber', 'branchname', 'select'];
  arrayBankAccount: Array<any>;

  actionEdit: boolean = true;
  actionDelete: boolean = true;

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  auth: any = JSON.parse(localStorage.getItem("auth"));

  constructor(
    public snackBar: MatSnackBar,
    private el: ElementRef,
    private router: Router,
    private getPermission: GetPermission,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private bankService: BankService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  selection = new SelectionModel<any>(true, []);
  urls: string = window.document.URL.substr(window.document.baseURI.length - 1, window.document.URL.length);

  ngOnInit() {
    this.initTable();
    this.setUpControlOnFormGroup();

    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.getMasterData();
        this.disableEnableInput(this.isViewMode);
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/company-config/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.messageService.add({ severity: 'warn', summary: 'Warning Message', detail: "Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ" })
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          }
          if (listCurrentActionResource.indexOf("delete") == -1) {
            this.actionDelete = false;
          }
          this.getMasterData();
          this.disableEnableInput(this.isViewMode);
        }
      }
    }, error => { });
  }

  /* #region  initTable() - set hearder for table */
  initTable() {
    this.colHeader = [
      { field: 'bankName', header: 'Tên viết tắt', textAlign: 'left', display: 'table-cell' },
      { field: 'bankDetail', header: 'Tên diễn giải', textAlign: 'left', display: 'table-cell' },
      { field: 'accountName', header: 'Chủ tài khoản', textAlign: 'left', display: 'table-cell' },
      { field: 'accountNumber', header: 'Số tài khoản', textAlign: 'right', display: 'table-cell' },
      { field: 'branchName', header: 'Chi nhánh', textAlign: 'left', display: 'table-cell' },
    ];
  }
  /* #endregion */

  /* #region  set value for company information form */
  setUpControlOnFormGroup() {
    let prc = '[a-zA-Z0-9]+$';
    let emailPattern = '^([" +"]?)+[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]+([" +"]?){2,64}';

    this.companyName = new FormControl(null, Validators.required);
    this.phoneNumber = new FormControl(null, [Validators.required, Validators.pattern(this.getPhonePattern())]);
    this.email = new FormControl(null, Validators.pattern(emailPattern));
    this.taxCode = new FormControl(null, Validators.pattern(prc));
    this.address = new FormControl(null);
    this.representative = new FormControl(null);
    this.position = new FormControl(null);

    this.configFormGroup = new FormGroup({
      companyName: this.companyName,
      phoneNumber: this.phoneNumber,
      email: this.email,
      taxCode: this.taxCode,
      address: this.address,
      representative: this.representative,
      position: this.position,
    });
  }
  /* #endregion */

  getMasterData() {
    this.companyService.getCompanyConfig().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.companyConfigModel = result.companyConfig;
        this.listBankAccount = result.listBankAccount;
        this.mapDataToForm();
      }
    }, error => { });
  }

  mapDataToForm() {
    this.companyName.setValue(this.companyConfigModel.companyName);
    this.phoneNumber.setValue(this.companyConfigModel.phone);
    this.email.setValue(this.companyConfigModel.email);
    this.taxCode.setValue(this.companyConfigModel.taxCode);
    this.address.setValue(this.companyConfigModel.companyAddress);
    this.representative.setValue(this.companyConfigModel.contactName);
    this.position.setValue(this.companyConfigModel.contactRole);
  }

  /*Click button Sửa*/
  onEditBtn() {
    this.isViewMode = false;
    this.disableEnableInput(this.isViewMode);
  }

  /*Click button Hủy*/
  onCancelBtn() {
    this.isViewMode = true;
    this.disableEnableInput(this.isViewMode);
    this.getMasterData();
  }

  /* #region  addBankAccountDialog() - */
  addBankAccountDialog() {
    let bankAccount: BankAccount = {
      bankAccountId: null,
      objectId: this.companyConfigModel.companyId,
      objectType: "COM",
      bankName: "", //Tên ngân hàng
      accountNumber: "",  //Số tài khoản
      accountName: "",  //Chủ tài khoản
      branchName: "", //Chi nhánh
      bankDetail: "",  //Diễn giải
      createdById: this.emptyGuid,
      createdDate: new Date()
    };

    let ref = this.dialogService.open(BankpopupComponent, {
      data: {
        bankAccount: bankAccount,
      },
      header: 'Thêm thông tin thanh toán',
      width: '55%',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "190px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result.status) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo mới thành công' };
          this.showMessage(msg);
          this.getMasterData();
        }
      }
    });
  }
  /* #endregion */

  /* #region delete item from table */
  deleteItem(row: any) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.bankService.deleteBankById(row.bankAccountId, row.objectId, row.objectType).subscribe(res => {
          let resultDel = <any>res;
          if (resultDel.statusCode === 202 || resultDel.statusCode === 200) {
            this.getMasterData();
          } else {
            this.messageService.add({ severity: 'error', summary: 'Error Message', detail: resultDel.messageCode });
          }
        });
      }
    });
  }
  /* #endregion */

  /* #region  edit one item of table */
  editItem(row: any) {
    alert('coming soon')
  }
  /* #endregion */

  /* #region even when click [Lưu] button */
  onSaveBtn() {
    if (!this.configFormGroup.valid) {
      Object.keys(this.configFormGroup.controls).forEach(key => {
        if (this.configFormGroup.controls[key].valid === false) {
          this.configFormGroup.controls[key].markAsTouched();
        }
      });
      let target: { focus: () => void; };
      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    }
    else {
      this.companyConfigModel.companyName = this.companyName.value;
      this.companyConfigModel.phone = this.phoneNumber.value;
      this.companyConfigModel.email = this.email.value;
      this.companyConfigModel.taxCode = this.taxCode.value;
      this.companyConfigModel.companyAddress = this.address.value;
      this.companyConfigModel.contactName = this.representative.value;
      this.companyConfigModel.contactRole = this.position.value;

      this.companyService.editCompanyConfig(this.companyConfigModel).subscribe(response => {
        let result = <any>response;

        if (result.statusCode === 202 || result.statusCode === 200) {
          this.messageService.add({severity: "success" , summary: "Success Message", detail: "Sửa thông tin công ty thành công"})
          this.isViewMode = true;
          this.disableEnableInput(this.isViewMode);
          this.getMasterData();
        }
        else {
          this.messageService.add({severity: "error" , summary: "Error Message", detail: "Sửa thông tin công ty thất bại"})
        }
      }, error => { });
    }

  }
  /* #endregion */

  disableEnableInput(bool: any) {
    if (bool === false) {
      this.companyName.enable();
      this.phoneNumber.enable();
      this.email.enable();
      this.taxCode.enable();
      this.address.enable();
      this.representative.enable();
      this.position.enable();
    } else {
      this.companyName.disable();
      this.phoneNumber.disable();
      this.email.disable();
      this.taxCode.disable();
      this.address.disable();
      this.representative.disable();
      this.position.disable();
    }
  }

  getPhonePattern() {
    let phonePatternObj = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultPhoneType");
    return phonePatternObj.systemValueString;
  }
}
