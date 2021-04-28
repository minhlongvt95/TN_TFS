export class BreadCrumMenuModel {
  Name: string;
  Path: string;
  ObjectType: string;
  CodeParent: string;
  LevelMenu: number;
  nameIcon: string;
  Active: Boolean;
  IsDefault: Boolean;
  LstChildren: Array<BreadCrumMenuModel>;
  Code: string;
  Display: string;

  //constructor(name: string, path: string, objectType: string, codeParent: string, levelMenu: number, active: Boolean) {
  //  this.Name = name;
  //  this.Path = path;
  //  this.ObjectType = objectType;
  //  this.CodeParent = codeParent;
  //  this.LevelMenu = levelMenu;
  //  this.Active = active;
  //};
  
}
