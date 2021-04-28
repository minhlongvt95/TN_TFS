import { Component, OnInit, ViewChild } from '@angular/core';

import { MatTableDataSource,  } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';

import { BudgetCreatePopupComponent } from '../budget-create-popup/budget-create-popup.component';
import { FormGroup, FormControl } from '@angular/forms';
import { BudgetListService } from '../../../services/budget-list.service';
import { BudgetRequestModel } from '../../../models/budget-request.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-budget-list',
  templateUrl: './budget-list.component.html',
  styleUrls: ['./budget-list.component.css']
})
export class BudgetListComponent implements OnInit {
  displayedColumns: string[] = ['DateCreate', 'EstimateCode', 'Content', 'AmountMoney','YearEstimate','MonthEstimate'];
  BudgetCreateDialog: MatDialogRef<BudgetCreatePopupComponent>;
  
  //lấy ngày giờ hiện tại 
  CurrentDate : Date = new Date();
  ProcurementYear : string = this.CurrentDate.getFullYear().toString();
  ProcurementMonth : string = (this.CurrentDate.getMonth() + 1).toString() ;

  //khai bao cac form group , form control
  searchBudgetForm : FormGroup ;
  ProcurementYearControl : FormControl ;
  ProcurementMonthControl : FormControl ;

  //khai báo mảng dữ liệu hiển thị trong bảng
  listAllBudget : Array<any> = [];
  dataSourcelistAllBudget : MatTableDataSource<BudgetRequestModel> ;

   //khai báo danh sách option trong autocomplete
   listYear : Array<any> = [];
   listMonth : Array<any> = [];


  constructor(
    public dialog: MatDialog,
    private budgetListService : BudgetListService,
    private router: Router
  ) { }
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  //hàm được chạy khi load trang
  ngOnInit() {
    this.ProcurementYearControl = new FormControl('');
    this.ProcurementMonthControl = new FormControl('');

    this.searchBudgetForm = new FormGroup({
      ProcurementYearControl : this.ProcurementYearControl,
      ProcurementMonthControl : this.ProcurementMonthControl
    });

    this.getAllBudget();
    this.SetProcurementPlanMonth();
    this.SetProcurementPlanYear();
  }
  
  //mở dialog tạo dự toán mới 
  openBudgetCreateDialog() : void {
    this.BudgetCreateDialog = this.dialog.open(BudgetCreatePopupComponent,
      {
        width: '550px',
        autoFocus: false,
        data: {}
      });

    this.BudgetCreateDialog.afterClosed().subscribe(result => {
      if (result == true) {
        this.getAllBudget();
        this.sort('createdDate');
      }
    });
  }

  //hàm làm mới bộ lọc
  refreshParameter(): void {
    this.ProcurementYear = new Date().getFullYear().toString();
    this.ProcurementMonth = (new Date().getMonth() + 1).toString();
  }

  async searchBudget() {
    var result : any = await this.budgetListService.searchBudget(this.ProcurementYear,this.ProcurementMonth);
    this.listAllBudget = result.procurementPlanList ;
    this.dataSourcelistAllBudget = new MatTableDataSource<BudgetRequestModel>(this.listAllBudget);
    this.dataSourcelistAllBudget.paginator = this.paginator ;
    this.sort('createdDate');
  }

  async getAllBudget() {
    var result : any = await this.budgetListService.getAllBudget();
    this.listAllBudget = result.procurementPlanList ;
    this.dataSourcelistAllBudget = new MatTableDataSource<BudgetRequestModel>(this.listAllBudget);
    this.dataSourcelistAllBudget.paginator = this.paginator ;
    this.sort('createdDate');
  }

  //hàm set options cho ProcurementYear
  SetProcurementPlanYear() : void {
    var minYear : number  = this.CurrentDate.getFullYear();
    var i : number = 0;
    for(i=0;i<100;i++){
      this.listYear[i] = minYear + i ;
    }
  }

  //hàm set options cho tháng dự toán
  SetProcurementPlanMonth() : void {
    var i : number = 0;
    for(i=0;i<12;i++){
      this.listMonth[i] = i+1 ;
    }
  }
 
  isAscending: boolean = false;
  sort(property: string) {
    this.isAscending = !this.isAscending;
    const value = this.isAscending;
    this.listAllBudget.sort((a: any, b: any) => {
      let x: any = '';
      let y: any = '';
      switch (property) {
        case 'createdDate':
          x = b.createdDate.toLowerCase().trim();
          y = a.createdDate.toLowerCase().trim();
          break;
        case 'procurementPlanCode':
          x = a.procurementPlanCode.toLowerCase().trim();
          y = b.procurementPlanCode.toLowerCase().trim();
          break;
        case 'procurementContent':
          x = a.procurementContent ===null ? "" : a.procurementContent.toLowerCase().trim();
          y = b.procurementContent ===null ? "" : b.procurementContent.toLowerCase().trim();
          break;
        case 'procurementAmount':
          x = a.procurementAmount;
          y = b.procurementAmount;
          break;
        case 'procurementYear':
          x = a.procurementYear;
          y = b.procurementYear;
          break;
        case 'procurementMonth':
          x = a.procurementMonth;
          y = b.procurementMonth;
          break;
        case 'updatedDate' :
        x = b.updatedDate === null ? "0" : b.updatedDate.toLowerCase().trim();
        y = a.updatedDate=== null ? "0" : a.updatedDate.toLowerCase().trim();
        break;
        
        default:
          break;
      }
      if (property === 'procurementAmount') return (value ? x-y : y-x);
      if (property === 'procurementYear') return (value ? x-y : y-x);
      if (property === 'procurementMonth') return (value ? x-y : y-x);

      return (value ? (x.localeCompare(y) === -1 ? -1 : 1) : (x.localeCompare(y) > -1 ? -1 : 1));
    });
    this.dataSourcelistAllBudget = new MatTableDataSource<BudgetRequestModel>(this.listAllBudget);
    
  }
  goToProcurementPlan(id: string){
    this.router.navigate(['/accounting/budget-detail', {procurementPlanId: id}]);
  }

}
