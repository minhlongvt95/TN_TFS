import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InStockReportComponent } from './in-stock-report.component';

describe('InStockReportComponent', () => {
  let component: InStockReportComponent;
  let fixture: ComponentFixture<InStockReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InStockReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InStockReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
