import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcurementRequestItemDialogComponent } from './procurement-request-item-dialog.component';

describe('ProcurementRequestItemDialogComponent', () => {
  let component: ProcurementRequestItemDialogComponent;
  let fixture: ComponentFixture<ProcurementRequestItemDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcurementRequestItemDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcurementRequestItemDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
