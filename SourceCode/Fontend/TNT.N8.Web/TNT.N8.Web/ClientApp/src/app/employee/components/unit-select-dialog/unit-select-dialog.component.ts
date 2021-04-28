import { Component, OnInit, Inject } from '@angular/core';
import { TreeviewModule, TreeviewItem, TreeviewConfig } from 'ngx-treeview';

import { IDialogData } from '../org-select-dialog/org-select-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-unit-select-dialog',
  templateUrl: './unit-select-dialog.component.html',
  styleUrls: ['./unit-select-dialog.component.css']
})
export class UnitSelectDialogComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IDialogData,
    public dialogRef: MatDialogRef<UnitSelectDialogComponent>,
  ) { }

  dropdownEnabled = true;
  items: TreeviewItem[];
  values: number[];
  config = TreeviewConfig.create({
      hasAllCheckBox: true,
      hasFilter: true,
      hasCollapseExpand: true,
      decoupleChildFromParent: false,
      maxHeight: 400
  });

  buttonClasses = [
      'btn-outline-primary',
      'btn-outline-secondary',
      'btn-outline-success',
      'btn-outline-danger',
      'btn-outline-warning',
      'btn-outline-info',
      'btn-outline-light',
      'btn-outline-dark'
  ];
  buttonClass = this.buttonClasses[0];

  ngOnInit() {
      // this.items = this.service.getBooks();
  }

  onFilterChange(value: string) {
  }

}
