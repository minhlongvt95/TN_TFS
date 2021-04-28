import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialogRef } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

  const ELEMENT_TEMPLATE_EMAIL = [
    {
      id: 1,
      title: "Chúc mừng sinh nhật",
      content: `Nhân dịp sinh nhật quý khách, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    },
    {
      id: 2,
      title: "Chúc mừng sinh nhật của Công ty",
      content: `Nhân dịp sinh nhật của Công ty, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    },
    {
      id: 3,
      title: "Chúc mừng ngày lễ mùng 8-3",
      content: `Nhân ngày Quốc tế phụ nữ, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    },
    {
      id: 4,
      title: "Chúc mừng Quốc khánh",
      content: `Nhân dịp Quốc khánh nước CHXHCN Việt Nam, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    },
    {
      id: 5,
      title: "Chúc mừng Tết dương lịch",
      content: `Nhân dịp Tết dương lịch, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    },
    {
      id: 6,
      title: "Mừng lễ giỗ tổ Hùng Vương",
      content: `Nhân lễ giỗ tổ Hùng Vương, Công ty ABC xin chân thành cảm ơn quý khách thời gian qua đã luôn ủng hộ sản phẩm của Công ty và Công ty xin có gửi đến quý khách một phần quà nhỏ theo địa chỉ XYZ. Chúc quý khách một ngày bình an và vui vẻ.`
    }
  ];

@Component({
  selector: 'app-template-email-dialog',
  templateUrl: './template-email-dialog.component.html',
  styleUrls: ['./template-email-dialog.component.css']
})
export class TemplateEmailLeadCusDialogComponent implements OnInit {
  title: string = null;
  content: string = null;

  /*Form serach*/
  formSearch: FormGroup;

  titleControl: FormControl;
  contentControl: FormControl;
  /*End*/

  /*Data table*/
  listDataEmail: Array<any> = [];
  displayedColumns: string[] = ['title', 'content', 'actions'];
  dataSource = new MatTableDataSource(this.listDataEmail);
  pageSizeOptions = [5];

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  /*End*/

  constructor(
    private fb: FormBuilder,
    private translate: TranslateService,
    public dialogRef: MatDialogRef<TemplateEmailLeadCusDialogComponent>,
  ) {
    translate.setDefaultLang('vi');
  }

  ngOnInit() {
    this.titleControl = new FormControl(null);
    this.contentControl = new FormControl(null);

    this.formSearch = new FormGroup({
      titleControl: this.titleControl,
      contentControl: this.contentControl
    });

    this.listDataEmail = ELEMENT_TEMPLATE_EMAIL;
    this.dataSource = new MatTableDataSource(this.listDataEmail);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.paginator._intl.itemsPerPageLabel = 'Số email mẫu mỗi trang: ';
  }

  slectedTemplateEmail(id: number) {
    let item = this.listDataEmail.find(e => e.id == id);
    this.dialogRef.close(item);
  }

  onCancelClick() {
    this.dialogRef.close();
  }

  search() {
    
  }
}
