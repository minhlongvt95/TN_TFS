import { TestBed, inject } from '@angular/core/testing';

import { EmployeeMonthySalaryService } from './employee-monthy-salary.service';

describe('EmployeeMonthySalaryService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeMonthySalaryService]
    });
  });

  it('should be created', inject([EmployeeMonthySalaryService], (service: EmployeeMonthySalaryService) => {
    expect(service).toBeTruthy();
  }));
});
