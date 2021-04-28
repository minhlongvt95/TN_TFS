import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { ForderConfigurationService } from '../folder-configuration/services/folder-configuration.service';
import { EmployeeService } from '../../../employee/services/employee.service';
import { BankService } from '../../../shared/services/bank.service';
import { GetPermission } from '../../../shared/permission/get-permission';

import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { TreeViewComponent, NodeSelectEventArgs } from "@syncfusion/ej2-angular-navigations"
import { TreeTableModule } from 'primeng/treetable';
import { TreeNode } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  listFile: Array<FileInFolder>;
  folderType: string;
  numberFile: number;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Component({
  selector: 'app-folder-configuration',
  templateUrl: './folder-configuration.component.html',
  styleUrls: ['./folder-configuration.component.css']
})
export class FolderConfigurationComponent implements OnInit {
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  actionEdit: boolean = true;
  actionDelete: boolean = true;
  loading: boolean = false;
  showDialog: boolean = false;
  showDialogAddFolder: boolean = false;
  showDialogAddFile: boolean = false;

  //Tabe TreeNode
  listFolderTreeNode: TreeNode[] = [];
  selectedFolderNode: TreeNode[] = [];
  folderName: string = "";
  folderNameResult: string = "";
  // Table 
  colsDialog: any;
  cols: any;
  selectedColumns: any;
  colsChild: any;
  selectedColumnsChild: any;
  //master data
  listFolder: Array<Folder> = [];
  @ViewChild('tree')
  public tree: TreeViewComponent;
  public listfields: Object;
  listDetailFolder: Array<Folder> = [];
  listChildFolder: Array<Folder> = [];

  fileName: string = '';

  @ViewChild('fileUpload', { static: true }) fileUpload: FileUpload;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  isUpdateAI: boolean = false;
  importFileExcel: any = null;
  messErrFile: any = [];
  cellErrFile: any = [];

  file: File;
  arrayBuffer: any;
  filelist: any;

  public loadRoutingContent(args: NodeSelectEventArgs): void {
    let data: any = this.tree.getTreeData(args.node)[0];
    this.listDetailFolder = [data];
    this.listChildFolder = this.listFolder.filter(e => e.parentId == data.folderId);
  }

  constructor(
    private employeeService: EmployeeService,
    private formBuilder: FormBuilder,
    private el: ElementRef,
    private router: Router,
    private getPermission: GetPermission,
    private bankService: BankService,
    private dialogService: DialogService,
    private messageService: MessageService,
    private folderService: ForderConfigurationService
  ) { }

  ngOnInit() {
    this.loading = true;
    this.employeeService.checkAdminLogin().subscribe(response => {
      let result = <any>response;
      if (result.statusCode === 202 || result.statusCode === 200) {
        this.initTable();
        this.getMasterData();
      }
      if (result.statusCode === 404) {
        let resource = "sys/admin/folder-config/";
        let permission: any = this.getPermission.getPermission(this.listPermissionResource, resource);
        if (permission.status == false) {
          this.router.navigate(['/home']);
        }
        else {
          let listCurrentActionResource = permission.listCurrentActionResource;
          if (listCurrentActionResource.indexOf("edit") == -1) {
            this.actionEdit = false;
          }
          if (listCurrentActionResource.indexOf("delete") == -1) {
            this.actionDelete = false;
          }

          this.initTable();
          this.getMasterData();
        }
      }
    });
  }

  showMessage(msg: any) {
    this.messageService.add(msg);
  }

  initTable() {
    this.cols = [
      { field: 'name', header: 'Tên folder', width: '40%', textAlign: 'left', color: '#f44336' },
      { field: 'url', header: 'Đường dẫn', width: '30%', textAlign: 'left', color: '#f44336' },
      { field: 'number', header: 'Số file', width: '15%', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Thao tác', width: '15%', textAlign: 'right', color: '#f44336' }
    ];

    this.colsDialog = [
      { field: 'name', header: 'Tên folder', width: '40%', textAlign: 'left', color: '#f44336' },
      { field: 'url', header: 'Đường dẫn', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'number', header: 'Số file', width: '10%', textAlign: 'right', color: '#f44336' },
    ];

    this.colsChild = [
      { field: 'fileName', header: 'Tên file', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'size', header: 'Size', width: '30%', textAlign: 'right', color: '#f44336' },
      { field: 'delete', header: 'Thao tác', width: '20%', textAlign: 'left', color: '#f44336' }
    ];
  }

  getMasterData() {
    this.folderService.getAllFolderActive().subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listFolder = result.listFolderActive;
        let isActive = this.listFolder.filter(x => x.parentId == null);
        if (isActive == null || isActive.length == 0) {
          this.listFolder = [];
        }
        this.listfields = { dataSource: this.listFolder, id: 'folderId', parentID: 'parentId', text: 'name', hasChildren: 'hasChild' };
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  getDataDefault() {
    this.loading = true;
    this.listFolderTreeNode = [];
    this.folderService.getAllFolderDefault().subscribe(response => {
      let result: any = response;
      this.loading = false;
      this.showDialog = true;
      if (result.statusCode == 200) {
        let listFolderCommon: Array<Folder> = result.listFolderDefault;
        let listDataRoot = listFolderCommon.filter(x => x.parentId == null);
        listDataRoot.forEach(item => {
          let nodeRoot: TreeNode = { data: item, children: this.mapDataTreeNode(listFolderCommon, item) }
          nodeRoot.expanded = true;
          this.listFolderTreeNode.push(nodeRoot);
        });

        this.listFolderTreeNode = [...this.listFolderTreeNode];
        this.selectedFolderNode = [];
        listFolderCommon.forEach(element => {
          if (element.active == true) {
            let treeNode: TreeNode = { data: element };
            this.selectedFolderNode.push(treeNode);
          }
        });
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  mapDataTreeNode(listFolderCommon: Array<Folder>, folder: Folder): Array<TreeNode> {
    let result: Array<TreeNode> = [];
    let listChildren = listFolderCommon.filter(x => x.parentId == folder.folderId);
    listChildren.forEach(item => {
      let dataRoot: Folder = new Folder();
      dataRoot = item;
      let listChildrenNode: Array<TreeNode> = [];
      if (item.hasChild) {
        listChildrenNode = this.mapDataTreeNode(listFolderCommon, item);
      }
      let node: TreeNode = { data: dataRoot, children: listChildrenNode }
      node.expanded = true;
      result.push(node);
    });
    return result;
  }

  settingFolder() {
    let listFolder: Array<Folder> = this.selectedFolderNode.map(x => x.data);
    this.loading = true;
    this.folderService.addOrUpdateFolder(listFolder).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listFolder = this.listFolder.filter(x => x.active == true && x.isDelete == true);
        this.selectedFolderNode = [];
        result.listFolder.forEach(element => {
          this.listFolder.push(element);
          let treeNode: TreeNode = { data: element };
          this.selectedFolderNode.push(treeNode);
        });
        let isActive = this.listFolder.filter(x => x.parentId == null);
        if (isActive == null || isActive.length == 0) {
          this.listFolder = [];
        }
        this.listfields = { dataSource: this.listFolder, id: 'folderId', parentID: 'parentId', text: 'name', hasChildren: 'hasChild' };
        let msg = { severity: 'success', summary: 'Thông báo', detail: "Cài đặt thành công!" };
        this.showMessage(msg);
        this.showDialog = false;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  addFile() {
    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();
      fileUpload.FileInFolder = new FileInFolder();
      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index, item.name.length - index);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileSave = item;
      listFileUploadModel.push(fileUpload)
    });
    let folderId: string = this.listDetailFolder[0].folderId;
    this.folderService.uploadFileByFolderId(folderId, listFileUploadModel).subscribe(response => {

      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listDetailFolder[0].listFile;
        this.uploadedFiles = [];
        this.fileUpload.files = [];
        let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm file thành công" };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
      this.showDialogAddFile = false;
    });

  }

  changeNameFolder() {
    this.folderNameResult = removeVietnameseTones(this.folderName);
  }

  addFolder() {
    if (this.folderNameResult.length > 30) {
      let msg = { severity: 'error', summary: 'Thông báo', detail: "Tên thư mục không vượt quá 30 kí tự" };
      this.showMessage(msg);
    } else {
      this.loading = true;
      let folder: Folder = this.listDetailFolder[0];
      this.folderService.createFolder(folder, this.folderNameResult).subscribe(response => {
        this.listDetailFolder = [];
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          let folder2: Folder = result.folder;
          this.listFolder.push(folder2);
          this.listFolder.forEach(element => {
            if (element.folderId == folder.folderId) {
              element.hasChild = true;
            }
          });
          this, this.folderName = "";
          this.folderNameResult = "";
          this.listFolder = [...this.listFolder];
          this.listfields = { dataSource: this.listFolder, id: 'folderId', parentID: 'parentId', text: 'name', hasChildren: 'hasChild' };
          let msg = { severity: 'success', summary: 'Thông báo', detail: "Thêm thành công!" };
          this.showMessage(msg);
          this.showDialogAddFolder = false;
        } else {
          let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
          this.showMessage(msg);
        }
      });
    }
  }

  removeFolder(data: Folder) {
    this.loading = true;
    this.folderService.deleteFolder(data).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.listFolder = this.listFolder.filter(x => x.folderId != data.folderId);
        this.listFolder.forEach(element => {
          if (element.folderId == data.parentId) {
            let children = this.listFolder.filter(x => x.parentId == element.folderId);
            if (children == null || children.length == 0) {
              element.hasChild = false;
            }
          }
        });
        this.listFolder = [...this.listFolder];
        this.listfields = { dataSource: this.listFolder, id: 'folderId', parentID: 'parentId', text: 'name', hasChildren: 'hasChild' };
        let msg = { severity: 'success', summary: 'Thông báo', detail: "Xóa thành công!" };
        this.showMessage(msg);
        this.showDialogAddFolder = false;
      } else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= 10000000) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  downloadFile(fileInFolderId: string, name: string, fileExtension: string) {
    this.folderService.downloadFile(fileInFolderId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        var binaryString = atob(result.fileAsBase64);
        var fileType = result.fileType;
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var idx = 0; idx < binaryLen; idx++) {
          var ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        var file = new Blob([bytes], { type: fileType });
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = name.substring(0, name.lastIndexOf('_')) + "." + fileExtension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      }
      else {
        let msg = { severity: 'error', summary: 'Thông báo', detail: result.messageCode };
        this.showMessage(msg);
      }
    });
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  chooseFile(event) {
    this.fileName = event.target.files[0].name;
    this.importFileExcel = event.target;
  }

}
function removeVietnameseTones(str) {
  str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
  str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
  str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
  str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
  str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
  str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
  str = str.replace(/đ/g, "d");
  str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
  str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
  str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
  str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
  str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
  str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
  str = str.replace(/Đ/g, "D");
  // Some system encode vietnamese combining accent as individual utf-8 characters
  // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
  str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
  str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
  // Remove extra spaces
  // Bỏ các khoảng trắng liền nhau
  str = str.replace(/ + /g, " ");
  str = str.trim();
  // Remove punctuations
  // Bỏ dấu câu, kí tự đặc biệt
  str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
  return str;
}