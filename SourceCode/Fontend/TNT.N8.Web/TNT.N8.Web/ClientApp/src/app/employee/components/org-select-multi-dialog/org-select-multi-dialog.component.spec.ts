import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgSelectMultiDialogComponent } from './org-select-multi-dialog.component';

describe('OrgSelectMultiDialogComponent', () => {
  let component: OrgSelectMultiDialogComponent;
  let fixture: ComponentFixture<OrgSelectMultiDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrgSelectMultiDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrgSelectMultiDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
