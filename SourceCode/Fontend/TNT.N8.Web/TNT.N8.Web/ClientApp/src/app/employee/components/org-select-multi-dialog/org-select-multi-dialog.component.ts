import { Component, OnInit, Inject, ElementRef } from '@angular/core';

import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

import { OrganizationModel } from '../../../shared/models/organization.model';
import { OrganizationService } from '../../../shared/services/organization.service';
import { TranslateService } from '@ngx-translate/core';
import { SelectionModel } from '@angular/cdk/collections';

export interface IDialogData {
  selectedId: string;
  selectedName: string;
  chosenItem: boolean;
  lstSelection: SelectionModel<any>;
  mode: string;
}

@Component({
  selector: 'app-org-select-multi-dialog',
  templateUrl: './org-select-multi-dialog.component.html',
  styleUrls: ['./org-select-multi-dialog.component.css']
})
export class OrgSelectMultiDialogComponent implements OnInit {
  orgList: Array<OrganizationModel>;
  selectedOrgId: string = '';
  selectedOrgName: string;
  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };
  selection: SelectionModel<any>;

  constructor(
    private organizationService: OrganizationService,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<OrgSelectMultiDialogComponent>,
    public snackBar: MatSnackBar, ) {
    translate.setDefaultLang('vi');
  }
  onCancelClick() {
    this.data.chosenItem = false;

    this.dialogRef.close(this.data);
  }

  onSelectClick() {

    this.data.lstSelection = this.selection;
    this.dialogRef.close(this.data);
  }

  getAllOrganization() {
    this.organizationService.getAllOrganization().subscribe(response => {
      let result = <any>response;
      this.orgList = result.organizationList;
    }, error => { });

  }

  hasChild(org): boolean {
    return ((org.orgChildList.length > 0) ? true : false);
  }
  ngOnInit() {
    this.selection = new SelectionModel(true, []);

    this.getAllOrganization();

    if (this.data.lstSelection != null) {
      this.data.lstSelection.selected.forEach(rows => {
        if (!this.selection.selected.includes(rows)) {
          this.selection.toggle(rows);
        }
      });
    }
  }
  ngAfterViewInit() {

  }

  isAllSelectedLV2(orgChild: any) {
    var numSelected = 0;
    orgChild.orgChildList.forEach(rows => {
      var oj = this.selection.selected.find(x => x.organizationId == rows.organizationId);
      if (oj != null) {
        numSelected = numSelected + 1;
      }
      //if (this.selection.selected.includes(rows)) {
      //  numSelected = numSelected + 1;
      //}
    });
    const numRows = orgChild.orgChildList.length;
    return numSelected === numRows;
  }

  masterToggleLV2(orgChild: any) {
    if (this.isAllSelectedLV2(orgChild)) {

      var oj = this.selection.selected.find(x => x.organizationId == orgChild.organizationId);
      if (oj != null) {
        this.selection.deselect(oj);
      }

      orgChild.orgChildList.forEach(rows => {
        var ojChild = this.selection.selected.find(x => x.organizationId == rows.organizationId);
        if (ojChild != null) {
          this.selection.deselect(ojChild);
        }
      });
    } else {
      var oj = this.selection.selected.find(x => x.organizationId == orgChild.organizationId);
      if (oj == null) {
        this.selection.select(orgChild);
      }
      orgChild.orgChildList.forEach(rows => {
        const item: any = rows;
        var ojChild = this.selection.selected.find(x => x.organizationId == rows.organizationId);
        if (ojChild == null) {
          this.selection.select(item);
        }
      });
    }
  }
  //kiem tra khi da lua chon va close dialog
  CheckExist(orgChild: any) {
    if (this.data.lstSelection == null) {
      return false;
    }
    else {
      var oj = this.selection.selected.find(x => x.organizationId == orgChild.organizationId)
      if (oj != null) {
        return true;
      }
      else { return false; }
    }
  }
  rowCheckboxClickLV3(event: MatCheckboxChange, orgChild, row) {
    if (event) {
      //var oj = this.selection.selected.find(x => x.organizationId == row.organizationId);
      //if (oj == null) {
      //  this.selection.toggle(row);
      //}
      //else {
      //  this.selection.toggle(oj);
      //}
      //var ojChild = this.selection.selected.find(x => x.organizationId == orgChild.organizationId);
      //if (ojChild == null) {
      //  this.selection.toggle(orgChild);
      //}
      //else {
      //  this.selection.toggle(ojChild);
      //}
      //this.selection.toggle(row);
      //if (!this.selection.selected.includes(orgChild)) {
      //  this.selection.toggle(orgChild);
      //}
    }
    if (event.checked) {
      var oj = this.selection.selected.find(x => x.organizationId == row.organizationId);
      if (oj == null) {
        this.selection.select(row);
      }
      else {
        //this.selection.select(oj);
      }
      var ojChild = this.selection.selected.find(x => x.organizationId == orgChild.organizationId);
      if (ojChild == null) {
        this.selection.select(orgChild);
      }
      else {
        //this.selection.select(ojChild);
      }
    } else {

      var oj = this.selection.selected.find(x => x.organizationId == row.organizationId);
      if (oj != null) {
        this.selection.deselect(oj);
      }
      //else {
      //  this.selection.deselect(oj);
      //}
      //var ojChild = this.selection.selected.find(x => x.organizationId == orgChild.organizationId);
      //if (ojChild == null) {
      //  this.selection.deselect(orgChild);
      //}
      //else {
      //  this.selection.deselect(ojChild);
      //}
    }
  }

}
