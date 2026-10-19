namespace MuniAnnouncements.Api.Models;

public sealed record AuthoritySiteSettings(
    string AuthorityCode,
    string AuthorityName,
    IReadOnlySet<string> EnabledCategories,
    IReadOnlyDictionary<string, string> CategoryDisplayNames,
    IReadOnlySet<string> UrgentCategories,
    int MaxItems
);