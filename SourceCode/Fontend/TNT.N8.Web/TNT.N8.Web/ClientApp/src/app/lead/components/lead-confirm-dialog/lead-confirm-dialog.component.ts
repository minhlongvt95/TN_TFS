import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { SelectionModel } from '@angular/cdk/collections';
import { EmployeeModel } from '../../../shared/models/employee.model';
import { LeadService } from '../../services/lead.service';

export interface IDialogData {
  picValue: string;
  statusValue: string;
  potentialValue: string;
  ok: boolean;
  empCCIdList: Array<string>;
}

@Component({
  selector: 'app-lead-confirm-dialog',
  templateUrl: './lead-confirm-dialog.component.html',
  styleUrls: ['./lead-confirm-dialog.component.css']
})
export class LeadConfirmDialogComponent implements OnInit {
  selection: SelectionModel<EmployeeModel>;
  managerArray: Array<any> = [];
  currentUserName: string = localStorage.getItem("UserFullName");
  auth: any = JSON.parse(localStorage.getItem("auth"));

  constructor(
    public dialogRef: MatDialogRef<LeadConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private leadService: LeadService) { }

  onCancelClick(): void {
    this.data.ok = false;
    this.dialogRef.close(this.data.ok);
  }

  onOkClick(): void {
    this.data.ok = true;
    this.dialogRef.close(this.data);
  }

  /**Checkbox event cho nguoi nhan thong bao email */
  checkboxClick(event: MatCheckboxChange, emp) {
    if (event.checked) {
      this.data.empCCIdList.push(emp.employeeId);
    } else {
      this.data.empCCIdList.splice(this.data.empCCIdList.indexOf(emp.employeeId), 1);
    }
  }

  isAllSelectedEmpToSendEmail() {
    const numSelected = this.selection.selected.length;
    const numRows = this.managerArray.length;
    return numSelected === numRows;
  }

  masterToggleEmpToSendEmail() {
    if (this.isAllSelectedEmpToSendEmail()) {
      this.selection.clear();
      this.data.empCCIdList = [];
    } else {
      this.managerArray.forEach(emp => {
        this.selection.select(emp);
        this.data.empCCIdList.push(emp.employeeId);
      });
    }
  }
  /**End */

  ngOnInit() {
    this.data.empCCIdList = [];

    this.leadService.getEmployeeWithNotificationPermisison().subscribe(response => {
      let result = <any>response;
      this.managerArray = result.employeeList;
      this.selection = new SelectionModel<EmployeeModel>(true, []);
    }, error => { });
  }
}
