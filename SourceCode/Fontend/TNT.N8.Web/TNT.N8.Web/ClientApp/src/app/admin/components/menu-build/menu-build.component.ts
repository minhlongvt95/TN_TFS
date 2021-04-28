import { Component, OnInit, HostListener, ViewChild, ElementRef, Renderer2, ChangeDetectorRef } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { GetPermission } from '../../../shared/permission/get-permission';
import * as $ from 'jquery';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MenuBuild } from '../../models/menu-build.model';
import { MenuBuildService } from '../../services/menu-build.service';

@Component({
  selector: 'app-menu-build',
  templateUrl: './menu-build.component.html',
  styleUrls: ['./menu-build.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class MenuBuildComponent implements OnInit {
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

  createMenuMouleForm: FormGroup;
  nameModuleControl: FormControl;
  codeModuleControl: FormControl;

  createSubMenuMouleForm: FormGroup;
  nameSubModuleControl: FormControl;
  codeSubModuleControl: FormControl;
  pathSubModuleControl: FormControl;
  
  createMenuPageForm: FormGroup;
  nameMenuPageControl: FormControl;
  pathMenuPageControl: FormControl;
  nameIconMenuPageControl: FormControl;
  isPageDetailMenuPageControl: FormControl;

  title: string = 'Menu Module';
  level: number = 0;
  selectedMenuModule: MenuBuild = null;
  selectedSubMenuModule: MenuBuild = null;

  cols1: any[];
  listMenuModule: Array<MenuBuild> = [];
  cols2: any[];
  listSubMenuModule: Array<MenuBuild> = [];
  cols3: any[];
  listMenuPage: Array<MenuBuild> = [];

  clonedData: { [s: string]: MenuBuild;} = {}

  constructor(
    private getPermission: GetPermission,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private renderer: Renderer2,
    private menuBuildService: MenuBuildService
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
    this.initForm();
    this.getMenuMoule();
  }

  initTable() {
    this.cols1 = [
      { field: 'indexOrder', header: '#', textAlign: 'center', width: '10%' },
      { field: 'code', header: 'Mã module', textAlign: 'left', width: '30%' },
      { field: 'name', header: 'Tên module', textAlign: 'left', width: '30%' },
      { field: 'nameIcon', header: 'Đường dẫn image', textAlign: 'left', width: '30%' },
    ];

    this.cols2 = [
      { field: 'indexOrder', header: '#', textAlign: 'center', width: '10%' },
      { field: 'code', header: 'Mã module', textAlign: 'left', width: '30%' },
      { field: 'name', header: 'Tên module', textAlign: 'left', width: '30%' },
      { field: 'path', header: 'Đường dẫn mặc định', textAlign: 'left', width: '30%' },
    ];
    
    this.cols3 = [
      { field: 'indexOrder', header: '#', textAlign: 'center', width: '10%' },
      { field: 'name', header: 'Tên module', textAlign: 'left', width: '30%' },
      { field: 'path', header: 'Đường dẫn', textAlign: 'left', width: '30%' },
      { field: 'nameIcon', header: 'Icon', textAlign: 'center', width: '20%' },
      { field: 'isPageDetail', header: 'Là page detail', textAlign: 'center', width: '10%' },
    ];
  }

  initForm() {
    this.nameModuleControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.codeModuleControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);

    this.createMenuMouleForm = new FormGroup({
      nameModuleControl: this.nameModuleControl,
      codeModuleControl: this.codeModuleControl
    });

    this.nameSubModuleControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.codeSubModuleControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.pathSubModuleControl = new FormControl(null, [forbiddenSpaceText]);

    this.createSubMenuMouleForm = new FormGroup({
      nameSubModuleControl: this.nameSubModuleControl,
      codeSubModuleControl: this.codeSubModuleControl,
      pathSubModuleControl: this.pathSubModuleControl,
    });

    this.nameMenuPageControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.pathMenuPageControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.nameIconMenuPageControl = new FormControl(null, [Validators.required, forbiddenSpaceText]);
    this.isPageDetailMenuPageControl = new FormControl(false);

    this.createMenuPageForm = new FormGroup({
      nameMenuPageControl: this.nameMenuPageControl,
      pathMenuPageControl: this.pathMenuPageControl,
      nameIconMenuPageControl: this.nameIconMenuPageControl,
      isPageDetailMenuPageControl: this.isPageDetailMenuPageControl
    });
  }

  getMenuMoule() {
    this.menuBuildService.getMenuModule().subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listMenuModule = [];
        this.listMenuModule = result.listMenuModule;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  saveMenuModule() {
    if (!this.createMenuMouleForm.valid) {
      Object.keys(this.createMenuMouleForm.controls).forEach(key => {
        if (this.createMenuMouleForm.controls[key].valid == false) {
          this.createMenuMouleForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let length = this.listMenuModule.length;

      let menuModule: MenuBuild = new MenuBuild();
      menuModule.code = this.codeModuleControl.value;
      menuModule.name = this.nameModuleControl.value;
      menuModule.level = 0;
      menuModule.indexOrder = length + 1;

      this.menuBuildService.createMenuBuild(menuModule).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.createMenuMouleForm.reset();
          this.getMenuMoule();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Lấy danh sách sub menu module theo menu module code*/
  getSubMenuModuleByMenuModuleCode() {
    this.listSubMenuModule = [];
    this.menuBuildService.getSubMenuModuleByMenuModuleCode(this.selectedMenuModule.code).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listSubMenuModule = result.listSubMenuModule;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Xem danh sách sub menu module của một menu module*/
  nextToSubMenuModule(data: MenuBuild) {
    this.title = 'Sub Menu Module của: ' + data.name;
    this.level = 1;
    this.selectedMenuModule = data;
    this.createSubMenuMouleForm.reset();
    this.getSubMenuModuleByMenuModuleCode();
  }

  /*Trở lại danh sách menu module*/
  backToMenuModule() {
    this.title = 'Menu Module';
    this.level = 0;
    this.selectedMenuModule = null;
    this.createMenuMouleForm.reset();
  }

  /*Thêm một sub menu module*/
  saveSubMenuModule() {
    if (!this.createSubMenuMouleForm.valid) {
      Object.keys(this.createSubMenuMouleForm.controls).forEach(key => {
        if (this.createSubMenuMouleForm.controls[key].valid == false) {
          this.createSubMenuMouleForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let length = this.listSubMenuModule.length;

      let subMenu: MenuBuild = new MenuBuild();
      subMenu.parentId = this.selectedMenuModule.menuBuildId;
      subMenu.code = this.codeSubModuleControl.value;
      subMenu.name = this.nameSubModuleControl.value;
      subMenu.path = this.pathSubModuleControl.value;
      subMenu.codeParent = this.selectedMenuModule.code;
      subMenu.level = 1;
      subMenu.indexOrder = length + 1;

      this.menuBuildService.createMenuBuild(subMenu).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.createSubMenuMouleForm.reset();
          this.getSubMenuModuleByMenuModuleCode();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  /*Xem danh sách menu page của sub menu module*/
  nextToMenuPageModule(data: MenuBuild) {
    this.title = 'Menu Page của: ' + data.name;
    this.level = 2;
    this.selectedSubMenuModule = data;
    this.createMenuPageForm.reset();
    this.getMenuPageBySubMenuCode();
  }

  /*Quay lại Xem danh sách sub menu module*/
  backToSubMenuModule() {
    this.title = 'Sub Menu Module của: ' + this.selectedMenuModule.name;
    this.level = 1;
    this.selectedSubMenuModule = null;
    this.createSubMenuMouleForm.reset();
    this.getSubMenuModuleByMenuModuleCode();
  }

  getMenuPageBySubMenuCode() {
    this.listMenuPage = [];
    this.menuBuildService.getMenuPageBySubMenuCode(this.selectedSubMenuModule.code).subscribe(response => {
      let result: any = response;

      if (result.statusCode == 200) {
        this.listMenuPage = result.listMenuPage;
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  saveMenuPage() {
    if (!this.createMenuPageForm.valid) {
      Object.keys(this.createMenuPageForm.controls).forEach(key => {
        if (this.createMenuPageForm.controls[key].valid == false) {
          this.createMenuPageForm.controls[key].markAsTouched();
        }
      });
    }
    else {
      let length = this.listMenuPage.length;

      let menuPage: MenuBuild = new MenuBuild();
      menuPage.parentId = this.selectedSubMenuModule.menuBuildId;
      menuPage.name = this.nameMenuPageControl.value;
      menuPage.code = null;
      menuPage.codeParent = this.selectedSubMenuModule.code;
      menuPage.level = 2;
      menuPage.path = this.pathMenuPageControl.value;
      menuPage.nameIcon = this.nameIconMenuPageControl.value;
      menuPage.indexOrder = length + 1;
      menuPage.isPageDetail = this.isPageDetailMenuPageControl.value ?? false;

      this.menuBuildService.createMenuBuild(menuPage).subscribe(response => {
        let result: any = response;

        if (result.statusCode == 200) {
          this.createMenuPageForm.reset();
          this.getMenuPageBySubMenuCode();
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  updateIsPageDetail(data: MenuBuild) {
    this.loading = true;
    this.menuBuildService.updateIsPageDetail(data.menuBuildId, data.isPageDetail).subscribe(response => {
      let result: any = response;
      this.loading = false;

      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo', detail: 'Lưu thành công' };
        this.showMessage(msg);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  toggleNotifiError() {
    this.isOpenNotifiError = !this.isOpenNotifiError;
  }

  getDefaultNumberType() {
    return this.systemParameterList.find(systemParameter => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  onRowEditInit(menu: MenuBuild) {
    this.clonedData[menu.menuBuildId] = { ...menu };
  }

  onRowEditSave(menu: MenuBuild) {
    this.loading = true;
    this.menuBuildService.updateMenuBuild(menu).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        let msg = { severity: 'success', summary: 'Thông báo', detail: 'Update thành công' };
        this.showMessage(msg);
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  onRowEditCancel(level: number, menu: MenuBuild, index: number) {
    if (level == 0) 
    {
      Object.keys(this.clonedData).forEach(key => {
        if (key == menu.menuBuildId) 
        {
          menu.indexOrder = this.clonedData[key].indexOrder;
          menu.code = this.clonedData[key].code;
          menu.name = this.clonedData[key].name;
          menu.nameIcon = this.clonedData[key].nameIcon;
        }
      });
    }
    else if (level == 1) 
    {
      Object.keys(this.clonedData).forEach(key => {
        if (key == menu.menuBuildId) 
        {
          menu.indexOrder = this.clonedData[key].indexOrder;
          menu.code = this.clonedData[key].code;
          menu.name = this.clonedData[key].name;
          menu.path = this.clonedData[key].path;
          menu.nameIcon = this.clonedData[key].nameIcon;
        }
      });
    } 
    else if (level == 2) 
    {
      Object.keys(this.clonedData).forEach(key => {
        if (key == menu.menuBuildId) 
        {
          menu.indexOrder = this.clonedData[key].indexOrder;
          menu.code = this.clonedData[key].code;
          menu.name = this.clonedData[key].name;
          menu.path = this.clonedData[key].path;
          menu.nameIcon = this.clonedData[key].nameIcon;
        }
      });
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
