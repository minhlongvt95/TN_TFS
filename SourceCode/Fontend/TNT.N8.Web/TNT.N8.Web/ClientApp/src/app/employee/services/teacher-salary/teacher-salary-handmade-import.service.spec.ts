import { TestBed, inject } from '@angular/core/testing';

import { TeacherSalaryHandmadeImportService } from './teacher-salary-handmade-import.service';

describe('TeacherSalaryHandmadeImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TeacherSalaryHandmadeImportService]
    });
  });

  it('should be created', inject([TeacherSalaryHandmadeImportService], (service: TeacherSalaryHandmadeImportService) => {
    expect(service).toBeTruthy();
  }));
});
