import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantSalaryExportComponent } from './assistant-salary-export.component';

describe('AssistantSalaryExportComponent', () => {
  let component: AssistantSalaryExportComponent;
  let fixture: ComponentFixture<AssistantSalaryExportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssistantSalaryExportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssistantSalaryExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
