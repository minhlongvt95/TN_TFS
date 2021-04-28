import { Pipe } from '@angular/core';
@Pipe({ name: 'PermissionSetModel' })
export class PermissionSetModel {
  PermissionSetId: string;
  PermissionSetName: string;
  PermissionSetCode: string;
  PermissionSetDescription: string;
  PermissionId: Array<any>;
  PermissionList: Array<any>;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  Active: boolean;
  NumberOfUserHasPermission: number;
}
