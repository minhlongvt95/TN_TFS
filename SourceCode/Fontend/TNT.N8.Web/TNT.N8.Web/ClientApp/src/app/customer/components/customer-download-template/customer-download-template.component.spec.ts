import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDownloadTemplateComponent } from './customer-download-template.component';

describe('CustomerDownloadTemplateComponent', () => {
  let component: CustomerDownloadTemplateComponent;
  let fixture: ComponentFixture<CustomerDownloadTemplateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerDownloadTemplateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerDownloadTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
