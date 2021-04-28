import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherSalaryHandmadeImportComponent } from './teacher-salary-handmade-import.component';

describe('TeacherSalaryHandmadeImportComponent', () => {
  let component: TeacherSalaryHandmadeImportComponent;
  let fixture: ComponentFixture<TeacherSalaryHandmadeImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeacherSalaryHandmadeImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherSalaryHandmadeImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
