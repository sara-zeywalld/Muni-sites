using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MuniAnnouncements.Api.Tests;

public sealed class AnnouncementsApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task UnknownAuthorityReturnsNotFound()
    {
        HttpResponseMessage response = await client.GetAsync("/announcements/api/unknown");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task KnownAuthorityReturnsNormalizedResponse()
    {
        HttpResponseMessage response = await client.GetAsync("/announcements/api/6500");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync());
        JsonElement root = document.RootElement;

        Assert.Equal("6500", root.GetProperty("authorityCode").GetString());
        Assert.Equal("עיריית חדרה", root.GetProperty("authorityName").GetString());
        Assert.True(root.GetProperty("announcements").ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task SecondAuthorityReturnsItsOwnTenantResponse()
    {
        HttpResponseMessage response = await client.GetAsync("/announcements/api/7000");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync());
        JsonElement root = document.RootElement;

        Assert.Equal("7000", root.GetProperty("authorityCode").GetString());
        Assert.Equal("עיריית דוגמה", root.GetProperty("authorityName").GetString());
        Assert.All(root.GetProperty("announcements").EnumerateArray(), announcement =>
            Assert.Equal("עבודות בכביש הראשי", announcement.GetProperty("title").GetString()));
    }
}