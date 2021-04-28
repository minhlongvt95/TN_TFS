import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogOrganizationPermissionsionComponent } from './dialog-organization-permissionsion.component';

describe('DialogOrganizationPermissionsionComponent', () => {
  let component: DialogOrganizationPermissionsionComponent;
  let fixture: ComponentFixture<DialogOrganizationPermissionsionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DialogOrganizationPermissionsionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogOrganizationPermissionsionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
