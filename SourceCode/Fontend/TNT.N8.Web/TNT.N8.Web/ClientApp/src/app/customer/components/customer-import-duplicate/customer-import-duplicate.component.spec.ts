import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerImportDuplicateComponent } from './customer-import-duplicate.component';

describe('CustomerImportDuplicateComponent', () => {
  let component: CustomerImportDuplicateComponent;
  let fixture: ComponentFixture<CustomerImportDuplicateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerImportDuplicateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerImportDuplicateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
