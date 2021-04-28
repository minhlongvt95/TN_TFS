import { Component, OnInit, HostListener, ElementRef } from '@angular/core';

import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BudgetListService } from '../../../services/budget-list.service';
import { BudgetRequestModel } from '../../../models/budget-request.model';
import { PopupComponent } from '../../../../shared/components/popup/popup.component';
import { BudgetCreateService } from '../../../services/budget-create.service';
import * as $ from 'jquery';
import { SuccessComponent } from '../../../../shared/toast/success/success.component';
import { ProductCategoryPopupComponent } from '../product-category-popup/product-category-popup.component';

@Component({
  selector: 'app-budget-detail',
  templateUrl: './budget-detail.component.html',
  styleUrls: ['./budget-detail.component.css']
})
export class BudgetDetailComponent implements OnInit {
  @HostListener('document:keyup', ['$event'])
	handleDeleteKeyboardEvent(event: KeyboardEvent) {
		if (event.key === 'Enter') {
		}
	}

  dialogDisplayProductCategory: MatDialogRef<ProductCategoryPopupComponent>;
  ProductCategoryName: string = "";
  //lấy ngày giờ hiện tại cho label
  CurrentDate : Date = new Date();
  
  //hiển thị các button theo option
  back : boolean = true ;
  edit : boolean = true ;
 
  successConfig: MatSnackBarConfig = { panelClass: 'success-dialog', horizontalPosition: 'end', duration: 5000 };
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  
  // Khai báo chung
	auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = JSON.parse(localStorage.getItem("auth")).UserId;
  procurementPlanId : string = "" ;
  BudgetModel : any ;
  dialogPopup: MatDialogRef<PopupComponent>;
  
  
  // Khai báo các Form Group
  procurementPlanDetail: FormGroup;

  //khai báo danh sách option trong autocomplete
  listYear : Array<any> = [];
  listMonth : Array<any> = [];
  
  //đối tượng truyền lên Controller
  BudgetRequest : BudgetRequestModel = new BudgetRequestModel();

  constructor(
    public snackBar: MatSnackBar,
		private router: Router,
		public dialogPop: MatDialog,
		private route: ActivatedRoute,
		public dialog: MatDialog,
    private formBuilder: FormBuilder,
    private budgetService : BudgetListService,
    private createBudgetService:BudgetCreateService,
    private el: ElementRef) { }

  ngOnInit() {
    this.procurementPlanDetail = this.formBuilder.group({
			ProcurementPlanCodeControl: [''],
      ProcurementPlanContentControl: ['',[Validators.required]],
      ProcurementPlanAmountControl: ['',[Validators.required,Validators.pattern('[\\d,]*')]],
      ProcurementPlanYearControl: ['',[Validators.required]],
      ProcurementPlanMonthControl: ['',[Validators.required]],
      productCategoryControl: ['',[Validators.required]]
    });
    this.controlManagement(true);
    this.route.params.subscribe(params => { this.procurementPlanId = params['procurementPlanId']; });
    this.getProcurementPlanById(this.procurementPlanId);
  }

   getProcurementPlanById(id : string)  {
     var result: any = this.budgetService.getBudgetById(id);

     this.budgetService.getBudgetById(id).subscribe(response => {
       let result = <any>response;
       this.BudgetRequest.ProcurementPlanId = result.procurementPlan.procurementPlanId;
       this.BudgetRequest.ProcurementPlanCode = result.procurementPlan.procurementPlanCode;
       this.BudgetRequest.ProcurementContent = result.procurementPlan.procurementContent;
       this.BudgetRequest.ProcurementAmount = result.procurementPlan.procurementAmount;
       this.BudgetRequest.ProcurementMonth = result.procurementPlan.procurementMonth;
       this.BudgetRequest.ProcurementYear = result.procurementPlan.procurementYear;
       this.BudgetRequest.ProductCategoryId = result.procurementPlan.productCategoryId;
       this.BudgetRequest.Active = result.procurementPlan.active;
       this.BudgetRequest.CreatedById = result.procurementPlan.createdById;
       this.BudgetRequest.CreatedDate = result.procurementPlan.createdDate;
       this.BudgetRequest.UpdatedById = result.procurementPlan.updatedById;
       this.BudgetRequest.UpdatedDate = result.procurementPlan.updatedDate;
       this.ProductCategoryName = result.procurementPlan.productCategoryName;
     }, error => { });
  }

  //ham quay tro lai list budget
  goBack():void{
    if(this.procurementPlanDetail.dirty){
			let _title = "XÁC NHẬN";
			let _content = "Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu.";
			this.dialogPopup = this.dialogPop.open(PopupComponent,
			{
				width: '500px',
				height: '250px',
				autoFocus: false,
				data: { title: _title, content: _content }
			});

			this.dialogPopup.afterClosed().subscribe(result => {
				if (result) {
					this.router.navigate(['/accounting/budget-list']);
				}
			});
		}else{
			this.router.navigate(['/accounting/budget-list']);
		}
  }
  
  editButtonClicked():void{
    this.back  = false ;
    this.edit  = false ;
    this.controlManagement(false);
  }
  cancel():void{
    this.back  = true ;
    this.edit  = true ;
    
    if(this.procurementPlanDetail.dirty){
			let _title = "XÁC NHẬN";
			let _content = "Bạn có chắc chắn hủy? Các dữ liệu sẽ không được lưu.";
			this.dialogPopup = this.dialogPop.open(PopupComponent,
			{
				width: '500px',
				height: '250px',
				autoFocus: false,
				data: { title: _title, content: _content }
			});

			this.dialogPopup.afterClosed().subscribe(result => {
				if (result) {
         this.controlManagement(true);
          this.getProcurementPlanById(this.procurementPlanId);
				}
			});
    } else {
      this.controlManagement(true);
      
		}
  }
  controlManagement(isViewModel : boolean){
    if(isViewModel){
      this.procurementPlanDetail.controls['ProcurementPlanContentControl'].disable() ;
      this.procurementPlanDetail.controls['ProcurementPlanAmountControl'].disable() ;
      this.procurementPlanDetail.controls['ProcurementPlanYearControl'].disable() ;
      this.procurementPlanDetail.controls['ProcurementPlanMonthControl'].disable() ;
		
		}else{
			this.procurementPlanDetail.controls['ProcurementPlanContentControl'].enable() ;
      this.procurementPlanDetail.controls['ProcurementPlanAmountControl'].enable() ;
		}
  }
  saveBudget(): void {
    if (!this.procurementPlanDetail.valid) {
      Object.keys(this.procurementPlanDetail.controls).forEach(key => {
        if (!this.procurementPlanDetail.controls[key].valid ) {
          this.procurementPlanDetail.controls[key].markAsTouched();
        }
      });

      let target;

      target = this.el.nativeElement.querySelector('.form-control.ng-invalid');

      if (target) {
        $('html,body').animate({ scrollTop: $(target).offset().top }, 'slow');
        target.focus();
      }
    } else {

      this.createBudgetService.editBudgetById(this.BudgetRequest).subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.back=true;
        this.edit=true;
        this.controlManagement(true);
        this.getProcurementPlanById(this.procurementPlanId);
        this.snackBar.openFromComponent(SuccessComponent, { data: result.messageCode, ...this.successConfig });
      };
      },
      error => {
        let result = <any>error;
      });
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
