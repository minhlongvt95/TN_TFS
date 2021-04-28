import { Pipe } from '@angular/core';
@Pipe({ name: 'PermissionModel' })
export class PermissionModel {
  PermissionId: string;
  PermissionCode: string;
  PermissionName: string;
  ParentId: string;
  PermissionDescription: string;
  Type: string;
  IconClass: string;
  Active: boolean;
  PermissionChildList: Array<PermissionModel>;
}
