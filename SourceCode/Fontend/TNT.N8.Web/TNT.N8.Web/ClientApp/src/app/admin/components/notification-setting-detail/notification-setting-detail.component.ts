import { Component, OnInit, ViewChild, HostListener, ElementRef, Renderer2, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { FormControl, Validators, FormGroup, ValidatorFn } from '@angular/forms';
import * as $ from 'jquery';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { GetPermission } from '../../../shared/permission/get-permission';
import { Router, ActivatedRoute } from '@angular/router';
import { ImageSettingsModel } from '@syncfusion/ej2-angular-richtexteditor';
import {
  ToolbarService, LinkService, ImageService,
  HtmlEditorService, TableService, RichTextEditorComponent
} from '@syncfusion/ej2-angular-richtexteditor';
import { Screen } from '../../models/screen.model';
import { NotifiAction } from '../../models/notifiAction.model';
import { InforScreen } from '../../models/inforScreen.model';
import { NotifiCondition } from '../../models/NotifiCondition.model';
import { NotifiSetting } from '../../models/NotifiSetting.model';
import { NotifiSpecial } from '../../models/NotifiSpecial.model';
import { NotifiSettingCondition } from '../../models/NotifiSettingCondition.model';
import { NotifiSettingService } from '../../services/notifi-setting.service';
import { NotifiSettingToken } from '../../models/notifiSettingToken.model';

@Component({
  selector: 'app-notification-setting-detail',
  templateUrl: './notification-setting-detail.component.html',
  styleUrls: ['./notification-setting-detail.component.css'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, TableService]
})
export class NotificationSettingDetailComponent implements OnInit, AfterViewInit {
  loading: boolean = false;
  awaitResult: boolean = false; //Khóa nút lưu, lưu và thêm mới

  fixed: boolean = false;
  withFiexd: string = "";
  withFiexdCol: string = "";
  withColCN: number = 0;
  withCol: number = 0;
  @HostListener('document:scroll', [])
  onScroll(): void {
    let num = window.pageYOffset;
    if (num > 100) {
      this.fixed = true;
      var width: number = $('#parent').width();
      this.withFiexd = width + 'px';
      var colT = 0;
      if (this.withColCN != width) {
        colT = this.withColCN - width;
        this.withColCN = width;
        this.withCol = $('#parentTH').width();
      }
      this.withFiexdCol = (this.withCol) + 'px';
    } else {
      this.fixed = false;
      this.withFiexd = "";
      this.withCol = $('#parentTH').width();
      this.withColCN = $('#parent').width();
      this.withFiexdCol = "";
    }
  }

  /* #region: Get Global Parameter */
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.getDefaultNumberType();  //Số chữ số thập phân sau dấu phẩy
  isManager: boolean = localStorage.getItem('IsManager') == "true" ? true : false;
  auth = JSON.parse(localStorage.getItem('auth'));
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  /* #endregion */

  /* #region: Check user permission  */
  actionAdd: boolean = true;
  /* #endregion */

  /* #region: Valid Form */
  isInvalidForm: boolean = false;
  emitStatusChangeForm: any;
  @ViewChild('toggleButton') toggleButton: ElementRef;
  isOpenNotifiError: boolean = false;
  @ViewChild('notifi') notifi: ElementRef;
  @ViewChild('saveAndCreate') saveAndCreate: ElementRef;
  @ViewChild('save') save: ElementRef;
  /* #endregion */

  /* #region: Define Form */
  settingForm: FormGroup;
  activeControl: FormControl;
  notifiSettingNameControl: FormControl;
  screenControl: FormControl;
  notifiActionControl: FormControl;
  isApprovedControl: FormControl;
  isParticipantControl: FormControl;
  isCreatedControl: FormControl;
  isPersonInchargeControl: FormControl;
  selectedNotifiSpecialControl: FormControl;
  sendInternalControl: FormControl;
  isEmailControl: FormControl;
  emailTitleControl: FormControl;
  emailContentControl: FormControl;
  objectBackHourInternalControl: FormControl;
  backHourInternalControl: FormControl;
  /* #endregion */

  listScreen: Array<Screen> = [];
  listAllNotifiAction: Array<NotifiAction> = [];
  listNotifiAction: Array<NotifiAction> = [];
  listAllInforScreen: Array<InforScreen> = [];
  listInforScreenInternal: Array<InforScreen> = [];
  listEmployee: Array<any> = [];
  notifiSettingId: string = null;
  notifiSetting: NotifiSetting = new NotifiSetting();
  listNotifiSpecial: Array<NotifiSpecial> = [];
  listAllNotifiSettingToken: Array<NotifiSettingToken> = [];
  listNotifiSettingToken: Array<NotifiSettingToken> = [];

  cols: Array<any> = [];

  /* #region: Kiểm tra xem con trỏ đang focus vào trường nào? */
  latestField: string = '';
  /* #endregion */

  /* #region: Editor */
  @ViewChild('templateRTE') rteEle: RichTextEditorComponent;
  tools: object = {
    type: 'Expand',
    items: ['Bold', 'Italic', 'Underline', 'StrikeThrough',
      'FontName', 'FontSize', 'FontColor', 'BackgroundColor',
      'LowerCase', 'UpperCase', '|',
      'Formats', 'Alignments', 'OrderedList', 'UnorderedList',
      'Outdent', 'Indent', '|',
      'CreateLink', 'Image', '|', 'ClearFormat', 'Print',
      'SourceCode', 'FullScreen', '|', 'Undo', 'Redo', 'CreateTable']
  };
  insertImageSettings: ImageSettingsModel = { 
    allowedTypes: ['.jpeg', '.jpg', '.png'], 
    display: 'inline', width: 'auto', 
    height: 'auto', 
    saveFormat: 'Base64', 
    saveUrl: null, 
    path: null, 
  };
  /* #endregion */
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private getPermission: GetPermission,
    private messageService: MessageService,
    private renderer: Renderer2,
    private confirmationService: ConfirmationService,
    private ref: ChangeDetectorRef,
    private notifiSettingService: NotifiSettingService
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
    this.setForm();
    this.initTable();
    this.route.params.subscribe(params => {
      this.notifiSettingId = params['notifiSettingId'];
      this.getMasterData(true);
    });
  }

  ngAfterViewInit() {
    this.ref.detectChanges();
  }

  setForm() {
    this.activeControl = new FormControl(false);
    this.notifiSettingNameControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.screenControl = new FormControl(null, [Validators.required]);
    this.notifiActionControl = new FormControl(null, [Validators.required]);
    this.isApprovedControl = new FormControl(false);
    this.isParticipantControl = new FormControl(false);
    this.isCreatedControl = new FormControl(false);
    this.isPersonInchargeControl = new FormControl(false);
    this.selectedNotifiSpecialControl = new FormControl(null);
    this.sendInternalControl = new FormControl(true);
    this.isEmailControl = new FormControl(true);
    this.emailTitleControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.emailContentControl = new FormControl(null, [Validators.required]);
    this.objectBackHourInternalControl = new FormControl(null);
    this.backHourInternalControl = new FormControl('0');

    this.settingForm = new FormGroup({
      activeControl: this.activeControl,
      notifiSettingNameControl: this.notifiSettingNameControl,
      screenControl: this.screenControl,
      notifiActionControl: this.notifiActionControl,
      isApprovedControl: this.isApprovedControl,
      isParticipantControl: this.isParticipantControl,
      isCreatedControl: this.isCreatedControl,
      isPersonInchargeControl: this.isPersonInchargeControl,
      selectedNotifiSpecialControl: this.selectedNotifiSpecialControl,
      sendInternalControl: this.sendInternalControl,
      isEmailControl: this.isEmailControl,
      emailTitleControl: this.emailTitleControl,
      emailContentControl: this.emailContentControl,
      objectBackHourInternalControl: this.objectBackHourInternalControl,
      backHourInternalControl: this.backHourInternalControl
    });
  }

  initTable() {
    this.cols = [
      { field: 'tokenCode', header: 'Mã', width: '50%', textAlign: 'left', display: 'table-cell' },
      { field: 'tokenLabel', header: 'Mô tả', width: '50%', textAlign: 'left', display: 'table-cell' },
    ];
  }

  getMasterData(first: boolean) {
    if (first) {
      this.loading = true;
    }
    
    this.notifiSettingService.getMasterDataNotifiSettingDetail(this.notifiSettingId).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        this.listScreen = result.listScreen;
        this.listAllNotifiAction = result.listNotifiAction;
        this.listEmployee = result.listEmployee;
        this.listAllInforScreen = result.listInforScreen;
        this.notifiSetting = result.notifiSetting;
        this.listNotifiSpecial = result.listNotifiSpecial;
        this.listAllNotifiSettingToken = result.listNotifiSettingToken;

        this.setDefaultValueForm();
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  setDefaultValueForm() {
    this.activeControl.setValue(this.notifiSetting.active ? this.notifiSetting.active : false);
    this.notifiSettingNameControl.setValue(this.notifiSetting.notifiSettingName);
    let screen = this.listScreen.find(x => x.screenId == this.notifiSetting.screenId);
    this.screenControl.setValue(screen ? screen : null);
    this.listNotifiAction = this.listAllNotifiAction.filter(x => x.screenId == this.notifiSetting.screenId);
    let notifiAction = this.listNotifiAction.find(x => x.notifiActionId == this.notifiSetting.notifiActionId);
    this.notifiActionControl.setValue(notifiAction ? notifiAction : null);
    this.isApprovedControl.setValue(this.notifiSetting.isApproved ? this.notifiSetting.isApproved : false);
    this.isParticipantControl.setValue(this.notifiSetting.isParticipant ? this.notifiSetting.isParticipant : false);
    this.isCreatedControl.setValue(this.notifiSetting.isCreated ? this.notifiSetting.isCreated : false);
    this.isPersonInchargeControl.setValue(this.notifiSetting.isPersonIncharge ? this.notifiSetting.isPersonIncharge : false);
    let listEmployeeId = this.listNotifiSpecial.map(x => x.employeeId);
    let listSelectedNotifiSpecial = this.listEmployee.filter(x => listEmployeeId.includes(x.employeeId));
    this.selectedNotifiSpecialControl.setValue(listSelectedNotifiSpecial);
    this.sendInternalControl.setValue(this.notifiSetting.sendInternal ? this.notifiSetting.sendInternal : false);
    this.listInforScreenInternal = this.listAllInforScreen.filter(x => x.screenId == this.notifiSetting.screenId && 
      x.isDateTime == true);
    let objectBackHourInternal = this.listInforScreenInternal.find(x => x.inforScreenId == this.notifiSetting.objectBackHourInternal);
    this.objectBackHourInternalControl.setValue(objectBackHourInternal ? objectBackHourInternal : null);
    this.backHourInternalControl.setValue(this.notifiSetting.backHourInternal ? this.notifiSetting.backHourInternal.toString() : '0');
    this.isEmailControl.setValue(this.notifiSetting.isEmail ? this.notifiSetting.isEmail : false);
    this.emailTitleControl.setValue(this.notifiSetting.emailTitle ? this.notifiSetting.emailTitle : null);
    this.emailContentControl.setValue(this.notifiSetting.emailContent ? this.notifiSetting.emailContent : null);
    this.listNotifiSettingToken = this.listAllNotifiSettingToken.filter(x => x.screenId == screen.screenId);
  }

  //Gửi nội bộ
  changeSendInternal() {
    let sendInternal = this.sendInternalControl.value;

    //Nếu là gửi nội bộ
    if (sendInternal) {
      let isEmail: boolean = this.isEmailControl.value;

      if (isEmail) {
        this.enableValiators();
      } else {
        this.disableValidators();
      }
    } 
    //Nếu bỏ tick gửi nội bộ
    else {
      this.disableValidators();
    }
  }

  //Gửi email nội bộ
  changeIsEmail() {
    let isEmail: boolean = this.isEmailControl.value;

    //Nếu là gửi email nội bộ
    if (isEmail) {
      this.enableValiators();
    }
    //Nếu bỏ tick gửi email nội bộ
    else {
      this.disableValidators();
    }
  }

  enableValiators() {
    this.emailTitleControl.setValidators([Validators.required, forbiddenSpaceText]);
    this.emailTitleControl.updateValueAndValidity();
    this.emailContentControl.setValidators([Validators.required]);
    this.emailContentControl.updateValueAndValidity();
  }

  disableValidators() {
    this.emailTitleControl.setValidators(null);
    this.emailTitleControl.updateValueAndValidity();
    this.emailContentControl.setValidators(null);
    this.emailContentControl.updateValueAndValidity();
  }

  /* Event: Thay đổi màn hình */
  changeScreen() {
    this.listNotifiAction = [];
    this.notifiActionControl.reset();
    this.listInforScreenInternal = [];
    this.objectBackHourInternalControl.reset();
    this.backHourInternalControl.setValue('0');
    this.listNotifiSettingToken = [];

    let screen: Screen = this.screenControl.value;

    if (screen) {
      //list Sự kiện
      this.listNotifiAction = this.listAllNotifiAction.filter(x => x.screenId == screen.screenId);
      if (this.listNotifiAction?.length == 1) {
        this.notifiActionControl.setValue(this.listNotifiAction[0]);
      }

      //list Gửi trước
      this.listInforScreenInternal = this.listAllInforScreen.filter(x => x.screenId == screen.screenId);

      //list token
      this.listNotifiSettingToken = this.listAllNotifiSettingToken.filter(x => x.screenId == screen.screenId);
    }
  }

  /* Event thay đổi trường thông tin để đặt lịch gửi */
  changeObjectBackHourInternal() {
    let objectBackHourInternal: InforScreen = this.objectBackHourInternalControl.value;

    if (!objectBackHourInternal) {
      this.backHourInternalControl.setValue('0');
    }
  }

  /* Thêm token vào nội dung */
  addToken(tokenCode: string) {
    if (this.latestField == 'titleEmail') {
      let newData = (this.emailTitleControl.value ? this.emailTitleControl.value : '') + tokenCode;
      this.emailTitleControl.setValue(newData);
    } else if (this.latestField == 'contentEmail') {
      let newData = (this.emailContentControl.value ? this.emailContentControl.value : '') + '<span>' + tokenCode + '</span>';
      this.emailContentControl.setValue(newData);
    }
  }

  /* Kiểm tra con trỏ có đang focus vào title email hay không */
  forcusTitleEmail(event: any) {
    this.latestField = 'titleEmail';
  }

  /* Kiểm tra con trỏ có đang focus vào content email hay không */
  forcusContentEmail(event: any) {
    this.latestField = 'contentEmail';
  }

  /* Cập nhật Cấu hình thông báo */
  updateNotifiSetting() {
    if (!this.settingForm.valid) {
      Object.keys(this.settingForm.controls).forEach(key => {
        if (!this.settingForm.controls[key].valid) {
          this.settingForm.controls[key].markAsTouched();
        }
      });
      this.isInvalidForm = true;  //Hiển thị icon-warning-active
      this.isOpenNotifiError = true;  //Hiển thị message lỗi
      this.emitStatusChangeForm = this.settingForm.statusChanges.subscribe((validity: string) => {
        switch (validity) {
          case "VALID":
            this.isInvalidForm = false;
            break;
          case "INVALID":
            this.isInvalidForm = true;
            break;
        }
      });
    } else { 
      let notifisetting = this.mapDataToNotifiSettingModel();
      let listNotifiSpecial: Array<NotifiSpecial> = this.mapDataToListNotifiSpecial();

      this.loading = true;
      this.notifiSettingService.updateNotifiSetting(notifisetting, listNotifiSpecial).subscribe(response => {
        let result: any = response;
        if (result.statusCode == 200) {
          this.getMasterData(false);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: "Lưu thành công" };
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  mapDataToNotifiSettingModel(): NotifiSetting {
    this.notifiSetting.notifiSettingName = this.notifiSettingNameControl?.value?.trim();
    this.notifiSetting.screenId = this.screenControl.value?.screenId;
    this.notifiSetting.notifiActionId = this.notifiActionControl.value?.notifiActionId;
    this.notifiSetting.isApproved = this.isApprovedControl.value ? this.isApprovedControl.value : false;
    this.notifiSetting.isParticipant = this.isParticipantControl.value ? this.isParticipantControl.value : false;
    this.notifiSetting.isCreated = this.isCreatedControl.value ? this.isCreatedControl.value : false;
    this.notifiSetting.isPersonIncharge = this.isPersonInchargeControl.value ? this.isPersonInchargeControl.value : false;
    this.notifiSetting.sendInternal = this.sendInternalControl.value ? this.sendInternalControl.value : false;
    this.notifiSetting.objectBackHourInternal = this.objectBackHourInternalControl.value?.inforScreenId;
    this.notifiSetting.backHourInternal = ParseStringToFloat(this.backHourInternalControl?.value);
    this.notifiSetting.isEmail = this.isEmailControl.value ? this.isEmailControl.value : false;
    this.notifiSetting.emailTitle = this.emailTitleControl.value?.trim();
    this.notifiSetting.emailContent = this.emailContentControl.value?.trim();
    this.notifiSetting.active = this.activeControl.value ? this.activeControl.value : false;

    //Nếu trường thông tin Gửi trước không có giá trị thì
    if (!this.notifiSetting.objectBackHourInternal) {
      this.notifiSetting.backHourInternal = null;
    }

    return this.notifiSetting;
  }

  mapDataToListNotifiSpecial(): Array<NotifiSpecial> {
    let listNotifiSpecial: Array<NotifiSpecial> = [];

    let listEmployee: Array<any> = this.selectedNotifiSpecialControl.value;

    if (listEmployee.length > 0) {
      listEmployee.forEach(item => {
        let notifiSpecial = new NotifiSpecial();
        notifiSpecial.employeeId = item.employeeId;
        listNotifiSpecial.push(notifiSpecial);
      });
    }

    return listNotifiSpecial;
  }

  /* Khởi tạo lại các giá trị ban đầu của form */
  resetForm() {
    this.activeControl.setValue(false);
    this.notifiSettingNameControl.reset();
    this.screenControl.reset();
    this.notifiActionControl.reset();
    this.isApprovedControl.setValue(false);
    this.isParticipantControl.setValue(false);
    this.isCreatedControl.setValue(false);
    this.isPersonInchargeControl.setValue(false);
    this.selectedNotifiSpecialControl.reset();
    this.sendInternalControl.setValue(true);
    this.isEmailControl.setValue(true);
    this.emailTitleControl.reset();
    this.emailContentControl.reset();
    this.objectBackHourInternalControl.reset();
    this.backHourInternalControl.setValue('0');

    this.awaitResult = false;
  }

  /* Làm tròn 1 số */
  roundNumber(number: number, unit: number): number {
    let result: number = number;
    switch (unit) {
      case -1: {
        result = result;
        break;
      }
      case 0: {
        result = Math.round(result);
        break;
      }
      case 1: {
        result = Math.round(number * 10) / 10;
        break;
      }
      case 2: {
        result = Math.round(number * 100) / 100;
        break;
      }
      case 3: {
        result = Math.round(number * 1000) / 1000;
        break;
      }
      case 4: {
        result = Math.round(number * 10000) / 10000;
        break;
      }
      default: {
        result = result;
        break;
      }
    }
    return result;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  clear() {
    this.messageService.clear();
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  cancel() {
    this.router.navigate(['/admin/notifi-setting-list']);
  }

  ngOnDestroy() {
    if (this.emitStatusChangeForm) {
      this.emitStatusChangeForm.unsubscribe();
    }
  }
}

function convertToUTCTime(time: any) {
  return new Date(Date.UTC(time.getFullYear(), time.getMonth(), time.getDate(), time.getHours(), time.getMinutes(), time.getSeconds()));
};

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
};

function ParseStringToFloat(str: string) {
  if (str === "") return 0;
  str = str.replace(/,/g, '');
  return parseFloat(str);
};
