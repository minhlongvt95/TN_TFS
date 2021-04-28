import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantSalaryHandmadeImportComponent } from './assistant-salary-handmade-import.component';

describe('AssistantSalaryHandmadeImportComponent', () => {
  let component: AssistantSalaryHandmadeImportComponent;
  let fixture: ComponentFixture<AssistantSalaryHandmadeImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssistantSalaryHandmadeImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssistantSalaryHandmadeImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
