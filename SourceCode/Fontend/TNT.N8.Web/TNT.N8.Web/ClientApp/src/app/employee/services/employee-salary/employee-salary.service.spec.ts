import { TestBed, inject } from '@angular/core/testing';

import { EmployeeSalaryService } from './employee-salary.service';

describe('EmployeeSalaryService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeSalaryService]
    });
  });

  it('should be created', inject([EmployeeSalaryService], (service: EmployeeSalaryService) => {
    expect(service).toBeTruthy();
  }));
});
