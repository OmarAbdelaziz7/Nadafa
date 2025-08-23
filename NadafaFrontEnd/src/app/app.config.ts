<<<<<<< HEAD
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
=======
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
>>>>>>> 232b30b9a631aada6b497df1034d5b30ba7ed3eb
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
<<<<<<< HEAD
    provideZoneChangeDetection({ eventCoalescing: true }),
=======
    provideZonelessChangeDetection(),
>>>>>>> 232b30b9a631aada6b497df1034d5b30ba7ed3eb
    provideRouter(routes)
  ]
};
