using Microsoft.AspNetCore.Mvc;
using MuniAnnouncements.Api.Services;

namespace MuniAnnouncements.Api.Controllers;

[ApiController]
[Route("announcements/api")]
public sealed class AnnouncementsController(IAnnouncementService announcementService) : ControllerBase
{
    [HttpGet("{authorityCode}")]
    public async Task<IActionResult> Get(string authorityCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(authorityCode))
        {
            return BadRequest("authorityCode is required.");
        }

        var response = await announcementService.GetActiveAsync(authorityCode, DateTimeOffset.UtcNow, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}