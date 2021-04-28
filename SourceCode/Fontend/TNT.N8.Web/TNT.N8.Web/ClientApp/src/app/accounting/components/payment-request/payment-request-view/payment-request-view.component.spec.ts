import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentRequestViewComponent } from './payment-request-view.component';

describe('PaymentRequestViewComponent', () => {
  let component: PaymentRequestViewComponent;
  let fixture: ComponentFixture<PaymentRequestViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentRequestViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentRequestViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
