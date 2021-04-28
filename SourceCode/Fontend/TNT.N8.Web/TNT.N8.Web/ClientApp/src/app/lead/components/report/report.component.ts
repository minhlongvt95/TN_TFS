import { Component, OnInit } from '@angular/core';
import { FormControl} from '@angular/forms';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {
  employeename = new FormControl();

  employeeNames: string[] = ['Person1','Person2','Person3'];

  selectedName: Array<any>

  // Khai báo các biến
  selected:   boolean = false;
  removable:  boolean = true;
  selectable: boolean = true;

  constructor() { }

  ngOnInit() {
  }

  selectedchange() :void {
    this.selected = (this.selectedName.length > 0) ? true:false;
  }
  
  clickalert(){
    alert("45555");
  }
  
  removeselected(item: string) :void {
    const index = this.selectedName.indexOf(item);
    if (index >=0)
    {   
      this.selectedName=this.selectedName.slice(0,index).concat(this.selectedName.slice(index+1));
    }
    if (this.selectedName.length <= 0) this.selected=false;
  }

  removeAll() : void {
    this.selectedName = [];
    this.selected = false;
  }


}
