import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleBiddingCreateComponent } from './sale-bidding-create.component';

describe('SaleBiddingCreateComponent', () => {
  let component: SaleBiddingCreateComponent;
  let fixture: ComponentFixture<SaleBiddingCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleBiddingCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleBiddingCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
