import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppRouting } from './app.routing';
import { SharedModule } from './shared/shared.module';
import { UserprofileComponent } from "./userprofile/userprofile.component";
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { CommonService } from './shared/services/common.service';
import { LocationStrategy, PathLocationStrategy } from '@angular/common';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { QueryBuilderModule } from "angular2-query-builder";
import { PanelMenuModule } from 'primeng/panelmenu';


export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserprofileComponent,

  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    AppRouting,
    SharedModule,
    NgMultiSelectDropDownModule.forRoot(),
    QueryBuilderModule,
    PanelMenuModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient],
      }
    })
  ],
  providers: [
    CommonService,
    { provide: LocationStrategy, useClass: PathLocationStrategy }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private commonService: CommonService) {
    localStorage.setItem('ApiEndPoint', 'http://localhost:5001');
    localStorage.setItem('Version', '1.0.0');

    // commonService.getApiEndPoint().subscribe(result => {
    //   if (result.value !== localStorage.getItem('ApiEndPoint')) {
    //     localStorage.setItem('ApiEndPoint', result.value);
    //   }
    // });
    // commonService.getVersion().subscribe(result => {
    //   if (result.value !== localStorage.getItem('Version')) {
    //     localStorage.setItem('Version', result.value);
    //   }
    // });
  }
}
