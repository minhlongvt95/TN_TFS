import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleBiddingListComponent } from './sale-bidding-list.component';

describe('SaleBiddingListComponent', () => {
  let component: SaleBiddingListComponent;
  let fixture: ComponentFixture<SaleBiddingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleBiddingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleBiddingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
