import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcurementRequestCreateComponent } from './procurement-request-create.component';

describe('ProcurementRequestCreateComponent', () => {
  let component: ProcurementRequestCreateComponent;
  let fixture: ComponentFixture<ProcurementRequestCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProcurementRequestCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcurementRequestCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
