import { Component, OnInit } from '@angular/core';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { OrganizationService } from "../../../shared/services/organization.service";
import { OrganizationModel } from "../../../shared/models/organization.model";
import * as $ from 'jquery';

interface ResultDialog {
  status: boolean,
  selectedOrgId: string,
  selectedOrgName: string
}

@Component({
  selector: 'app-organization-dialog',
  templateUrl: './organization-dialog.component.html',
  styleUrls: ['./organization-dialog.component.css']
})
export class OrganizationDialogComponent implements OnInit {
  loading: boolean = false;

  orgList: Array<OrganizationModel>;
  selectedOrgId: string = '';
  selectedOrgName: string = '';
  chooseFinancialIndependence: boolean = false; //Là chọn đơn vị độc lập tài chính

  constructor(
    public ref: DynamicDialogRef, 
    public config: DynamicDialogConfig,
    private organizationService: OrganizationService,
  ) { 
    this.chooseFinancialIndependence = this.config.data.chooseFinancialIndependence != null ? this.config.data.chooseFinancialIndependence : false;
  }

  ngOnInit() {
    this.getAllOrganization();
  }

  async getAllOrganization() {
    this.loading = true;
    let result: any = await this.organizationService.getAllOrganizationAsync();
    this.loading = false;
    this.orgList = result.organizationList;

    setTimeout(() => {
      $(".tree-item table td:last-child").click((evt) => {
        //Có là chọn đơn vị độc lập tài chính?
        if (this.chooseFinancialIndependence) {
          //Kiểm tra đơn vị được chọn có phải đơn vị độc lập tài chính không
          let isFinancialIndependence = $(evt.currentTarget).closest('table').attr('title');
          if (isFinancialIndependence == 'true') {
            $(".tree-item table td:last-child").removeClass('selected-item');
            $(evt.currentTarget).toggleClass('selected-item');
            this.selectedOrgId = $(evt.currentTarget).closest('table').attr('id');
            this.selectedOrgName = $(evt.currentTarget).closest('table').find('span').text();
          }
        } else {
          $(".tree-item table td:last-child").removeClass('selected-item');
          $(evt.currentTarget).toggleClass('selected-item');
          this.selectedOrgId = $(evt.currentTarget).closest('table').attr('id');
          this.selectedOrgName = $(evt.currentTarget).closest('table').find('span').text();
        }
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

  save() {
    let result: ResultDialog = {
      status: true, //nhấn chọn
      selectedOrgId: this.selectedOrgId,
      selectedOrgName: this.selectedOrgName
    };

    if (result.selectedOrgId != '') {
      this.ref.close(result);
    }
  }

  cancel() {
    let result: ResultDialog = {
      status: false,  //không chọn
      selectedOrgId: '',
      selectedOrgName: ''
    };
    this.ref.close(result);
  }
}
