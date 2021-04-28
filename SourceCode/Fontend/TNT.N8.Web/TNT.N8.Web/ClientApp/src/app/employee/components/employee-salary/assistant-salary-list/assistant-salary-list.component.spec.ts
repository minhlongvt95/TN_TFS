import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantSalaryListComponent } from './assistant-salary-list.component';

describe('AssistantSalaryListComponent', () => {
  let component: AssistantSalaryListComponent;
  let fixture: ComponentFixture<AssistantSalaryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssistantSalaryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssistantSalaryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
