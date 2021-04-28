import { Component, OnInit, Inject, ElementRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { OrganizationModel } from '../../../shared/models/organization.model';
import { OrganizationService } from '../../../shared/services/organization.service';
import { TranslateService } from '@ngx-translate/core';
import { FailComponent } from '../../../shared/toast/fail/fail.component';
import * as $ from 'jquery';

export interface IDialogData {
  selectedId: string;
  selectedName: string;
  chosenItem: boolean;
  mode: string;
}

@Component({
  selector: 'app-org-select-dialog',
  templateUrl: './org-select-dialog.component.html',
  styleUrls: ['./org-select-dialog.component.css']
})
export class OrgSelectDialogComponent implements OnInit {
  orgList: Array<OrganizationModel>;
  selectedOrgId: string = '';
  selectedOrgName: string;

  failConfig: MatSnackBarConfig = { panelClass: 'fail-dialog', horizontalPosition: 'end', duration: 5000 };

  constructor(
    private organizationService: OrganizationService,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<OrgSelectDialogComponent>,
    public snackBar: MatSnackBar,) {
    translate.setDefaultLang('vi');
  }

  onCancelClick() {
    this.data.chosenItem = false;
    this.dialogRef.close(this.data);
  }

  onSelectClick() {
    if (this.selectedOrgId === null || this.selectedOrgId === '') {
      this.snackBar.openFromComponent(FailComponent, { data: 'Vui lòng chọn 1 Đơn vị!', ...this.failConfig });
    } else {
      this.data.chosenItem = true;
      this.data.selectedId = this.selectedOrgId;
      this.data.selectedName = this.selectedOrgName;
      this.dialogRef.close(this.data);
    }
  }

  getAllOrganization() {
    // this.organizationService.getAllOrganization().subscribe(response => {
    //   let result = <any>response;
    //   this.orgList = result.organizationList;
    // }, error => { });

    this.organizationService.getChildrenByOrganizationId(this.data.selectedId).subscribe(response => {
      let result = <any>response;
      this.orgList = result.organizationList;
    }, error => { });

    setTimeout(() => {
      $('.tree-item table td:last-child').click((evt) => {
        $('.tree-item table td:last-child').removeClass('selected-item');
        $(evt.currentTarget).toggleClass('selected-item');
        this.selectedOrgId = $(evt.currentTarget).closest('table').attr('id');
        this.selectedOrgName = $(evt.currentTarget).closest('table').find('span').text();
      });

      $('.tree-icon').click((evt) => {
        let currentText = $(evt.currentTarget).text().trim();
        $(evt.currentTarget).text(currentText === 'arrow_drop_down' ? 'arrow_right' : 'arrow_drop_down');
      });

      /*Khi mo dialog se chon item da duoc chon lan load truoc*/
      if (this.data.selectedId !== null && this.data.selectedId !== '') {
        $('.tree-item').find('table#' + this.data.selectedId + ' td:last-child').click();
      }
    }, 500);
  }

  hasChild(org): boolean {
    return ((org.orgChildList.length > 0) ? true : false);
  }

  ngOnInit() {
    this.getAllOrganization();
  }
}
