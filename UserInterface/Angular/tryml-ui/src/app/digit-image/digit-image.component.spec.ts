import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DigitImageComponent } from './digit-image.component';

describe('DigitImageComponent', () => {
  let component: DigitImageComponent;
  let fixture: ComponentFixture<DigitImageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DigitImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DigitImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
