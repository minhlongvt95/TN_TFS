import { TestBed, inject } from '@angular/core/testing';

import { EmployeeSalaryHandmadeImportService } from './employee-salary-handmade-import.service';

describe('EmployeeSalaryHandmadeImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeSalaryHandmadeImportService]
    });
  });

  it('should be created', inject([EmployeeSalaryHandmadeImportService], (service: EmployeeSalaryHandmadeImportService) => {
    expect(service).toBeTruthy();
  }));
});
