using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using MuniAnnouncements.Api.Models;
using MuniAnnouncements.Api.Repositories;

namespace MuniAnnouncements.Api.Services;

public sealed class AnnouncementService(
    ICmsAnnouncementClient cmsClient,
    IAuthoritySiteSettingsRepository settingsRepository) : IAnnouncementService
{
    private const string PublishedStatus = "Published";
    private const int MaxSummaryLength = 150;
    private const string SummaryEllipsis = "...";

    public async Task<NormalizedAnnouncementResponse?> GetActiveAsync(string authorityCode, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authorityCode))
        {
            return null;
        }

        AuthoritySiteSettings? settings = await settingsRepository.GetAsync(authorityCode, cancellationToken);
        if (settings is null || !string.Equals(settings.AuthorityCode, authorityCode, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        IReadOnlyList<Announcement> rawAnnouncements = await cmsClient.GetAnnouncementsAsync(authorityCode, cancellationToken);
        IReadOnlyList<NormalizedAnnouncement> normalized = rawAnnouncements
            .Where(item => string.Equals(item.AuthorityCode, authorityCode, StringComparison.OrdinalIgnoreCase))
            .Where(item => string.Equals(item.Status, PublishedStatus, StringComparison.OrdinalIgnoreCase))
            .Where(item => item.PublishDate <= now && item.ExpiryDate > now)
            .Where(item => settings.EnabledCategories.Contains(item.Category))
            .OrderByDescending(item => settings.UrgentCategories.Contains(item.Category))
            .ThenByDescending(item => item.PublishDate)
            .Take(Math.Max(0, settings.MaxItems))
            .Select(item => new NormalizedAnnouncement(
                item.Id,
                item.Title,
                ToPlainTextSummary(item.Body),
                settings.CategoryDisplayNames.TryGetValue(item.Category, out string? displayName) ? displayName : item.Category,
                settings.UrgentCategories.Contains(item.Category),
                item.PublishDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
            .ToList();

        return new NormalizedAnnouncementResponse(settings.AuthorityCode, settings.AuthorityName, normalized);
    }

    private static string ToPlainTextSummary(string html)
    {
        string withoutTags = Regex.Replace(html, "<[^>]+>", " ");
        string decoded = WebUtility.HtmlDecode(withoutTags) ?? string.Empty;
        string normalizedWhitespace = Regex.Replace(decoded, "\\s+", " ").Trim();
        if (normalizedWhitespace.Length <= MaxSummaryLength)
        {
            return normalizedWhitespace;
        }

        int contentLength = MaxSummaryLength - SummaryEllipsis.Length;
        return normalizedWhitespace[..contentLength].TrimEnd() + SummaryEllipsis;
    }
}