import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationpopupComponent } from './organizationpopup.component';

describe('OrganizationpopupComponent', () => {
  let component: OrganizationpopupComponent;
  let fixture: ComponentFixture<OrganizationpopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganizationpopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganizationpopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
