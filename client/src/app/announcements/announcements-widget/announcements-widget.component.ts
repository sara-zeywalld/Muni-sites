import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { AnnouncementResponse } from '../announcement.model';
import { AnnouncementsApiService } from '../announcements-api.service';

type AnnouncementsViewState =
  | { status: 'loading' }
  | { status: 'error' }
  | { status: 'loaded'; data: AnnouncementResponse };

@Component({
  selector: 'muni-announcements-widget',
  standalone: true,
  imports: [AsyncPipe, NgFor, NgIf],
  templateUrl: './announcements-widget.component.html',
  styleUrl: './announcements-widget.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnnouncementsWidgetComponent {
  readonly authorityCode = input.required<string>();
  private readonly api = inject(AnnouncementsApiService);

  protected readonly state$ = toObservable(this.authorityCode).pipe(
    switchMap(authorityCode => this.api.getForAuthority(authorityCode).pipe(
      map(data => ({ status: 'loaded', data } as const)),
      startWith({ status: 'loading' } as const),
      catchError(() => of({ status: 'error' } as const))
    ))
  );
}