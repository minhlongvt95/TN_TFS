import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryvoucherCreateSerialComponent } from './deliveryvoucher-create-serial.component';

describe('DeliveryvoucherCreateSerialComponent', () => {
  let component: DeliveryvoucherCreateSerialComponent;
  let fixture: ComponentFixture<DeliveryvoucherCreateSerialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeliveryvoucherCreateSerialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeliveryvoucherCreateSerialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
