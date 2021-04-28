import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogImportPotentialcustomerComponent } from './dialog-import-potentialcustomer.component';

describe('DialogImportPotentialcustomerComponent', () => {
  let component: DialogImportPotentialcustomerComponent;
  let fixture: ComponentFixture<DialogImportPotentialcustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DialogImportPotentialcustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogImportPotentialcustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
