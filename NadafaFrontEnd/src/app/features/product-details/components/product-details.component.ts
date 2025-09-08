import { Component } from '@angular/core';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.css'],
})
export class ProductDetails{
  pricePerKg: number = 0;
  amountKg: number = 0;
  totalPrice: number = 0;

  ngDoCheck() {
    this.totalPrice = this.pricePerKg * this.amountKg;
  }

  addToCart() {
    if (this.amountKg <= 0 || this.pricePerKg <= 0) {
      alert('Please enter valid price and amount!');
      return;
    }
    alert(`Added ${this.amountKg} kg of papers for $${this.totalPrice.toFixed(2)} to cart ✅`);
  }
}
