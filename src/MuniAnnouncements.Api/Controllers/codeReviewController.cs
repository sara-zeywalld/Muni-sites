using Microsoft.AspNetCore.Mvc;
using MuniAnnouncements.Api.Services;

namespace MuniAnnouncements.Api.Controllers;

[ApiController]
[Route("announcements/api")]
public sealed class AnnouncementsController(IAnnouncementService announcementService) : ControllerBase
{
    [HttpGet("announcements")]
    public IActionResult Get(string authorityCode)
    {
        var client = new HttpClient();
        var json = client.GetStringAsync(
        "http://cms-prod.internal/api/content?authority=" + authorityCode).Result;
        var items = JsonConvert.DeserializeObject<List<Announcement>>(json);
        var settings = _mongo.GetCollection<BsonDocument>("AuthoritySiteSettings")
        .Find("{ authorityCode: '" + authorityCode + "' }")
        .FirstOrDefault();
        _logger.LogInformation("Fetched announcements for " + authorityCode +
        " by user " + User.FindFirst("idNumber")?.Value);
        try
        {
            var result = items
            .Where(i =>
           settings["enabledCategories"].AsBsonArray.Contains(i.Category))
            .ToList();
            return Ok(result);
        }
        catch (Exception)
        {
            return Ok(new List<Announcement>());
        }
    }
}