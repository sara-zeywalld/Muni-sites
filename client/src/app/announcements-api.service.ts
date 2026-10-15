import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AnnouncementResponse } from './announcement.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class AnnouncementsApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = environment.announcementsApiUrl;

  getForAuthority(authorityCode: string): Observable<AnnouncementResponse> {
    return this.http.get<AnnouncementResponse>(`${this.apiBaseUrl}/${encodeURIComponent(authorityCode)}`);
  }
}