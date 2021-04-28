import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductCategoryPopupComponent } from './product-category-popup.component';

describe('ProductCategoryPopupComponent', () => {
  let component: ProductCategoryPopupComponent;
  let fixture: ComponentFixture<ProductCategoryPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductCategoryPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductCategoryPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
