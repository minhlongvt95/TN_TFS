import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ListEmployeeRequestComponent } from './list-employee-request.component';

describe('ListEmployeeRequestComponent', () => {
  let component: ListEmployeeRequestComponent;
  let fixture: ComponentFixture<ListEmployeeRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListEmployeeRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListEmployeeRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
