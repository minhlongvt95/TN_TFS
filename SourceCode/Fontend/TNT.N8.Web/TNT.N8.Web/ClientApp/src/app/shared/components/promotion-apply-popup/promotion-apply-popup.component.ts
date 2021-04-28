import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PromotionService } from '../../../promotion/services/promotion.service';
import { DialogService } from 'primeng/dynamicdialog';
import { PromotionApply } from '../../../promotion/models/promotion-apply.model';
import { PromotionProductApply } from '../../../promotion/models/promotion-product-apply.model';
import { PromotionObjectApply } from '../../../promotion/models/promotion-object-apply.model';
import { PromotionProductMappingApplyPopupComponent } from '../promotion-product-mapping-apply-popup/promotion-product-mapping-apply-popup.component';
import { PromotionProductMappingApply } from '../../../promotion/models/promotion-product-mapping-apply.model';

@Component({
  selector: 'app-promotion-apply-popup',
  templateUrl: './promotion-apply-popup.component.html',
  styleUrls: ['./promotion-apply-popup.component.css'],
  providers: [ PromotionService, DialogService ]
})
export class PromotionApplyPopupComponent implements OnInit {
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  awaitResponse: boolean = false;
  conditionsType: number = null;
  customerId: string = null;
  totalAmountAfterVat: number = 0;
  productId: string = null;
  quantity: number = 0;
  cols1: any;
  cols2: any;
  listPromotionApply: Array<PromotionApply> = [];
  selectedPromotionApply: Array<PromotionApply> = [];
  listReshowPromotionApply: Array<PromotionObjectApply> = [];

  constructor(
    public ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private promotionService: PromotionService,
    private dialogService: DialogService,
  ) { 
    this.conditionsType = this.config.data.conditionsType;
    this.customerId = this.config.data.customerId;
    this.totalAmountAfterVat = this.config.data.totalAmountAfterVat;
    this.productId = this.config.data.productId ?? null;
    this.quantity = this.config.data.quantity ?? 0;
    this.listReshowPromotionApply = this.config.data.listReshowPromotionApply;
  }

  ngOnInit(): void {
    this.initTable();

    //Lấy chương trình khuyến mãi
    this.promotionService.getApplyPromotion(this.conditionsType, this.customerId, 
      this.totalAmountAfterVat, this.productId, this.quantity).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listPromotionApply = result.listPromotionApply;

        //Nếu hiển thị lại các quà tặng đã chọn
        if (this.listReshowPromotionApply.length > 0) {
          let listPromotionId = [...new Set(this.listReshowPromotionApply.map(x => x.promotionId))];

          this.listPromotionApply.forEach(item => {
            if (listPromotionId.includes(item.promotionId)) 
            {
              let listPromotionMappingId = this.listReshowPromotionApply.map(x => x.promotionMappingId);
              item.selectedPromotionProductApply = item.listPromotionProductApply.filter(x => listPromotionMappingId.includes(x.promotionMappingId));
              
              let promotionObject = this.listReshowPromotionApply.find(x => x.promotionId == item.promotionId);
              if (promotionObject.conditionsType == 2) 
              {
                //Mua hàng giảm giá hàng
                if (promotionObject.propertyType == 1) 
                {
                  item.selectedPromotionProductMappingApply = [];
                  promotionObject.selectedPromotionObjectApplyMapping.forEach(_product => {
                    let temp = item.listPromotionProductMappingApply.find(x => x.productId == _product.productId);
                    
                    let newProduct = new PromotionProductMappingApply();
                    newProduct.promotionMappingId = temp.promotionMappingId;
                    newProduct.productId = temp.productId;
                    newProduct.quantity = _product.quantity;
                    newProduct.productName = temp.productName;
                    newProduct.productUnitName = temp.productUnitName;

                    item.selectedPromotionProductMappingApply.push(newProduct);
                  });
                  
                  item.selectedDetail = promotionObject.selectedDetail;
                } 
                //Mua hàng tặng hàng
                else if (promotionObject.propertyType == 2) 
                {
                  item.selectedPromotionProductMappingApply = [];
                  let listGroup = this.listReshowPromotionApply.filter(x => x.promotionId == item.promotionId);
                  
                  listGroup.forEach(_product => {
                    let newProduct = new PromotionProductMappingApply();
                    newProduct.promotionMappingId = _product.promotionMappingId;
                    newProduct.productId = _product.productId;
                    newProduct.quantity = _product.soLuongTang;
                    newProduct.productName = _product.promotionProductName;
                    newProduct.productUnitName = _product.productUnitName;

                    item.selectedPromotionProductMappingApply.push(newProduct);
                  });

                  item.selectedDetail = promotionObject.selectedDetail;
                }
              }

              this.selectedPromotionApply = [...this.selectedPromotionApply, item];
            }
          });
        }
      }
      else {
        let msg = { severity: 'error', key: 'popup', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  initTable() {
    this.cols1 = [
      { field: 'promotionName', header: 'Chương trình khuyến mãi', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'propertyTypeName', header: 'Hình thức khuyến mãi', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'object', header: 'Chi tiết', width: '150px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];

    this.cols2 = [
      { field: 'promotionName', header: 'Chương trình khuyến mãi', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'propertyTypeName', header: 'Hình thức khuyến mãi', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'object', header: 'Chi tiết', width: '150px', textAlign: 'left', display: 'table-cell', color: '#f44336' },
      { field: 'actions', header: '', width: '60px', textAlign: 'center', display: 'table-cell', color: '#f44336' },
    ];
  }

  changePromotionProductApply() {

  }

  editProductApply(promotionApply: PromotionApply) {
    let ref = this.dialogService.open(PromotionProductMappingApplyPopupComponent, {
      data: { 
        propertyType: promotionApply.propertyType,
        soLuongTang: promotionApply.soLuongTang,
        chiChonMot: promotionApply.chiChonMot,
        listPromotionProductMappingApply: promotionApply.listPromotionProductMappingApply,
        selectedPromotionProductMappingApply: promotionApply.selectedPromotionProductMappingApply
      },
      header: 'Chọn sản phẩm',
      width: '40%',
      baseZIndex: 1032,
      contentStyle: {
        "min-height": "280px",
        "max-height": "600px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe(result => {
      if (result) {
        promotionApply.selectedPromotionProductMappingApply = [];
        promotionApply.selectedDetail = '';
        
        if (promotionApply.chiChonMot) 
        {
          let selectedPromotionProductMappingApply: PromotionProductMappingApply = result;
          promotionApply.selectedPromotionProductMappingApply.push(selectedPromotionProductMappingApply);
          promotionApply.selectedDetail = selectedPromotionProductMappingApply.productName + ' - Số lượng: ' + selectedPromotionProductMappingApply.quantity;
        }
        else 
        {
          let selectedPromotionProductMappingApply: Array<PromotionProductMappingApply> = result;
          promotionApply.selectedPromotionProductMappingApply = [...promotionApply.selectedPromotionProductMappingApply, ...selectedPromotionProductMappingApply];
          promotionApply.selectedPromotionProductMappingApply.forEach(item => {
            promotionApply.selectedDetail += '<p>' + item.productName + ' - Số lượng: ' + item.quantity + '</p>';
          });
        }
      }
    });
  }

  applyPromotion() {
    this.ref.close(this.selectedPromotionApply);
  }

  cancelApplyPromotion() {
    this.ref.close();
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

}
