import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcurementRequestViewComponent } from './procurement-request-view.component';

describe('ProcurementRequestViewComponent', () => {
  let component: ProcurementRequestViewComponent;
  let fixture: ComponentFixture<ProcurementRequestViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcurementRequestViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcurementRequestViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
