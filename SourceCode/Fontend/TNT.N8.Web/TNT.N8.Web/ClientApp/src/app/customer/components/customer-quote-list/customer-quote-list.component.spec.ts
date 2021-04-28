import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerQuoteListComponent } from './customer-quote-list.component';

describe('CustomerQuoteListComponent', () => {
  let component: CustomerQuoteListComponent;
  let fixture: ComponentFixture<CustomerQuoteListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerQuoteListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerQuoteListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
