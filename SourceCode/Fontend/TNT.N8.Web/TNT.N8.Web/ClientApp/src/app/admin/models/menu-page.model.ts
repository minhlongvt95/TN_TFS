export class MenuPage {
  name: string; //Tên page
  codeParent: string; //Mã sub module
  isShow: boolean;  //ẩn-hiện trên menubar
  nameIcon: string; //class icon
  path: string; //Đường dẫn page
  active: boolean;  //highlight trên menu bar

  constructor() {
    this.isShow = true;
    this.active = false;
  }
}