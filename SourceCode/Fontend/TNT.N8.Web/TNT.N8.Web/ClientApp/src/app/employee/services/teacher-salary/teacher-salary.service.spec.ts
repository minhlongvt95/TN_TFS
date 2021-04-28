import { TestBed, inject } from '@angular/core/testing';

import { TeacherSalaryService } from './teacher-salary.service';

describe('TeacherSalaryService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TeacherSalaryService]
    });
  });

  it('should be created', inject([TeacherSalaryService], (service: TeacherSalaryService) => {
    expect(service).toBeTruthy();
  }));
});
