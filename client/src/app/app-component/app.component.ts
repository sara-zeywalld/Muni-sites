import { NgFor } from '@angular/common';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { AnnouncementsWidgetComponent } from '../announcements-widget.component';

@Component({
  selector: 'muni-root',
  standalone: true,
  imports: [AnnouncementsWidgetComponent, NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  protected readonly authorities = [
    { code: '6500', name: 'עיריית חדרה' },
    { code: '7000', name: 'עיריית דוגמה' }
  ];
  protected readonly authorityCode = signal(this.authorities[0].code);

  protected selectAuthority(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.authorityCode.set(select.value);
  }
}