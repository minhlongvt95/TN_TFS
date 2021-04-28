import { TestBed, inject } from '@angular/core/testing';

import { BudgetListService } from './budget-list.service';

describe('BudgetListService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BudgetListService]
    });
  });

  it('should be created', inject([BudgetListService], (service: BudgetListService) => {
    expect(service).toBeTruthy();
  }));
});
