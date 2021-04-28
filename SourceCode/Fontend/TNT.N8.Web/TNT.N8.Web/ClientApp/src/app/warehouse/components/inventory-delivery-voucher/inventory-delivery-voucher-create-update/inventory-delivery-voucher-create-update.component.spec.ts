import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryDeliveryVoucherCreateUpdateComponent } from './inventory-delivery-voucher-create-update.component';

describe('InventoryDeliveryVoucherCreateUpdateComponent', () => {
  let component: InventoryDeliveryVoucherCreateUpdateComponent;
  let fixture: ComponentFixture<InventoryDeliveryVoucherCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventoryDeliveryVoucherCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryDeliveryVoucherCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
