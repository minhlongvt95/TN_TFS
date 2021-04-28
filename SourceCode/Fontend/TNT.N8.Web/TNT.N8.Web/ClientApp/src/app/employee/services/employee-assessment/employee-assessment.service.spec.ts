import { TestBed, inject } from '@angular/core/testing';

import { EmployeeAssessmentService } from './employee-assessment.service';

describe('EmployeeAssessmentService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeAssessmentService]
    });
  });

  it('should be created', inject([EmployeeAssessmentService], (service: EmployeeAssessmentService) => {
    expect(service).toBeTruthy();
  }));
});
