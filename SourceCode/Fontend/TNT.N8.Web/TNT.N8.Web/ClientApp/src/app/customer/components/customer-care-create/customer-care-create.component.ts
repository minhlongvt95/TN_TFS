import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, FormControl, ValidatorFn } from '@angular/forms';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
// import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';

import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { Router, ActivatedRoute } from '@angular/router';
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

//ADAPTER Date: dd/mm/yyyy
import { AppDateAdapter, APP_DATE_FORMATS } from '../../../shared/adapter/date.adapter';

//MODELS
import { CustomerCareModel } from '../../models/customer-care.model';
import { CustomerCareFeedBack } from '../../models/customer-care-feed-back.model';

//SEVICES
import { CustomerService } from "../../services/customer.service";
import { EmployeeService } from "../../../employee/services/employee.service";
import { CategoryService } from '../../../shared/services/category.service';
import { ProductService } from '../../../product/services/product.service';
import { QuoteService } from '../../services/quote.service';
import { CustomerCareService } from '../../services/customer-care.service';

//DIALOG COMPONENT
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { TemplateEmailDialogComponent } from '../template-email-dialog/template-email-dialog.component';
import { TemplateSmsDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';
import { GetPermission } from '../../../shared/permission/get-permission';
import { WarningComponent } from '../../../shared/toast/warning/warning.component';

import { MessageService } from 'primeng/api';

const ELEMENT_CRITERIA = [
  {
    criteriaId: 1,
    criteriaName: 'Loại khách hàng',
    field: 'CustomerType'
  },
  {
    criteriaId: 2,
    criteriaName: 'Độ tuổi',
    field: 'Age'
  },
  {
    criteriaId: 3,
    criteriaName: 'Giới tính',
    field: 'Gender'
  },
  // {
  //   criteriaId: 4,
  //   criteriaName: 'Doanh thu',
  //   field: 'TotalSaleValue'
  // },
  {
    criteriaId: 5,
    criteriaName: 'Sản phẩm',
    field: 'ProductId'
  },
  // {
  //   criteriaId: 6,
  //   criteriaName: 'Số điểm hiện có',
  //   field: 'Point'
  // },
  {
    criteriaId: 7,
    criteriaName: 'Ngày sinh nhật',
    field: 'BirthDay'
  },
  {
    criteriaId: 8,
    criteriaName: 'Nhóm khách hàng',
    field: 'CustomerGroupId'
  },
  // {
  //   criteriaId: 9,
  //   criteriaName: 'Ngày tạo',
  //   field: 'CreatedDate'
  // },
];

const ELEMENT_CONDITION_GROUP = [
  {
    conditionId: 1,
    conditionName: 'Bằng',
    criteriaId: 1,
    operator: '='
  },
  {
    conditionId: 2,
    conditionName: 'Không bằng',
    criteriaId: 1,
    operator: '!='
  }
];

const ELEMENT_CONDITION_AGE = [
  {
    conditionId: 3,
    conditionName: 'Bằng',
    criteriaId: 2,
    operator: '='
  },
  {
    conditionId: 4,
    conditionName: 'Không bằng',
    criteriaId: 2,
    operator: '!='
  },
  {
    conditionId: 5,
    conditionName: 'Lớn hơn',
    criteriaId: 2,
    operator: '>'
  },
  {
    conditionId: 6,
    conditionName: 'Nhỏ hơn',
    criteriaId: 2,
    operator: '<'
  },
  {
    conditionId: 7,
    conditionName: 'Trong khoảng',
    criteriaId: 2,
    operator: '<>'
  }
];

const ELEMENT_CONDITION_SEX = [
  {
    conditionId: 8,
    conditionName: 'Bằng',
    criteriaId: 3,
    operator: '='
  },
  {
    conditionId: 9,
    conditionName: 'Không bằng',
    criteriaId: 3,
    operator: '!='
  }
];

const ELEMENT_CONDITION_REVENUE = [
  {
    conditionId: 10,
    conditionName: 'Bằng',
    criteriaId: 4,
    operator: '='
  },
  {
    conditionId: 11,
    conditionName: 'Không bằng',
    criteriaId: 4,
    operator: '!='
  },
  {
    conditionId: 12,
    conditionName: 'Lớn hơn',
    criteriaId: 4,
    operator: '>'
  },
  {
    conditionId: 13,
    conditionName: 'Nhỏ hơn',
    criteriaId: 4,
    operator: '<'
  },
  {
    conditionId: 14,
    conditionName: 'Trong khoảng',
    criteriaId: 4,
    operator: '<>'
  },
  // {
  //   conditionId: 15,
  //   conditionName: 'Phát sinh trong khoảng',
  //   criteriaId: 4,
  //   operator: '<>'
  // },
  // {
  //   conditionId: 16,
  //   conditionName: 'Không phát sinh trong khoảng',
  //   criteriaId: 4,
  //   operator: '<>'
  // }
];

const ELEMENT_CONDITION_PRODUCT = [
  {
    conditionId: 17,
    conditionName: 'Bao gồm',
    criteriaId: 5,
    operator: 'in'
  }
];

const ELEMENT_CONDITION_POINT = [
  {
    conditionId: 18,
    conditionName: 'Bằng',
    criteriaId: 6,
    operator: '='
  },
  {
    conditionId: 19,
    conditionName: 'Không bằng',
    criteriaId: 6,
    operator: '!='
  },
  {
    conditionId: 20,
    conditionName: 'Lớn hơn',
    criteriaId: 6,
    operator: '>'
  },
  {
    conditionId: 21,
    conditionName: 'Nhỏ hơn',
    criteriaId: 6,
    operator: '<'
  },
  {
    conditionId: 22,
    conditionName: 'Trong khoảng',
    criteriaId: 6,
    operator: '<>'
  }
];

const ELEMENT_CONDITION_BIRTHDAY = [
  {
    conditionId: 23,
    conditionName: 'Trong tháng',
    criteriaId: 7,
    operator: '='
  }
];

const ELEMENT_CONDITION_CUSTOMER_GROUP = [
  {
    conditionId: 24,
    conditionName: 'Bằng',
    criteriaId: 8,
    operator: '='
  }
];

const ELEMENT_CONDITION_CREATEDDATE = [
  {
    conditionId: 25,
    conditionName: 'Trong ngày',
    criteriaId: 9,
    operator: '='
  }
];

const ELEMENT_ALL_CONDITIONS = [
  {
    conditionId: 1,
    conditionName: 'Bằng',
    criteriaId: 1,
    operator: '='
  },
  {
    conditionId: 2,
    conditionName: 'Không bằng',
    criteriaId: 1,
    operator: '!='
  },
  {
    conditionId: 3,
    conditionName: 'Bằng',
    criteriaId: 2,
    operator: '='
  },
  {
    conditionId: 4,
    conditionName: 'Không bằng',
    criteriaId: 2,
    operator: '!='
  },
  {
    conditionId: 5,
    conditionName: 'Lớn hơn',
    criteriaId: 2,
    operator: '>'
  },
  {
    conditionId: 6,
    conditionName: 'Nhỏ hơn',
    criteriaId: 2,
    operator: '<'
  },
  {
    conditionId: 7,
    conditionName: 'Trong khoảng',
    criteriaId: 2,
    operator: '<>'
  },
  {
    conditionId: 8,
    conditionName: 'Bằng',
    criteriaId: 3,
    operator: '='
  },
  {
    conditionId: 9,
    conditionName: 'Không bằng',
    criteriaId: 3,
    operator: '!='
  },
  {
    conditionId: 10,
    conditionName: 'Bằng',
    criteriaId: 4,
    operator: '='
  },
  {
    conditionId: 11,
    conditionName: 'Không bằng',
    criteriaId: 4,
    operator: '!='
  },
  {
    conditionId: 12,
    conditionName: 'Lớn hơn',
    criteriaId: 4,
    operator: '>'
  },
  {
    conditionId: 13,
    conditionName: 'Nhỏ hơn',
    criteriaId: 4,
    operator: '<'
  },
  {
    conditionId: 14,
    conditionName: 'Trong khoảng',
    criteriaId: 4,
    operator: '<>'
  },
  {
    conditionId: 15,
    conditionName: 'Phát sinh trong khoảng',
    criteriaId: 4,
    operator: '<<>>'
  },
  {
    conditionId: 16,
    conditionName: 'Không phát sinh trong khoảng',
    criteriaId: 4,
    operator: '><'
  },
  {
    conditionId: 17,
    conditionName: 'Bao gồm',
    criteriaId: 5,
    operator: 'in'
  },
  {
    conditionId: 18,
    conditionName: 'Bằng',
    criteriaId: 6,
    operator: '='
  },
  {
    conditionId: 19,
    conditionName: 'Không bằng',
    criteriaId: 6,
    operator: '!='
  },
  {
    conditionId: 20,
    conditionName: 'Lớn hơn',
    criteriaId: 6,
    operator: '>'
  },
  {
    conditionId: 21,
    conditionName: 'Nhỏ hơn',
    criteriaId: 6,
    operator: '<'
  },
  {
    conditionId: 22,
    conditionName: 'Trong khoảng',
    criteriaId: 6,
    operator: '<>'
  },
  {
    conditionId: 23,
    conditionName: 'Trong tháng',
    criteriaId: 7,
    operator: '='
  },
  {
    conditionId: 24,
    conditionName: 'Bằng',
    criteriaId: 8,
    operator: '='
  },
  // {
  //   conditionId: 25,
  //   conditionName: 'Trong ngày',
  //   criteriaId: 9,
  //   operator: '='
  // },
];

const ELEMENT_VALUE_GROUP = [
  {
    valueGroupId: 1,
    valueGroupName: "Doanh nghiệp"
  },
  {
    valueGroupId: 2,
    valueGroupName: "Cá nhân"
  },
  // {
  //   valueGroupId: 3,
  //   valueGroupName: "Hộ kinh doanh cá thể"
  // }
];

const ELEMENT_VALUE_SEX = [
  {
    valueSexId: "NAM",
    valueSexName: "Nam"
  },
  {
    valueSexId: "NU",
    valueSexName: "Nữ"
  }
];

const ELEMENT_VALUE_BIRTHDAY = [
  {
    valueBirthDayId: 1,
    valueBirthDayName: "1"
  },
  {
    valueBirthDayId: 2,
    valueBirthDayName: "2"
  },
  {
    valueBirthDayId: 3,
    valueBirthDayName: "3"
  },
  {
    valueBirthDayId: 4,
    valueBirthDayName: "4"
  },
  {
    valueBirthDayId: 5,
    valueBirthDayName: "5"
  },
  {
    valueBirthDayId: 6,
    valueBirthDayName: "6"
  },
  {
    valueBirthDayId: 7,
    valueBirthDayName: "7"
  },
  {
    valueBirthDayId: 8,
    valueBirthDayName: "8"
  },
  {
    valueBirthDayId: 9,
    valueBirthDayName: "9"
  },
  {
    valueBirthDayId: 10,
    valueBirthDayName: "10"
  },
  {
    valueBirthDayId: 11,
    valueBirthDayName: "11"
  },
  {
    valueBirthDayId: 12,
    valueBirthDayName: "12"
  }
];

const ELEMENT_CUSTOMER_PER = [
  {
    categoryId: 0,
    categoryName: "-- Không chọn --"
  },
  {
    categoryId: 2,
    categoryName: "Khách hàng cá nhân"
  }
];

const ELEMENT_CUSTOMER_ENTERPRISE = [
  {
    categoryId: 0,
    categoryName: "-- Không chọn --"
  },
  {
    categoryId: 1,
    categoryName: "Khách hàng doanh nghiệp"
  }
];

const ELEMENT_CUSTOMER_TYPE = [
  {
    categoryId: 1,
    categoryName: "Khách hàng doanh nghiệp"
  },
  {
    categoryId: 2,
    categoryName: "Khách hàng cá nhân"
  }
];

@Component({
  selector: 'app-customer-care-create',
  templateUrl: './customer-care-create.component.html',
  styleUrls: ['./customer-care-create.component.css'],
  providers: [
    {
      provide: DateAdapter, useClass: AppDateAdapter
    },
    {
      provide: MAT_DATE_FORMATS, useValue: APP_DATE_FORMATS
    }
  ]
})
export class CustomerCareCreateComponent implements OnInit {
  // Loading config
  @ViewChild('ngxLoading') ngxLoadingComponent: NgxLoadingComponent;
  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  loadingConfig: any = {
    'animationType': ngxLoadingAnimationTypes.circle,
    'backdropBackgroundColour': 'rgba(0,0,0,0.1)',
    'backdropBorderRadius': '4px',
    'primaryColour': '#ffffff',
    'secondaryColour': '#999999',
    'tertiaryColour': '#ffffff'
  }
  loading: boolean = false;

  isManager: boolean = null;
  employeeId: string = '00000000-0000-0000-0000-000000000000';
  auth: any = JSON.parse(localStorage.getItem('auth'));
  userId = this.auth.UserId;
  step: number = 1;

  isSkip: boolean = false;

  /* Action*/
  actionAdd: boolean = true;
  /*END*/

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /*Hiển thị lại một list bộ lọc đã được lưu*/
  reShowListFilters: boolean = false;
  dataTest: Array<any> = [
    {criteriaId: 1, conditionId: 1, valueGroupId: 2},
    {criteriaId: 2, conditionId: 7, valueAgeFirst: 22, valueAgeLast: 60},
    {criteriaId: 3, conditionId: 8, valueSexId: "NAM"},
    {criteriaId: 4, conditionId: 14, valueRevenueFirst: 19000000, valueRevenueLast: 50000000},
    {
      criteriaId: 5,
      conditionId: 17,
      listProducts: [
        {productId: 'd897b3e9-8a58-4abc-9c46-60e01f58d0e4', productName: "Khởi nghiệp sáng tạo"}, 
        {productId: '0f60c80a-e9e9-4f16-a121-8e0531e69849', productName: "Laptop"}
      ]
    },
    {criteriaId: 6, conditionId: 19, valuePoint: 5000},
    {criteriaId: 4, conditionId: 15, valueDateRevenueFirst: new Date(), valueDateRevenueLast: new Date("09/01/2019")}
  ];
  /*End*/

  filterForm: FormGroup;
  rowFilterList: FormArray;

  /*Popup*/
  dialogPopup: MatDialogRef<PopupComponent>;
  /*End*/
  
  /*Flag show/hidden form controls for value*/
  listConditionForm: Array<number> = [];
  listValueAgeForm: Array<number> = [];
  listValueGroupForm: Array<number> = [];
  listValueAgeComboForm: Array<number> = [];
  listValueSexForm: Array<number> = [];
  listValueRevenueForm: Array<number> = [];
  listValueRevenueComboForm: Array<number> = [];
  listValueDateRevenueComboForm: Array<number> = [];
  listValueProductForm: Array<number> = [];
  listValuePointForm: Array<number> = [];
  listValuePointComboForm: Array<number> = [];
  listValueBirthDayForm: Array<number> = [];
  listValueCustomerGroupForm: Array<number> = [];
  /*End*/

  listParentCriteria: Array<any> = [];  //Loại khách hàng
  listCriteria: Array<any> = [];

  listParentCondition: Array<any> = [];
  listCondition: Array<any> = [];

  listParentGroup: Array<any> = [];
  listGroup: Array<any> = [];

  listParentSex: Array<any> = [];
  listSex: Array<any> = [];

  //setting for filter: product
  listDropdownList: Array<any> = [];
  listProducts: Array<any> = [];
  selectedItems: Array<any> = [];
  dropdownSettings = {
    singleSelection: false,
    idField: 'productId',
    textField: 'productName',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    allowSearchFilter: true
  };

  listParentBirthDay: Array<any> = [];
  listBirthDay: Array<any> = [];

  listParentCustomerGroup: Array<any> = []; //Nhóm khách hàng
  listCustomerGroup: Array<any> = [];

  //data
  listFilterModel: Array<any> = [];

  //total of index
  index: number = 0;

  /*RESULT FILTERED CUSTOMER*/
  resultFilteredCustomer: number = 0;
  listDataUser: Array<any> = [];
  displayedColumns: string[] = ['action', 'customerName', 'dateOfBirth', 'customerEmail', 'customerPhone', 'picName'];
  dataSource = new MatTableDataSource(this.listDataUser);
  pageSizeOptions = [5, 10, 15, 100];
  masterCheck: boolean = true;
  isNoData: number = 1;
  msgNoData: number = 1;

  sort: any;
  paginator: any;
  @ViewChild(MatSort) set matSort(ms: MatSort) {
    this.sort = ms;
    this.setDataSourceAttributes();
  }

  @ViewChild(MatPaginator) set matPaginator(mp: MatPaginator) {
    this.paginator = mp;
    this.setDataSourceAttributes();
  }

  setDataSourceAttributes() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  /*END*/

  /*TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/
  name: string = "{{name}}";
  hotline: string = "{{hotline}}";
  address: string = "{{address}}";

  listCustomerId: Array<string> = [];

  //Config editor
  editorConfig: any = {
    'editable': true,
    'height': '220px',
    'minHeight': '220px',
    'width': 'auto',
    'minWidth': '0',
    'translate': 'yes',
    'enableToolbar': true,
    'showToolbar': true,
    'toolbar': [
      ['bold', 'italic', 'underline'],
      ['fontName', 'fontSize', 'color'],
      ['justifyLeft', 'justifyCenter', 'justifyRight'],
      ['link', 'unlink'],
    ]
  };

  customerCareModel = new CustomerCareModel();

  listEmployeeCharge: Array<any> = [];
  listCustomerCareContactType: Array<any> = [];
  listStatusId: Array<any> = [];
  listGiftTypeId1: Array<any> = [];
  listGiftTypeId2: Array<any> = [];
  list_customer_types: Array<any> = [];

  filteredSeller: Observable<string[]>; //list nhân phụ trách

  isSendSMS: boolean = false;
  isSendEmail: boolean = false;
  isSendGift: boolean = false;
  isSendPhone: boolean = false;
  isFaceToFace: boolean = false;

  isSendEmailAt: boolean = false;
  isSendSMSAt: boolean = false;

  isShowOptionsGiftType1: boolean = true;
  isShowOptionsGiftType2: boolean = true;
  isHasValueCustomerType: boolean = true; //Flag: Báo lỗi phải chọn ít nhất một loại khách hàng

  listDefaultIdCusPer: Array<any> = [];
  listDefaultIdCusEnterprise: Array<any> = [];
  SelectedTypeCustomer: Array<any> = [];
  customerType: Array<any> = [];

  contentProcessCustomerCareForm: FormGroup;
  employeeChargeControl: FormControl;
  effecttiveFromDateControl: FormControl;
  effecttiveToDateControl: FormControl;
  customerCareTitleControl: FormControl;
  customerCareContentControl: FormControl;
  customerCareContactTypeControl: FormControl;
  expectedAmountControl: FormControl;
  statusIdControl: FormControl;
  customerCareContentEmailControl: FormControl;
  sendEmailDateControl: FormControl;
  sendEmailHourControl: FormControl;
  customerCareContentSMSControl: FormControl;
  sendDateControl: FormControl;
  sendHourControl: FormControl;
  giftCustomerType1Control: FormControl;
  giftTypeId1Control: FormControl;
  giftTotal1Control: FormControl;
  giftCustomerType2Control: FormControl;
  giftTypeId2Control: FormControl;
  giftTotal2Control: FormControl;

  dialogSelectTemplateEmail: MatDialogRef<TemplateEmailDialogComponent>;
  dialogSelectTemplateSMS: MatDialogRef<TemplateSmsDialogComponent>;
  /*END: TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/

  get rowFilterFormGroup() {
    return this.filterForm.get('rowFilters') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private getPermission: GetPermission,
    public dialogPop: MatDialog,
    private customerService: CustomerService,
    private employeeService: EmployeeService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private quoteService: QuoteService,
    private customerCareService: CustomerCareService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.customerType = [
      { name : 'Khách hàng', code : 'HDO' },
      { name: 'Khách hàng tiềm năng', code : 'MOI' },
    ];
    //Check permission
    let resource = "crm/customer/care-create";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.messageService.add({ severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' });
      this.router.navigate(['/home']);
    }
    else {
      let listCurrentActionResource = permission.listCurrentActionResource;
      if (listCurrentActionResource.indexOf("add") == -1) {
        this.actionAdd = false;
      }
      //Kiểm tra xem user đang đăng nhập là quản lý hay nhân viên
      this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
      this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

      this.filterForm = this.fb.group({
        rowFilters: this.fb.array([this.createFilter()])
      });

      // set rowFilterList to this field
      this.rowFilterList = this.filterForm.get('rowFilters') as FormArray;

      let ttt = this.route.params.subscribe(params => {
        let isEmptyObj = this.isEmpty(params);
        if (!isEmptyObj) {
          let ListFilter = params['ListFilter'];
          this.listFilterModel = JSON.parse(ListFilter);
          this.reShowListFilters = true;
        }
      });

      if (!this.reShowListFilters) {
        //Nếu là tạo mới bộ lọc lần đầu tiên
        this.getMasterData();
      } else {
        //Nếu là hiển thị lại bộ lọc
        this.getReShowListFilters()
      }

      /*TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/
      this.getMasterDataForContentCustomerCare();
      let expectedAmountPattern = '^([0-9][0-9]{0,9}|10000000000)$';
      let greaterThanZero = '^([1-9]|[1-9][0-9]|[1][0-9][0-9]|20[0-0])$';
      let today = new Date();

      this.employeeChargeControl = new FormControl('', [Validators.required]);
      this.effecttiveFromDateControl = new FormControl(new Date(), [Validators.required]);
      this.effecttiveToDateControl = new FormControl(new Date(today.setDate(today.getDate() + 7)), [Validators.required]);
      this.customerCareTitleControl = new FormControl('', [Validators.required]);
      this.customerCareContentControl = new FormControl(null, [Validators.required]);
      this.customerCareContactTypeControl = new FormControl('', [Validators.required]);
      this.expectedAmountControl = new FormControl('', [Validators.required, Validators.pattern(expectedAmountPattern)]);
      this.statusIdControl = new FormControl('', [Validators.required]);
      this.customerCareContentEmailControl = new FormControl('', [Validators.required]);
      this.sendEmailDateControl = new FormControl(new Date(), [Validators.required]);
      this.sendEmailHourControl = new FormControl('00:00', [Validators.required]);
      this.customerCareContentSMSControl = new FormControl('', [Validators.required]);
      this.sendDateControl = new FormControl(new Date(), [Validators.required]);
      this.sendHourControl = new FormControl('00:00', [Validators.required]);
      this.giftCustomerType1Control = new FormControl('');
      this.giftTypeId1Control = new FormControl('');
      this.giftTotal1Control = new FormControl('', [Validators.required, Validators.pattern(greaterThanZero)]);
      this.giftCustomerType2Control = new FormControl('');
      this.giftTypeId2Control = new FormControl('');
      this.giftTotal2Control = new FormControl('', [Validators.required, Validators.pattern(greaterThanZero)]);

      this.contentProcessCustomerCareForm = new FormGroup({
        employeeChargeControl: this.employeeChargeControl,
        effecttiveFromDateControl: this.effecttiveFromDateControl,
        effecttiveToDateControl: this.effecttiveToDateControl,
        customerCareTitleControl: this.customerCareTitleControl,
        customerCareContentControl: this.customerCareContentControl,
        customerCareContactTypeControl: this.customerCareContactTypeControl,
        expectedAmountControl: this.expectedAmountControl,
        customerCareContentEmailControl: this.customerCareContentEmailControl,
        sendEmailDateControl: this.sendEmailDateControl,
        sendEmailHourControl: this.sendEmailHourControl,
        customerCareContentSMSControl: this.customerCareContentSMSControl,
        sendDateControl: this.sendDateControl,
        sendHourControl: this.sendHourControl,
        giftCustomerType1Control: this.giftCustomerType1Control,
        giftTypeId1Control: this.giftTypeId1Control,
        giftTotal1Control: this.giftTotal1Control,
        giftCustomerType2Control: this.giftCustomerType2Control,
        giftTypeId2Control: this.giftTypeId2Control,
        giftTotal2Control: this.giftTotal2Control,
        statusIdControl: this.statusIdControl
      });
      /*END: TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/
    }
  }

  async getMasterData() {
    //just using for filters form

    /*Get List Default Data*/
    this.listCriteria = await ELEMENT_CRITERIA;
    this.listGroup = ELEMENT_VALUE_GROUP;
    this.listSex = ELEMENT_VALUE_SEX;
    this.listBirthDay = ELEMENT_VALUE_BIRTHDAY;
    let result_list_product: any = await this.productService.searchProductAsync('', '', [], []);
    this.listProducts = result_list_product.productList;
    let result_lst_customer_group: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("NHA");
    let lstCustomerGroup: Array<any> = result_lst_customer_group.category;
    if (lstCustomerGroup.length > 0) {
      lstCustomerGroup.forEach(item => {
        let option = {
          valueCustomerGroupId: item.categoryId,
          valueCustomerGroupName: item.categoryName
        }
        this.listCustomerGroup.push(option);
      });
    }
    /*End*/

    let filter = {
      criteriaId: null
    }
    this.listFilterModel.push(filter);
    this.listParentCriteria.push(this.listCriteria);
    this.listParentCondition.push([]);
    this.listParentGroup.push([]);
    this.listParentSex.push([]);
    this.listDropdownList.push([]);
    this.selectedItems.push([]);
    this.listParentBirthDay.push([]);
    this.listParentCustomerGroup.push([]);
  }

  async getReShowListFilters() {
    /*Get List Default Data*/
    this.listCriteria = await ELEMENT_CRITERIA;
    this.listGroup = ELEMENT_VALUE_GROUP;
    this.listSex = ELEMENT_VALUE_SEX;
    this.listBirthDay = ELEMENT_VALUE_BIRTHDAY;
    let result_list_product: any = await this.productService.searchProductAsync('', '', [], []);
    this.listProducts = result_list_product.productList;
    let result_lst_customer_group: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("NHA");
    let lstCustomerGroup: Array<any> = result_lst_customer_group.category;
    if (lstCustomerGroup.length > 0) {
      lstCustomerGroup.forEach(item => {
        let option = {
          valueCustomerGroupId: item.categoryId,
          valueCustomerGroupName: item.categoryName
        }
        this.listCustomerGroup.push(option);
      });
    }
    /*End*/

    // this.listFilterModel = JSON.parse(JSON.stringify(this.dataTest));

    this.listParentCriteria.push(this.listCriteria);
    this.listParentCondition.push([]);
    this.listParentGroup.push([]);
    this.listParentSex.push([]);
    this.listDropdownList.push([]);
    this.selectedItems.push([]);
    this.listParentBirthDay.push([]);
    this.listParentCustomerGroup.push([]);

    this.listFilterModel.forEach((item, index) => {
      this.reAddFilter(item);
    });
    this.listFilterModel.unshift([]);
    this.removeFilter(0);
  }

  async getMasterDataForContentCustomerCare() {
    //List nhân viên phụ trách
    this.employeeService.getEmployeeCareStaff(this.isManager, this.employeeId).subscribe(response => {
      let result = <any>response;
      this.listEmployeeCharge = result.employeeList.filter(e => e.active == true);
      this.customerCareModel.EmployeeCharge = this.employeeId;
      this.listEmployeeCharge.forEach(x => {
        x.employeeName = x.employeeCode + ' - ' + x.employeeName;
      })

      // set default cho nhan vien phu trach
      let emp = this.listEmployeeCharge.find(x => x.employeeId == this.employeeId);
      this.employeeChargeControl.setValue(emp);
      
      // dùng cho input của mat
      // this.filteredSeller = this.employeeChargeControl.valueChanges
      // .pipe(
      //   startWith(''),
      //   map(state => state ? this._filterSeller(state, this.listEmployeeCharge) : this.listEmployeeCharge.slice())
      // );
      // const toSelectSeller = this.listEmployeeCharge.find(c => c.employeeId == this.employeeId);
      // if (toSelectSeller != null) {
      //   this.employeeChargeControl.setValue(toSelectSeller.employeeCode + ' - ' + toSelectSeller.employeeName);
      // }
      
    }, error => { });

    //list Hình thức
    this.categoryService.getAllCategoryByCategoryTypeCode("HCS").subscribe(response => {
      let result = <any>response;
      this.listCustomerCareContactType = result.category;
      //Hình thức mặc định là Gửi email
      let defaultId = '';
      if (this.listCustomerCareContactType.length > 0) {
        defaultId = this.listCustomerCareContactType.find(item => item.categoryCode == 'Email').categoryId //Gửi email
      }
      this.customerCareModel.CustomerCareContactType = defaultId;
      this.isSendEmail = true;
      let contactType = this.listCustomerCareContactType.find(item => item.categoryCode == 'Email');
      this.customerCareContactTypeControl.setValue(contactType);
    })

    //list trạng thái
    this.categoryService.getAllCategoryByCategoryTypeCode("TCS").subscribe(response => {
      let result = <any>response;
      this.listStatusId = result.category;
      //Trạng thái mặc định là Mới tạo
      let defaultId = '';
      if (this.listStatusId.length > 0) {
        defaultId = this.listStatusId.find(item => item.categoryCode == 'New').categoryId;  //Mới tạo
      }
      this.customerCareModel.StatusId = defaultId;
      let status = this.listStatusId.find(item => item.categoryCode == 'New');
      this.statusIdControl.setValue(status);
    })

    //list loại quà
    let list_gift_results: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("LQU");
    this.listGiftTypeId1 = list_gift_results.category;
    this.listGiftTypeId2 = list_gift_results.category;
  }

  /* event khi thay doi nhan vien phu trach */
  // private _filterSeller(value: string, array: any) {
  //   return array.filter(state =>
  //     state.employeeName.toLowerCase().indexOf(value.toLowerCase()) >= 0 || state.employeeCode.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  // }

  selectSeller(event: any) {
    this.customerCareModel.EmployeeCharge = event.value.employeeId;
  }
  /*End*/

  async setDefaultValueCustomerGift() {
    this.isHasValueCustomerType = true;

    let defaultId1 = '';
    let defaultId2 = '';
    
    if (this.listGiftTypeId1.length > 0) {
      defaultId1 = this.listGiftTypeId1[0].categoryId;
      this.customerCareModel.GiftTypeId1 = defaultId1;
    }
    
    if (this.listGiftTypeId2.length > 0) {
      defaultId2 = this.listGiftTypeId2[0].categoryId;
      this.customerCareModel.GiftTypeId2 = defaultId2;
    }

    //List Loại khách hàng
    this.list_customer_types = ELEMENT_CUSTOMER_TYPE;

    //Value dafault Chọn Khách hàng cá nhân
    this.listDefaultIdCusPer = ELEMENT_CUSTOMER_PER;
    let giftType1 = this.listDefaultIdCusPer.find(x => x.categoryId != 0);
    this.customerCareModel.GiftCustomerType1 = giftType1.categoryId;
    this.giftCustomerType1Control.setValue(giftType1);

    //Value dafault Chọn Khách hàng doanh nghiệp
    this.listDefaultIdCusEnterprise = ELEMENT_CUSTOMER_ENTERPRISE;
    let giftType2 = this.listDefaultIdCusEnterprise.find(x => x.categoryId != 0);
    this.customerCareModel.GiftCustomerType2 = giftType2.categoryId;
    this.giftCustomerType2Control.setValue(giftType2);

    //Value default Số lượng quà
    this.customerCareModel.GiftTotal1 = 1;
    this.customerCareModel.GiftTotal2 = 1;

    this.isShowOptionsGiftType1 = true;
    this.isShowOptionsGiftType2 = true;
  }

  /*Bước 1:*/
  stepOne() {
    this.step = 1;
  }
  /*End*/

  /*Bước 2:*/
  stepTwo() {
    let _title = "XÁC NHẬN";
    let _content = "Bạn chắc chắn muốn tạo chương trình CSKH cho toàn bộ KH?";
    this.dialogPopup = this.dialogPop.open(PopupComponent,
      {
        width: '500px',
        height: 'auto',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogPopup.afterClosed().subscribe( async result => {
      if (result) {
        let count_filter = this.rowFilterList.length;
        if(count_filter > 0) {
          //reset toàn bộ dữ liệu đã lưu của list bộ lọc
          this.listFilterModel = [];

          //Ẩn toàn bộ form control của list bộ lọc
          this.listConditionForm = [];
          this.listValueAgeForm = [];
          this.listValueGroupForm = [];
          this.listValueAgeComboForm = [];
          this.listValueSexForm = [];
          this.listValueRevenueForm = [];
          this.listValueRevenueComboForm = [];
          this.listValueDateRevenueComboForm = [];
          this.listValueProductForm = [];
          this.listValuePointForm = [];
          this.listValuePointComboForm = [];
          this.listValueBirthDayForm = [];
          this.listValueCustomerGroupForm = [];

          this.index = 0;

          //xóa các bộ lọc
          for (let i = 1; i < count_filter; i++) {
            this.rowFilterList.removeAt(1);
          }

          //Reset lại value của dropdown list của bộ lọc
          this.rowFilterList.reset();

          this.getMasterData();

          //reset kết quả lọc
          this.listDataUser = [];
          this.resultFilteredCustomer = 0;
          this.isNoData = 1;
          this.msgNoData = 1;
          this.dataSource = new MatTableDataSource(this.listDataUser);

          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';
        }

        this.isSkip = true;
        this.step = 2;
        this.listCustomerId = [];
        /*Get Customer List*/
        const result_customer_list: any = await this.customerService.getAllCustomerAsync();
        /*END*/

        result_customer_list.customerList.forEach((item, index) => {
          this.listCustomerId.push(item.customerId);
        });
      }
    });
  }
  /*End*/

  /*Reset lại toàn bộ bộ lọc*/
  resetAllFilters() {
    let count_filter = this.rowFilterList.length;

    if(count_filter > 0) {
      let _title = "XÁC NHẬN";
      let _content = "Bạn có muốn hủy bỏ bộ lọc hiện tại?";
      this.dialogPopup = this.dialogPop.open(PopupComponent,
        {
          width: '500px',
          height: '300px',
          autoFocus: false,
          data: { title: _title, content: _content }
      });

      this.dialogPopup.afterClosed().subscribe( async result => {
        if (result) {
          //reset toàn bộ dữ liệu đã lưu của list bộ lọc
          this.listFilterModel = [];

          //Ẩn toàn bộ form control của list bộ lọc
          this.listConditionForm = [];
          this.listValueAgeForm = [];
          this.listValueGroupForm = [];
          this.listValueAgeComboForm = [];
          this.listValueSexForm = [];
          this.listValueRevenueForm = [];
          this.listValueRevenueComboForm = [];
          this.listValueDateRevenueComboForm = [];
          this.listValueProductForm = [];
          this.listValuePointForm = [];
          this.listValuePointComboForm = [];
          this.listValueBirthDayForm = [];
          this.listValueCustomerGroupForm = [];

          this.index = 0;

          //xóa các bộ lọc
          for (let i = 1; i < count_filter; i++) {
            this.rowFilterList.removeAt(1);
          }

          //Reset lại value của dropdown list của bộ lọc
          this.rowFilterList.reset();

          this.getMasterData();

          //reset nội dung CSKH
          this.resetFormContentCustomerCare();

          //reset kết quả lọc
          this.listDataUser = [];
          this.resultFilteredCustomer = 0;
          this.isNoData = 1;
          this.msgNoData = 1;
          this.dataSource = new MatTableDataSource(this.listDataUser);

          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';

          //reset tập khách hàng
          this.listCustomerId = [];
        }
      });
    }
  }
  /*End*/

  /*Tạo một bộ lọc:*/
  createFilter(): FormGroup {
    let number_require = '^[0-9][0-9]*$'; //số nguyên lơn hơn hoặc bằng 0
    return this.fb.group({
      criteriaControl: ['', Validators.compose([Validators.required])],
      conditionControl: ['', Validators.compose([Validators.required])],
      valueAgeControl: ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      'valueAgeFirstControl': ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      'valueAgeLastControl': ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      valueListGroupControl: ['', Validators.compose([Validators.required])],
      valueListSexControl: ['', Validators.compose([Validators.required])],
      valueRevenueControl: ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      'valueRevenueFirstControl': ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      'valueRevenueLastControl': ['', Validators.compose([Validators.pattern(number_require), Validators.required])],
      valueDateRevenueFirstControl: ['', Validators.compose([Validators.required])],
      valueDateRevenueLastControl: ['', Validators.compose([Validators.required])],
      valueListProductControl: ['', Validators.compose([Validators.required])],
      valuePointControl: ['', Validators.compose([Validators.required])],
      valuePointFirstControl: ['', Validators.compose([Validators.required])],
      valuePointLastControl: ['', Validators.compose([Validators.required])],
      valueListBirthDayControl: ['', Validators.compose([Validators.required])],
      valueListCustomerGroupControl: ['', Validators.compose([Validators.required])]
    },
      {
        validator: [
          this.equalValueValidator('valueAgeFirstControl', 'valueAgeLastControl'),
          this.equalValueValidator('valueRevenueFirstControl', 'valueRevenueLastControl')
        ]
      }
    );
  }
  /*End*/

  equalValueValidator(targetKey: string, toMatchKey: string): ValidatorFn {
    return (group: FormGroup): {[key: string]: any} => {
      const target = group.controls[targetKey];
      const toMatch = group.controls[toMatchKey];
      if (target.valueChanges && toMatch.valueChanges) {
        const isMatch = target.value <= toMatch.value;
        // set equal value error on dirty controls
        if (!isMatch && target.valid && toMatch.valid) {
          toMatch.setErrors({equalValue: targetKey});
          const message = "Giá trị đầu phải nhỏ hơn giá trị cuối trong khoảng";
          return {'equalValue': message};
        }
        if (isMatch && toMatch.hasError('equalValue')) {
          toMatch.setErrors(null);
        }
      }
  
      return null;
    };
  }

  getFiltersFormGroup(index): FormGroup {
    this.rowFilterList = this.filterForm.get('rowFilters') as FormArray;
    const formGroup = this.rowFilterList.controls[index] as FormGroup;
    return formGroup;
  }

  /*Thêm một hàng lọc*/
  addFilter() {
    this.index++; //Lưu lại giá trị index
    this.rowFilterList.push(this.createFilter());

    //Khởi tạo các giá trị mặc định
    this.listParentCriteria[this.index] = this.listCriteria;
    this.listParentCondition[this.index] = [];
    this.listParentGroup[this.index] = [];
    this.listParentSex[this.index] = [];
    this.listDropdownList[this.index] = [];
    this.selectedItems[this.index] = [];
    this.listParentBirthDay[this.index] = [];
    this.listParentCustomerGroup[this.index] = [];

    let filter = {
      criteriaId: null
    }
    this.listFilterModel.push(filter);
  }
  /*End*/

  /*Thêm một hàng lọc từ điều kiện hiển thị lại bộ lọc*/
  reAddFilter(option: any) {
    this.index++; //Lưu lại giá trị index
    this.rowFilterList.push(this.createFilter());

    /*Khởi tạo các giá trị mặc định*/
    this.listParentCriteria[this.index] = this.listCriteria;
    let setValueDefaultCriteria = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["criteriaControl"].setValue(option.criteriaId);
    let criteriaName = this.listCriteria.find(item => item.criteriaId == option.criteriaId).criteriaName;

    this.listParentCondition[this.index] = [];
    this.listParentGroup[this.index] = [];
    this.listParentSex[this.index] = [];
    this.listDropdownList[this.index] = [];
    this.selectedItems[this.index] = [];
    this.listParentBirthDay[this.index] = [];
    this.listParentCustomerGroup[this.index] = [];
    
    switch (criteriaName)
    {
      case "Loại khách hàng" : {
        this.listCondition = ELEMENT_CONDITION_GROUP;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;
        
        this.listParentGroup[this.index] = this.listGroup;
        let setValueDefaultGroup = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueListGroupControl"].setValue(option.valueGroupId);
        this.listValueGroupForm[this.index] = 1;
        break;
      }
      case "Độ tuổi" : {
        this.listCondition = ELEMENT_CONDITION_AGE;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;
        let conditionName = this.listParentCondition[this.index].find(item => item.conditionId == option.conditionId).conditionName;

        if (conditionName == "Trong khoảng") {
          this.listValueAgeComboForm[this.index] = 1;
          let setValueDefaultAgeFirst = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueAgeFirstControl"].setValue(option.valueAgeFirst);
          let setValueDefaultAgeLast = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueAgeLastControl"].setValue(option.valueAgeLast);
        } else {
          this.listValueAgeForm[this.index] = 1;
          let setValueDefaultAge = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueAgeControl"].setValue(option.valueAge);
        }
        break;
      }
      case "Giới tính" : {
        this.listCondition = ELEMENT_CONDITION_SEX;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;

        this.listParentSex[this.index] = this.listSex;
        let setValueDefaultSex = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueListSexControl"].setValue(option.valueSexId);
        this.listValueSexForm[this.index] = 1;
        break;
      }
      case "Doanh thu" : {
        this.listCondition = ELEMENT_CONDITION_REVENUE;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;
        let conditionName = this.listParentCondition[this.index].find(item => item.conditionId == option.conditionId).conditionName;

        if (conditionName == "Trong khoảng") {
          this.listValueRevenueComboForm[this.index] = 1;
          let setValueDefaultRevenueFirst = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueRevenueFirstControl"].setValue(option.valueRevenueFirst);
          let setValueDefaultRevenueLast = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueRevenueLastControl"].setValue(option.valueRevenueLast);
        } else if (conditionName == "Phát sinh trong khoảng" || conditionName == "Không phát sinh trong khoảng") {
          this.listValueDateRevenueComboForm[this.index] = 1;
          let setValueDefaultDateRevenueFirst = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueDateRevenueFirstControl"].setValue(option.valueDateRevenueFirst);
          let setValueDefaultDateRevenueLast = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueDateRevenueLastControl"].setValue(option.valueDateRevenueLast); 
        } else {
          this.listValueRevenueForm[this.index] = 1;
          let setValueDefaultRevenue = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueRevenueControl"].setValue(option.valueRevenue);
        }
        break;
      }
      case "Số điểm hiện có" : {
        this.listCondition = ELEMENT_CONDITION_POINT;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;
        let conditionName = this.listParentCondition[this.index].find(item => item.conditionId == option.conditionId).conditionName;

        if (conditionName == "Trong khoảng") {
          this.listValuePointComboForm[this.index] = 1;
          let setValueDefaultPointFirst = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valuePointFirstControl"].setValue(option.valuePointFirst);
          let setValueDefaultPointLast = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valuePointLastControl"].setValue(option.valuePointLast);
        } else {
          this.listValuePointForm[this.index] = 1;
          let setValueDefaultPoint = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valuePointControl"].setValue(option.valuePoint);
        }
        break;
      }
      case "Sản phẩm" : {
        this.listCondition = ELEMENT_CONDITION_PRODUCT;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;
        
        this.listDropdownList[this.index] = this.listProducts;
        this.selectedItems[this.index] = option.listProducts;
        this.listValueProductForm[this.index] = 1;
        break;
      }
      case "Ngày sinh nhật" : {
        this.listCondition = ELEMENT_CONDITION_BIRTHDAY;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;

        this.listParentBirthDay[this.index] = this.listBirthDay;
        let setValueDefaultBirthDay = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueListBirthDayControl"].setValue(option.valueBirthDayId);
        this.listValueBirthDayForm[this.index] = 1;
        break;
      }
      case "Nhóm khách hàng" : {
        this.listCondition = ELEMENT_CONDITION_CUSTOMER_GROUP;
        this.listParentCondition[this.index] = this.listCondition;
        let setValueDefaultCondition = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["conditionControl"].setValue(option.conditionId);
        this.listConditionForm[this.index] = 1;

        this.listParentCustomerGroup[this.index] = this.listCustomerGroup;
        let setValueDefaultCustomerGroup = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(this.index)).controls["valueListCustomerGroupControl"].setValue(option.valueCustomerGroupId);
        this.listValueCustomerGroupForm[this.index] = 1;
        break;
      }
      default : {
        return;
      }
    }
    /*End*/
  }
  /*End*/

  /*Event: Khi thay đổi Tiêu chí*/
  changedCriteria(index, event: any) {
    //Reset dữ liệu của drop downd list
    this.listParentCondition[index] = [];
    this.listParentGroup[index] = [];
    this.listParentSex[index] = [];
    this.listDropdownList[index] = [];
    this.selectedItems[index] = [];
    this.listParentBirthDay[index] = [];
    this.listParentCustomerGroup[index] = [];

    //Reset dữ liệu đã lưu của input
    if (this.listFilterModel[index]) {
      delete this.listFilterModel[index].conditionId;
      delete this.listFilterModel[index].valueGroupId;
      delete this.listFilterModel[index].valueAge;
      delete this.listFilterModel[index].valueAgeFirst;
      delete this.listFilterModel[index].valueAgeLast;
      delete this.listFilterModel[index].valueSexId;
      delete this.listFilterModel[index].valueRevenue;
      delete this.listFilterModel[index].valueRevenueFirst;
      delete this.listFilterModel[index].valueRevenueLast;
      delete this.listFilterModel[index].valueDateRevenueFirst;
      delete this.listFilterModel[index].valueDateRevenueLast;
      delete this.listFilterModel[index].valuePointFirst;
      delete this.listFilterModel[index].valuePointLast;
      delete this.listFilterModel[index].listProducts;
      delete this.listFilterModel[index].valueBirthDayId;
      delete this.listFilterModel[index].valueCustomerGroupId;
    }

    //Reset trạng thái show/hidden của form control
    if (this.listConditionForm[index] == 1) 
    {
      this.listConditionForm[index] = 0;
    }
    if (this.listValueGroupForm[index] == 1)
    {
      this.listValueGroupForm[index] = 0;
    }
    if (this.listValueAgeForm[index] == 1)
    {
      this.listValueAgeForm[index] = 0;
    }
    if (this.listValueAgeComboForm[index] == 1)
    {
      this.listValueAgeComboForm[index] = 0;
    }
    if (this.listValueSexForm[index] == 1)
    {
      this.listValueSexForm[index] = 0;
    }
    if (this.listValueRevenueForm[index] == 1) 
    {
      this.listValueRevenueForm[index] = 0;
    }
    if (this.listValueRevenueComboForm[index] == 1) 
    {
      this.listValueRevenueComboForm[index] = 0;
    }
    if (this.listValueDateRevenueComboForm[index] == 1) 
    {
      this.listValueDateRevenueComboForm[index] = 0;
    }
    if (this.listValueProductForm[index] == 1) 
    {
      this.listValueProductForm[index] = 0;
    }
    if (this.listValuePointForm[index] == 1) 
    {
      this.listValuePointForm[index] = 0;
    }
    if (this.listValuePointComboForm[index] == 1) 
    {
      this.listValuePointComboForm[index] = 0;
    }
    if (this.listValueBirthDayForm[index] == 1) 
    {
      this.listValueBirthDayForm[index] = 0;
    }
    if (this.listValueCustomerGroupForm[index] == 1) 
    {
      this.listValueCustomerGroupForm[index] = 0;
    }

    //Hiển thị form control: condition
    this.listConditionForm[index] = 1;
    //reset value của dropdown list
    var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["conditionControl"].setValue(null);

    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };

    //Lưu giá trị đã chọn
    let count = this.listFilterModel.length;
    if (count != 0 && index < this.index) {
      this.listFilterModel[index].criteriaId = selectedData.value;
    } else if (count != 0 && index == this.index) {
      if(this.listFilterModel[index]) {
        this.listFilterModel[index].criteriaId = selectedData.value;
      } else {
        let filter = {
          criteriaId: selectedData.value,
          conditionId: null 
        }
        this.listFilterModel.push(filter);
      }
    } else {
      let filter = {
        criteriaId: selectedData.value,
        conditionId: null
      }
      this.listFilterModel.push(filter);
    }

    //Xét các trường hợp để show form control: condition
    if (selectedData.text === 'Loại khách hàng') {
      this.listCondition = ELEMENT_CONDITION_GROUP;
    } else if (selectedData.text === 'Độ tuổi') {
      this.listCondition = ELEMENT_CONDITION_AGE;
    } else if (selectedData.text === 'Giới tính') {
      this.listCondition = ELEMENT_CONDITION_SEX;
    } else if (selectedData.text === 'Doanh thu') {
      this.listCondition = ELEMENT_CONDITION_REVENUE;
    } else if (selectedData.text === 'Sản phẩm') {
      this.listCondition = ELEMENT_CONDITION_PRODUCT;
    } else if (selectedData.text === 'Số điểm hiện có') {
      this.listCondition = ELEMENT_CONDITION_POINT;
    } else if (selectedData.text === 'Ngày sinh nhật') {
      this.listCondition = ELEMENT_CONDITION_BIRTHDAY;
    } else if (selectedData.text === 'Nhóm khách hàng') {
      this.listCondition = ELEMENT_CONDITION_CUSTOMER_GROUP;
    }

    this.listParentCondition[index] = this.listCondition;
  }
  /*End*/

  /*Event: Khi thay đổi Điều kiện*/
  changedCondition(index, event: any) {
    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };
    
    //Tìm tên của Tiêu chí trong một bộ lọc
    let criteriaId = this.listFilterModel[index].criteriaId;
    let criteriaName = this.listCriteria.find(item => item.criteriaId == criteriaId).criteriaName;
    
    //Tìm tên của Điều kiện trong một bộ lọc
    let conditionId = selectedData.value;
    let conditionName = this.listParentCondition[index].find(item => item.conditionId == conditionId).conditionName;
    
    //Lưu giá trị đã chọn
    this.listFilterModel[index].conditionId = selectedData.value;

    //Xét các trường hợp để show form control: value
    if (criteriaName == 'Loại khách hàng') {
      this.listValueGroupForm[index] = 1;
      this.listParentGroup[index] = this.listGroup;

      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueGroup(index);

      //reset value của dropdown list
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueListGroupControl"].setValue(null)
    } else if (criteriaName == 'Độ tuổi' && conditionName != 'Trong khoảng') {
      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueAge(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeControl"].setValue(null);
      
      this.listValueAgeForm[index] = 1;
    } else if (criteriaName == 'Độ tuổi' && conditionName == 'Trong khoảng') {
      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueAge(index);
      
      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeFirstControl"].setValue(null);
      var b = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeLastControl"].setValue(null);

      this.listValueAgeComboForm[index] = 1;
    } else if (criteriaName == 'Giới tính') {
      this.listValueSexForm[index] = 1;
      this.listParentSex[index] = this.listSex;

      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueSex(index);

      //reset value của dropdown list
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueListSexControl"].setValue(null)
    } else if (criteriaName == 'Doanh thu' && 
      (conditionName == 'Bằng' || conditionName == 'Không bằng' || conditionName == 'Lớn hơn' || conditionName == 'Nhỏ hơn')) {
      //reset lại value của bộ lọc
      this.resetFormValueRevenue(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueRevenueControl"].setValue(null);
      
      this.listValueRevenueForm[index] = 1;
    } else if (criteriaName == 'Doanh thu' && conditionName == 'Trong khoảng') {
      //reset lại value của bộ lọc
      this.resetFormValueRevenue(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueRevenueFirstControl"].setValue(null);
      var b = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueRevenueLastControl"].setValue(null);

      this.listValueRevenueComboForm[index] = 1;
    } else if (criteriaName == 'Doanh thu' && (conditionName == 'Phát sinh trong khoảng' || conditionName == 'Không phát sinh trong khoảng')) {
      //reset lại value của bộ lọc
      this.resetFormValueRevenue(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueDateRevenueFirstControl"].setValue(null);
      var b = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueDateRevenueLastControl"].setValue(null);

      this.listValueDateRevenueComboForm[index] = 1;
    } else if (criteriaName == 'Sản phẩm') {
      this.listValueProductForm[index] = 1;
      this.listDropdownList[index] = this.listProducts;
      //reset value của dropdown list
      this.selectedItems[index] = [];
    } else if (criteriaName == 'Số điểm hiện có' && 
      (conditionName == 'Bằng' || conditionName == 'Không bằng' || conditionName == 'Lớn hơn' || conditionName == 'Nhỏ hơn')) {
      //reset lại value của bộ lọc
      this.resetFormValuePoint(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valuePointControl"].setValue(null);
    
      this.listValuePointForm[index] = 1;
    } else if (criteriaName == 'Số điểm hiện có' && conditionName == 'Trong khoảng') {
      this.resetFormValuePoint(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valuePointFirstControl"].setValue(null);
      var b = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valuePointLastControl"].setValue(null);

      this.listValuePointComboForm[index] = 1;
    } else if (criteriaName == 'Ngày sinh nhật') {
      this.listValueBirthDayForm[index] = 1;
      this.listParentBirthDay[index] = this.listBirthDay;

      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueBirthDay(index);

      //reset value của dropdown list
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueListBirthDayControl"].setValue(null)
    } else if (criteriaName == 'Nhóm khách hàng') {
      this.listValueCustomerGroupForm[index] = 1;
      this.listParentCustomerGroup[index] = this.listCustomerGroup;

      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueCustomerGroup(index);

      //reset value của dropdown list
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueListCustomerGroupControl"].setValue(null)
    }
  }
  /*End*/

  /*Event: Khi thay đổi Điều kiện: Loại khách hàng*/
  changedlistValueListGroup(index, event: any) {
    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };

    //Lưu giá trị đã chọn
    this.listFilterModel[index].valueGroupId = selectedData.value;
  }
  /*End*/

  /*Event: Khi thay đổi giá trị của tuổi*/
  changedValueAge(index, event: any) {
    let age = Number(event.target.value);
    if (age == 0) {
      (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeControl"].setValue(0);
    }
    this.listFilterModel[index].valueAge = age;
  }

  changedValueAgeFirst(index, event: any) {
    let age_first = Number(event.target.value);
    if (age_first == 0) {
      (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeFirstControl"].setValue(0);
    }
    this.listFilterModel[index].valueAgeFirst = age_first;
  }

  changedValueAgeLast(index, event: any) {
    let age_last = Number(event.target.value);
    if (age_last == 0) {
      (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueAgeLastControl"].setValue(0);
    }
    this.listFilterModel[index].valueAgeLast = age_last;
  }
  /*End*/

  /*Event: Khi thay đổi giá trị của Giới tính*/
  changedlistValueSex(index, event: any) {
    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };

    //Lưu giá trị đã chọn
    this.listFilterModel[index].valueSexId = selectedData.value;
  }
  /*End*/

  /*Event: Khi thay đổi giá trị của Doanh thu*/
  changedValueRevenue(index, event: any) {
    let revenue = event.target.value;
    let str = '';
    if (revenue.indexOf(",") != -1) {
      let array_number = revenue.split(",");
      array_number.forEach(function (value, index) {
        str += value;
      });
      var result = Number(str);
    } else {
      if (revenue == '') {
        result = null;
      } else {
        result = Number(revenue);
      }
    }
    this.listFilterModel[index].valueRevenue = result;
  }

  changedValueRevenueFirst(index, event: any) {
    let revenue_first = (event.target.value);
    let str = '';
    if (revenue_first.indexOf(",") != -1) {
      let array_number = revenue_first.split(",");
      array_number.forEach(function (value, index) {
        str += value;
      });
      var result = Number(str);
    } else {
      if (revenue_first == '') {
        result = null;
      } else {
        result = Number(revenue_first);
      }
    }
    this.listFilterModel[index].valueRevenueFirst = result;
  }

  changedValueRevenueLast(index, event: any) {
    let revenue_last = (event.target.value);
    let str = '';
    if (revenue_last.indexOf(",") != -1) {
      let array_number = revenue_last.split(",");
      array_number.forEach(function (value, index) {
        str += value;
      });
      var result = Number(str);
    } else {
      if (revenue_last == '') {
        result = null;
      } else {
        result = Number(revenue_last);
      }
    }
    this.listFilterModel[index].valueRevenueLast = result;
  }

  setValueDateRevenueFirst(index, event: any) {
    let revenue_date_first = (event.target.value);
    this.listFilterModel[index].valueDateRevenueFirst = revenue_date_first;
  }

  setValueDateRevenueLast(index, event: any) {
    let revenue_date_last = (event.target.value);
    this.listFilterModel[index].valueDateRevenueLast = revenue_date_last;
  }
  /*End*/

  /*Event: Khi thay đổi Sản phẩm*/
  onItemSelect(index, item: any) {
    this.listFilterModel[index].listProducts = this.selectedItems[index];
  }

  onSelectAll(index, items: any) {
    this.listFilterModel[index].listProducts = items;
  }

  onItemDeSelect(index, item: any) {
    this.listFilterModel[index].listProducts = this.selectedItems[index];
  }

  onFilterChange(index, item: any) {
    //trả về ký tự nhập ở ô search
  }
  /*End*/

  /*Event: Khi thay đổi Điểm hiện có*/
  changedValuePoint(index, event: any) {
    let point = Number(event.target.value);
    this.listFilterModel[index].valuePoint = point;
  }

  changedValuePointFirst(index, event: any) {
    let point_first = Number(event.target.value);
    this.listFilterModel[index].valuePointFirst = point_first;
  }

  changedValuePointLast(index, event: any) {
    let point_last = Number(event.target.value);
    this.listFilterModel[index].valuePointLast = point_last;
  }
  /*End*/

  /*Event: Khi thay đổi giá trị của Ngày sinh nhật*/
  changedlistValueBirthDay(index, event: any) {
    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };

    //Lưu giá trị đã chọn
    this.listFilterModel[index].valueBirthDayId = selectedData.value;
  }
  /*End*/

  /*Event: Khi thay đổi giá trị của Nhóm khách hàng*/
  changedlistValueCustomerGroup(index, event: any) {
    let target = event.source.selected.viewValue;
    let selectedData = {
      value: event.value,
      text: target
    };

    //Lưu giá trị đã chọn
    this.listFilterModel[index].valueCustomerGroupId = selectedData.value;
  }
  /*End*/

  /*Xóa một row filter*/
  removeFilter(index) {
    this.rowFilterList.removeAt(index);
    this.listFilterModel.splice(index, 1);
    this.listConditionForm.splice(index, 1);
    this.listValueGroupForm.splice(index, 1);
    this.listValueAgeForm.splice(index, 1);
    this.listValueAgeComboForm.splice(index, 1);
    this.listValueSexForm.splice(index, 1);
    this.listValueRevenueForm.splice(index, 1);
    this.listValueRevenueComboForm.splice(index, 1);
    this.listValueDateRevenueComboForm.splice(index, 1);
    this.listValueProductForm.splice(index, 1);
    this.listValuePointForm.splice(index, 1);
    this.listValuePointComboForm.splice(index, 1);
    this.listValueBirthDayForm.splice(index, 1);
    this.listValueCustomerGroupForm.splice(index, 1);

    //Xóa value của dropdown list theo index
    if (this.listParentCriteria[index]) {
      this.listParentCriteria.splice(index, 1);
    }
    if (this.listParentCondition[index]) {
      this.listParentCondition.splice(index, 1);
    }
    if (this.listParentGroup[index]) {
      this.listParentGroup.splice(index, 1);
    }
    if (this.listParentSex[index]) {
      this.listParentSex.splice(index, 1);
    }
    if (this.listDropdownList[index]) {
      this.listDropdownList.splice(index, 1);
    }
    if (this.selectedItems[index]) {
      this.selectedItems.splice(index, 1);
    }
    if (this.listParentBirthDay[index]) {
      this.listParentBirthDay.splice(index, 1);
    }
    if (this.listParentCustomerGroup[index]) {
      this.listParentCustomerGroup.splice(index, 1);
    }

    this.index--;
  }
  /*End*/

  /*Function Support*/
  resetFormValueGroup(index) {
    delete this.listFilterModel[index].valueGroupId;
  }

  resetFormValueAge(index) {
    this.listValueAgeForm[index] = 0;
    delete this.listFilterModel[index].valueAge;

    this.listValueAgeComboForm[index] = 0;
    delete this.listFilterModel[index].valueAgeFirst;
    delete this.listFilterModel[index].valueAgeLast;
  }

  resetFormValueSex(index) {
    delete this.listFilterModel[index].valueSexId;
  }

  resetFormValueRevenue(index) {
    this.listValueRevenueForm[index] = 0;
    delete this.listFilterModel[index].valueRevenue;

    this.listValueRevenueComboForm[index] = 0;
    delete this.listFilterModel[index].valueRevenueFirst;
    delete this.listFilterModel[index].valueRevenueLast;

    this.listValueDateRevenueComboForm[index] = 0;
    delete this.listFilterModel[index].valueDateRevenueFirst;
    delete this.listFilterModel[index].valueDateRevenueLast;
  }

  resetFormValuePoint(index) {
    this.listValuePointForm[index] = 0;
    delete this.listFilterModel[index].valuePoint;

    this.listValuePointComboForm[index] = 0;
    delete this.listFilterModel[index].valuePointFirst;
    delete this.listFilterModel[index].valuePointLast;
  }

  resetFormValueBirthDay(index) {
    delete this.listFilterModel[index].valueBirthDayId;
  }

  resetFormValueCustomerGroup(index) {
    delete this.listFilterModel[index].valueCustomerGroupId;
  }
  /*End*/

  /*Send Filters*/
  sendFilters() {
    //Hiển thị validate trên view
    this.rowFilterList = this.filterForm.get('rowFilters') as FormArray;

    for(let i = 0; i < (this.index + 1); i++) {
      let formGroup = this.rowFilterList.controls[i] as FormGroup;
      Object.keys(formGroup.controls).forEach((key, index) => {
        if (!formGroup.controls[key].valid) {
          formGroup.controls[key].markAsTouched();
        }
      });
    }

    //Kiểm tra validate theo dữ liệu trả về
    let check = this.validateFilters();

    if (!check) {
      this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'kiếm' });
    } else {
      this.loading = true;
      this.masterCheck = true;
      /*Xử lý bộ lọc thành câu query*/
      let query: any = this.generateQuery(this.listFilterModel);
      let listTypeCustomerCode: Array<any> = [];
      this.SelectedTypeCustomer.forEach( x => {
        listTypeCustomerCode.push(x.code);
      })
      this.customerCareService.filterCustomer(this.basicRulesetToSQL(query), listTypeCustomerCode, this.userId).subscribe(response => {
        let result = <any>response;
        
        if (result.statusCode == 200) {
          this.loading = false;
          this.listDataUser = result.listCustomer;

          //Nếu có dữ liệu thì show còn không thì thông báo không có dữ liệu
          this.resultFilteredCustomer = this.listDataUser.length;
          if (this.resultFilteredCustomer > 0) {
            this.isNoData = 0;
            this.msgNoData = 0;
          } else {
            this.isNoData = 1;
            this.msgNoData = 1;
          }
          
          for (let item of this.listDataUser) {
            item.isChecked = true;
          }
          
          this.dataSource = new MatTableDataSource(this.listDataUser);

          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';
        } else {
          this.loading = false;
          this.listDataUser = [];
          this.resultFilteredCustomer = 0;
          this.isNoData = 1;
          this.msgNoData = 1;
          this.dataSource = new MatTableDataSource(this.listDataUser);

          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';

          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Lọc khách hàng thất bại' });
        }
      }, error => { });
      /*End*/
    }
  }
  /*End*/

  /*Xử lý kết quả bộ lọc để generate ra query*/
  generateQuery  (result: any) {
    let query = {
      condition: "and",
      rules: []
    }

    result.forEach((obj, index) => {
      let field = this.listCriteria.find(item => item.criteriaId == obj.criteriaId).field;
      let operator = ELEMENT_ALL_CONDITIONS.find(item => item.conditionId == obj.conditionId).operator;
      let criteriaName = this.listCriteria.find(item => item.criteriaId == obj.criteriaId).criteriaName;

      switch (criteriaName)
      {
        case "Loại khách hàng" : {
          let value = obj.valueGroupId;
          let subQuery = {
            field: field,
            operator: operator,
            value: value
          };
          query.rules.push(subQuery);
          break;
        }
        case "Độ tuổi" : {
          if (operator == "<>") {
            let value1 = obj.valueAgeFirst;
            let subQuery1 = {
              field: field,
              operator: ">=",
              value: value1
            };
            query.rules.push(subQuery1);

            let value2 = obj.valueAgeLast;
            let subQuery2 = {
              field: field,
              operator: "<=",
              value: value2
            };
            query.rules.push(subQuery2);
          } else {
            let value = obj.valueAge;
            let subQuery = {
              field: field,
              operator: operator,
              value: value
            };
            query.rules.push(subQuery);
          }
          break;
        }
        case "Giới tính" : {
          let value = obj.valueSexId;
          let subQuery = {
            field: field,
            operator: operator,
            value: value
          };
          query.rules.push(subQuery);
          break;
        }
        case "Doanh thu" : {
          if (operator == "<>") {
            //Trong khoảng
            let value1 = obj.valueRevenueFirst;
            let subQuery1 = {
              field: field,
              operator: ">=",
              value: value1
            };
            query.rules.push(subQuery1);

            let value2 = obj.valueRevenueLast;
            let subQuery2 = {
              field: field,
              operator: "<=",
              value: value2
            };
            query.rules.push(subQuery2);
          } else if (operator == "<<>>") {
            //Phát sinh trong khoảng thời gian
            let value1 = obj.valueDateRevenueFirst;
            let subQuery1 = {
              field: field,
              operator: ">=",
              value: value1
            };
            query.rules.push(subQuery1);

            let value2 = obj.valueDateRevenueLast;
            let subQuery2 = {
              field: field,
              operator: "<=",
              value: value2
            };
            query.rules.push(subQuery2);
          } else if (operator == "><") {
            //Không phát sinh trong khoảng thời gian
            let value1 = obj.valueDateRevenueFirst;
            let subQuery1 = {
              field: field,
              operator: "<=",
              value: value1
            };
            query.rules.push(subQuery1);

            let value2 = obj.valueDateRevenueLast;
            let subQuery2 = {
              field: field,
              operator: ">=",
              value: value2
            };
            query.rules.push(subQuery2);
          } else {
            let value = obj.valueRevenue;
            let subQuery = {
              field: field,
              operator: operator,
              value: value
            };
            query.rules.push(subQuery);
          }
          break;
        }
        case "Số điểm hiện có" : {
          if (operator == "<>") {
            let value1 = obj.valuePointFirst;
            let subQuery1 = {
              field: field,
              operator: ">=",
              value: value1
            };
            query.rules.push(subQuery1);

            let value2 = obj.valuePointLast;
            let subQuery2 = {
              field: field,
              operator: "<=",
              value: value2
            };
            query.rules.push(subQuery2);
          } else {
            let value = obj.valuePoint;
            let subQuery = {
              field: field,
              operator: operator,
              value: value
            };
            query.rules.push(subQuery);
          }
          break;
        }
        case "Sản phẩm" : {
          let listProducts = obj.listProducts;
          let value: Array<any> = [];
          listProducts.forEach((product, key) => {
            value.push(product.productId);
          });

          let subQuery = {
            field: field,
            operator: operator,
            value: value
          };
          query.rules.push(subQuery);
          break;
        }
        case "Ngày sinh nhật" : {
          let value = obj.valueBirthDayId;
          let subQuery = {
            field: field,
            operator: operator,
            value: value
          };
          query.rules.push(subQuery);
          break;
        }
        case "Nhóm khách hàng" : {
          let value = obj.valueCustomerGroupId;
          let subQuery = {
            field: field,
            operator: operator,
            value: value
          };
          query.rules.push(subQuery);
          break;
        }
        default : {
          return;
        }
      }
    });

    return query;
  }

  valueToSQL(value) {
    switch (typeof value) {
      case 'string':
        return "'" + value + "'";
      case 'boolean':
        return value ? '1' : '0';
      case 'number':
        if (isFinite(value)) return value;
    }
  }

  isDefined(value) {
    return value !== undefined;
  }

  basicRulesetToSQL(ruleset) {
    return ruleset.rules.map((rule) => {
      if (rule.rules) {
        return "(" + this.basicRulesetToSQL(rule) + ")";
      }
      var column = rule.field,
        operator, value;

      switch (rule.operator) {
        case "is null":
        case "is not null":
            operator = rule.operator;
            value = "";
            break;
        case "in":
        case "not in":
            operator = rule.operator;
            if (Array.isArray(rule.value) && rule.value.length)
                value = "(" + rule.value.map(this.valueToSQL).filter(this.isDefined).join(", ") + ")";
            break;
        default:
            operator = rule.operator;
            value = this.valueToSQL(rule.value);
            break;
      }

      if (this.isDefined(column) && this.isDefined(value) && this.isDefined(operator)) {
          return "(" + (column + " " + operator + " " + value).trim() + ")";
      }
    }).filter(this.isDefined).join(" " + ruleset.condition + " ");
  }
  /*End*/

  /*Event: Xử lý các nút checkbox trên table result customer*/
  selectAll() {
    if (this.masterCheck) {
      for (var item of this.listDataUser) {
        if (item.isChecked) {
          item.isChecked = !item.isChecked;
        }
      }
    } else {
      for (var item of this.listDataUser) {
        item.isChecked = !item.isChecked;
      }
    }
    this.masterCheck = !this.masterCheck;

    if (this.listDataUser.find(item => item.isChecked == true) == undefined) {
      this.resultFilteredCustomer = 0;
    } else {
      var count = 0;
      this.listDataUser.forEach(item => {
        if (item.isChecked) {
          count++;
        }
      });
      this.resultFilteredCustomer = count;
    }

    if (this.resultFilteredCustomer == 0) {
      this.isNoData = 1;
    } else {
      this.isNoData = 0;
    }
  }

  changeChecked(customerId) {
    if (!this.masterCheck) {
      this.masterCheck = !this.masterCheck
    }
    for (var item of this.listDataUser) {
      if (item.customerId == customerId) {
        item.isChecked = !item.isChecked;
      }
    }
    var count = 0;
    if (this.masterCheck) {
      for (let item of this.listDataUser) {
        if (item.isChecked) {
          count++;
        }
      }
      if (count == 0) {
        this.masterCheck = false;
      }
    }
    this.resultFilteredCustomer = count;
    if (count == 0) {
      this.isNoData = 1;
    } else {
      this.isNoData = 0;
    }
  }
  /*End*/

  validateFilters() {
    let result = true;
    if (this.listFilterModel.length > 0) {
      //kiểm tra dữ liệu trả về đã đủ chưa
      this.listFilterModel.forEach((item, index) => {
        if (item.criteriaId == null) {
          result = false;
        } else {
          let criteriaId = item.criteriaId;
          let criteriaName = this.listCriteria.find(item => item.criteriaId == criteriaId).criteriaName;
          if (item.conditionId == null) {
            result = false;
          } else {
            switch (criteriaName)
            {
              case "Loại khách hàng" : {
                let valueGroupId = item.valueGroupId;
                if (valueGroupId == null) {
                  result = false;
                }
                break;
              }
              case "Độ tuổi" : {
                let conditionId = item.conditionId;
                let conditionName = this.listParentCondition[index].find(item => item.conditionId == conditionId).conditionName;
                
                if (conditionName == "Trong khoảng") {
                  if (item.valueAgeFirst == null || item.valueAgeLast == null || item.valueAgeFirst < 0 || item.valueAgeLast < 0) {
                    result = false;
                  } else if (item.valueAgeFirst > item.valueAgeLast) {
                    result = false;
                  }
                } else {
                  if (item.valueAge == null || item.valueAge < 0) {
                    result = false;
                  }
                }
                break;
              }
              case "Giới tính" : {
                let valueSexId = item.valueSexId;
                if (valueSexId == null) {
                  result = false;
                }
                break;
              }
              case "Doanh thu" : {
                let conditionId = item.conditionId;
                let conditionName = this.listParentCondition[index].find(item => item.conditionId == conditionId).conditionName;

                if (conditionName == "Trong khoảng") {
                  if (item.valueRevenueFirst == null || item.valueRevenueLast == null || item.valueRevenueFirst < 0 || item.valueRevenueLast < 0) {
                    result = false;
                  } else if (item.valueRevenueFirst > item.valueRevenueLast) {
                    result = false;
                  }
                } else if (conditionName == "Phát sinh trong khoảng" || conditionName == "Không phát sinh trong khoảng") {
                  if (item.valueDateRevenueFirst == null || item.valueDateRevenueLast == null) {
                    result = false;
                  }
                } else {
                  if (item.valueRevenue == null || item.valueRevenue < 0) {
                    result = false;
                  }
                }
                break;
              }
              case "Số điểm hiện có" : {
                let conditionId = item.conditionId;
                let conditionName = this.listParentCondition[index].find(item => item.conditionId == conditionId).conditionName;

                if (conditionName == "Trong khoảng") {
                  if (item.valuePointFirst == null || item.valuePointLast == null) {
                    result = false;
                  }
                } else {
                  if (item.valuePoint == null) {
                    result = false;
                  }
                }
                break;
              }
              case "Sản phẩm" : {
                if (!item.listProducts || item.listProducts.length == 0) {
                  result = false;
                }
                break;
              }
              case "Ngày sinh nhật" : {
                let valueBirthDayId = item.valueBirthDayId;
                if (valueBirthDayId == null) {
                  result = false;
                }
                break;
              }
              case "Nhóm khách hàng" : {
                let valueCustomerGroupId = item.valueCustomerGroupId;
                if (valueCustomerGroupId == null) {
                  result = false;
                }
                break;
              }
              default : {
                result = true;
              }
            }
          }
        }
      });
    } else {
      //chưa tạo bộ lọc
      result = false;
    }

    return result;
  }

  goToFollowProcessCustomerCare() {
    //Redirect đến trang Theo dõi chương trình chăm sóc khách hàng
    let _title = "XÁC NHẬN";
    let _content = "Bạn có muốn hủy bỏ sửa nội dung chương trình CSKH?";
    this.dialogPopup = this.dialogPop.open(PopupComponent,
      {
        width: '500px',
        height: '300px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogPopup.afterClosed().subscribe( async result => {
      if (result) {
        this.router.navigate(['/customer/care-list', {}]);
      }
    });
  }

  goToCreateProcessCustomerCare() {
    //Sang bước 2 với tập khách hàng đã được lọc ra
    this.isSkip = false;
    this.listCustomerId = [];
    this.listDataUser.forEach(item => {
      if (item.isChecked) {
        this.listCustomerId.push(item.customerId);
      }
    });
    this.step = 2;
  }

  /*EVENT: TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/
  
  //Thay đổi Hình thức
  changedCustomerCareContactType(event: any) {
    let selectedData = event.value;

    if (selectedData.categoryCode == "SMS") {
      //Nếu là Gửi SMS
      this.isSendSMS = true;
      this.isSendEmail = false;
      this.isSendGift = false;

      if (this.customerCareModel.IsSendNow == null) {
        this.customerCareModel.IsSendNow = true;
      }
    } else if (selectedData.categoryCode == "Email") {
      //Nếu là gửi email
      this.isSendEmail = true;
      this.isSendSMS = false;
      this.isSendGift = false;

      if (this.customerCareModel.IsSendEmailNow == null) {
        this.customerCareModel.IsSendEmailNow = true;
      }
    } else if (selectedData.categoryCode == "Gift") {
      //Nếu là tặng quà
      this.isSendGift = true;
      this.isSendEmail = false;
      this.isSendSMS = false;

      if (this.customerCareModel.GiftCustomerType1 != null && this.customerCareModel.GiftCustomerType2 != null) {
        //Nếu tặng quà cho cả 2 loại khách hàng
        this.isShowOptionsGiftType1 = true;
        this.isShowOptionsGiftType2 = true;
      } else if (this.customerCareModel.GiftCustomerType1 != null && this.customerCareModel.GiftCustomerType2 == null) {
        //Nếu chỉ tặng quà cho khách hàng cá nhân
        this.isShowOptionsGiftType1 = true;
        this.isShowOptionsGiftType2 = false;
      } else if (this.customerCareModel.GiftCustomerType1 == null && this.customerCareModel.GiftCustomerType2 != null) {
        //Nếu chỉ tặng quà cho khách hàng doanh nghiệp
        this.isShowOptionsGiftType1 = false;
        this.isShowOptionsGiftType2 = true;
      } else {
        //Nếu chưa tặng quà cho loại nào
        this.setDefaultValueCustomerGift();
      }
    } else if (selectedData.categoryCode == "CallPhone") {
      //Nếu Gọi điện
      this.isSendEmail = false;
      this.isSendSMS = false;
      this.isSendGift = false;
    }
  }

  //Reset Customer Care Model
  resetCustomerCareModel() {
    //Reset value gửi email
    this.customerCareModel.IsSendEmailNow = null;
    this.customerCareModel.SendEmailDate = null;
    this.customerCareModel.SendEmailHour = null;
    this.customerCareModel.CustomerCareContentEmail = null;
    this.isSendEmailAt = false;
    this.isSendEmail = false;

    //Reset value gửi SMS
    this.customerCareContentSMSControl.reset();
    this.customerCareModel.IsSendNow = null;
    this.customerCareModel.SendDate = null;
    this.customerCareModel.SendHour = null;
    this.customerCareModel.CustomerCareContentSMS = null;
    this.isSendSMSAt = false;
    this.isSendSMS = false;

    //Reset value tặng quà
    this.giftTotal1Control.reset();
    this.giftTotal2Control.reset();
    this.customerCareModel.GiftCustomerType1 = null;
    this.customerCareModel.GiftTypeId1 = null;
    this.customerCareModel.GiftTotal1 = null;
    this.customerCareModel.GiftCustomerType2 = null;
    this.customerCareModel.GiftTypeId2 = null;
    this.customerCareModel.GiftTotal2 = null;
    this.isSendGift = false;
    this.isShowOptionsGiftType1 = false;
    this.isShowOptionsGiftType2 = false;
    this.isHasValueCustomerType = true;
  }

  //Reset Form Content Customer Care
  resetFormContentCustomerCare() {
    this.resetCustomerCareModel();
    this.contentProcessCustomerCareForm.reset();
    this.customerCareModel = new CustomerCareModel();
    this.getMasterDataForContentCustomerCare();
  }

  onChangeRadioSendEmail(event: any) {
    if (event.value == 'true') {
      //Nếu là gửi ngay
      this.sendEmailDateControl.reset();
      this.sendEmailHourControl.reset();
      this.isSendEmailAt = false;
      this.customerCareModel.IsSendEmailNow = true;
      this.customerCareModel.SendEmailDate = null;
      this.customerCareModel.SendEmailHour = null;
    } else {
      //Nếu là gửi vào khoảng thời gian
      this.isSendEmailAt = true;
      this.customerCareModel.IsSendEmailNow = false;
      this.customerCareModel.SendEmailDate = convertToUTCTime(this.sendEmailDateControl.value);
      this.customerCareModel.SendEmailHour = this.sendEmailHourControl.value;
    }
  }

  //Chọn email mẫu
  selectEmailTemplate() {
    this.dialogSelectTemplateEmail = this.dialogPop.open(TemplateEmailDialogComponent,
      {
        width: '1200px',
        height: 'auto',
        autoFocus: false,
        data: {}
      });

    this.dialogSelectTemplateEmail.afterClosed().subscribe(result => {
      if (result) {
        this.customerCareModel.CustomerCareContentEmail = result.content;
      }
    }, error => { });
  }

  onChangeRadioSendSMS(event: any) {
    if (event.value == 'true') {
      //Nếu là gửi ngay
      this.isSendSMSAt = false;
      this.customerCareModel.IsSendNow = true;
      this.customerCareModel.SendDate = null;
      this.customerCareModel.SendHour = null;
    } else {
      //Nếu là gửi vào khoảng thời gian
      this.isSendSMSAt = true;
      this.customerCareModel.IsSendNow = false;
      this.customerCareModel.SendDate = convertToUTCTime(this.sendDateControl.value);
      this.customerCareModel.SendHour = this.sendHourControl.value;
    }
  }

  //Chọn tin nhắn mẫu
  selectSMSTemplate() {
    this.dialogSelectTemplateSMS = this.dialogPop.open(TemplateSmsDialogComponent,
      {
        width: '1200px',
        height: 'auto',
        autoFocus: false,
        data: {}
      });

    this.dialogSelectTemplateSMS.afterClosed().subscribe(result => {
      if (result) {
        this.customerCareModel.CustomerCareContentSMS = result.content;
      }
    }, error => { });
  }

  changedGiftCustomerType1(event: any) {
    let selectedData = event.value;

    if (selectedData.categoryId == 0) {
      this.isShowOptionsGiftType1 = false;
      this.customerCareModel.GiftCustomerType1 = null;
      this.customerCareModel.GiftTypeId1 = null;
      this.customerCareModel.GiftTotal1 = null;

      if (this.isShowOptionsGiftType2 == false) {
        //Nếu không chọn Loại khách hàng nào thì bật flag báo lỗi
        this.isHasValueCustomerType = false;
      }
    } else {
      this.isHasValueCustomerType = true; //Flag: Báo lỗi phải chọn ít nhất một loại khách hàng
      this.isShowOptionsGiftType1 = true;
      this.customerCareModel.GiftCustomerType1 = this.listDefaultIdCusPer[0].categoryId;
      
      let defaultId1 = '';
      if (this.listGiftTypeId1.length > 0) {
        defaultId1 = this.listGiftTypeId1[0].categoryId;
        this.customerCareModel.GiftTypeId1 = defaultId1;
      }
      this.customerCareModel.GiftTotal1 = 1;
    }
  }

  changedGiftCustomerType2(event: any) {
    let selectedData = event.value;

    if (selectedData.categoryId == 0) {
      this.isShowOptionsGiftType2 = false;
      this.customerCareModel.GiftCustomerType2 = null;
      this.customerCareModel.GiftTypeId2 = null;
      this.customerCareModel.GiftTotal2 = null;

      if (this.isShowOptionsGiftType1 == false) {
        //Nếu không chọn Loại khách hàng nào thì bật flag báo lỗi
        this.isHasValueCustomerType = false;
      }
    } else {
      this.isHasValueCustomerType = true; //Flag: Báo lỗi phải chọn ít nhất một loại khách hàng
      this.isShowOptionsGiftType2 = true;
      this.customerCareModel.GiftCustomerType2 = this.listDefaultIdCusEnterprise[0].categoryId;

      let defaultId2 = '';
      if (this.listGiftTypeId1.length > 0) {
        defaultId2 = this.listGiftTypeId2[0].categoryId;
        this.customerCareModel.GiftTypeId2 = defaultId2;
      }
      this.customerCareModel.GiftTotal2 = 1;
    }
  }

  //Lưu nội dung chương trình chăm sóc khách hàng
  saveContentCustomerCare() {
    Object.keys(this.contentProcessCustomerCareForm.controls).forEach(key => {
      if (!this.contentProcessCustomerCareForm.controls[key].valid) {
        this.contentProcessCustomerCareForm.controls[key].markAsTouched();
      }
    });

    let check = this.ValidateContentCustomerCare();
    if (check) {
      if (this.isSkip) {
        //Nếu là bỏ qua bước lọc thì
        this.listFilterModel = [];
      }
      let stringFilterModel = this.listFilterModel;
      var strFilterModel = JSON.stringify(stringFilterModel);
      //Nếu tất cả dữ liệu nhập đúng
      if (this.isSendEmail) {
        //Nếu là Gửi email
        this.resetCustomerCareByContactType('isSendEmail');

        this.customerCareModel.CustomerCareContentEmail = this.customerCareContentEmailControl.value;
      } else if (this.isSendSMS) {
        //Nếu là Gửi SMS
        this.resetCustomerCareByContactType('isSendSMS');
        this.customerCareModel.CustomerCareContentSMS = this.customerCareContentSMSControl.value;
      } else if (this.isSendGift) {
        //Nếu là Gửi quà
        this.resetCustomerCareByContactType('isSendGift');
        // loại KH
        this.customerCareModel.GiftCustomerType1 = this.giftCustomerType1Control.value.categoryId
        this.customerCareModel.GiftCustomerType2 = this.giftCustomerType2Control.value.categoryId

        // loai qua
        this.customerCareModel.GiftTypeId1 = this.giftTypeId1Control.value.categoryId
        this.customerCareModel.GiftTypeId2 = this.giftTypeId2Control.value.categoryId
      } else if (this.isSendEmail == false && this.isSendSMS == false && this.isSendGift == false) {
        //Nếu là Gọi điện
        this.resetCustomerCareByContactType('isSendPhone');
      }
      
      if (this.listCustomerId.length == 1) {
        this.customerCareModel.CustomerCareType = 1;
      } else if (this.listCustomerId.length > 1) {
        this.customerCareModel.CustomerCareType = 2;
      }

      // thoi gian ap dung
      this.customerCareModel.EffecttiveFromDate = convertToUTCTime(this.effecttiveFromDateControl.value)
      this.customerCareModel.EffecttiveToDate = convertToUTCTime(this.effecttiveToDateControl.value)

      // hinh thuc
      this.customerCareModel.CustomerCareContactType = this.customerCareContactTypeControl.value.categoryId;

      // trang thai
      this.customerCareModel.StatusId = this.statusIdControl.value.categoryId

      this.customerCareModel.CreateById = this.userId;
      let listTypeCustomerCode: Array<any> = [];
      this.SelectedTypeCustomer.forEach(x => {
        listTypeCustomerCode.push(x.code);
      })

      this.customerCareService.createCustomerCare(this.customerCareModel, this.listCustomerId, listTypeCustomerCode, strFilterModel, this.userId)
      .subscribe(response => {
        let result = <any>response;
        if (result.statusCode !== 200) {
          // Thông báo thay đổi thành công || thay đổi thất bại
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: result.messageCode });

        } else {
          this.router.navigate(['/customer/care-list', {}]);
        }
      }, error => { });
    }
  }
  /*END*/

  /*Validate Nội dung CSKH*/
  ValidateContentCustomerCare() {
    let result = false; //Mặc định trả về lỗi dữ liệu

    if (this.effecttiveFromDateControl.valid && this.effecttiveToDateControl.valid &&
        this.customerCareTitleControl.valid && this.customerCareContentControl.valid &&
        this.expectedAmountControl.valid) {
      //Nếu dữ liệu nhập vào chính xác thì kiểm tra tiếp xem form đang ở nội dung nào: Send Email, Send SMS, Tặng quà?
      if (this.isSendEmail) {
        if (this.isSendEmailAt) {
          if (this.sendEmailDateControl.valid && this.sendEmailHourControl.valid) {
            //Tạo nội dung CSKH
            result = true;
          }
        } else {
          if (this.customerCareContentEmailControl.valid) {
            //Tạo nội dung CSKH
            result = true;
          }
        }
      } else if (this.isSendSMS) {
        if (this.isSendSMSAt) {
          if (this.sendDateControl.valid && this.sendHourControl.valid) {
            //Tạo nội dung CSKH
            result = true;
          }
        } else {
          if (this.customerCareContentSMSControl.valid) {
            //Tạo nội dung CSKH
            result = true;
          }
        }
      } else if (this.isSendGift) {
        if (this.isHasValueCustomerType) {
          if (this.giftTotal1Control.valid && this.giftTotal2Control.valid) {
            //Tạo nội dung CSKH
            result = true;
          }
        }
      } else {
        result = true;
      }
    }
    return result;
  }
  /*End*/

  //Kiểm tra xem 1 object có null hay không
  isEmpty(obj) {
    for(var key in obj) {
      if(obj.hasOwnProperty(key))
        return false;
    }
    return true;
  }

  resetCustomerCareByContactType(code: string) {
    if (code == 'isSendEmail') {
      this.customerCareModel.CustomerCareContentSMS = null;
      this.customerCareModel.IsSendNow = null;
      this.customerCareModel.SendDate = null;
      this.customerCareModel.SendHour = null;

      this.customerCareModel.CustomerCareVoucher = null;
      this.customerCareModel.DiscountAmount = null;
      this.customerCareModel.PercentDiscountAmount = null;

      this.customerCareModel.GiftCustomerType1 = null;
      this.customerCareModel.GiftTypeId1 = null;
      this.customerCareModel.GiftTotal1 = null;

      this.customerCareModel.GiftCustomerType2 = null;
      this.customerCareModel.GiftTypeId2 = null;
      this.customerCareModel.GiftTotal2 = null;
    } else if (code == 'isSendSMS') {
      this.customerCareModel.CustomerCareContentEmail = null;
      this.customerCareModel.IsSendEmailNow = null;
      this.customerCareModel.SendEmailDate = null;
      this.customerCareModel.SendEmailHour = null;

      this.customerCareModel.CustomerCareVoucher = null;
      this.customerCareModel.DiscountAmount = null;
      this.customerCareModel.PercentDiscountAmount = null;

      this.customerCareModel.GiftCustomerType1 = null;
      this.customerCareModel.GiftTypeId1 = null;
      this.customerCareModel.GiftTotal1 = null;

      this.customerCareModel.GiftCustomerType2 = null;
      this.customerCareModel.GiftTypeId2 = null;
      this.customerCareModel.GiftTotal2 = null;
    } else if (code == 'isSendGift') {
      this.customerCareModel.CustomerCareContentSMS = null;
      this.customerCareModel.IsSendNow = null;
      this.customerCareModel.SendDate = null;
      this.customerCareModel.SendHour = null;

      this.customerCareModel.CustomerCareContentEmail = null;
      this.customerCareModel.IsSendEmailNow = null;
      this.customerCareModel.SendEmailDate = null;
      this.customerCareModel.SendEmailHour = null;

      this.customerCareModel.CustomerCareVoucher = null;
      this.customerCareModel.DiscountAmount = null;
      this.customerCareModel.PercentDiscountAmount = null;
    } else if (code == 'isSendPhone') {
      this.customerCareModel.CustomerCareContentSMS = null;
      this.customerCareModel.IsSendNow = null;
      this.customerCareModel.SendDate = null;
      this.customerCareModel.SendHour = null;

      this.customerCareModel.CustomerCareContentEmail = null;
      this.customerCareModel.IsSendEmailNow = null;
      this.customerCareModel.SendEmailDate = null;
      this.customerCareModel.SendEmailHour = null;

      this.customerCareModel.CustomerCareVoucher = null;
      this.customerCareModel.DiscountAmount = null;
      this.customerCareModel.PercentDiscountAmount = null;

      this.customerCareModel.GiftCustomerType1 = null;
      this.customerCareModel.GiftTypeId1 = null;
      this.customerCareModel.GiftTotal1 = null;

      this.customerCareModel.GiftCustomerType2 = null;
      this.customerCareModel.GiftTypeId2 = null;
      this.customerCareModel.GiftTotal2 = null;
    }
  }

  replate_token(token: string) {
    let current_content = this.customerCareContentEmailControl.value;
    current_content = current_content + token;
    this.customerCareContentEmailControl.setValue(current_content);
  }

  replate_token_sms(token: string) {
    this.customerCareModel.CustomerCareContentSMS = this.customerCareModel.CustomerCareContentSMS + token;
  }

  goToCustomerDetail(customerId, contactId) {
    this.router.navigate(['/customer/detail', {customerId: customerId, contactId: contactId, defaultTab: 3}]);
  }

  //Filter Validator ngày không <= ngày hiện tại
  myFilter = (d: Date): boolean => {
    let day = d;
    const now = new Date();
    now.setHours(0,0,0,0);
    return (day >= now);
  }

  /*Xóa dữ liệu ô người phụ trách*/
  clearDataEmployeeCharge() {
    this.employeeChargeControl.reset();  
    this.customerCareModel.EmployeeCharge = null;
  }
  /*End*/
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};
