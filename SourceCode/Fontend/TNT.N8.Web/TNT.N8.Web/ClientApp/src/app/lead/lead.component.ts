import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lead',
  templateUrl: './lead.component.html',
  styleUrls: ['./lead.component.css']
})
export class LeadComponent implements OnInit {
  constructor(private router: Router) {
    
  }

  ngOnInit() {
    if (this.router.url === '/lead') this.router.navigate(['lead/dashboard']);
  }

}
