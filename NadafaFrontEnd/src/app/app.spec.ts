<<<<<<< HEAD
=======
import { provideZonelessChangeDetection } from '@angular/core';
>>>>>>> 232b30b9a631aada6b497df1034d5b30ba7ed3eb
import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
<<<<<<< HEAD
=======
      providers: [provideZonelessChangeDetection()]
>>>>>>> 232b30b9a631aada6b497df1034d5b30ba7ed3eb
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, NadafaFrontEnd');
  });
});
