import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetCreatePopupComponent } from './budget-create-popup.component';

describe('BudgetCreatePopupComponent', () => {
  let component: BudgetCreatePopupComponent;
  let fixture: ComponentFixture<BudgetCreatePopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BudgetCreatePopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BudgetCreatePopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
