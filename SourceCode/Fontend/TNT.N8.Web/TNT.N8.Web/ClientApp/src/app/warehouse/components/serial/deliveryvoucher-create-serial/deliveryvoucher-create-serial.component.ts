import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { FormControl, Validators, FormGroup, FormBuilder } from '@angular/forms';
import { WarehouseService } from '../../../services/warehouse.service';
import * as XLSX from 'xlsx';
import { SerialModel } from '../../../models/serials.model';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';

type AOA = any[][];

@Component({
  selector: 'app-deliveryvoucher-create-serial',
  templateUrl: './deliveryvoucher-create-serial.component.html',
  styleUrls: ['./deliveryvoucher-create-serial.component.css']
})
export class DeliveryvoucherCreateSerialComponent implements OnInit {
  popupForm: FormGroup;
  serialControl: FormControl;
  listSerialReturn: Array<SerialModel> = [];
  lstSerialGet: Array<any> = [];
  listserial = [];
  listserialProduct = [];
  dateNow: any;
  displayedColumns = ['serial'];
  dataObject: any;
  displayDialog: Boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  rows = 10;
  selectSerial: Array<any> = [];
  lstSerialOld: Array<any> = [];

  constructor(
    public snackBar: MatSnackBar,
    public dialog: MatDialog,
    public warehouseService: WarehouseService,
    private confirmationService: ConfirmationService,
    public messageService: MessageService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.dataObject = this.config.data.object;
    if (this.dataObject.listSerial != null) {
      this.lstSerialOld.push.apply(this.lstSerialOld, this.dataObject.listSerial);
    };

    this.serialControl = new FormControl('', [Validators.required]);
    this.dateNow = Date.now();
    this.getMasterData();
  }
  async getMasterData() {
    let resultGetTop10: any = await this.warehouseService.getSerialAsync(this.dataObject.wareHouseId, this.dataObject.productId);
    this.lstSerialGet = resultGetTop10.lstSerial;
  };

  addRowSerial() {
    if (this.dataObject.listSerial == null) { this.dataObject.listSerial = [] };
    if (this.dataObject.listSerial.length >= this.dataObject.quantity) {
      alert("Số serial nhập chỉ được nhỏ hơn hoặc bằng số lượng nhập");
    }
    else {
      this.dataObject.totalSerial += 1;

      var serial: SerialModel = {
        SerialId: this.emptyGuid,
        SerialCode: '',
        ProductId: this.dataObject.productId,
        StatusId: this.emptyGuid,
        Active: true,
        WarehouseId: this.dataObject.wareHouseId,
        CreatedDate: new Date()
      }


      this.dataObject.listSerial.unshift(serial);
    }
  }

  setValueSerial(stt: number) {
    this.listserial[stt - 1].serial = this.serialControl.value;
  }

  download() {
    this.warehouseService.downloadTemplateSerial().subscribe(response => {
      const result = <any>response;
      if (result.excelFile != null && result.statusCode === 202 || result.statusCode === 200) {
        const binaryString = window.atob(result.excelFile);
        const binaryLen = binaryString.length;
        const bytes = new Uint8Array(binaryLen);
        for (let idx = 0; idx < binaryLen; idx++) {
          const ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        const blob = new Blob([bytes], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        const fileName = result.nameFile + ".xlsm";
        link.download = fileName;
        link.click();

      } else {

      }

    }, error => { });
  }

  wopts: XLSX.WritingOptions = { bookType: 'xlsx', type: 'array' };

  onFileChange(evt: any) {
    /* wire up file reader */
    const target: DataTransfer = <DataTransfer>(evt.target);
    if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

      /* grab first sheet */
      const wsname: string = wb.SheetNames[0];
      const ws: XLSX.WorkSheet = wb.Sheets[wsname];

      /* save data */
      let data = <AOA>(XLSX.utils.sheet_to_json(ws, { header: 1 }));
      //check duplicate trong file excel
      let DataNoDuplicate = Array.from(new Set(data.map(a => a[0])));
      if (data.length > DataNoDuplicate.length) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "File excel tồn tại số serial trùng nhau", detail: 'Thêm Serial' });
        return;
      }
      if (this.lstSerialGet.length == 0) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Không có số Serial cho sản phẩm này trong kho nhập", detail: 'Thêm Serial' });
        return;
      }
      //else {
      //  for (var i = 1; i < data.length; ++i) {
      //    let checkexistSerial = this.lstSerialGet.find(f => f.serialCode == data[i][0]);
      //    if (checkexistSerial == null) {
      //      this.messageService.clear();
      //      this.messageService.add({ key: 'error', severity: 'error', summary: "Có số Serial trong file excel không tồn tại trong kho nhập", detail: 'Thêm Serial' });
      //      return;
      //    }
      //  }
      //}
      if (this.dataObject.listSerial == null || this.dataObject.listSerial.length == 0) {
        this.dataObject.listSerial = []

        if ((data.length - 1) > this.dataObject.quantity) {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: "Số serial nhập vượt quá số lượng nhập", detail: 'Thêm Serial' });
        }
        else {

          if ((data.length - 1 + this.dataObject.totalSerial) > this.dataObject.quantity) {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: "Tổng số serial đã nhập và trong file excel vượt quá số lượng nhập", detail: 'Thêm Serial' });
            return;
          }

          for (let i = 1; i < data.length; i++) {
            let checkexistSerial = this.lstSerialGet.find(f => f.serialCode == data[i][0]);
            if (checkexistSerial != null) {
              var serial: SerialModel = {
                SerialId: checkexistSerial.serialId,
                SerialCode: data[i][0],
                ProductId: this.dataObject.productId,
                StatusId: this.emptyGuid,
                Active: true,
                WarehouseId: this.dataObject.wareHouseId,
                CreatedDate: new Date()
              }
              this.dataObject.listSerial.unshift(serial);
            }
            else {
              this.messageService.clear();
              this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial " + data[i][0]+" trong file excel không tồn tại trong kho nhập", detail: 'Thêm Serial' });
            }
          }
          this.dataObject.totalSerial = this.dataObject.listSerial.length;
        }
      }
      else {
        if ((data.length - 1) > this.dataObject.quantity) {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: "Số serial nhập vượt quá số lượng nhập", detail: 'Thêm Serial' });
          return;
        }

        if ((data.length - 1 + this.dataObject.totalSerial) > this.dataObject.quantity) {
          this.messageService.clear();
          this.messageService.add({ key: 'error', severity: 'error', summary: "Tổng số serial đã nhập và trong file excel vượt quá số lượng nhập", detail: 'Thêm Serial' });
          return;
        }

        for (let i = 1; i < data.length; i++) {
          let checkExistSerialCode = this.dataObject.listSerial.find(f => f.SerialCode == data[i][0]);
          if (checkExistSerialCode == null) {
            let checkexistSerial = this.lstSerialGet.find(f => f.serialCode == data[i][0]);
            if (checkexistSerial != null) {
              var serial: SerialModel = {
                SerialId: checkexistSerial.serialId,
                SerialCode: data[i][0],
                ProductId: this.dataObject.productId,
                StatusId: this.emptyGuid,
                Active: true,
                WarehouseId: this.dataObject.wareHouseId,
                CreatedDate: new Date()
              }
              this.dataObject.listSerial.unshift(serial);
            }
            else {
              this.messageService.clear();
              this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial " + data[i][0] + " trong file excel không tồn tại trong kho nhập", detail: 'Thêm Serial' });
            }
          }
          //else {
          //  this.messageService.clear();
          //  this.messageService.add({ key: 'info', severity: 'info', summary: "Serial code " + data[i][0] + " đã có", detail: 'Thêm Serial' });
          //}
        }

        this.dataObject.totalSerial = this.dataObject.listSerial.length;

      };

    };
    reader.readAsBinaryString(target.files[0]);
  }

  onCancelClick() {

    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.dataObject.listSerial = [];
        this.dataObject.listSerial.push.apply(this.dataObject.listSerial, this.lstSerialOld);
        this.dataObject.totalSerial = this.lstSerialOld.length;
        this.ref.close();
      },
      reject: () => {
        return;
      }
    });

  }

  onSaveClick() {

    if (this.dataObject.listSerial != null) {
      if (this.dataObject.listSerial.length > this.dataObject.quantity) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng row có dữ liệu không được lớp hơn số lượng nhập", detail: 'Thêm Serial' });
        return;
      }
      this.dataObject.totalSerial = this.dataObject.listSerial.length;
    }
    this.ref.close();
  }
  pageChange(event: any) {
  }
  closePanelSerial() {
    if (this.dataObject.listSerial == null) {
      this.dataObject.listSerial = [];
      for (var i = 0; i < this.selectSerial.length; ++i) {
        var serial: SerialModel = {
          SerialId: this.selectSerial[i].serialId,
          SerialCode: this.selectSerial[i].serialCode,
          ProductId: this.dataObject.productId,
          StatusId: this.emptyGuid,
          Active: true,
          WarehouseId: this.dataObject.wareHouseId,
          CreatedDate: new Date()
        }
        this.dataObject.listSerial.unshift(serial);
      }
      this.dataObject.totalSerial = this.dataObject.listSerial.length;
    }
    else {
      for (var i = 0; i < this.selectSerial.length; ++i) {
        let checkExistSerialCode = this.dataObject.listSerial.find(f => f.SerialCode == this.selectSerial[i].serialCode);
        if (checkExistSerialCode == null) {
          var serial: SerialModel = {
            SerialId: this.selectSerial[i].serialId,
            SerialCode: this.selectSerial[i].serialCode,
            ProductId: this.dataObject.productId,
            StatusId: this.emptyGuid,
            Active: true,
            WarehouseId: this.dataObject.wareHouseId,
            CreatedDate: new Date()
          }
          this.dataObject.listSerial.unshift(serial);
        }
      }
      this.dataObject.totalSerial = this.dataObject.listSerial.length;
    }
  }

  cancelRow(dataRow: any) {
    let indexOf = this.dataObject.listSerial.indexOf(dataRow);
    this.dataObject.listSerial.splice(indexOf, 1);
  }
  clearAllData() {
    this.dataObject.listSerial = [];
  }
}
