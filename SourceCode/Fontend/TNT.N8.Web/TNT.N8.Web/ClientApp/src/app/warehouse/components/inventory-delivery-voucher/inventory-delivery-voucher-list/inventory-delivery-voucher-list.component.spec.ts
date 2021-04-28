import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryDeliveryVoucherListComponent } from './inventory-delivery-voucher-list.component';

describe('InventoryDeliveryVoucherListComponent', () => {
  let component: InventoryDeliveryVoucherListComponent;
  let fixture: ComponentFixture<InventoryDeliveryVoucherListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventoryDeliveryVoucherListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventoryDeliveryVoucherListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
