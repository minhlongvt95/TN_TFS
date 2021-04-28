import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

import { PopupComponent } from '../../../shared/components/popup/popup.component';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

//MODELS
import { CustomerCareModel } from '../../models/customer-care.model';

//SEVICES
import { CustomerService } from "../../services/customer.service";
import { EmployeeService } from "../../../employee/services/employee.service";
import { CategoryService } from '../../../shared/services/category.service';
import { ProductService } from '../../../product/services/product.service';
import { QuoteService } from '../../services/quote.service';
import { CustomerCareService } from '../../services/customer-care.service';

//DIALOG COMPONENT
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { SuccessComponent } from '../../../shared/toast/success/success.component';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import { TemplateEmailDialogComponent } from '../template-email-dialog/template-email-dialog.component';
import { TemplateSmsDialogComponent } from '../template-sms-dialog/template-sms-dialog.component';

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
  // }
  {
    criteriaId: 7,
    criteriaName: 'Ngày sinh nhật',
    field: 'BirthDay'
  },
  {
    criteriaId: 8,
    criteriaName: 'Nhóm khách hàng',
    field: 'CustomerGroupId'
  }
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
  {
    conditionId: 15,
    conditionName: 'Phát sinh trong khoảng',
    criteriaId: 4,
    operator: '<>'
  },
  {
    conditionId: 16,
    conditionName: 'Không phát sinh trong khoảng',
    criteriaId: 4,
    operator: '<>'
  }
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
  }
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

const ELEMENT_CUSTOMER_TYPE = [
  {
    categoryId: 1,
    categoryName: "Khách hàng doanh nghiệp"
  },
  {
    categoryId: 2,
    categoryName: "Khách hàng cá nhân"
  }
]

const ELEMENT_CUSTOMER_ENTERPRISE = [
  {
    categoryId: 0,
    categoryName: "-- Không chọn --"
  },{
    categoryId: 1,
    categoryName: "Khách hàng doanh nghiệp"
  }
]

const ELEMENT_CUSTOMER_PER = [
  {
    categoryId: 0,
    categoryName: "-- Không chọn --"
  },{
    categoryId: 2,
    categoryName: "Khách hàng cá nhân"
  }
]

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
]

@Component({
  selector: 'app-customer-care-detail',
  templateUrl: './customer-care-detail.component.html',
  styleUrls: ['./customer-care-detail.component.css']
})
export class CustomerCareDetailComponent implements OnInit {
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

  /*Hiển thị lại một list bộ lọc đã được lưu*/
  reShowListFilters: boolean = true;
  CustomerCareId: string = '';
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

  listParentCriteria: Array<any> = [];
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

  listParentCustomerGroup: Array<any> = [];
  listCustomerGroup: Array<any> = [];

  //data
  listFilterModel: Array<any> = [];

  //total of index
  index: number = 0;

  /*RESULT FILTERED CUSTOMER*/
  resultFilteredCustomer: number = 0;
  listDataUser: Array<any> = [];
  displayedColumns: string[] = ['customerName', 'dateOfBirth', 'customerEmail', 'customerPhone', 'picName', 'customerCareCustomerStatusId'];
  dataSource = new MatTableDataSource(this.listDataUser);
  pageSizeOptions = [5, 10, 15, 100];
  masterCheck: boolean = true;
  isNoData: number = 1;
  msgNoData: number = 1;
  customerCareCustomerStatusList: Array<any> = [];

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

  isGetDataFalse: boolean = null;
  msgGetDataFalse: string = '';
  isNotSave: boolean = null;
  listCustomerId: Array<string> = [];

  filteredSeller: Observable<string[]>; //list nhân phụ trách

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

  SelectedTypeCustomer: Array<any> = [];
  customerType: Array<any> = [];

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

  defaultIdCusPer: number = null; //Id mặc định của loại Khách hàng cá nhân
  listDefaultIdCusPer: Array<any> = [];
  defaultIdCusEnterprise: number = null; //Id mặc định của loại Khách hàng doanh nghiệp
  listDefaultIdCusEnterprise: Array<any> = [];

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
    public dialogPop: MatDialog,
    private customerService: CustomerService,
    private employeeService: EmployeeService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private messageService: MessageService,

    private quoteService: QuoteService,
    private customerCareService: CustomerCareService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.customerType = [
      { name: 'Khách hàng', code: 'HDO' },
      { name: 'Khách hàng tiềm năng', code: 'MOI' },
    ];
    //Kiểm tra xem user đang đăng nhập là quản lý hay nhân viên
    this.isManager = localStorage.getItem('IsManager') === "true" ? true : false;
    this.employeeId = JSON.parse(localStorage.getItem('auth')).EmployeeId;

    this.filterForm = this.fb.group({ 
      rowFilters: this.fb.array([this.createFilter()])
    });

    // set rowFilterList to this field
    this.rowFilterList = this.filterForm.get('rowFilters') as FormArray;

    if (!this.reShowListFilters) {
      //Nếu là tạo mới bộ lọc lần đầu tiên
      this.getMasterData();
    } else {
      //Nếu là hiển thị lại bộ lọc
      let ttt = this.route.params.subscribe(params => { 
        let isEmptyObj = this.isEmpty(params);
        if (!isEmptyObj) {
          this.CustomerCareId = params['CustomerCareId'];
          this.getReShowListFilters(this.CustomerCareId);
        } else {
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Sai đường dẫn, vui lòng kiểm tra lại' });
        }
      });
    }

    /*TẠO CHƯƠNG TRÌNH CHĂM SÓC KHÁCH HÀNG*/
    // this.getMasterDataForContentCustomerCare();

    let expectedAmountPattern = '^([0-9][0-9]{0,9}|10000000000)$';
    let greaterThanZero = '^([1-9]|[1-9][0-9]|[1][0-9][0-9]|20[0-0])$';

    this.employeeChargeControl = new FormControl('', [Validators.required]);
    this.effecttiveFromDateControl = new FormControl(null, [Validators.required]);
    this.effecttiveToDateControl = new FormControl(null, [Validators.required]);
    this.customerCareTitleControl = new FormControl('', [Validators.required]);
    this.customerCareContentControl = new FormControl(null, [Validators.required]);
    this.customerCareContactTypeControl = new FormControl('', [Validators.required]);
    this.expectedAmountControl = new FormControl('', [Validators.required, Validators.pattern(expectedAmountPattern)]);
    this.statusIdControl = new FormControl('', [Validators.required]);
    this.customerCareContentEmailControl = new FormControl('', [Validators.required]);
    this.sendEmailDateControl = new FormControl('', [Validators.required]);
    this.sendEmailHourControl = new FormControl('', [Validators.required]);
    this.customerCareContentSMSControl = new FormControl('', [Validators.required]);
    this.sendDateControl = new FormControl('', [Validators.required]);
    this.sendHourControl = new FormControl('', [Validators.required]);
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

  async getReShowListFilters(CustomerCareId) {
    this.loading = true;
    /*Get List Default Data*/
    this.listCriteria = await ELEMENT_CRITERIA;
    this.listGroup = ELEMENT_VALUE_GROUP;
    this.listSex = ELEMENT_VALUE_SEX;
    this.listBirthDay = ELEMENT_VALUE_BIRTHDAY;

    let result: any = await this.productService.searchProductAsync('', '', [], []);
    this.listProducts = result.productList;

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

    let result_customer_care: any = await this.customerCareService.getCustomerCareByIdAsync(CustomerCareId, this.userId);
    if (result_customer_care.queryFilter == "null" || result_customer_care.queryFilter == null || result_customer_care.queryFilter == '') {
      this.listFilterModel = [];
    } else {
      this.listFilterModel = JSON.parse(result_customer_care.queryFilter);
    }
    let selectTypeCustomer = result_customer_care.typeCustomer;
    if (selectTypeCustomer == 1 || selectTypeCustomer == 4){
      this.SelectedTypeCustomer = this.customerType;
    } else if (selectTypeCustomer == 2) {
      this.SelectedTypeCustomer = this.customerType.filter(x => x.code == 'HDO');
    } else if (selectTypeCustomer == 3) {
      this.SelectedTypeCustomer = this.customerType.filter(x => x.code == 'MOI');
    }
    this.listDataUser = result_customer_care.listCustomer;
    this.loading = false;
    this.setCustomerCareModel(result_customer_care.customerCare);
    
    
    //mapDataToForm
    this.customerCareContentEmailControl.setValue(this.customerCareModel.CustomerCareContentEmail);
    this.customerCareContentSMSControl.setValue(this.customerCareModel.CustomerCareContentSMS);
    
    this.effecttiveFromDateControl.setValue(this.customerCareModel.EffecttiveFromDate);
    this.effecttiveToDateControl.setValue(this.customerCareModel.EffecttiveToDate);

    //List trạng thái của khách hàng
    let customerCareCustomerStatusList: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("TKH");
    this.customerCareCustomerStatusList = customerCareCustomerStatusList.category;
    
    let categoryStatusId: any = await this.categoryService.getCategoryByIdAsync(this.customerCareModel.StatusId);
    if (categoryStatusId.statusCode != 200) {
      //Thông báo lỗi
      this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: result.messageCode });
    } else {
      await this.getInforCustomerCare(categoryStatusId);
    }



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
    
    let cusCareStatusCode = this.listStatusId.find(x => x.categoryId == result_customer_care.customerCare.statusId).categoryCode;
    let cusCareContactTypeCode = this.listCustomerCareContactType.find(x => x.categoryId == this.customerCareModel.CustomerCareContactType).categoryCode;
    if (cusCareStatusCode != 'Active') {
      //Nếu trạng thái của chương trình CSKH không phải là: Kích hoạt thì không cho sửa trạng thái của khách hàng
      for (let item of this.listDataUser) {
        item.isEditStatus = false;
      }
    } else {
      if (cusCareContactTypeCode == 'Email' || cusCareContactTypeCode == 'SMS') {
        //Nếu hình thức CSKH là: Gửi email hoặc Gửi SMS
        for (let item of this.listDataUser) {
          if (this.customerCareCustomerStatusList.find(x => x.categoryId == item.customerCareCustomerStatusId).categoryCode == 'DSO') {
            //Nếu trạng thái của khách hàng đang là: Đã chăm sóc thì không cho sửa
            item.isEditStatus = false;
          } else {
            item.isEditStatus = true;
          }
        }
      } else {
        for (let item of this.listDataUser) {
          item.isEditStatus = true;
        }
      }
    }
    
    this.dataSource = new MatTableDataSource(this.listDataUser);

    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.paginator._intl.itemsPerPageLabel = 'Số khách hàng mỗi trang: ';

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

  setCustomerCareModel(obj) {
    this.customerCareModel.CustomerCareId = obj.customerCareId;
    this.customerCareModel.CustomerCareCode = obj.customerCareCode;
    this.customerCareModel.EmployeeCharge = obj.employeeCharge;
    this.customerCareModel.EffecttiveFromDate = new Date(obj.effecttiveFromDate);
    this.customerCareModel.EffecttiveToDate = new Date(obj.effecttiveToDate);
    this.customerCareModel.CustomerCareTitle = obj.customerCareTitle;
    this.customerCareModel.CustomerCareContent = obj.customerCareContent;
    this.customerCareModel.CustomerCareContactType = obj.customerCareContactType;
    this.customerCareModel.CustomerCareContentSMS = obj.customerCareContentSms;
    this.customerCareModel.ExpectedAmount = obj.expectedAmount;
    this.customerCareModel.IsSendNow = obj.isSendNow;
    this.customerCareModel.IsEvent = obj.isEvent;
    this.customerCareModel.SendDate = new Date(obj.sendDate);
    this.customerCareModel.SendHour = obj.sendHour;
    this.customerCareModel.CustomerCareEvent = obj.customerCareEvent;
    this.customerCareModel.CustomerCareEventHour = obj.customerCareEventHour;
    this.customerCareModel.CustomerCareContentEmail = obj.customerCareContentEmail;
    this.customerCareModel.IsSendEmailNow = obj.isSendEmailNow;
    this.customerCareModel.SendEmailDate = new Date(obj.sendEmailDate);
    this.customerCareModel.SendEmailHour = obj.sendEmailHour;
    this.customerCareModel.CustomerCareVoucher = obj.customerCareVoucher;
    this.customerCareModel.DiscountAmount = obj.discountAmount;
    this.customerCareModel.PercentDiscountAmount = obj.percentDiscountAmount;
    this.customerCareModel.GiftCustomerType1 = obj.giftCustomerType1;
    this.customerCareModel.GiftTypeId1 = obj.giftTypeId1;
    this.customerCareModel.GiftTotal1 = obj.giftTotal1;
    this.customerCareModel.GiftCustomerType2 = obj.giftCustomerType2;
    this.customerCareModel.GiftTypeId2 = obj.giftTypeId2;
    this.customerCareModel.GiftTotal2 = obj.giftTotal2;
    this.customerCareModel.CustomerCareType = obj.customerCareType;
    this.customerCareModel.StatusId = obj.statusId;
    this.customerCareModel.CreateDate = new Date(obj.createDate);
    this.customerCareModel.CreateById = obj.createById;
    this.customerCareModel.UpdateDate = new Date(obj.updateDate);
    this.customerCareModel.UpdateById = obj.updateById;
  }

  async getInforCustomerCare(categoryStatusId) {
    //List nhân viên phụ trách
    let resultListEmpCharge: any = await this.employeeService.getEmployeeCareStaffAsyc(this.isManager, this.employeeId);
    this.listEmployeeCharge = resultListEmpCharge.employeeList;
    this.listEmployeeCharge.forEach(x => {
      x.employeeName = x.employeeCode + ' - ' + x.employeeName;
    })
    let emp = this.listEmployeeCharge.find(x => x.employeeId == this.customerCareModel.EmployeeCharge);
    this.employeeChargeControl.setValue(emp);

    // this.filteredSeller = this.employeeChargeControl.valueChanges
    // .pipe(
    //   startWith(''),
    //   map(state => state ? this._filterSeller(state, this.listEmployeeCharge) : this.listEmployeeCharge.slice())
    // );
    // const toSelectSeller = this.listEmployeeCharge.find(c => c.employeeId == this.customerCareModel.EmployeeCharge);
    // if (toSelectSeller != null) {
    //   this.employeeChargeControl.setValue(toSelectSeller.employeeCode + ' - ' + toSelectSeller.employeeName);
    // }

    //list Hình thức
    let resultListCusCareContactType: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("HCS");
    this.listCustomerCareContactType = resultListCusCareContactType.category;
    let contactType = this.listCustomerCareContactType.find(x => x.categoryId == this.customerCareModel.CustomerCareContactType);
    this.customerCareContactTypeControl.setValue(contactType);

    //list trạng thái
    let resultListStatus: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("TCS");
    this.listStatusId = resultListStatus.category;
    let status = this.listStatusId.find(x => x.categoryId == this.customerCareModel.StatusId);
    this.statusIdControl.setValue(status);
    
    let cusCareStatusCode = this.listStatusId.find(x => x.categoryId == categoryStatusId.category.categoryId).categoryCode;

    switch (cusCareStatusCode)
    {
      case "New" : {
        //Mới tạo
        this.isNotSave = false;
        break;
      }
      case "Active" : {
        //Kích hoạt
        this.isNotSave = false;
        break;
      }
      case "Stoped" : {
        //Đã dừng
        this.isNotSave = false;
        break;
      }
      case "Closed" : {
        //Đóng
        this.isNotSave = true;
        break;
      }
      default : {
        
      }
    };

    //List Loại khách hàng
    this.list_customer_types = ELEMENT_CUSTOMER_TYPE;

    //Value dafault Chọn Khách hàng cá nhân
    this.listDefaultIdCusPer = ELEMENT_CUSTOMER_PER;
    let giftCustomerType1 = this.listDefaultIdCusPer.find(x => x.categoryId == this.customerCareModel.GiftCustomerType1);
    this.giftCustomerType1Control.setValue(giftCustomerType1);

    //Value dafault Chọn Khách hàng doanh nghiệp
    this.listDefaultIdCusEnterprise = ELEMENT_CUSTOMER_ENTERPRISE;
    let giftCustomerType2 = this.listDefaultIdCusEnterprise.find(x => x.categoryId == this.customerCareModel.GiftCustomerType2);
    this.giftCustomerType2Control.setValue(giftCustomerType2);

    //list loại quà
    let list_gift_results: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("LQU");
    this.listGiftTypeId1 = list_gift_results.category;
    let giftType1 = this.listGiftTypeId1.find(x => x.categoryId == this.customerCareModel.GiftTypeId1);
    this.giftTypeId1Control.setValue(giftType1);

    this.listGiftTypeId2 = list_gift_results.category;
    let giftType2 = this.listGiftTypeId2.find(x => x.categoryId == this.customerCareModel.GiftTypeId2);
    this.giftTypeId2Control.setValue(giftType2);

    let categoryCustomerContactType: any = await this.categoryService.getCategoryByIdAsync(this.customerCareModel.CustomerCareContactType);
    
    switch (categoryCustomerContactType.category.categoryCode)
    {
      case "Email" : {
        //Gửi email
        this.isSendEmail = true;
        if (this.customerCareModel.IsSendEmailNow != true) {
          this.isSendEmailAt = true;
        }
        break;
      }
      case "SMS" : {
        //Gửi SMS
        this.isSendSMS = true;
        if (this.customerCareModel.IsSendNow != true) {
          this.isSendSMSAt = true;
        }
        break;
      }
      case "Gift" : {
        //Tặng quà
        this.isSendGift = true;
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
        }
        break;
      }
      case "CallPhone" : {
        //Gọi điện
        this.isSendEmail = false;
        this.isSendEmailAt = false;
        this.isSendSMS = false;
        this.isSendSMSAt = false;
        this.isSendGift = false;
        break;
      }
      default : {
        
      }
    };
  }

  /*Auto complete employee*/
  // private _filterSeller(value: string, array: any) {
  //   return array.filter(state =>
  //     state.EmployeeName.toLowerCase().indexOf(value.toLowerCase()) >= 0 || state.EmployeeCode.toLowerCase().indexOf(value.toLowerCase()) >= 0);
  // }

  selectSeller(event: any) {
    // this.employeeChargeControl.setValue(event.option.viewValue);
    this.customerCareModel.EmployeeCharge = event.value.employeeId;
  }
  /*End*/

  async getMasterDataForContentCustomerCare() {
    //List nhân viên phụ trách
    this.employeeService.getEmployeeCareStaff(this.isManager, this.employeeId).subscribe(response => {
      let result = <any>response;
      this.listEmployeeCharge = result.employeeList;
      this.customerCareModel.EmployeeCharge = this.employeeId;
    });

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
    })

    //list trạng thái
    this.categoryService.getAllCategoryByCategoryTypeCode("TCS").subscribe(response => {
      let result = <any>response;
      this.listStatusId = result.category;
      //Trạng thái mặc định là Mới tạo
      let defaultId = '';
      if (this.listStatusId.length > 0) {
        defaultId = this.listStatusId.find(item => item.categoryCode == 'New').categoryId; //Mới tạo
      }
      this.customerCareModel.StatusId = defaultId;
    })

    //list loại quà
    let list_gift_results: any = await this.categoryService.getAllCategoryByCategoryTypeCodeAsyc("LQU");
    this.listGiftTypeId1 = list_gift_results.category;
    this.listGiftTypeId2 = list_gift_results.category;
  }

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
    this.customerCareModel.GiftCustomerType1 = this.listDefaultIdCusPer[0].categoryId;

    //Value dafault Chọn Khách hàng doanh nghiệp
    this.listDefaultIdCusEnterprise = ELEMENT_CUSTOMER_ENTERPRISE;
    this.customerCareModel.GiftCustomerType2 = this.listDefaultIdCusEnterprise[0].categoryId;

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
    let _content = "Bạn có muốn bỏ qua bước lọc khách hàng và tiếp tục bước 2?";
    this.dialogPopup = this.dialogPop.open(PopupComponent,
      {
        width: '500px',
        height: '300px',
        autoFocus: false,
        data: { title: _title, content: _content }
      });

    this.dialogPopup.afterClosed().subscribe( async result => {
      if (result) {
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

  /*Tạo một bộ lọc:*/
  createFilter(): FormGroup {
    return this.fb.group({
      criteriaControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      conditionControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueAgeControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueAgeFirstControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueAgeLastControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueListGroupControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueListSexControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueRevenueControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueRevenueFirstControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueRevenueLastControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueDateRevenueFirstControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueDateRevenueLastControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueListProductControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valuePointControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valuePointFirstControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valuePointLastControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueListBirthDayControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])],
      valueListCustomerGroupControl: [{ value: '', disabled: true }, Validators.compose([Validators.required])]
    });
  }
  /*End*/

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

  /*Event: Khi thay đổi Điều kiện: List Group Customer*/
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
    this.listFilterModel[index].valueAge = age;
  }

  changedValueAgeFirst(index, event: any) {
    let age_first = Number(event.target.value);
    this.listFilterModel[index].valueAgeFirst = age_first;
  }

  changedValueAgeLast(index, event: any) {
    let age_last = Number(event.target.value);
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
      result = Number(revenue);
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
      result = Number(revenue_first);
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
      result = Number(revenue_last);
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

  async goToCreateProcessCustomerCare() {
    //Sang bước 2 với tập khách hàng đã được lọc ra
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
      this.customerCareModel.SendEmailDate = new Date();
      this.sendEmailHourControl.setValue('00:00');
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
    });
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
      this.customerCareModel.SendDate = new Date();
      this.sendHourControl.setValue('00:00');
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
    });
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
      let stringFilterModel = this.listFilterModel;
      //Nếu tất cả dữ liệu nhập đúng
      if (this.isSendEmail) {
        //Nếu là Gửi email
        this.resetCustomerCareByContactType('isSendEmail');

        if (!this.customerCareModel.IsSendEmailNow) {
          this.customerCareModel.SendEmailDate = convertToUTCTime(this.customerCareModel.SendEmailDate);
        }

        this.customerCareModel.CustomerCareContentEmail = this.customerCareContentEmailControl.value;
      } else if (this.isSendSMS) {
        //Nếu là Gửi SMS
        this.resetCustomerCareByContactType('isSendSMS');

        if (!this.customerCareModel.IsSendNow) {
          this.customerCareModel.SendDate = convertToUTCTime(this.customerCareModel.SendDate);
        }
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

      // thoi gian ap dung
      this.customerCareModel.EffecttiveFromDate = convertToUTCTime(this.effecttiveFromDateControl.value);
      this.customerCareModel.EffecttiveToDate = convertToUTCTime(this.effecttiveToDateControl.value);

      // hinh thuc
      this.customerCareModel.CustomerCareContactType = this.customerCareContactTypeControl.value.categoryId;

      // trang thai
      this.customerCareModel.StatusId = this.statusIdControl.value.categoryId

      
      this.customerCareService.updateCustomerCare(this.customerCareModel, this.listCustomerId, 
        JSON.stringify(stringFilterModel), this.userId).subscribe(response => {
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

  //Copy bộ lọc khách hàng
  copyFilters() {
    let stringFilterModel = this.listFilterModel;
    this.router.navigate(['/customer/care-create', { 
      ListFilter: JSON.stringify(stringFilterModel)
    }]);
  }

  //Hiển thị màu theo trạng thái của Lead
  checkColor(statusId) {
    if (statusId) {
      let statusCode = this.customerCareCustomerStatusList.find(x => x.categoryId == statusId.toLowerCase()).categoryCode;
      switch (statusCode) {
        case 'CSO':
        //Chưa chăm sóc
          return 'status red';
        case 'DSO':
        //Đã chăm sóc
          return 'status blue';
      }
    }
  }

  //Thay đổi trạng thái của khách hàng
  async changedCustomerCareCustomerStatus(event: any, customerCareCustomerId: string) {
    let selectedData = {
      value: event.value,
      text: event.source.selected.viewValue
    };
    let cusCareContactTypeCode = this.listCustomerCareContactType.find(x => x.categoryId == this.customerCareModel.CustomerCareContactType).categoryCode;
    let statusCode = this.customerCareCustomerStatusList.find(x => x.categoryId == selectedData.value.toLowerCase()).categoryCode;
    if (cusCareContactTypeCode == 'Email' || cusCareContactTypeCode == 'SMS') {
      //Nếu Hình thức CSKH là Gửi email hoặc Gửi SMS
      if (statusCode == 'CSO') {
        //Nếu chuyển trạng thái từ Đã chăm sóc => Chưa chăm sóc thì
        this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: 'Không được chuyển trạng thái Đã chăm sóc => Chưa chăm sóc' });
      } else if (statusCode == 'DSO') {
        let result: any = await this.customerCareService.updateStatusCustomerCareCustomerAsync(customerCareCustomerId, selectedData.value, this.userId);
        if (result.statusCode !== 200) {
          // Thông báo thay đổi thành công || thay đổi thất bại
          this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: result.messageCode });
        } else {
          //Chuyển từ select sang label nếu thay đổi trạng thái từ Chưa chăm sóc => Đã chăm sóc
          this.listDataUser.find(item => item.customerCareCustomerId == customerCareCustomerId).isEditStatus = false;
          this.listDataUser.find(item => item.customerCareCustomerId == customerCareCustomerId).customerCareCustomerStatusName = "Đã chăm sóc";
          this.messageService.add({ severity: 'infor', summary: 'Thông báo:', detail: result.messageCode });
        }
      }
    } else {
      let result: any = await this.customerCareService.updateStatusCustomerCareCustomerAsync(customerCareCustomerId, selectedData.value, this.userId);
      if (result.statusCode !== 200) {
        // Thông báo thay đổi thành công || thay đổi thất bại
        this.messageService.add({ severity: 'error', summary: 'Thông báo:', detail: result.messageCode });
      } else {
        this.messageService.add({ severity: 'infor', summary: 'Thông báo:', detail: result.messageCode });
      }
    }
  }

  replate_token(token: string) {
    let current_content = this.customerCareContentEmailControl.value;
    current_content = current_content + token;
    this.customerCareContentEmailControl.setValue(current_content);
  }

  replate_token_sms(token: string) {
    this.customerCareModel.CustomerCareContentSMS = this.customerCareModel.CustomerCareContentSMS + token
  }

  goToCustomerDetail(customerId, contactId) {
    this.router.navigate(['/customer/detail', {customerId: customerId, contactId: contactId, defaultTab: 3}]);
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