import { Component, OnInit, Inject, ElementRef } from '@angular/core';

import { MatSelectChange} from '@angular/material/select';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { BudgetRequestModel } from '../../../models/budget-request.model';
import { BudgetCreateService } from '../../../services/budget-create.service';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BudgetListService } from '../../../services/budget-list.service';
import * as $ from 'jquery';
import { ProductCategoryPopupComponent } from '../product-category-popup/product-category-popup.component';

@Component({
  selector: 'app-budget-create-popup',
  templateUrl: './budget-create-popup.component.html',
  styleUrls: ['./budget-create-popup.component.css']
})
export class BudgetCreatePopupComponent implements OnInit {

  dialogDisplayProductCategory: MatDialogRef<ProductCategoryPopupComponent>;

  userId = JSON.parse(localStorage.getItem("auth")).UserId;
  MDT = "<Mã tự sinh sau khi lưu>";

  //danh sach cac dự toán 
  listAllBudget : Array<any> = [];


  //lấy ngày giờ hiện tại cho label
  CurrentDate : Date = new Date();
  createDate : string;
  
  //khai báo các form group ,form control
  CreateProcurementPlanForm : FormGroup ;
  
  //đối tượng truyền lên Controller
  BudgetRequest : BudgetRequestModel = new BudgetRequestModel();
  
  //ô thông báo thành công
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  
  //khai báo danh sách option trong autocomplete
  listYear : Array<any> = [];
  listMonth : Array<any> = [];

  ProductCategoryName: string = "";
  
  constructor(
    public dialogRef: MatDialogRef<BudgetCreatePopupComponent>,
    private budgetService : BudgetCreateService,
    public snackBar: MatSnackBar,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private budgetListService : BudgetListService,
    private el: ElementRef) 
  { }

  //ham được chạy khi trang được truy cập
  ngOnInit() {
    
    //lấy ngày tháng hiện tại theo dạng dd/mm/yyyy
    this.createDate = this.CurrentDate.getDate().toString() + "/" + (this.CurrentDate.getMonth()+1).toString()
    + "/" + this.CurrentDate.getFullYear().toString();
    
    //set up các formgroup và form control
    this.CreateProcurementPlanForm = this.formBuilder.group({
      ProcurementPlanCodeControl: [''],
      ProcurementPlanContentControl: ['',[Validators.required]],
      ProcurementPlanAmountControl: ['',[Validators.required]],
      ProcurementPlanYearControl: ['',[Validators.required]],
      ProcurementPlanMonthControl: ['',[Validators.required]],
      productCategoryControl: ['',[Validators.required]]
    });
    this.SetProcurementPlanYear();
    this.SetProcurementPlanMonth(1);
  }
  
  //hàm tắt pop-up khi không click vào pop-up
  onNoClick(): void {
    this.dialogRef.close();
  }
  
  //hàm tạo mới đối tượng Budget
  createBudget() : void {
    
    if (!this.CreateProcurementPlanForm.valid) {
      Object.keys(this.CreateProcurementPlanForm.controls).forEach(key => {
        if (!this.CreateProcurementPlanForm.controls[key].valid ) {
          this.CreateProcurementPlanForm.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {
      this.BudgetRequest.CreatedById = this.userId;
      this.BudgetRequest.CreatedDate = new Date();
     
      this.budgetService.createBudget(this.BudgetRequest).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
      };
      },
      error => {
        let result = <any>error;
      });
      this.dialogRef.close(true);
      
    }
  }

  
  //hàm set options cho ProcurementYear lúc vào trang
  SetProcurementPlanYear() : void {
    var minYear : number  = this.CurrentDate.getFullYear();
    var i : number = 0;
    for(i=0;i<100;i++){
      this.listYear[i] = minYear + i ;
    }
  }

  //hàm set options cho tháng dự toán lúc vào trang
  SetProcurementPlanMonth(startMonth : number) : void {
    this.listMonth = [];
    var i : number = 0;
    while(startMonth<=12){
      this.listMonth[i]=startMonth ;
      startMonth++;
      i++;
    }
  }

  //nếu chọn tháng < tháng hiện tại thì chỉ được chọn những năm tiếp theo
  setListMonth(event: MatSelectChange){
    if(event.value == this.CurrentDate.getFullYear()){
      this.SetProcurementPlanMonth(this.CurrentDate.getMonth()+1);
    }else{
      this.SetProcurementPlanMonth(1);
    }
  }
  //nếu chọn năm nay thì chỉ chọn được những tháng > tháng hiện tại
  setListYear(event: MatSelectChange){
    if(event.value < (this.CurrentDate.getMonth()+1)){
      this.listYear.splice(0,1);
    }else{
      this.SetProcurementPlanYear();
    }
  }

  openProductCategoryPopup(){
    this.dialogDisplayProductCategory = this.dialog.open(ProductCategoryPopupComponent,
      {
        width: '550px',
        height: '443px',
        autoFocus: false,
        data: { selectedproductCategoryId: this.BudgetRequest.ProductCategoryId, selectedName: this.ProductCategoryName }
      });

    this.dialogDisplayProductCategory.afterClosed().subscribe(result => {
      if(result != undefined && result != null){
        if (result.selectedproductCategoryId || result.selectedName) {
          this.ProductCategoryName = result.selectedName;
          this.BudgetRequest.ProductCategoryId = result.selectedproductCategoryId;
        }
      }
    }, error => { });
  }
}
