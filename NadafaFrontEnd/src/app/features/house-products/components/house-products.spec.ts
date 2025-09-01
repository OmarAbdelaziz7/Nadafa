import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HouseProducts } from './house-products.component';

describe('HouseProducts', () => {
  let component: HouseProducts;
  let fixture: ComponentFixture<HouseProducts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HouseProducts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HouseProducts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
