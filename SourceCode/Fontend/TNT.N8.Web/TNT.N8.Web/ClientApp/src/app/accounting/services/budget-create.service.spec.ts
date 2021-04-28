import { TestBed, inject } from '@angular/core/testing';

import { BudgetCreateService } from './budget-create.service';

describe('BudgetCreateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BudgetCreateService]
    });
  });

  it('should be created', inject([BudgetCreateService], (service: BudgetCreateService) => {
    expect(service).toBeTruthy();
  }));
});
