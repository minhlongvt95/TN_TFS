import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LeadImportDuplicateComponent } from './lead-import-duplicate.component';

describe('LeadImportDuplicateComponent', () => {
  let component: LeadImportDuplicateComponent;
  let fixture: ComponentFixture<LeadImportDuplicateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LeadImportDuplicateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LeadImportDuplicateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
