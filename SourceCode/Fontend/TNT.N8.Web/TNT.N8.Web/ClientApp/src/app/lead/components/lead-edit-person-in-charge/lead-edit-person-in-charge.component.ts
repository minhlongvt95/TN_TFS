import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FailComponent } from '../../../shared/toast/fail/fail.component';

//SERVICES
import { LeadService } from '../../services/lead.service';
import { EmployeeService } from '../../../employee/services/employee.service';
import { PopupComponent } from '../../../shared/components/popup/popup.component';

@Component({
  selector: 'app-lead-edit-person-in-charge',
  templateUrl: './lead-edit-person-in-charge.component.html',
  styleUrls: ['./lead-edit-person-in-charge.component.css']
})
export class LeadEditPersonInChargeComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;

  listEmloyee: Array<any> = [];
  employeeId: string = null;
  employeeName: any
  leadIdList: Array<string> = [];

  /*Set Form*/
  editPersonInChargeForm: FormGroup;
  personInChargeControl: FormControl;
  /*End*/

  message_error: string = "Bạn phải chọn đúng người phụ trách trong danh sách";
  dialogPopup: MatDialogRef<PopupComponent>;

  filteredPic: Observable<string[]>;

  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(
    @Inject(MAT_DIALOG_DATA) public dataDialog: any,
    public dialogRef: MatDialogRef<LeadEditPersonInChargeComponent>,
    private leadService: LeadService,
    public dialog: MatDialog,
    public employeeService: EmployeeService,
    public dialogPop: MatDialog,
    public snackBar: MatSnackBar,
  ) { }

  ngOnInit() {
    this.leadIdList = this.dataDialog.leadIdList;
    this.setForm();
    this.getMasterData();
  }

  setForm() {
    this.personInChargeControl = new FormControl('', [Validators.required]);

    this.editPersonInChargeForm = new FormGroup({
      personInChargeControl: this.personInChargeControl
    });
  }

  async getMasterData() {
    //Lấy danh sách nhân viên phụ trách (chỉ nhân viên có quyền view màn hình chi tiết khách hàng tiềm năng)
    //let result: any = await this.leadService.getPersonInCharge();
    //if (result.statusCode == 200) 
    //{
    //  this.listEmloyee = result.listPersonInCharge;

    //  //set list autocomplate Pic
    //  this.filteredPic = this.personInChargeControl.valueChanges.pipe(
    //    startWith(''),
    //    map((item: string) => item ? this._filterPic(item, this.listEmloyee) : this.listEmloyee.slice())
    //  );
    //}
    //else
    //{
    //  this.snackBar.openFromComponent(FailComponent, { data: result.messageCode, ...this.failConfig });
    //}

    await this.employeeService.getEmployeeByPositionCode("NV").subscribe(response => {
      const result = <any>response;
      if (result.statusCode == 200 || result.statusCode == 202) {
        this.listEmloyee = result.employeeList

        this.filteredPic = this.personInChargeControl.valueChanges
          .pipe(
            startWith(''),
            map(name => name ? this._filterPic(name, this.listEmloyee) : this.listEmloyee.slice())
          );
      } else {
        this.snackBar.openFromComponent(FailComponent, { data: "Lấy người phụ trách thất bại", ...this.failConfig });
      }
    });
  }

  /*Autocomplate Pic*/
  private _filterPic(value: any, array: any): string[] {
    let filterValue = value;

    return array.filter(item => item.employeeName.toLowerCase().indexOf(filterValue.toLowerCase()) >= 0 || item.employeeCode.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  }

  selectedPicFn(event: MatAutocompleteSelectedEvent): void {
    this.personInChargeControl.setValue(event.option.viewValue);
    this.employeeId = event.option.value;
    this.employeeName = event.option.viewValue;
  }

  changePersonInCharge() {
    if (this.personInChargeControl.value.trim() == "") {
      this.employeeId = null;
    } else {
      let count = this.listEmloyee.filter(item => item.employeeName.toLowerCase() == this.personInChargeControl.value.toLowerCase().trim() || item.employeeCode.toLowerCase() == this.personInChargeControl.value.toLowerCase().trim()).length;
      if (count != 1) {
        this.employeeId = null;
      } else {
        this.employeeId = this.listEmloyee.find(item => item.employeeName.toLowerCase() == this.personInChargeControl.value.toLowerCase().trim() || item.employeeCode.toLowerCase() == this.personInChargeControl.value.toLowerCase().trim()).employeeId;
      }
    }
  }
  /*End*/

  onCancelClick() {
    this.dialogRef.close();
  }

  btnSave() {
    if (!this.editPersonInChargeForm.valid) {
      Object.keys(this.editPersonInChargeForm.controls).forEach(key => {
        if (!this.editPersonInChargeForm.controls[key].valid) {
          this.editPersonInChargeForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      if (this.employeeId == null) {
        alert(this.message_error);
      }
      else {
        let _title = 'Thông báo';

        let _content = 'Bạn có chắc chắn muốn thực hiện?';
        let _contentline = 'Bạn đang thực hiện chỉnh sửa người phụ trách thành [' + this.employeeName + ']';
        this.dialogPopup = this.dialog.open(PopupComponent,
          {
            width: '500px',
            height: '300px',
            autoFocus: false,
            data: { title: _title, content: _content, contentline: _contentline }
          });
        this.dialogPopup.afterClosed().subscribe(result => {
          if (result) {
            //Updated Lead
            this.leadService.editPersonInCharge(this.leadIdList, this.employeeId, this.userId).subscribe(response => {
              let result: any = response;
              if (result.statusCode == 200) {
                let item = {
                  message: "success",
                  messageCode: result.messageCode
                }
                this.dialogRef.close(item);
              }
              else {
                let item = {
                  message: "faild",
                  messageCode: result.messageCode
                }
                this.dialogRef.close(item);
              }
            });
          }
        });
      }
    }
  }

}
