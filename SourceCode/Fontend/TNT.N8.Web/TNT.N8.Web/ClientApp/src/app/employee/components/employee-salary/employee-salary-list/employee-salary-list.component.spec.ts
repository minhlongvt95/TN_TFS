import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeSalaryListComponent } from './employee-salary-list.component';

describe('EmployeeSalaryListComponent', () => {
  let component: EmployeeSalaryListComponent;
  let fixture: ComponentFixture<EmployeeSalaryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeSalaryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeSalaryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
