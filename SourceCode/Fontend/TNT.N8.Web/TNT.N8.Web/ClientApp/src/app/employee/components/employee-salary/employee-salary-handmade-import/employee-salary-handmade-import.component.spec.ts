import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeSalaryHandmadeImportComponent } from './employee-salary-handmade-import.component';

describe('EmployeeSalaryHandmadeImportComponent', () => {
  let component: EmployeeSalaryHandmadeImportComponent;
  let fixture: ComponentFixture<EmployeeSalaryHandmadeImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeSalaryHandmadeImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeSalaryHandmadeImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
