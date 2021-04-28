import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeCreateSalaryPopupComponent } from './employee-create-salary-popup.component';

describe('EmployeeCreateSalaryPopupComponent', () => {
  let component: EmployeeCreateSalaryPopupComponent;
  let fixture: ComponentFixture<EmployeeCreateSalaryPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeCreateSalaryPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeCreateSalaryPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
