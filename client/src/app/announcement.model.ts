export interface NormalizedAnnouncement {
  id: string;
  title: string;
  summary: string;
  categoryName: string;
  isUrgent: boolean;
  publishDate: string;
}

export interface AnnouncementResponse {
  authorityCode: string;
  authorityName: string;
  announcements: NormalizedAnnouncement[];
}