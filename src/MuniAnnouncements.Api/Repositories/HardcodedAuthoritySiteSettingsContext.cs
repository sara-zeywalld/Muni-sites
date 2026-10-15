using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public sealed class HardcodedAuthoritySiteSettingsContext : IAuthoritySiteSettingsContext
{
    public IReadOnlyList<AuthoritySiteSettings> AuthoritySiteSettings { get; } =
    [
        new(
            "6500",
            "עיריית חדרה",
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "roads", "water", "events" },
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["roads"] = "תחבורה וכבישים",
                ["water"] = "מים וביוב",
                ["events"] = "אירועים"
            },
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "water" },
            5),
        new(
            "7000",
            "עיריית דוגמה",
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "roads" },
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["roads"] = "כבישים"
            },
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            5)
    ];
}