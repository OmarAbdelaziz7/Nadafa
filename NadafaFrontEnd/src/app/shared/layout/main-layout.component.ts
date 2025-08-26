import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
    selector: 'app-main-layout',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        NavbarComponent,
        FooterComponent
    ],
    template: `
    <div class="main-layout">
      <app-navbar></app-navbar>
      
      <main class="main-content">
        <router-outlet></router-outlet>
      </main>
      
      <app-footer></app-footer>
    </div>
  `,
    styles: [`
    .main-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .main-content {
      flex: 1;
      padding-top: 80px; /* Increased padding for navbar + extra space */
      display: flex;
      flex-direction: column;
      overflow: visible;
      min-height: calc(100vh - 64px);
    }

    /* Ensure proper spacing for different page types */
    ::ng-deep .main-content > * {
      flex: 1;
      overflow: visible;
    }
  `]
})
export class MainLayoutComponent { }
