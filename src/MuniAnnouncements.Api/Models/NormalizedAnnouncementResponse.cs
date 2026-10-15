namespace MuniAnnouncements.Api.Models;

public sealed record NormalizedAnnouncementResponse(
    string AuthorityCode,
    string AuthorityName,
    IReadOnlyList<NormalizedAnnouncement> Announcements
);

public sealed record NormalizedAnnouncement(
    string Id,
    string Title,
    string Summary,
    string CategoryName,
    bool IsUrgent,
    string PublishDate
);