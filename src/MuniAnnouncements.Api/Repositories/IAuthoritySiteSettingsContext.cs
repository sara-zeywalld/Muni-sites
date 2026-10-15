using MuniAnnouncements.Api.Models;

namespace MuniAnnouncements.Api.Repositories;

public interface IAuthoritySiteSettingsContext
{
    IReadOnlyList<AuthoritySiteSettings> AuthoritySiteSettings { get; }
}