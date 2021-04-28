import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddVendorOrderProductComponent } from './add-vendor-order-product.component';

describe('AddVendorOrderProductComponent', () => {
  let component: AddVendorOrderProductComponent;
  let fixture: ComponentFixture<AddVendorOrderProductComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddVendorOrderProductComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddVendorOrderProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
