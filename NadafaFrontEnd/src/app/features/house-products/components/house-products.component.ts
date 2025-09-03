import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-house-product',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './house-products.component.html',
  styleUrls: ['./house-products.css']
})
export class HouseProducts {
  products = [
    { id: 1, name: 'Papers', description: 'Any papers you do not use it', price: 20, img: 'https://images.freeimages.com/images/large-previews/b24/papers-1-1200480.jpg' },
    { id: 2, name: 'Glass', description: 'Any glass you do not use it', price: 40, img: 'https://tse3.mm.bing.net/th/id/OIP.wwf9wojjHK2oikjCiqmBjgAAAA?rs=1&pid=ImgDetMain&o=7&rm=3' },
    { id: 3, name: 'Metals', description: 'Any Metals you do not use it', price: 50, img: 'https://tse1.mm.bing.net/th/id/OIP.ZKvnwd2f-KKGW0wlYTuK1gHaD4?rs=1&pid=ImgDetMain&o=7&rm=3' }
  ];
}
