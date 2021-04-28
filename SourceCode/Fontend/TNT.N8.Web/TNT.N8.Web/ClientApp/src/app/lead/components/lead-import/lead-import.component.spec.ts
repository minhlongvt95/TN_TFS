import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LeadImportComponent } from './lead-import.component';

describe('LeadImportComponent', () => {
  let component: LeadImportComponent;
  let fixture: ComponentFixture<LeadImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LeadImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LeadImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
