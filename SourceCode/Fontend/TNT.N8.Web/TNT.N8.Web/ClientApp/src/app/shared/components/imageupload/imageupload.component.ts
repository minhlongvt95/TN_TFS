import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';

@Component({
  selector: 'app-imageupload',
  templateUrl: './imageupload.component.html',
  styleUrls: ['./imageupload.component.css'],
})
export class ImageUploadComponent implements OnInit {
  fileToUpload: File;
  fileName:string;
  employeeId: string;
  data: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router) 
  {
    this.data = {};
  }
  
  ngOnInit() {
    
  }

}
