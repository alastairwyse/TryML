import { TestBed } from '@angular/core/testing';

import { DigitImageService } from './digit-image.service';

describe('DigitImageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DigitImageService = TestBed.get(DigitImageService);
    expect(service).toBeTruthy();
  });
});
