import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherSalaryListComponent } from './teacher-salary-list.component';

describe('TeacherSalaryListComponent', () => {
  let component: TeacherSalaryListComponent;
  let fixture: ComponentFixture<TeacherSalaryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeacherSalaryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherSalaryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
