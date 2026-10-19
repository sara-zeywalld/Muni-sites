using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public interface ICmsAnnouncementClient
{
    Task<IReadOnlyList<Announcement>> GetAnnouncementsAsync(string authorityCode, CancellationToken cancellationToken);
}