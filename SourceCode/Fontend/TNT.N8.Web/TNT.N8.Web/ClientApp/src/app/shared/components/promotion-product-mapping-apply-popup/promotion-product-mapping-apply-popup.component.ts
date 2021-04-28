import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { PromotionProductMappingApply } from '../../../promotion/models/promotion-product-mapping-apply.model';

@Component({
  selector: 'app-promotion-product-mapping-apply-popup',
  templateUrl: './promotion-product-mapping-apply-popup.component.html',
  styleUrls: ['./promotion-product-mapping-apply-popup.component.css'],
  providers: [ 
    DialogService,
    MessageService
  ]
})
export class PromotionProductMappingApplyPopupComponent implements OnInit {
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();
  cols: any;
  propertyType: number = 0;
  headerTypeName: string = null;
  soLuongTang: number = 0;
  chiChonMot: boolean = false;
  listPromotionProductMappingApply: Array<PromotionProductMappingApply> = [];
  selectedPromotionProductMappingApply: any;

  constructor(
    private dialogService: DialogService,
    public _ref: DynamicDialogRef,
    public _config: DynamicDialogConfig,
    private messageService: MessageService,
  ) {
    this.propertyType = this._config.data.propertyType;
    this.headerTypeName = this.propertyType == 1 ? 'Tổng số lượng giảm giá tối đa' : 'Tổng số lượng sản phẩm được tặng';
    this.soLuongTang = this._config.data.soLuongTang;
    this.chiChonMot = this._config.data.chiChonMot;
    this.listPromotionProductMappingApply = this._config.data.listPromotionProductMappingApply;

    let result = this._config.data.selectedPromotionProductMappingApply;
    if (this.chiChonMot) {
      if (result.length > 0) {
        let selected = {
          promotionMappingId: result[0].promotionMappingId,
          productId: result[0].productId,
          quantity: result[0].quantity,
          productName: result[0].productName,
          productUnitName: result[0].productUnitName,
        };

        let changeQuanity = this.listPromotionProductMappingApply.find(x => x.productId == selected.productId);
        changeQuanity.quantity = selected.quantity;
  
        this.selectedPromotionProductMappingApply = selected;
      }
    }
    else {
      this.listPromotionProductMappingApply.forEach(item => {
        let product = result.find(x => x.promotionMappingId == item.promotionMappingId);
        if (product) {
          item.quantity = product.quantity;
        }
      });

      this.selectedPromotionProductMappingApply = result;
    }
  }

  ngOnInit(): void {
    this.initTable();
  }

  initTable() {
    this.cols = [
      { field: 'productName', header: 'Sản phẩm', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'quantity', header: 'Số lượng', width: '100px', textAlign: 'right', display: 'table-cell', color: '#f44336' },
    ];
  }

  changeQuantity(rowData: PromotionProductMappingApply) {
    if (rowData.quantity.toString() == '') {
      rowData.quantity = 0;
    }
  }

  cancelApplyPromotion() {
    this._ref.close();
  }

  applyPromotion() {
    let isValid = this.validForm();

    if (!isValid) 
    {
      let msg = { severity: 'error', key: 'popupInner', summary: 'Thông báo:', detail: 'Số lượng không hợp lệ' };
      this.showMessage(msg);
    }
    else 
    {
      if (this.chiChonMot) 
      {
        let result = {
          promotionMappingId: this.selectedPromotionProductMappingApply.promotionMappingId,
          productId: this.selectedPromotionProductMappingApply.productId,
          quantity: this.selectedPromotionProductMappingApply.quantity,
          productName: this.selectedPromotionProductMappingApply.productName,
          productUnitName: this.selectedPromotionProductMappingApply.productUnitName
        };

        this._ref.close(result);
      }
      else 
      {
        let listResult = [];
        this.selectedPromotionProductMappingApply.forEach(item => {
          let result = {
            promotionMappingId: item.promotionMappingId,
            productId: item.productId,
            quantity: item.quantity,
            productName: item.productName,
            productUnitName: item.productUnitName
          };

          listResult.push(result);
        });

        this._ref.close(listResult);
      }
    }
  }

  validForm() {
    let isValid = true;

    if (this.chiChonMot) 
    {
      if (this.selectedPromotionProductMappingApply?.quantity) 
      {
        let quantity = ParseStringToFloat(this.selectedPromotionProductMappingApply.quantity.toString());

        if (quantity > this.soLuongTang || quantity == 0) 
        {
          isValid = false;
        }
      }
      else 
      {
        isValid = false;
      }
    }
    else 
    {
      let temp = 0;
      this.selectedPromotionProductMappingApply.forEach(item => {
        temp += ParseStringToFloat(item.quantity.toString());
      });

      if (temp > this.soLuongTang || temp == 0) 
      {
        isValid = false;
      }
    }

    return isValid;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => 
      systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}

function ParseStringToFloat(str: any) {
  if (str === "") return 0;
  str = String(str).replace(/,/g, '');
  return parseFloat(str);
}
