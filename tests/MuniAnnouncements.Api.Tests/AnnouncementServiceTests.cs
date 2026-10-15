using MuniAnnouncements.Api.Models;
using MuniAnnouncements.Api.Repositories;
using MuniAnnouncements.Api.Services;
using Xunit;

namespace MuniAnnouncements.Api.Tests;

public sealed class AnnouncementServiceTests
{
    private static AnnouncementService CreateService(IReadOnlyList<Announcement> announcements, AuthoritySiteSettings? settings = null)
    {
        ICmsAnnouncementClient cms = new FakeCmsClient(announcements);
        IAuthoritySiteSettingsRepository repository = new FakeSettingsRepository(settings ?? new(
            "6500", "עיריית חדרה", new HashSet<string> { "roads", "water" },
            new Dictionary<string, string> { ["roads"] = "תחבורה וכבישים", ["water"] = "מים וביוב" },
            new HashSet<string> { "water" }, 5));
        return new AnnouncementService(cms, repository);
    }

    [Fact]
    public async Task FiltersByPublishedWindowAndEnabledCategory()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AnnouncementService service = CreateService([
            new("active", "6500", "Active", "<p>Text</p>", "roads", "Published", now.AddDays(-1), now.AddDays(1)),
            new("draft", "6500", "Draft", "Text", "roads", "Draft", now.AddDays(-1), now.AddDays(1)),
            new("expired", "6500", "Expired", "Text", "roads", "Published", now.AddDays(-2), now),
            new("disabled", "6500", "Disabled", "Text", "events", "Published", now.AddDays(-1), now.AddDays(1))
        ]);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Single(result!.Announcements);
        Assert.Equal("active", result.Announcements[0].Id);
    }

    [Fact]
    public async Task SortsUrgentFirstThenNewestAndAppliesMaxItems()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AuthoritySiteSettings settings = new("6500", "Test", new HashSet<string> { "roads", "water" },
            new Dictionary<string, string> { ["roads"] = "Roads", ["water"] = "Water" }, new HashSet<string> { "water" }, 2);
        AnnouncementService service = CreateService([
            new("roads-new", "6500", "Roads", "Text", "roads", "Published", now.AddHours(-1), now.AddDays(1)),
            new("water-old", "6500", "Water", "Text", "water", "Published", now.AddDays(-1), now.AddDays(1)),
            new("water-new", "6500", "Water", "Text", "water", "Published", now.AddHours(-2), now.AddDays(1))
        ], settings);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Equal(["water-new", "water-old"], result!.Announcements.Select(item => item.Id));
    }

    [Fact]
    public async Task ConvertsHtmlToPlainTextAndLimitsSummary()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string longHtml = "<p>" + new string('א', 180) + "</p>";
        AnnouncementService service = CreateService([
            new("a", "6500", "Title", longHtml, "roads", "Published", now.AddMinutes(-1), now.AddDays(1))
        ]);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.DoesNotContain("<p>", result!.Announcements[0].Summary);
        Assert.Equal(150, result.Announcements[0].Summary.Length);
        Assert.EndsWith("...", result.Announcements[0].Summary);
    }

    [Fact]
    public async Task ExcludesAnnouncementsBelongingToAnotherAuthority()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AnnouncementService service = CreateService([
            new("local", "6500", "Local", "Text", "roads", "Published", now.AddHours(-1), now.AddDays(1)),
            new("foreign", "7000", "Foreign", "Text", "roads", "Published", now.AddHours(-1), now.AddDays(1))
        ]);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Single(result!.Announcements);
        Assert.Equal("local", result.Announcements[0].Id);
    }

    [Fact]
    public async Task RejectsSettingsForAnotherAuthority()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AuthoritySiteSettings foreignSettings = new(
            "7000", "Other Authority", new HashSet<string> { "roads" },
            new Dictionary<string, string> { ["roads"] = "Roads" }, new HashSet<string>(), 5);
        AnnouncementService service = CreateService([], foreignSettings);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExcludesFutureAndExpiredAnnouncementsAtTimeBoundaries()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AnnouncementService service = CreateService([
            new("future", "6500", "Future", "Text", "roads", "Published", now.AddMinutes(1), now.AddDays(1)),
            new("expired", "6500", "Expired", "Text", "roads", "Published", now.AddDays(-1), now),
            new("active", "6500", "Active", "Text", "roads", "Published", now, now.AddMinutes(1))
        ]);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Single(result!.Announcements);
        Assert.Equal("active", result.Announcements[0].Id);
    }

    [Fact]
    public async Task ReturnsNoAnnouncementsWhenMaxItemsIsZero()
    {
        DateTimeOffset now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);
        AuthoritySiteSettings settings = new(
            "6500", "Test", new HashSet<string> { "roads" },
            new Dictionary<string, string> { ["roads"] = "Roads" }, new HashSet<string>(), 0);
        AnnouncementService service = CreateService([
            new("active", "6500", "Active", "Text", "roads", "Published", now.AddHours(-1), now.AddDays(1))
        ], settings);

        var result = await service.GetActiveAsync("6500", now, CancellationToken.None);

        Assert.Empty(result!.Announcements);
    }

    private sealed class FakeCmsClient(IReadOnlyList<Announcement> data) : ICmsAnnouncementClient
    {
        public Task<IReadOnlyList<Announcement>> GetAnnouncementsAsync(string authorityCode, CancellationToken cancellationToken) => Task.FromResult(data);
    }

    private sealed class FakeSettingsRepository(AuthoritySiteSettings? settings) : IAuthoritySiteSettingsRepository
    {
        public Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken cancellationToken) => Task.FromResult(settings);
    }

}