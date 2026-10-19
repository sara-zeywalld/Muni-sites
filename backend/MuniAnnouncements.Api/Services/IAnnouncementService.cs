using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Services;

public interface IAnnouncementService
{
    Task<NormalizedAnnouncementResponse?> GetActiveAsync(string authorityCode, DateTimeOffset now, CancellationToken cancellationToken);
}