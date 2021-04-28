import { MenuSubModule } from './menu-sub-module.model';

export class MenuModule {
  name: string;
  code: string;
  nameIcon: string;
  children: Array<MenuSubModule>;  //list menu sub module của menu module

  constructor() {
    this.children = [];
  }
}