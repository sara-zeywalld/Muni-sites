using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public interface IAuthoritySiteSettingsRepository
{
    Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken cancellationToken);
}