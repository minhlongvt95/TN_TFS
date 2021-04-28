import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherSapiGetComponent } from './teacher-sapi-get.component';

describe('TeacherSapiGetComponent', () => {
  let component: TeacherSapiGetComponent;
  let fixture: ComponentFixture<TeacherSapiGetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeacherSapiGetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherSapiGetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
