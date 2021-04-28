import { Component, OnInit, Input, AfterContentChecked, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthenticationService } from '../../services/authentication.service';
import { PermissionModel } from '../../models/permission.model';
import { Router } from '@angular/router';
import { CommonService } from '../../services/common.service';
import { BreadCrumMenuModel } from '../../../shared/models/breadCrumMenu.model';
import { EventEmitterService } from '../../../shared/services/event-emitter.service';  
import * as $ from 'jquery';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  providers: [AuthenticationService]
})
export class MenuComponent implements OnInit {
  value: any;
  isAdmin: boolean;
  isCustomer: boolean;
  isSales: boolean;
  isShopping: boolean;
  isAccounting: boolean;
  isHrm: boolean;
  isWareHouse: boolean;
  permissionAdmin = false;
  moduled: string;

  listPermissionResource: Array<string> = localStorage.getItem('ListPermissionResource').split(',');
  listModule: Array<string> = [];
  listResource: Array<string> = [];
  lstBreadCrumLeftMenu: Array<BreadCrumMenuModel> = []; 
  lstBreadCrum: Array<BreadCrumMenuModel> = [];

  isClickMiniLogo: false;
  isToggleCick: Boolean = true;

  rowclick: number = -1;
  rowclickParent: number = -1;
  active: boolean = true;
  activeParent: boolean = true;
  countLengthParrent: number = 0;

  constructor(
    public ref: ChangeDetectorRef,
    private authService: AuthenticationService,
    private translate: TranslateService,
    private router: Router,
    private eventEmitterService: EventEmitterService,
    private commonService: CommonService) {
    this.translate.setDefaultLang('vi');
    $("body").addClass('sidebar-collapse');
  }

  ngOnInit() {
    //Call Update LeftMenu với eventEmitterService
    if (this.eventEmitterService.subsVar == undefined) {
      this.eventEmitterService.subsVar = this.eventEmitterService.
        invokeFirstComponentFunction.subscribe((name: string) => {
          this.updateLeftMenu();
        });
    }
    //Call Update IsToggle với eventEmitterService
    if (this.eventEmitterService.subsVar2 == undefined) {
      this.eventEmitterService.subsVar2 = this.eventEmitterService.
        invokeUpdateIsToggleFunction.subscribe((name: string) => {
          this.updateLeftIsToggle();
        });
    }   
    var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');
    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);
    var X = localStorage.getItem('menuMapPath');
    this.lstBreadCrum = JSON.parse(X);

    //kiem tra xem co toggle ko
    if ($("body").hasClass("sidebar-collapse")) {
      this.isToggleCick = true;
    }
    else {
      this.isToggleCick = false;
    }​

    const menuIndex = localStorage.getItem('menuIndex');

    if (menuIndex !== '' && menuIndex !== 'null' && menuIndex != null) {
      if (menuIndex === 'admin') {
        this.permissionAdmin = true;
      }
      else {
        this.permissionAdmin = false;
      }
      this.changeMenuTo(menuIndex);
    }

    this.getListModuleAndResource();

  }
  
  ngAfterViewInit() {
    var indexTree = localStorage.getItem('menuIndexTree');
    this.gotoPage(indexTree);
    this.ref.detectChanges();
  }

  gotoPage(path) {
    localStorage.setItem('menuIndexTree',path);
    
    var indexTree = localStorage.getItem('menuIndexTree');
    this.lstBreadCrumLeftMenu.forEach(item => {
      if(item.LstChildren !== null && item.LstChildren !== undefined && item.LstChildren !== []){
        item.LstChildren.forEach(detail => {
          if(detail.Path === indexTree){
            detail.Display = 'block';
          }
          else{ 
            detail.Display = 'none';            
          }
        });
        let x = item.LstChildren.filter(c => c.Display === 'none');
        if(x.length === item.LstChildren.length){
          item.Display = 'none';
        }
        else{
          item.Display = 'block';
        }
      }
      else{item.Display = 'none';}
    });

    for(let i = 0; i< this.lstBreadCrumLeftMenu.length; i++){
      if(this.lstBreadCrumLeftMenu[i].Display === 'none'){
        $(".module-remove" + i).hide();
        $(".module-add" + i).show();
      }
      else{
        $(".module-remove" + i).show();
        $(".module-add" + i).hide();
        this.rowclick = i;
        this.active = true;
      }
    }
  }
  //Lấy ra list module của user
  getListModuleAndResource() 
  {
    if (this.listPermissionResource.length > 0) 
    {
      this.listPermissionResource.forEach(item => 
      {
        let moduleName = item.slice(0, item.indexOf('/'));
        if (this.listModule.indexOf(moduleName) == -1) 
        {
          this.listModule.push(moduleName);
        }

        let resourceName = item.slice(item.indexOf('/') + 1, item.lastIndexOf('/'));
        if (this.listResource.indexOf(resourceName) == -1)
        {
          this.listResource.push(resourceName);
        }
      });
    }
  }

  //Kiểm tra user có được quyền nhìn thấy các resource nào trên menu:
  checkUserResource(resourceName)
  {
    let result = false;
    if (this.listResource.indexOf(resourceName) !== -1)
    {
      result = true;
    }
    return result;
  }

  onHomeClick() {
    localStorage.removeItem('menuIndex');
    this.router.navigate(['/home']);
  }

  changeMenuTo(module) {
    this.setModuleToFalse();
    switch (module) {
      case 'admin':
        this.isAdmin = true;
        break;
      case 'customer':
        this.isCustomer = true;
        break;
      case 'sales':
        this.isSales = true;
        break;
      case 'shopping':
        this.isShopping = true;
        break;
      case 'accounting':
        this.isAccounting = true;
        break;
      case 'employee':
        this.isHrm = true;
        break;
      case 'admin-con':
        this.isAdmin = true;
        break;
      case 'warehouse':
        this.isWareHouse = true;
        break;
      default:
        break;
    }
    localStorage.setItem('menuIndex', module);
  }

  setModuleToFalse() {
    this.isCustomer = false;
    this.isSales = false;
    this.isShopping = false;
    this.isAccounting = false;
    this.isHrm = false;
    this.isAdmin = false;
    this.isWareHouse = false;
  }

  updateLeftMenu() {
    var leftMenu = localStorage.getItem('lstBreadCrumLeftMenu');
    this.lstBreadCrumLeftMenu = [];
    this.lstBreadCrumLeftMenu = JSON.parse(leftMenu);
    var indexTree = localStorage.getItem('menuIndexTree');
    this.gotoPage(indexTree);
  }

  updateLeftIsToggle() {
    this.isToggleCick = JSON.parse(localStorage.getItem('isToggleCick'));
  }

  openMenuLevel4(resource: BreadCrumMenuModel, resourceParent: BreadCrumMenuModel) {
    this.router.navigate([resource.Path]);
  }

  updateActivelstBreadCrumLeftMenu(resource: BreadCrumMenuModel) {
    this.lstBreadCrumLeftMenu.forEach(function (item) {
      item.LstChildren.forEach(function (itemChildren) {
        if (itemChildren.Active) itemChildren.Active = false;
        if (itemChildren.Path == resource.Path && itemChildren.CodeParent == resource.CodeParent) {
          itemChildren.Active = true;
        }
      })
    });
  }
  
  addRemoveIcon(index){
    for(let i = 0; i< this.lstBreadCrumLeftMenu.length; i++){
      $(".module-remove" + i).hide();
      $(".module-add" + i).show();
    }
    if(this.rowclick !== index){
      $(".module-remove" + index).show();
      $(".module-add" + index).hide();
      this.active = true;

      for(let i = 0; i< this.countLengthParrent; i++){
        $(".module-remove-parent" + i).hide();
        $(".module-add-parent" + i).show();
      }
      this.activeParent = true;
    }
    else{
      if(!this.active){        
        $(".module-remove" + index).show();
        $(".module-add" + index).hide();
      }
      else{    
        $(".module-remove" + index).hide();
        $(".module-add" + index).show();
      }
      this.active = !this.active;
    }
    
    this.rowclick = index;
  }

  addRemoveIconParren(index,countLength){
    this.countLengthParrent = countLength;
    for(let i = 0; i< countLength; i++){
      $(".module-remove-parent" + i).hide();
      $(".module-add-parent" + i).show();
    }
    if(this.rowclickParent !== index){
      $(".module-remove-parent" + index).show();
      $(".module-add-parent" + index).hide();
      this.activeParent = true;
    }
    else{
      if(!this.activeParent){        
        $(".module-remove-parent" + index).show();
        $(".module-add-parent" + index).hide();
      }
      else{    
        $(".module-remove-parent" + index).hide();
        $(".module-add-parent" + index).show();
      }
      this.activeParent = !this.activeParent;
    }

    this.rowclickParent = index;
  }
}
