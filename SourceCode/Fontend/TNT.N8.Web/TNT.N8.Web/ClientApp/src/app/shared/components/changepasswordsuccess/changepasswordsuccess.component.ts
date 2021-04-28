import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface datadialog{
  accountName: string
}

@Component({
  selector: 'app-changepasswordsuccess',
  templateUrl: './changepasswordsuccess.component.html',
  styleUrls: ['./changepasswordsuccess.component.css']
})
export class ChangepasswordsuccessComponent implements OnInit {

  constructor(
    public router:Router,
    public dialogref: MatDialogRef<ChangepasswordsuccessComponent>,
    @Inject(MAT_DIALOG_DATA) public data: datadialog
  ) { }

  ngOnInit() {
  }

  backtologin() {
    this.router.navigate(['/login']);
    this.dialogref.close(); 
  }
}
