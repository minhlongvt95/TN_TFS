import { TestBed, inject } from '@angular/core/testing';

import { EmployeeInsuranceService } from './employee-insurance.service';

describe('EmployeeInsuranceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeInsuranceService]
    });
  });

  it('should be created', inject([EmployeeInsuranceService], (service: EmployeeInsuranceService) => {
    expect(service).toBeTruthy();
  }));
});
