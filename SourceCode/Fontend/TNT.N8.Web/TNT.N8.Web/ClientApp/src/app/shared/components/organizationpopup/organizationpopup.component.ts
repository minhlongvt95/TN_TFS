import { Component, OnInit, ElementRef, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, } from '@angular/material/snack-bar';
import { OrganizationModel } from "../../../shared/models/organization.model";
import { OrganizationService } from "../../../shared/services/organization.service";
import { TranslateService } from '@ngx-translate/core';
import * as $ from 'jquery';

export interface IDialogData {
  selectedOrgId: string;
  selectedOrgName: string;
}

@Component({
  selector: 'app-organizationpopup',
  templateUrl: './organizationpopup.component.html',
  styleUrls: ['./organizationpopup.component.css']
})
export class OrganizationpopupComponent implements OnInit {

  orgList: Array<OrganizationModel>;
  selectedOrgId: string = '';
  selectedOrgName: string;
  constructor(private el: ElementRef,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<OrganizationpopupComponent>,
    private organizationService: OrganizationService,
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    private translate: TranslateService,
    public snackBar: MatSnackBar) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.getAllOrganization();
  }

  async getAllOrganization() {
    let result: any = await this.organizationService.getAllOrganizationAsync();
    this.orgList = result.organizationList;

    this.organizationService.getAllOrganization().subscribe(response => {
    	let result = <any>response;
    	this.orgList = result.organizationList;
    }, error => { });

    setTimeout(() => {
      $(".tree-item table td:last-child").click((evt) => {
        $(".tree-item table td:last-child").removeClass('selected-item');
        $(evt.currentTarget).toggleClass('selected-item');
        this.data.selectedOrgId = $(evt.currentTarget).closest('table').attr('id');
        this.data.selectedOrgName = $(evt.currentTarget).closest('table').find('span').text();
      });

      $(".tree-icon").click((evt) => {
        let currentText = $(evt.currentTarget).text().trim();
        $(evt.currentTarget).text(currentText === 'arrow_drop_down' ? 'arrow_right' : 'arrow_drop_down');
      });

      $(".tree-item.lvl-1 table:first td:last-child").click();
    }, 500);
  }

  hasChild(org): boolean {
    return ((org.orgChildList.length > 0) ? true : false);
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  onSaveClick() {
    this.dialogRef.close(this.data);
  }
}
