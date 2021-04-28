import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, Validators, FormGroup, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { WarehouseService } from '../../../services/warehouse.service';
import * as XLSX from 'xlsx';
import { SerialModel } from '../../../models/serials.model';

import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';


type AOA = any[][];


@Component({
  selector: 'app-pop-create-serial',
  templateUrl: './pop-create-serial.component.html',
  styleUrls: ['./pop-create-serial.component.css']
})
export class PopupCreateSerialComponent implements OnInit {
  popupForm: FormGroup;
  serialControl: FormControl;
  listSerialReturn: Array<SerialModel> = [];
  listserial = [];
  listserialProduct = [];
  lstSerialGet: Array<any> = [];
  dateNow: any;
  displayedColumns = ['serial', 'date'];
  dataObject: any;
  displayDialog: Boolean = false;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  rows = 10;
  lstSerialOld: Array<any> = [];

  constructor(
    public snackBar: MatSnackBar,
    public dialog: MatDialog,
    public services: WarehouseService,
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
    let resultGetTop10: any = await this.services.getSerialAsync(null, null);
    this.lstSerialGet = resultGetTop10.lstSerial;
  };


  addRowSerial() {
    if (this.dataObject.listSerial == null) { this.dataObject.listSerial = [] };
    if (this.dataObject.listSerial.length >= this.dataObject.quantity) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng row có dữ liệu không được lớp hơn số lượng nhập", detail: 'Thêm Serial' });
        return;
    }
    else {
      this.dataObject.totalSerial += 1;

      //var serial: SerialModel = {
      //  SerialId: this.emptyGuid,
      //  SerialCode: '',
      //  ProductId: this.dataObject.productId,
      //  StatusId: this.emptyGuid,
      //  Active: true,
      //  WarehouseId: this.dataObject.wareHouseId,
      //  CreatedDate: new Date()
      //}
      var serial = {
        SerialId: this.emptyGuid,
        SerialCode: '',
        ProductId: this.dataObject.productId,
        StatusId: this.emptyGuid,
        Active: true,
        WarehouseId: this.dataObject.wareHouseId,
        CreatedDate: new Date(),
        error: false
      }


      this.dataObject.listSerial.unshift(serial);
    }
  }

  setValueSerial(stt: number) {
    this.listserial[stt - 1].serial = this.serialControl.value;
  }

  download() {
    this.services.downloadTemplateSerial().subscribe(response => {
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
      data.shift();
      data.sort();
      ///
      if (data.length > 0) {
        for (var i = 0; i < data.length; ++i) {
          let checkexistSerial = this.lstSerialGet.find(f => f.serialCode == data[i][0]);
          if (checkexistSerial != null) {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: "Số serial " + data[i][0] + " đã trùng với số serial của sản phẩm đã lưu trong hệ thống", detail: 'Thêm Serial' });
            return;
          }
          if (data[i][0] == null || data[i][0] == '') {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: "File excel không được chứa dòng trống", detail: 'Thêm Serial' });
            return;
          }
          if (String(data[i][0]).length > 30) {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial tối đa 30 kí tự. Mời kiểm tra lại", detail: 'Thêm Serial' });
            return;
          }
          if (String(data[i][0]).indexOf(' ') >= 0) {
            this.messageService.clear();
            this.messageService.add({ key: 'error', severity: 'error', summary: "Số serial không chứa kí tự space đầu cuối", detail: 'Thêm Serial' });
            return;
          }

        }

      }
      //check duplicate trong file excel
      let DataNoDuplicate = Array.from(new Set(data.map(a => a[0])));
      if (data.length > DataNoDuplicate.length) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "File excel tồn tại số serial trùng nhau", detail: 'Thêm Serial' });
        return;
      }
      ////
      if (this.dataObject.listSerial == null) { this.dataObject.listSerial = [] };

      if ((this.dataObject.listSerial.length + data.length) > this.dataObject.quantity) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Số lượng row có dữ liệu không được lớp hơn số lượng nhập", detail: 'Thêm Serial' });
        return;
      }
      else {

        this.listserialProduct = [];
        for (let i = 0; i < data.length; i++) {

          //var serial: SerialModel = {
          //  SerialId: this.emptyGuid,
          //  SerialCode: data[i][0],
          //  ProductId: this.dataObject.productId,
          //  StatusId: this.emptyGuid,
          //  Active: true,
          //  WarehouseId: this.dataObject.wareHouseId,
          //  CreatedDate: new Date()
          //}
          var serial = {
            SerialId: this.emptyGuid,
            SerialCode: data[i][0],
            ProductId: this.dataObject.productId,
            StatusId: this.emptyGuid,
            Active: true,
            WarehouseId: this.dataObject.wareHouseId,
            CreatedDate: new Date(),
            error: false
          }

          this.dataObject.listSerial.push(serial);
        }
        this.dataObject.totalSerial = this.dataObject.listSerial.length;

      }
    };
    reader.readAsBinaryString(target.files[0]);
  }

  onCancelClick() {
    this.confirmationService.confirm({
      message: 'Các thay đổi sẽ không được lưu lại.Hành động này không thể được hoàn tác ,bạn có chắc chắn muốn hủy?',
      header: 'Thông báo',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        //if (this.dataObject.listSerial == null) { this.dataObject.listSerial = [] };
        //var arrayTemp = [];
        //for (var i = 0; i < this.dataObject.listSerial.length; ++i) {
        //  if (this.dataObject.listSerial[i].SerialId !== '' && this.dataObject.listSerial[i].SerialId !== null
        //    && this.dataObject.listSerial[i].SerialId !== this.emptyGuid) {
        //    arrayTemp.push(this.dataObject.listSerial[i]);
        //  }
        //}
        //this.dataObject.listSerial = [];
        //this.dataObject.listSerial.push.apply(this.dataObject.listSerial, arrayTemp);
        //this.dataObject.totalSerial = arrayTemp.length;
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
    var isError = false;
    for (let i = 0; i < this.dataObject.listSerial.length; i++) {
      if (this.dataObject.listSerial[i].error == true) {
        isError = true;
        break;
      }
    }
    if (isError == true) {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Có lỗi trong danh sách ", detail: 'Thêm Serial' });
      return;
    }

    for (let i = 0; i < this.dataObject.listSerial.length; i++) {
      if (this.dataObject.listSerial[i].SerialCode == '' || this.dataObject.listSerial[i].SerialCode == null) {
        this.messageService.clear();
        this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial Code không được để trống ", detail: 'Thêm Serial' });
        this.dataObject.listSerial[i].error = true;
        return;
      }
    }

    this.ref.close();
  }
  pageChange(event: any) {
  }
  changeText(rowData: any) {
    rowData.error = false;
    rowData.SerialCode = rowData.SerialCode.trim();
    if (rowData.SerialCode.length > 30) {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial tối đa 30 kí tự. Mời kiểm tra lại", detail: 'Thêm Serial' });
      rowData.error = true;
      return;
    }
    let checkExistInSystem = this.lstSerialGet.find(f => f.serialCode == rowData.SerialCode);
    if (checkExistInSystem != null) {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial đã tồn tại trong hệ thống", detail: 'Thêm Serial' });
      rowData.error = true;
      return;
    }

    let DataNoDuplicate = Array.from(new Set(this.dataObject.listSerial.map(a => a.SerialCode)));
    if (this.dataObject.listSerial.length > DataNoDuplicate.length) {
      this.messageService.clear();
      this.messageService.add({ key: 'error', severity: 'error', summary: "Số Serial không được lặp lại trong danh sách", detail: 'Thêm Serial' });
      rowData.error = true;
      return;
    }
  }
  cancelRow(rowData: any) {
    var index = this.dataObject.listSerial.findIndex(f => f.SerialCode == rowData.SerialCode);
    this.dataObject.listSerial.splice(index, 1);
    this.dataObject.totalSerial = this.dataObject.listSerial.length;

  }
  clearAllData() {
    this.dataObject.listSerial = [];
    this.dataObject.totalSerial = this.dataObject.listSerial.length;
  }

}
