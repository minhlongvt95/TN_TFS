import { Component, OnInit } from '@angular/core';
import { Observable ,  Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, MessageService } from 'primeng/api';

@Component({
  selector: 'main-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  version: string = localStorage.getItem('Version');
  systemParameterList: Array<any> = JSON.parse(localStorage.getItem('systemParameterList'));
 
  messages: any = { version: '', copyright: '' };
  constructor(
    private translate: TranslateService,
    private messageService: MessageService,
  ) {
    this.translate.setDefaultLang('vi');
    
    this.translate.get('common.version').subscribe(value => {      
      this.messages.version = value;
    });
    
    let companySetting = this.systemParameterList.find(e => e.systemKey === "CompanyName").systemValueString;
    let _copyright = `License được cấp cho ${companySetting}. Sản phẩm được phát triển bởi <a target='_blank' href='http://www.tringhiatech.vn/vi/'>Công ty TNHH Công nghệ, Thương mại và Dịch vụ TRÍ NGHĨA.</a>`;
    this.messages.copyright = _copyright;
  }

  ngOnInit() {
    
  }

}
