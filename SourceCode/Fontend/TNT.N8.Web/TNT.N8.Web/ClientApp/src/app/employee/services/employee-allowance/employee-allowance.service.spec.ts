import { TestBed, inject } from '@angular/core/testing';

import { EmployeeAllowanceService } from './employee-allowance.service';

describe('EmployeeAllowanceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeAllowanceService]
    });
  });

  it('should be created', inject([EmployeeAllowanceService], (service: EmployeeAllowanceService) => {
    expect(service).toBeTruthy();
  }));
});
