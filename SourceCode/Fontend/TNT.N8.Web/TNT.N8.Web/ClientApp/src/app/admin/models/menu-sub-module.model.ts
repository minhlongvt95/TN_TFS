import { MenuPage } from './menu-page.model';

export class MenuSubModule {
  indexOrder: number; //Số thứ tự
  name: string; //Tên sub module
  code: string; //Mã sub module
  codeParent: string; //Mã module
  path: string; //Đường dẫn vào page mặc định của sub module
  isShow: boolean;  //ẩn-hiện trên menubar
  active: boolean;  //highlight trên menubar
  isShowChildren: boolean; //ẩn-hiện list menu page của sub module
  children: Array<MenuPage>;  //list menu page của sub module

  constructor() {
    this.isShow = true;
    this.active = false;
    this.isShowChildren = false;
    this.children = [];
  }
}