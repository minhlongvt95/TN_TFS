import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentRequestCreateComponent } from './payment-request-create.component';

describe('PaymentRequestCreateComponent', () => {
  let component: PaymentRequestCreateComponent;
  let fixture: ComponentFixture<PaymentRequestCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentRequestCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentRequestCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
