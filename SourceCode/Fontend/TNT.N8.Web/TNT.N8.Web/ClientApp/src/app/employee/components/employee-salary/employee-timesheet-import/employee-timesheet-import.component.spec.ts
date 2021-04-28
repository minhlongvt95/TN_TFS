import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeTimesheetImportComponent } from './employee-timesheet-import.component';

describe('EmployeeTimesheetImportComponent', () => {
  let component: EmployeeTimesheetImportComponent;
  let fixture: ComponentFixture<EmployeeTimesheetImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeTimesheetImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeTimesheetImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
