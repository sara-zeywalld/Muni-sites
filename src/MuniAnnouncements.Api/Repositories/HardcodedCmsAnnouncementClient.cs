using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public sealed class HardcodedCmsAnnouncementClient : ICmsAnnouncementClient
{
    public Task<IReadOnlyList<Announcement>> GetAnnouncementsAsync(string authorityCode, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IReadOnlyList<Announcement> announcements =
        [
            new("a-1001", "6500", "סגירת רחוב הרברט סמואל", "<p>בשל עבודות תשתית ייסגר הרחוב לתנועה</p>", "roads", "Published", now.AddDays(-14), now.AddDays(6)),
            new("a-1002", "6500", "הפסקת מים מתוכננת", "<p>ביום רביעי תיתכן הפסקת מים בשכונות המזרחיות.</p>", "water", "Published", now.AddDays(-5), now.AddDays(3)),
            new("a-1003", "6500", "אירועי החודש", "<p>אירועים עירוניים לכל המשפחה.</p>", "events", "Draft", now.AddDays(-5), now.AddDays(13)),
            new("b-2001", "7000", "עבודות בכביש הראשי", "<p>הכביש הראשי יהיה סגור לתנועה.</p>", "roads", "Published", now.AddDays(-10), now.AddDays(10))
        ];

        IReadOnlyList<Announcement> matchingAnnouncements = announcements
            .Where(item => string.Equals(item.AuthorityCode, authorityCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(matchingAnnouncements);
    }
}