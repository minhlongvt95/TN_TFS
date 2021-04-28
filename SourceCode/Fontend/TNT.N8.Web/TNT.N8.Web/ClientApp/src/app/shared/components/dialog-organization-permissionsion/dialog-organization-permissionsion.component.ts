import { Component, OnInit, ElementRef, Inject, ViewChild } from '@angular/core';
import { OrganizationService } from '../../services/organization.service';
import { DynamicDialogRef, DynamicDialogConfig, DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { TreeViewComponent, NodeSelectEventArgs } from "@syncfusion/ej2-angular-navigations"

class ResultDialog {
  status: boolean;
  selectedOrganization: organizationModel;
}

class organizationModel {
  organizationId: string;
  organizationName: string;
  organizationCode: string;
  parentId: string;
  level: number;
  hasChildren: boolean;
}

@Component({
  selector: 'app-dialog-organization-permissionsion',
  templateUrl: './dialog-organization-permissionsion.component.html',
  styleUrls: ['./dialog-organization-permissionsion.component.css'],
  providers: [DialogService, MessageService]
})
export class DialogOrganizationPermissionsionComponent implements OnInit {
  /* system parameter */
  auth: any = JSON.parse(localStorage.getItem("auth"));
  isManager: boolean = JSON.parse(localStorage.getItem("IsManager"));
  loading: boolean = false;
  /* ejs treeview */
  @ViewChild('tree', { static: true }) public tree: TreeViewComponent;
  public listfields: Object
  /* Master data */
  listOrganization: Array<organizationModel> = [];
  listValidSelectionOrganization: Array<string> = []; /* danh sách phòng ban hợp lệ có thể chọn */
  /* dialog variable */
  selectedOrganization: organizationModel = null;
  canSelectOrganization: boolean = true;
  constructor(
    private el: ElementRef,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private messageService: MessageService,
    public dialogService: DialogService,
    public organizationService: OrganizationService
  ) { }

  ngOnInit(): void {
    this.getMasterdata();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  async getMasterdata() {
    this.loading = true;
    let result: any = await this.organizationService.getOrganizationByUser(this.auth.UserId);
    this.loading = false;
    if (result.statusCode === 200) {
      this.listOrganization = result.listOrganization ?? [];
      this.listValidSelectionOrganization = result.listValidSelectionOrganization;
      this.listfields = { dataSource: this.listOrganization, id: 'organizationId', parentID: 'parentId', text: 'organizationName', hasChildren: 'hasChildren' };
    } else {
      this.showToast('error', 'Thông báo', 'Lấy dữ liệu thất bại')
    }
  }

  public loadRoutingContent(args: NodeSelectEventArgs): void {
    let data: any = this.tree.getTreeData(args.node)[0];
    this.selectedOrganization = this.listOrganization.find(e => e.organizationId == data.organizationId);
    /* nếu là nhân viên thì chỉ được chọn phòng ban của nó */
    let _checkExistOrg = this.listValidSelectionOrganization.find(e => e == this.selectedOrganization.organizationId);
    if (_checkExistOrg) {
      this.canSelectOrganization = true;
    } else {
      this.canSelectOrganization = false;
    }

    console.log(this.canSelectOrganization)
  }

  /*Event Hủy dialog*/
  cancel() {
    this.ref.close();
  }
  /*End*/

  save() {
    let result: ResultDialog = {
      status: true,  //Lưu
      selectedOrganization: this.selectedOrganization
    };
    this.ref.close(result);
  }

}
