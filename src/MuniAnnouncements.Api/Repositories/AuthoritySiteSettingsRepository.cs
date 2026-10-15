using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public sealed class AuthoritySiteSettingsRepository(
    IAuthoritySiteSettingsContext context) : IAuthoritySiteSettingsRepository
{
    public Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        AuthoritySiteSettings? settings = context.AuthoritySiteSettings
            .FirstOrDefault(item => string.Equals(
                item.AuthorityCode,
                authorityCode,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(settings);
    }
}