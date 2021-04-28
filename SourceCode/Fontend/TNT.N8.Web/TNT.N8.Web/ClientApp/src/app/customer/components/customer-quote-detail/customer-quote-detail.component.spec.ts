import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerQuoteDetailComponent } from './customer-quote-detail.component';

describe('CustomerQuoteDetailComponent', () => {
  let component: CustomerQuoteDetailComponent;
  let fixture: ComponentFixture<CustomerQuoteDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerQuoteDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerQuoteDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
