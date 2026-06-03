import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

type MotionMode = 'subtle' | 'lively';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  readonly motionMode = signal<MotionMode>('subtle');
  private readonly motionStorageKey = 'smart-home-motion-mode';

  constructor() {
    try {
      const stored = localStorage.getItem(this.motionStorageKey);
      if (stored === 'subtle' || stored === 'lively') {
        this.motionMode.set(stored);
      }
    } catch {
      this.motionMode.set('subtle');
    }
  }

  setMotionMode(mode: MotionMode) {
    this.motionMode.set(mode);
    try {
      localStorage.setItem(this.motionStorageKey, mode);
    } catch {
      // No-op if storage is unavailable.
    }
  }
}
