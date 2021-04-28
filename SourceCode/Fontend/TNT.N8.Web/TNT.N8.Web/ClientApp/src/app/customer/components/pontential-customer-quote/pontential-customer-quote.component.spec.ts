import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PontentialCustomerQuoteComponent } from './pontential-customer-quote.component';

describe('PontentialCustomerQuoteComponent', () => {
  let component: PontentialCustomerQuoteComponent;
  let fixture: ComponentFixture<PontentialCustomerQuoteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PontentialCustomerQuoteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PontentialCustomerQuoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
