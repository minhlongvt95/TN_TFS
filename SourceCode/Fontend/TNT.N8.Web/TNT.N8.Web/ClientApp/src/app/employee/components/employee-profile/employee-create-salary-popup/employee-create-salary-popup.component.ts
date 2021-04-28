import { Component, OnInit, Inject, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { EmployeeSalaryService } from '../../../services/employee-salary/employee-salary.service';
import { EmployeeSalaryModel } from '../../../models/employee-salary.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import * as $ from 'jquery';

export interface IDialogData{
  empName: string;
  empId: string;
}

@Component({
  selector: 'app-employee-create-salary-popup',
  templateUrl: './employee-create-salary-popup.component.html',
  styleUrls: ['./employee-create-salary-popup.component.css']
})
export class EmployeeCreateSalaryPopupComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  createSalaryPopupFormGroup: FormGroup;

  empSalaryModel = new EmployeeSalaryModel();
  constructor(
    private formBuilder: FormBuilder,
    private employeeSalaryService: EmployeeSalaryService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private el: ElementRef,
    public dialogRef: MatDialogRef<EmployeeCreateSalaryPopupComponent>
  ) { }

  ngOnInit() {
    this.empSalaryModel.EffectiveDate = new Date();
    this.createSalaryPopupFormGroup = this.formBuilder.group({
      empName: [''],
      empSalary: ['',[Validators.required]],
      responsibilitySalary: ['']
    })
  }
  onCreateEmployeeSalary(){
    if (!this.createSalaryPopupFormGroup.valid) {
    Object.keys(this.createSalaryPopupFormGroup.controls).forEach(key => {
      if (this.createSalaryPopupFormGroup.controls[key].valid === false) {
        this.createSalaryPopupFormGroup.controls[key].markAsTouched();
      }
    });

    let target;

    target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

    if (target) {
      $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
      target.focus();
    }
  } else {
    this.empSalaryModel.EmployeeId = this.data.empId;
    this.employeeSalaryService.creatEmpSalary(this.empSalaryModel,this.userId ).subscribe(response => {
      let result = <any>response;
    });
    this.dialogRef.close();
    }
  }

  onCancel(){
    this.dialogRef.close();
  }

}
