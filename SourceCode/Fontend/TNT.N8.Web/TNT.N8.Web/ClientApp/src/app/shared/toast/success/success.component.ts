import { Component, OnInit, ViewEncapsulation, Inject } from '@angular/core';
import { MatSnackBar, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

@Component({
  selector: 'app-success',
  templateUrl: './success.component.html',
  styleUrls: ['./success.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class SuccessComponent implements OnInit {

  constructor(public snackBar: MatSnackBar,
    @Inject(MAT_SNACK_BAR_DATA) public data: any) {
  }

  ngOnInit() {
  }

  closeToast() {
      this.snackBar.dismiss();
  }
}
