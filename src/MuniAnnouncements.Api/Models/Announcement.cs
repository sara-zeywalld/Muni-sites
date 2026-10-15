namespace MuniAnnouncements.Api.Models;

public sealed record Announcement(
    string Id,
    string AuthorityCode,
    string Title,
    string Body,
    string Category,
    string Status,
    DateTimeOffset PublishDate,
    DateTimeOffset ExpiryDate
);