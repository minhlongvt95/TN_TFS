import { Component, OnInit, HostListener, ViewChild, ElementRef, Renderer2, ChangeDetectorRef } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import * as $ from 'jquery';
import { PromotionService } from './../../services/promotion.service';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray, ValidatorFn } from '@angular/forms';
import { Table } from 'primeng/table';
import { FileUpload } from 'primeng/fileupload';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Promotion } from '../../models/promotion.model';
import { PromotionMapping } from '../../models/promotion-mapping.model';
import { PromotionProductMapping } from '../../models/promotion-product-mapping.model';

class TypeBoolean {
  name: string;
  value: boolean;
}

class TypeInt {
  name: string;
  value: number;
}

class TypeProperty {
  name: string;
  value: number;
  type: number;
}

class Product {
  productId: string;
  productName: string;
  productCode: string;
  productCodeName: string;
}

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
  {
    criteriaId: 9,
    criteriaName: 'Ngày tạo',
    field: 'CreatedDate'
  },
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
    conditionName: 'Bằng',
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
  {
    conditionId: 25,
    conditionName: 'Bằng',
    criteriaId: 9,
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
  selector: 'app-promotion-create',
  templateUrl: './promotion-create.component.html',
  styleUrls: ['./promotion-create.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class PromotionCreateComponent implements OnInit {
  loading: boolean = false;
  awaitResult: boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");

  /* Cố định thanh chứa button đầu trang */
  fixed: boolean = false;
  isShow: boolean = true;
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
  /* End */

  /* Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  /* End */

  /* Dữ liệu */
  listConditionsType: Array<TypeInt> = [
    { name: 'Theo khách hàng', value: 1 }, 
    { name: 'Theo sản phẩm', value: 2 }, 
    { name: 'Theo tổng giá trị sản phẩm', value: 3 }
  ];
  conditionsType: number = 1;
  listProduct: Array<Product> = [];

  selectedProduct: Product = null;
  listCustomerGroup: Array<any> = [];
  /* End */

  createForm: FormGroup;
  promotionCodeControl: FormControl;
  promotionNameControl: FormControl;
  descriptionControl: FormControl;
  activeControl: FormControl;
  effectiveDateControl: FormControl;
  expirationDateControl: FormControl;
  conditionsTypeControl: FormControl;
  propertyTypeControl: FormControl;
  notMultiplitionControl: FormControl;
  noteControl: FormControl;
  customerHasOrderControl: FormControl;

  /* Bộ lọc */
  filterForm: FormGroup;
  rowFilterList: FormArray;
  /* End */

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
  listValueCreatedDateForm: Array<number> = [];
  /*End*/

  listParentCriteria: Array<any> = [];  //Loại khách hàng
  listCriteria: Array<any> = [];

  listParentCondition: Array<any> = [];
  listCondition: Array<any> = [];

  listParentGroup: Array<any> = [];
  listGroup: Array<any> = [];

  listParentSex: Array<any> = [];
  listSex: Array<any> = [];

  listDropdownList: Array<any> = [];
  listProductFilter: Array<any> = [];
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
  listConvertCustomerGroup: Array<any> = [];

  //data
  listFilterModel: Array<any> = [];

  //total of index
  index: number = 0;

  get rowFilterFormGroup() {
    return this.filterForm.get('rowFilters') as FormArray;
  }

  //Sản phẩm khuyến mãi
  cols1: any;
  cols2: any;
  cols3: any;
  cols4: any;
  cols5: any;
  listPromotionMapping: Array<PromotionMapping> = [];
  listConditionsMapping: Array<TypeBoolean> = [
    { name: 'Và', value: true }, { name: 'Hoặc', value: false }
  ];
  listPropertyMapping: Array<TypeInt> = [
    { name: 'Quà tặng', value: 1 },
    { name: 'Phiếu giảm giá', value: 2 }
  ];

  listCommonPropertyType: Array<TypeProperty> = [
    { name: 'Tặng hàng', value: 1, type: 1 },
    { name: 'Phiếu giảm giá', value: 2, type: 1 },
    { name: 'Tặng hàng', value: 1, type: 3 },
    { name: 'Phiếu giảm giá', value: 2, type: 3 },
    { name: 'Mua hàng giảm giá hàng', value: 1, type: 2 },
    { name: 'Mua hàng tặng hàng', value: 2, type: 2 },
    { name: 'Mua hàng tặng phiếu giảm giá', value: 3, type: 2 },
  ];
  listPropertyType: Array<TypeProperty> = [];
  propertyType: number = null;

  constructor(
    private fb: FormBuilder,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private promotionService: PromotionService,
    private renderer: Renderer2,
    private ref: ChangeDetectorRef,
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
        if (this.saveAndCreate) {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target) &&
            !this.saveAndCreate.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        } else {
          if (!this.toggleButton.nativeElement.contains(e.target) &&
            !this.notifi.nativeElement.contains(e.target) &&
            !this.save.nativeElement.contains(e.target)) {
            this.isOpenNotifiError = false;
          }
        }
      }
    });
  }

  ngOnInit(): void {
    this.initTable();
    this.setForm();

    let resource = "crm/promotion/create/view";
    let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
    if (permission.status == false) {
      this.router.navigate(['/home']);
    }
    else {
      this.getMasterData();
    }
  }

  initTable() {
    //Theo khách hàng - tặng hàng
    this.cols1 = [
      { field: 'indexOrder', header: 'STT', width: '40px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'objectName', header: 'Hàng khuyến mãi', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'Số lượng tặng', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giá trị', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];

    //Theo tổng giá trị sản phẩm - tặng hàng
    this.cols2 = [
      { field: 'indexOrder', header: 'STT', width: '40px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soTienTu', header: 'Số tiền từ', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'objectName', header: 'Hàng khuyến mãi', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'Số lượng tặng', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giá trị', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];

    //Theo sản phẩm - Mua hàng giảm giá hàng
    this.cols3 = [
      { field: 'indexOrder', header: 'STT', width: '40px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'objectName', header: 'Sản phẩm mua', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongMua', header: 'SL mua', width: '60px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'object', header: 'Sản phẩm giảm giá', width: '180px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'SL giảm giá', width: '80px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'chiChonMot', header: 'Chỉ chọn 1', width: '85px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giảm giá', width: '115px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '120px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];

    //Theo sản phẩm - Mua hàng tặng hàng
    this.cols4 = [
      { field: 'indexOrder', header: 'STT', width: '40px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'objectName', header: 'Sản phẩm mua', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongMua', header: 'SL mua', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'object', header: 'Sản phẩm tặng', width: '180px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'SL tặng', width: '120px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'chiChonMot', header: 'Chỉ chọn 1', width: '85px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];

    //Theo sản phẩm - Mua hàng tặng phiếu giảm giá
    this.cols5 = [
      { field: 'indexOrder', header: 'STT', width: '40px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'objectName', header: 'Sản phẩm mua', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongMua', header: 'SL mua', width: '100px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'object', header: 'Hàng khuyến mãi', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'soLuongTang', header: 'SL giảm giá', width: '120px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'giaTri', header: 'Giá trị', width: '115px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'loaiGiaTri', header: 'Loại giá trị', width: '120px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '50px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];
  }

  setForm() {
    this.promotionCodeControl = new FormControl({value: null, disabled: true});
    this.promotionNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.descriptionControl = new FormControl(null);
    this.activeControl = new FormControl(true);
    this.effectiveDateControl = new FormControl(null, [Validators.required]);
    this.expirationDateControl = new FormControl(null, [Validators.required]);
    this.conditionsTypeControl = new FormControl(null);
    this.propertyTypeControl = new FormControl(null);
    this.notMultiplitionControl = new FormControl(false);
    this.noteControl = new FormControl(null);
    this.customerHasOrderControl = new FormControl(false);

    this.createForm = new FormGroup({
      promotionCodeControl: this.promotionCodeControl,
      promotionNameControl: this.promotionNameControl,
      descriptionControl: this.descriptionControl,
      activeControl: this.activeControl,
      effectiveDateControl: this.effectiveDateControl,
      expirationDateControl: this.expirationDateControl,
      conditionsTypeControl: this.conditionsTypeControl,
      propertyTypeControl: this.propertyTypeControl,
      notMultiplitionControl: this.notMultiplitionControl,
      noteControl: this.noteControl,
      customerHasOrderControl: this.customerHasOrderControl
    });

    this.filterForm = this.fb.group({
      rowFilters: this.fb.array([this.createFilter()])
    });

    // set rowFilterList to this field
    this.rowFilterList = this.filterForm.get('rowFilters') as FormArray;
  }

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
      valueListCustomerGroupControl: ['', Validators.compose([Validators.required])],
      valueCreatedDateControl: [null, Validators.compose([Validators.required])]
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

  getMasterData() {
    this.promotionService.getMasterDataCreatePromotion().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listProduct = result.listProduct;
        this.listProductFilter = result.listProduct;
        this.listCustomerGroup = result.listCustomerGroup;
        this.setDefaultValue();
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValue() {
    //Khuyến mãi theo
    this.conditionsTypeControl.setValue(this.listConditionsType[0]);
    this.conditionsType = this.listConditionsType[0].value;

    //Hình thức
    this.listPropertyType = this.listCommonPropertyType.filter(x => x.type == this.conditionsType);
    this.propertyTypeControl.setValue(this.listPropertyType[0]);
    this.propertyType = this.listPropertyType[0].value;

    /*Filter*/
    this.listCriteria = ELEMENT_CRITERIA;
    this.listGroup = ELEMENT_VALUE_GROUP;
    this.listSex = ELEMENT_VALUE_SEX;
    this.listBirthDay = ELEMENT_VALUE_BIRTHDAY;
    this.listCustomerGroup.forEach(item => {
      let option = {
        valueCustomerGroupId: item.categoryId,
        valueCustomerGroupName: item.categoryName
      }
      this.listConvertCustomerGroup.push(option);
    });

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
    /*End*/
  }

  /*Thay đổi loại điều kiện*/
  changeConditionsType(event: any) {
    let selectedConditionsType: TypeInt = this.conditionsTypeControl.value;
    this.conditionsType = selectedConditionsType.value;

    //reset list promotionMapping
    this.listPromotionMapping = [];

    //Lấy list hình thức theo loại điều kiện được chọn
    this.listPropertyType = this.listCommonPropertyType.filter(x => x.type == this.conditionsType);

    //SET giá trị mặc định của hình thức
    this.propertyTypeControl.setValue(this.listPropertyType[0]);
    this.propertyType = this.listPropertyType[0].value;
  }

  /*Thay đổi loại hình thức*/
  changePropertyType() {
    let hinhThuc: TypeProperty = this.propertyTypeControl.value;
    this.propertyType = hinhThuc.value;

    //reset list promotionMapping
    this.listPromotionMapping = [];
  }

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
      delete this.listFilterModel[index].valueCreatedDate;
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
    if (this.listValueCreatedDateForm[index] == 1)
    {
      this.listValueCreatedDateForm[index] = 0;
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
    } else if (selectedData.text === 'Ngày tạo') {
      this.listCondition = ELEMENT_CONDITION_CREATEDDATE;
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
      this.listDropdownList[index] = this.listProductFilter;
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
      this.listParentCustomerGroup[index] = this.listConvertCustomerGroup;

      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueCustomerGroup(index);

      //reset value của dropdown list
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueListCustomerGroupControl"].setValue(null)
    } else if (criteriaName == 'Ngày tạo' && conditionName == 'Bằng') {
      //reset lại các giá trị đã lưu của bộ lọc
      this.resetFormValueCreatedDate(index);

      //reset lại value của input
      var a = (<FormGroup>(<FormArray>this.filterForm.controls['rowFilters']).at(index)).controls["valueCreatedDateControl"].setValue(null);
      
      this.listValueCreatedDateForm[index] = 1;
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

  /*Event: Khi thay đổi giá trị của Ngày tạo*/
  changedValueCreatedDate(index, event: any) {
    let createdDate = (event.target.value);
    this.listFilterModel[index].valueCreatedDate = this.convertDateToString(createdDate); //convertToUTCTime(createdDate).toString()
  }
  /*End*/

  convertDateToString(data: Date) {
    let date = data.getDate();
    let month = data.getMonth() + 1;
    let year = data.getFullYear();

    let date_result = '';
    if (date < 10) 
    {
      date_result = '0' + date;
    }
    else 
    {
      date_result = date.toString();
    }

    let month_result = '';
    if (month < 10) 
    {
      month_result = '0' + month;
    }
    else 
    {
      month_result = month.toString();
    }

    return year + '-' + month_result + '-' + date_result;
  }

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
    this.listValueCreatedDateForm.splice(index, 1);

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

  resetFormValueCreatedDate(index) {
    this.listValueCreatedDateForm[index] = 0;
    delete this.listFilterModel[index].valueCreatedDate;
  }
  /*End*/

  /*Thêm 1 hàng trong bảng sản phẩm khuyến mãi*/
  addPromotionMapping() {
    let index = this.listPromotionMapping.length + 1;

    //Theo khách hàng
    if (this.conditionsType == 1) {
      let newItem = new PromotionMapping();
      newItem.indexOrder = index;
      newItem.soLuongTang = 1;
      newItem.loaiGiaTri = false;

      this.listPromotionMapping = [...this.listPromotionMapping, newItem];
    }
    //Theo sản phẩm
    else if (this.conditionsType == 2) {
      //Mua hàng giảm giá hàng
      if (this.propertyType == 1) {
        let newItem = new PromotionMapping();
        newItem.indexOrder = index;
        newItem.soLuongMua = 1;
        newItem.soLuongTang = 1;
        newItem.loaiGiaTri = false;

        this.listPromotionMapping = [...this.listPromotionMapping, newItem];
      }
      else if (this.propertyType == 2) {
        let newItem = new PromotionMapping();
        newItem.indexOrder = index;
        newItem.soLuongMua = 1;
        newItem.soLuongTang = 1;
        newItem.loaiGiaTri = false;

        this.listPromotionMapping = [...this.listPromotionMapping, newItem];
      } 
      else if (this.propertyType == 3) {
        let newItem = new PromotionMapping();
        newItem.indexOrder = index;
        newItem.soLuongMua = 1;
        newItem.soLuongTang = 1;
        newItem.loaiGiaTri = false;

        this.listPromotionMapping = [...this.listPromotionMapping, newItem];
      }
    }
    //Theo tổng giá trị sản phẩm
    else if (this.conditionsType == 3) {
      let newItem = new PromotionMapping();
      newItem.indexOrder = index;
      newItem.soLuongTang = 1;
      newItem.loaiGiaTri = false;

      this.listPromotionMapping = [...this.listPromotionMapping, newItem];

      this.getListDublicatePrice();
    }
  }

  /*Thay đổi loại giá trị*/
  changeLoaiGiaTri(promotionMapping: PromotionMapping, value: boolean) {
    promotionMapping.loaiGiaTri = value;
  }

  /*Xóa 1 sản phẩm khuyến mãi*/
  deletePromotionMapping(promotionMapping: PromotionMapping) {
    this.listPromotionMapping = this.listPromotionMapping.filter(x => x.indexOrder != promotionMapping.indexOrder);

    //set lại index
    this.listPromotionMapping.forEach((item, index) => {
      item.indexOrder = index + 1;
    });

    //Theo khách hàng
    if (this.conditionsType == 1) {
      //Báo lỗi các sản phẩm được chọn nhiều hơn 1 lần
      let listProductId = this.listPromotionMapping.map(item => item.selectedHangKhuyenMai?.productId).filter(y => y != undefined);

      let listDublicateProductId = this.getListDublicateProductId(listProductId);

      this.listPromotionMapping.forEach(item => {
        if (listDublicateProductId.includes(item.selectedHangKhuyenMai?.productId)) {
          item.error = true;
        }
        else {
          item.error = false;
        }
      });
    }
    //Theo sản phẩm
    else if (this.conditionsType == 2) {
      //Báo lỗi các sản phẩm được chọn nhiều hơn 1 lần
      let listProductId = this.listPromotionMapping.map(item => item.selectedSanPhamMua?.productId).filter(y => y != undefined);

      let listDublicateProductId = this.getListDublicateProductId(listProductId);

      this.listPromotionMapping.forEach(item => {
        if (listDublicateProductId.includes(item.selectedSanPhamMua?.productId)) {
          item.error = true;
        }
        else {
          item.error = false;
        }
      });
    }
    //Theo tổng giá trị sản phẩm
    else if (this.conditionsType == 3) {
      this.getListDublicatePrice();
    }
  }

  /*Thay đổi sản phẩm mua*/
  changeSanPhamMua(indexOrder: number) {
    let item = this.listPromotionMapping.find(x => x.indexOrder == indexOrder);
    
    item.sanPhamMua = item.selectedSanPhamMua.productId;

    //Báo lỗi các sản phẩm được chọn nhiều hơn 1 lần
    let listProductId = this.listPromotionMapping.map(item => item.selectedSanPhamMua?.productId).filter(y => y != undefined);

    let listDublicateProductId = this.getListDublicateProductId(listProductId);

    this.listPromotionMapping.forEach(item => {
      if (listDublicateProductId.includes(item.selectedSanPhamMua?.productId)) {
        item.error = true;
      }
      else {
        item.error = false;
      }
    });
  }

  /*Thay đổi Hàng khuyến mãi*/
  changeHangKhuyenMai(indexOrder: number) {
    let item = this.listPromotionMapping.find(x => x.indexOrder == indexOrder);
    
    item.hangKhuyenMai = item.selectedHangKhuyenMai.productId;

    //Nếu không phải theo tổng giá trị sản phẩm
    if (this.conditionsType != 3) {
      //Báo lỗi các sản phẩm được chọn nhiều hơn 1 lần
      let listProductId = this.listPromotionMapping.map(item => item.selectedHangKhuyenMai?.productId).filter(y => y != undefined);

      let listDublicateProductId = this.getListDublicateProductId(listProductId);

      this.listPromotionMapping.forEach(item => {
        if (listDublicateProductId.includes(item.selectedHangKhuyenMai?.productId)) {
          item.error = true;
        }
        else {
          item.error = false;
        }
      });
    }
  }

  /*Lấy list ProductId đã được chọn nhiều hơn 1 lần*/
  getListDublicateProductId(listProductId: Array<string>): Array<string> {
    let uniq = listProductId
      .map((productId) => {
        return {
          count: 1,
          productId: productId
        }
      })
      .reduce((a, b) => {
        a[b.productId] = (a[b.productId] || 0) + b.count
        return a
      }, {});

    let duplicates = Object.keys(uniq).filter((a) => uniq[a] > 1);

    return duplicates;
  }
  /*End*/

  /*Lấy list Price có giá trị đã được nhập nhiều hơn 1 lần giống nhau*/
  getListDublicatePrice() {
    let listPrice = this.listPromotionMapping.map(x => x.soTienTu.toString());

    let uniq = listPrice
      .map((price) => {
        return {
          count: 1,
          price: price
        }
      })
      .reduce((a, b) => {
        a[b.price] = (a[b.price] || 0) + b.count
        return a
      }, {});

    let duplicates = Object.keys(uniq).filter((a) => uniq[a] > 1);

    this.listPromotionMapping.forEach(item => {
      if (duplicates.includes(item.soTienTu.toString())) {
        item.error = true;
      }
      else {
        item.error = false;
      }
    });
  }
  /*End*/

  /*Thay đổi sản phẩm giảm giá*/
  changeSanPhamGiamGia(promotionMapping: PromotionMapping) {
    if (promotionMapping.selectedSanPhamGiamGia) {
      //Thêm vào list chips
      promotionMapping.selectedChips = promotionMapping.selectedSanPhamGiamGia.map(x => x.productCodeName);
    }
    else {
      promotionMapping.selectedChips = [];
    }
  }

  /*Xóa 1 sản phẩm trong list Chips*/
  removeChip(promotionMapping: PromotionMapping, event: any) {
    let productCodeName = event.value;
    promotionMapping.selectedSanPhamGiamGia = promotionMapping.selectedSanPhamGiamGia.filter(x => x.productCodeName != productCodeName);
  }

  /*Thay đổi sản phẩm tặng*/
  changeSanPhamTang(promotionMapping: PromotionMapping) {
    if (promotionMapping.selectedSanPhamTang) {
      //Thêm vào list chips
      promotionMapping.selectedChips = promotionMapping.selectedSanPhamTang.map(x => x.productCodeName);
    }
    else {
      promotionMapping.selectedChips = [];
    }
  }

  /*Xóa 1 sản phẩm trong list Chips*/
  removeChipSanPhamTang(promotionMapping: PromotionMapping, event: any) {
    let productCodeName = event.value;
    promotionMapping.selectedSanPhamTang = promotionMapping.selectedSanPhamTang.filter(x => x.productCodeName != productCodeName);
  }

  changeSoLuongTang(promotionMapping: PromotionMapping) {
    //Theo khách hàng
    if (this.conditionsType == 1) {
      if (promotionMapping.soLuongTang.toString() == '' || promotionMapping.soLuongTang.toString() == '0') {
        promotionMapping.soLuongTang = 1;
      }
    }
    //Theo sản phẩm
    else if (this.conditionsType == 2) {
      //Mua hàng giảm giá hàng
      if (this.propertyType == 1) {
        if (promotionMapping.soLuongTang.toString() == '' || promotionMapping.soLuongTang.toString() == '0') {
          promotionMapping.soLuongTang = 1;
        }
      }
      //Mua hàng tặng hàng
      else if (this.propertyType == 2) {
        if (promotionMapping.soLuongTang.toString() == '' || promotionMapping.soLuongTang.toString() == '0') {
          promotionMapping.soLuongTang = 1;
        }
      }
      //Mua hàng tặng phiếu giảm giá
      else if (this.propertyType == 3) {
        if (promotionMapping.soLuongTang.toString() == '' || promotionMapping.soLuongTang.toString() == '0') {
          promotionMapping.soLuongTang = 1;
        }
      }
    }
    //Theo tổng giá trị sản phẩm
    else if (this.conditionsType == 3) {
      if (promotionMapping.soLuongTang.toString() == '' || promotionMapping.soLuongTang.toString() == '0') {
        promotionMapping.soLuongTang = 1;
      }
    }
  }

  changeGiaTri(promotionMapping: PromotionMapping) {
    //Theo khách hàng
    if (this.conditionsType == 1) {
      if (promotionMapping.giaTri.toString() == '') {
        promotionMapping.giaTri = 0;
      }
    }
    //Theo sản phẩm
    else if (this.conditionsType == 2) {
      //Mua hàng giảm giá hàng
      if (this.propertyType == 1) {
        if (promotionMapping.giaTri.toString() == '') {
          promotionMapping.giaTri = 0;
        }
      }
      //Mua hàng tặng hàng
      else if (this.propertyType == 2) {
        
      }
      //Mua hàng tặng phiếu giảm giá
      else if (this.propertyType == 3) {
        if (promotionMapping.giaTri.toString() == '') {
          promotionMapping.giaTri = 0;
        }
      }
    }
    //Theo tổng giá trị sản phẩm
    else if (this.conditionsType == 3) {
      if (promotionMapping.giaTri.toString() == '') {
        promotionMapping.giaTri = 0;
      }
    }
  }

  changeSoLuongMua(promotionMapping: PromotionMapping) {
    if (promotionMapping.soLuongMua.toString() == '' || promotionMapping.soLuongMua.toString() == '0') {
      promotionMapping.soLuongMua = 1;
    }
  }

  changeSoTienTu(promotionMapping: PromotionMapping) {
    if (promotionMapping.soTienTu.toString() == '') {
      promotionMapping.soTienTu = 0;
    }

    this.getListDublicatePrice();
  }

  /*Tạo mới Chương trình khuyến mãi*/
  createPromotion(mode: boolean) {
    if (!this.createForm.valid) {
      Object.keys(this.createForm.controls).forEach(key => {
        if (this.createForm.controls[key].valid == false) {
          this.createForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.createForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } 
    else if (this.listPromotionMapping.length == 0) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa thêm thông tin khuyến mãi' };
      this.showMessage(msg);
    }
    else {
      //Nếu điều kiện theo khách hàng
      if (this.conditionsType == 1) {
        /*Kiểm tra lỗi list sản phẩm khuyến mãi*/
        let checkValidPromotionMapping = false;
        this.listPromotionMapping.forEach(item => {
          if (item.error) {
            checkValidPromotionMapping = true;
          }
        });
        /*End*/

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

        //Lấy theo bộ lọc
        if (this.listFilterModel.length > 0) {
          if (!this.validateFilters()) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa nhập đủ dữ liệu bộ tiêu chí' };
            this.showMessage(msg);
          }
          else if (checkValidPromotionMapping && this.propertyType == 1) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hàng khuyến mãi không hợp lệ' };
            this.showMessage(msg);
          }
          else {
            let errorHandleList = this.handleListPromotionMapping();

            if (errorHandleList) {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn hàng khuyến mãi' };
              this.showMessage(msg);
            }
            else {
              /*Xử lý bộ lọc thành câu query*/
              let query: any = this.basicRulesetToSQL(this.generateQuery(this.listFilterModel));

              /*Map data Promotion to model*/
              let promotion = new Promotion();
              promotion.promotionName = this.promotionNameControl.value.trim();
              promotion.effectiveDate = convertToUTCTime(this.effectiveDateControl.value);
              promotion.expirationDate = convertToUTCTime(this.expirationDateControl.value);
              promotion.description = this.descriptionControl.value?.trim();
              promotion.active = this.activeControl.value;
              promotion.conditionsType = this.conditionsTypeControl.value.value;
              promotion.propertyType = this.propertyType;
              promotion.filterContent = JSON.stringify(this.listFilterModel);
              promotion.filterQuery = query;
              promotion.notMultiplition = this.notMultiplitionControl.value;
              promotion.customerHasOrder = this.customerHasOrderControl.value;
              /*End*/

              /*Map data list PromotionMapping to model*/
              let newListPromotionMapping = this.mapDataPromotionMapping();
              /*End*/

              this.loading = true;
              this.awaitResult = true;
              this.promotionService.createPromotion(promotion, newListPromotionMapping).subscribe(response => {
                let result: any = response;
                if (result.statusCode == 200) {
                  if (mode) {
                    if (this.emitStatusChangeForm) {
                      this.emitStatusChangeForm.unsubscribe();
                    }
                    this.resetForm();
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                    this.showMessage(msg);

                    setTimeout(() => {
                      this.loading = false;
                      this.awaitResult = false;
                    }, 2000);
                  }
                  else {
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                    this.showMessage(msg);

                    setTimeout(() => {
                      this.router.navigate(['/promotion/detail', { promotionId: result.promotionId }]);
                    }, 2000);
                  }
                }
                else {
                  this.loading = false;
                  let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                  this.showMessage(msg);
                }
              });
            }
          }
        }
        //Lấy tất cả khách hàng
        else {
          if (checkValidPromotionMapping && this.propertyType == 1) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hàng khuyến mãi không hợp lệ' };
            this.showMessage(msg);
          }
          else {
            let errorHandleList = this.handleListPromotionMapping();

            if (errorHandleList) {
              let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn hàng khuyến mãi' };
              this.showMessage(msg);
            }
            else {
              /*Map data Promotion to model*/
              let promotion = new Promotion();
              promotion.promotionName = this.promotionNameControl.value.trim();
              promotion.effectiveDate = convertToUTCTime(this.effectiveDateControl.value);
              promotion.expirationDate = convertToUTCTime(this.expirationDateControl.value);
              promotion.description = this.descriptionControl.value?.trim();
              promotion.active = this.activeControl.value;
              promotion.conditionsType = this.conditionsTypeControl.value.value;
              promotion.propertyType = this.propertyType;
              promotion.notMultiplition = this.notMultiplitionControl.value;
              promotion.customerHasOrder = this.customerHasOrderControl.value;
              /*End*/

              /*Map data list PromotionMapping to model*/
              let newListPromotionMapping = this.mapDataPromotionMapping();
              /*End*/

              this.loading = true;
              this.awaitResult = true;
              this.promotionService.createPromotion(promotion, newListPromotionMapping).subscribe(response => {
                let result: any = response;
                if (result.statusCode == 200) {
                  if (mode) {
                    if (this.emitStatusChangeForm) {
                      this.emitStatusChangeForm.unsubscribe();
                    }
                    this.resetForm();
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                    this.showMessage(msg);

                    setTimeout(() => {
                      this.loading = false;
                      this.awaitResult = false;
                    }, 2000);
                  }
                  else {
                    let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                    this.showMessage(msg);

                    setTimeout(() => {
                      this.router.navigate(['/promotion/detail', { promotionId: result.promotionId }]);
                    }, 2000);
                  }
                }
                else {
                  this.loading = false;
                  let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                  this.showMessage(msg);
                }
              });
            }
          }
        }
      }
      //Nếu điều kiện theo sản phẩm
      else if (this.conditionsType == 2) {
        /*Kiểm tra lỗi list sản phẩm khuyến mãi*/
        let checkValidPromotionMapping = false;
        this.listPromotionMapping.forEach(item => {
          if (item.error) {
            checkValidPromotionMapping = true;
          }
        });
        /*End*/

        if (checkValidPromotionMapping) {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hàng khuyến mãi không hợp lệ' };
          this.showMessage(msg);
        }
        else {
          let errorHandleList = this.handleListPromotionMapping();

          if (errorHandleList) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn hàng khuyến mãi' };
            this.showMessage(msg);
          }
          else {
            /*Map data Promotion to model*/
            let promotion = new Promotion();
            promotion.promotionName = this.promotionNameControl.value.trim();
            promotion.effectiveDate = convertToUTCTime(this.effectiveDateControl.value);
            promotion.expirationDate = convertToUTCTime(this.expirationDateControl.value);
            promotion.description = this.descriptionControl.value?.trim();
            promotion.active = this.activeControl.value;
            promotion.conditionsType = this.conditionsTypeControl.value.value;
            promotion.propertyType = this.propertyType;
            promotion.notMultiplition = this.notMultiplitionControl.value;
            /*End*/

            /*Map data list PromotionMapping to model*/
            let newListPromotionMapping = this.mapDataPromotionMapping();
            /*End*/

            this.loading = true;
            this.awaitResult = true;
            this.promotionService.createPromotion(promotion, newListPromotionMapping).subscribe(response => {
              let result: any = response;
              if (result.statusCode == 200) {
                if (mode) {
                  if (this.emitStatusChangeForm) {
                    this.emitStatusChangeForm.unsubscribe();
                  }
                  this.resetForm();
                  let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                  this.showMessage(msg);

                  setTimeout(() => {
                    this.loading = false;
                    this.awaitResult = false;
                  }, 2000);
                }
                else {
                  let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                  this.showMessage(msg);

                  setTimeout(() => {
                    this.router.navigate(['/promotion/detail', { promotionId: result.promotionId }]);
                  }, 2000);
                }
              }
              else {
                this.loading = false;
                let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                this.showMessage(msg);
              }
            });
          }
        }
      }
      //Nếu điều kiện theo tổng giá trị sản phẩm
      else if (this.conditionsType == 3) {
        /*Kiểm tra lỗi list sản phẩm khuyến mãi*/
        let checkValidPromotionMapping = false;
        this.listPromotionMapping.forEach(item => {
          if (item.error) {
            checkValidPromotionMapping = true;
          }
        });
        /*End*/

        if (checkValidPromotionMapping) {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hàng khuyến mãi không hợp lệ' };
          this.showMessage(msg);
        }
        else {
          let errorHandleList = this.handleListPromotionMapping();

          if (errorHandleList) {
            let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Chưa chọn hàng khuyến mãi' };
            this.showMessage(msg);
          }
          else {
            /*Map data Promotion to model*/
            let promotion = new Promotion();
            promotion.promotionName = this.promotionNameControl.value.trim();
            promotion.effectiveDate = convertToUTCTime(this.effectiveDateControl.value);
            promotion.expirationDate = convertToUTCTime(this.expirationDateControl.value);
            promotion.description = this.descriptionControl.value?.trim();
            promotion.active = this.activeControl.value;
            promotion.conditionsType = this.conditionsTypeControl.value.value;
            promotion.propertyType = this.propertyType;
            promotion.notMultiplition = this.notMultiplitionControl.value;
            /*End*/

            /*Map data list PromotionMapping to model*/
            let newListPromotionMapping = this.mapDataPromotionMapping();
            /*End*/

            this.loading = true;
            this.awaitResult = true;
            this.promotionService.createPromotion(promotion, newListPromotionMapping).subscribe(response => {
              let result: any = response;
              if (result.statusCode == 200) {
                if (mode) {
                  if (this.emitStatusChangeForm) {
                    this.emitStatusChangeForm.unsubscribe();
                  }
                  this.resetForm();
                  let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                  this.showMessage(msg);

                  setTimeout(() => {
                    this.loading = false;
                    this.awaitResult = false;
                  }, 2000);
                }
                else {
                  let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Tạo thành công' };
                  this.showMessage(msg);

                  setTimeout(() => {
                    this.router.navigate(['/promotion/detail', { promotionId: result.promotionId }]);
                  }, 2000);
                }
              }
              else {
                this.loading = false;
                let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
                this.showMessage(msg);
              }
            });
          }
        }
      }
    }
  }

  /*validate filters*/
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
              case "Ngày tạo" : {
                let valueCreatedDate = item.valueCreatedDate;
                if (!valueCreatedDate) {
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

  /*Xử lý kết quả bộ lọc để generate ra query*/
  generateQuery(result: any) {
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
        case "Ngày tạo" : {
          let value = obj.valueCreatedDate;
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

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  /*Xử lý list khuyến mãi trước khi lưu*/
  handleListPromotionMapping(): boolean {
    let total = this.listPromotionMapping.length;
    let totalErr = 0;

    if (this.conditionsType == 1) {
      if (this.propertyType == 1) {
        this.listPromotionMapping.forEach(item => {
          if (!item.hangKhuyenMai) {
            totalErr++;
          }
        });
        
        if (total == totalErr) {
          //Nếu tất cả sản phẩm đều chưa chọn hàng khuyến mãi thì báo lỗi
          return true;
        }
        else {
          //Xóa những item chưa chọn hàng khuyến mãi
          this.listPromotionMapping = this.listPromotionMapping.filter(x => x.hangKhuyenMai != null && x.hangKhuyenMai != undefined);
          //set lại index
          this.listPromotionMapping.forEach((item, index) => {
            item.indexOrder = index + 1;
          });
          return false;
        }
      }
    }
    else if (this.conditionsType == 2) {
      this.listPromotionMapping.forEach(item => {
        if (!item.sanPhamMua) {
          totalErr++;
        }
      });
      
      if (total == totalErr) {
        //Nếu tất cả sản phẩm đều chưa chọn sản phẩm mua thì báo lỗi
        return true;
      }
      else {
        //Xóa những item chưa chọn sản phẩm mua
        this.listPromotionMapping = this.listPromotionMapping.filter(x => x.sanPhamMua != null && x.sanPhamMua != undefined);
        //set lại index
        this.listPromotionMapping.forEach((item, index) => {
          item.indexOrder = index + 1;
        });
        return false;
      }
    }
    else if (this.conditionsType == 3) {
      if (this.propertyType == 1) {
        this.listPromotionMapping.forEach(item => {
          if (!item.hangKhuyenMai) {
            totalErr++;
          }
        });
        
        if (total == totalErr) {
          //Nếu tất cả sản phẩm đều chưa chọn hàng khuyến mãi thì báo lỗi
          return true;
        }
        else {
          //Xóa những item chưa chọn hàng khuyến mãi
          this.listPromotionMapping = this.listPromotionMapping.filter(x => x.hangKhuyenMai != null && x.hangKhuyenMai != undefined);
          //set lại index
          this.listPromotionMapping.forEach((item, index) => {
            item.indexOrder = index + 1;
          });
          return false;
        }
      }
    }

    return false;
  }
  /*End*/

  /*Map data list PromotionMapping to model*/
  mapDataPromotionMapping() {
    let newListPromotionMapping: Array<PromotionMapping> = [];
    if (this.conditionsType == 1) {
      this.listPromotionMapping.forEach(item => {
        let newItem = new PromotionMapping();
        newItem.indexOrder = item.indexOrder;
        newItem.hangKhuyenMai = item.hangKhuyenMai;
        newItem.soLuongTang = ParseStringToFloat(item.soLuongTang.toString());
        newItem.giaTri = ParseStringToFloat(item.giaTri.toString());
        newItem.loaiGiaTri = item.loaiGiaTri;
  
        newListPromotionMapping.push(newItem);
      });
    }
    else if (this.conditionsType == 3) {
      this.listPromotionMapping.forEach(item => {
        let newItem = new PromotionMapping();
        newItem.indexOrder = item.indexOrder;
        newItem.soTienTu = ParseStringToFloat(item.soTienTu.toString());
        newItem.hangKhuyenMai = item.hangKhuyenMai;
        newItem.soLuongTang = ParseStringToFloat(item.soLuongTang.toString());
        newItem.giaTri = ParseStringToFloat(item.giaTri.toString());
        newItem.loaiGiaTri = item.loaiGiaTri;
  
        newListPromotionMapping.push(newItem);
      });
    }
    //Khuyến mãi theo Sản phẩm
    else if (this.conditionsType == 2) {
      //Mua hàng giảm giá hàng
      if (this.propertyType == 1) {
        this.listPromotionMapping.forEach(item => {
          let newItem = new PromotionMapping();
          newItem.indexOrder = item.indexOrder;
          newItem.sanPhamMua = item.sanPhamMua;
          newItem.soLuongMua = ParseStringToFloat(item.soLuongMua.toString());
          newItem.soLuongTang = ParseStringToFloat(item.soLuongTang.toString()); //Số lượng giảm giá
          newItem.chiChonMot = item.chiChonMot;
          newItem.giaTri = ParseStringToFloat(item.giaTri.toString());
          newItem.loaiGiaTri = item.loaiGiaTri;

          //Thêm list sản phẩm giảm giá
          item.selectedSanPhamGiamGia?.forEach(product => {
            let newProduct = new PromotionProductMapping();
            newProduct.productId = product.productId;

            newItem.listPromotionProductMapping.push(newProduct);
          });
    
          newListPromotionMapping.push(newItem);
        });
      }
      //Mua hàng tặng hàng
      else if (this.propertyType == 2) {
        this.listPromotionMapping.forEach(item => {
          let newItem = new PromotionMapping();
          newItem.indexOrder = item.indexOrder;
          newItem.sanPhamMua = item.sanPhamMua;
          newItem.soLuongMua = ParseStringToFloat(item.soLuongMua.toString());
          newItem.soLuongTang = ParseStringToFloat(item.soLuongTang.toString()); //Số lượng giảm giá
          newItem.chiChonMot = item.chiChonMot;

          //Thêm list sản phẩm tặng
          item.selectedSanPhamTang?.forEach(product => {
            let newProduct = new PromotionProductMapping();
            newProduct.productId = product.productId;

            newItem.listPromotionProductMapping.push(newProduct);
          });
    
          newListPromotionMapping.push(newItem);
        });
      }
      //Mua hàng tặng phiếu giảm giá
      else if (this.propertyType == 3) {
        this.listPromotionMapping.forEach(item => {
          let newItem = new PromotionMapping();
          newItem.indexOrder = item.indexOrder;
          newItem.sanPhamMua = item.sanPhamMua;
          newItem.soLuongMua = ParseStringToFloat(item.soLuongMua.toString());
          newItem.soLuongTang = ParseStringToFloat(item.soLuongTang.toString()); //Số lượng giảm giá
          newItem.giaTri = ParseStringToFloat(item.giaTri.toString());
          newItem.loaiGiaTri = item.loaiGiaTri;
    
          newListPromotionMapping.push(newItem);
        });
      }
    }

    return newListPromotionMapping;
  }

  resetForm() {
    this.createForm.reset();

    this.activeControl.setValue(true);

    //Khuyến mãi theo
    this.conditionsTypeControl.setValue(this.listConditionsType[0]);
    this.conditionsType = 1;

    //Hình thức
    this.listPropertyType = this.listCommonPropertyType.filter(x => x.type == this.conditionsType);
    this.propertyTypeControl.setValue(this.listPropertyType[0]);
    this.propertyType = this.listPropertyType[0].value;

    //reset bộ tiêu chí
    let totalRow = this.index;
    for (let i = 0; i < totalRow + 1; i++) {
      this.removeFilter(this.index);
    }

    //reset list sản phẩm khuyến mãi
    this.listPromotionMapping = [];
  }

  goBack() {
    this.router.navigate(['/promotion/list']);
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }

}

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = String(str).replace(/,/g, '');
  return parseFloat(str);
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

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
}
