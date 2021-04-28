import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn, AbstractControl } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';

import { VendorService } from '../../services/vendor.service';
import { MessageService } from 'primeng/api';

interface CostModel {
  index: number,
  costId: string,
  costCode: string,
  costName: string,
  costCodeName: string,
  unitPrice: any,
  statusId: string,
  organizationId: string
}

interface ResultCostDialog {
  status: boolean,  //Lưu thì true, Hủy là false
  costModel: CostModel,
}

@Component({
  selector: 'app-popup-add-edit-cost-vendor-order',
  templateUrl: './popup-add-edit-cost-vendor-order.component.html',
  styleUrls: ['./popup-add-edit-cost-vendor-order.component.css']
})
export class PopupAddEditCostVendorOrderComponent implements OnInit {
  auth: any = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  loading: boolean = false;

  isCreate: boolean = true;
  listCost: Array<CostModel> = [];
  currentIndex: number = 0;
  costModel: CostModel = {
    index: 0,
    costId: '',
    costCode: '',
    costName: '',
    costCodeName: '',
    unitPrice: 0,
    statusId: '',
    organizationId: ''
  }

  /*Define Form*/
  costForm: FormGroup;

  costControl: FormControl;
  costNameControl: FormControl;
  unitPriceControl: FormControl;
  /*End*/

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private vendorService: VendorService,
    private messageService: MessageService
  ) { 
    this.isCreate = this.config.data.isCreate;
    this.currentIndex = this.config.data.currentIndex;

    if (!this.isCreate) {
      this.costModel = this.config.data.costModel;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  ngOnInit() {
    this.setForm();

    this.loading = true;
    this.vendorService.getDataAddEditCostVendorOrder().subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listCost = result.listCost;

        //Nếu là sửa Chi phí thì map lại data vào form
        if (!this.isCreate) {
          let currentCost = this.listCost.find(x => x.costId == this.costModel.costId);
          this.costControl.setValue(currentCost);
          
          this.costNameControl.setValue(this.costModel.costName);
          this.unitPriceControl.setValue(this.costModel.unitPrice.toString());
        }
      } else {
        let msg = {key: 'popup', severity:'error', summary: 'Thông báo:', detail: result.messageCode};
        this.showMessage(msg);
      }
    });
  }

  setForm() {
    this.costControl = new FormControl(null, [Validators.required]);
    this.costNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.unitPriceControl = new FormControl('0', [Validators.required]);

    this.costForm = new FormGroup({
      costControl: this.costControl,
      costNameControl: this.costNameControl,
      unitPriceControl: this.unitPriceControl,
    });
  }

  changeCost(event: any) {
    if (event) {
      let costName = event.costName;
      this.costNameControl.setValue(costName);
    } else {
      this.costNameControl.setValue('');
    }
  }

  cancel() {
    let result: ResultCostDialog = {
      status: false,  //Hủy
      costModel: null
    };

    this.ref.close(result);
  }

  save() {
    if (!this.costForm.valid) {
      Object.keys(this.costForm.controls).forEach(key => {
        if (this.costForm.controls[key].valid == false) {
          this.costForm.controls[key].markAsTouched();
        }
      });
    } else {
      if (this.isCreate) {
        let cost: CostModel = this.costControl.value;

        let result: ResultCostDialog = {
          status: true,  //Lưu
          costModel: {
            index: this.currentIndex + 1,
            costId: cost.costId,
            costCode: cost.costCode,
            costName: this.costNameControl.value,
            costCodeName: cost.costCodeName,
            unitPrice: ParseStringToFloat(this.unitPriceControl.value),
            organizationId: cost.organizationId,
            statusId: cost.statusId
          }
        };
        
        this.ref.close(result);
      } else {
        let cost: CostModel = this.costControl.value;

        let result: ResultCostDialog = {
          status: true,  //Lưu
          costModel: {
            index: this.currentIndex,
            costId: cost.costId,
            costCode: cost.costCode,
            costName: this.costNameControl.value,
            costCodeName: cost.costCodeName,
            unitPrice: ParseStringToFloat(this.unitPriceControl.value),
            organizationId: cost.organizationId,
            statusId: cost.statusId
          }
        };
    
        this.ref.close(result);
      }
    }
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

}

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
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