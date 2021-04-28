import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { ProductService } from '../../services/product.service';

class Note {
  public code: string;
  public name: string;
}

class InventoryReport {
  public warehouseId: string;
  public productId: string;
  public warehouseNameByLevel: string;
  public quantityMinimum: string;
  public quantityMaximum: string;
  public startQuantity: string;
  public openingBalance: string;
  public listSerial: Array<serialModel>;
  public note: string;

  constructor() {
    this.listSerial = [];
  }
}

class serialModel {
  public serialId: string;
  public productId: string;
  public warehouseId: string;
  public serialCode: string;
  public statusId: string;
  public createdDate: Date;

  public status: boolean;
  public listNote: Array<string>;

  constructor() {
    this.serialId = '00000000-0000-0000-0000-000000000000';
    this.statusId = '00000000-0000-0000-0000-000000000000';
  }
}

interface DialogResult {
  status: boolean;
  inventoryReport: InventoryReport;
}

@Component({
  selector: 'app-add-edit-serial',
  templateUrl: './add-edit-serial.component.html',
  styleUrls: ['./add-edit-serial.component.css'],
  providers: [DialogService]
})
export class AddEditSerialComponent implements OnInit {
  listNote: Array<Note> = [
    { code: "required_serial", name: "Không được để trống" },
    { code: "exist_serial", name: "Số serial đã tồn tại trong hệ thống" },
    { code: "duplicate_inform", name: "Số serial đã tồn tại trong danh sách" },
    { code: "duplicate_other_warehouse", name: "Số serial đã tồn tại ở kho khác" },
  ];
  loading: boolean = false;
  rows: number = 10;
  cols: any
  listSerial: Array<serialModel> = [];
  //routing data
  selectedInventoryReport: InventoryReport = null;
  listInventory: Array<InventoryReport> = [];
  //master data
  listSerialCodeMasterdata: Array<string> = [];
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private productService: ProductService,
    private messageService: MessageService
  ) { 
    this.selectedInventoryReport = this.config.data.inventoryReport;
    this.listInventory = this.config.data.listInventory;
    if (this.selectedInventoryReport.listSerial.length > 0) {
      this.listSerial = [...this.selectedInventoryReport.listSerial];
    }
  }

  ngOnInit() {
    this.initTable();
    this.getMasterdata();
  }

  initTable() {
    this.cols = [
      { field: 'serialCode', header: 'Serial', textAlign: 'left', width: '50px' },
      { field: 'createdDate', header: 'Ngày nhập', textAlign: 'right', width: '50px' },
      { field: 'status', header: 'Status', textAlign: 'center', width: '20px' },
      { field: 'note', header: 'Ghi chú', textAlign: 'left', width: '150px' },
    ];
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.productService.addSerialNumber(this.selectedInventoryReport.productId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listSerialCodeMasterdata = result.listSerialNumber;
    } else {
      let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
      this.clearMessage();
      this.showMessage(msg);
      return;
    }
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clearMessage() {
    this.messageService.clear();
  }

  onAddSerial() {
    if (Number(this.selectedInventoryReport.startQuantity) === 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Số lượng đầu kỳ bằng 0' };
      this.clearMessage();
      this.showMessage(msg);
      return;
    }

    let compare = Number(this.selectedInventoryReport.startQuantity) - this.listSerial.length;
    if (compare === 0) {
      let msg = { severity: 'warn', summary: 'Thông báo', detail: 'Đã nhập đủ Serial' };
      this.clearMessage();
      this.showMessage(msg);
      return;
    }
    //valid
    let newSerial: serialModel = new serialModel();
    newSerial.serialCode = "";
    newSerial.createdDate = new Date();

    this.listSerial = [...this.listSerial, newSerial];
  }

  deleteSerial(rowData: serialModel) {
    this.listSerial = this.listSerial.filter(e => e != rowData);
  }

  onCancel() {
    let dialogResult: DialogResult = {
      status: false,
      inventoryReport: null
    }
    this.ref.close(dialogResult);
  }

  checkStatus(): boolean {
    let dataStatus = true;
    this.listSerial.forEach(rowData => {
      //reset status
      rowData.status = true;
      rowData.listNote = [];
      //check empty string
      if (rowData.serialCode.trim() === '') {
        rowData.status = false;
        rowData.listNote = [...rowData.listNote, this.listNote.find(e => e.code == 'required_serial').name];
      }
      //check serial with database
      let duplicateDatabase = this.listSerialCodeMasterdata.find(e => e === rowData.serialCode.trim());
      if (duplicateDatabase !== undefined) {
        rowData.status = false;
        rowData.listNote = [...rowData.listNote, this.listNote.find(e => e.code == 'exist_serial').name];
      }
      //check serial in form
      let otherSerial = this.listSerial.filter(e => e !== rowData && rowData.serialCode.trim() !== '').map(e => e.serialCode.trim());
      let duplicateInForm = otherSerial.find(e => e === rowData.serialCode);
      if (duplicateInForm !== undefined) {
        rowData.status = false;
        rowData.listNote = [...rowData.listNote, this.listNote.find(e => e.code == 'duplicate_inform').name];
      }
      //check serial with other warehouse
      let otherWarehouse: Array<InventoryReport> = this.listInventory.filter(e => e.warehouseId !== this.selectedInventoryReport.warehouseId);
      otherWarehouse.forEach(warehouse => {
        warehouse.listSerial.forEach(serial => {
          if (serial.serialCode.trim() == rowData.serialCode.trim()) {
            rowData.status = false;
            rowData.listNote = [...rowData.listNote, this.listNote.find(e => e.code == 'duplicate_other_warehouse').name];
          }
        });
      });
      //check dataStatus
      if (rowData.status === false) dataStatus = false;
    }); 
    return dataStatus;
  }

  onSave() {
    let dataStatus = this.checkStatus();
    if (dataStatus === true) {
      let validProductSerialModel: InventoryReport = new InventoryReport();
      validProductSerialModel.warehouseId = this.selectedInventoryReport.warehouseId;
      validProductSerialModel.warehouseNameByLevel = this.selectedInventoryReport.warehouseNameByLevel;
      validProductSerialModel.startQuantity = this.selectedInventoryReport.startQuantity;
      validProductSerialModel.listSerial = this.listSerial.map((e) => {
        let newSerial = new serialModel();
        newSerial.serialId = e.serialId;
        newSerial.statusId = e.statusId;
        newSerial.serialCode = e.serialCode;
        newSerial.createdDate = e.createdDate;
        return newSerial;
      });

      let dialogResult: DialogResult = {
        status: true,
        inventoryReport: validProductSerialModel
      }
      this.ref.close(dialogResult);

    } else {
      let msg = { severity: 'error', summary: 'Thông báo', detail: 'Danh sách Serial không hợp lệ' };
      this.clearMessage();
      this.showMessage(msg);
      return;
    }
  }

}
