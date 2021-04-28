import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailEmployeeRequestComponent } from './detail-employee-request.component';

describe('DetailEmployeeRequestComponent', () => {
  let component: DetailEmployeeRequestComponent;
  let fixture: ComponentFixture<DetailEmployeeRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailEmployeeRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailEmployeeRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
