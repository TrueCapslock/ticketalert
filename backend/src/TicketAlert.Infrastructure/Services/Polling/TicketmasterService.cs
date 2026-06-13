using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Domain.Entities;

namespace TicketAlert.Infrastructure.Services.Polling;

public class TicketmasterService : ITicketmasterService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TicketmasterService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Ticketmaster:ApiKey"]!;
    }

    public async Task<IReadOnlyList<Event>> SearchEventsAsync(
        string? keyword, string? artist, string? city,
        int page, int pageSize)
    {
        var query = $"apikey={_apiKey}&size={pageSize}&page={page - 1}";
        if (!string.IsNullOrEmpty(keyword)) query += $"&keyword={Uri.EscapeDataString(keyword)}";
        if (!string.IsNullOrEmpty(artist)) query += $"&attractionId={Uri.EscapeDataString(artist)}";
        if (!string.IsNullOrEmpty(city)) query += $"&city={Uri.EscapeDataString(city)}";

        var response = await _http.GetAsync(
            $"https://app.ticketmaster.com/discovery/v2/events.json?{query}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return ParseEvents(body);
    }

    public async Task<Event?> GetEventDetailsAsync(string ticketmasterEventId)
    {
        var response = await _http.GetAsync(
            $"https://app.ticketmaster.com/discovery/v2/events/{ticketmasterEventId}.json?apikey={_apiKey}");

        if (!response.IsSuccessStatusCode) return null;

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return ParseEvent(body);
    }

    public async Task<bool> CheckTicketAvailabilityAsync(string ticketmasterEventId)
    {
        var (available, _) = await GetInventoryStatusAsync(ticketmasterEventId);
        return available;
    }

    public async Task<(bool Available, int? TotalCount)> GetInventoryStatusAsync(string ticketmasterEventId)
    {
        try
        {
            var response = await _http.GetAsync(
                $"https://app.ticketmaster.com/inventory-status/v1/events/{ticketmasterEventId}?apikey={_apiKey}");

            if (!response.IsSuccessStatusCode) return (false, null);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();

            var available = body.TryGetProperty("available", out var avail) && avail.GetBoolean();
            var totalCount = body.TryGetProperty("totalCount", out var count) ? (int?)count.GetInt32() : null;

            return (available, totalCount);
        }
        catch
        {
            return (false, null);
        }
    }

    private IReadOnlyList<Event> ParseEvents(JsonElement body)
    {
        var events = new List<Event>();

        if (!body.TryGetProperty("_embedded", out var embedded) ||
            !embedded.TryGetProperty("events", out var eventsJson))
            return events;

        foreach (var evJson in eventsJson.EnumerateArray())
        {
            var ev = ParseEvent(evJson);
            if (ev is not null) events.Add(ev);
        }

        return events;
    }

    private static Event? ParseEvent(JsonElement el)
    {
        try
        {
            var id = el.GetProperty("id").GetString()!;
            var name = el.GetProperty("name").GetString()!;
            var url = el.GetProperty("url").GetString()!;

            string? artist = null;
            if (el.TryGetProperty("_embedded", out var embedded) &&
                embedded.TryGetProperty("attractions", out var attractions) &&
                attractions.EnumerateArray().Any())
            {
                artist = attractions.EnumerateArray().First()
                    .GetProperty("name").GetString();
            }

            string? venue = null;
            string? city = null;
            if (embedded.TryGetProperty("venues", out var venues) &&
                venues.EnumerateArray().Any())
            {
                var venueEl = venues.EnumerateArray().First();
                venue = venueEl.TryGetProperty("name", out var vn) ? vn.GetString() : null;
                if (venueEl.TryGetProperty("city", out var cityEl))
                    city = cityEl.GetProperty("name").GetString();
            }

            DateTime eventDate = default;
            if (el.TryGetProperty("dates", out var dates) &&
                dates.TryGetProperty("start", out var start) &&
                start.TryGetProperty("dateTime", out var dt))
            {
                eventDate = dt.GetDateTime();
            }

            string? imageUrl = null;
            if (el.TryGetProperty("images", out var images))
            {
                var img = images.EnumerateArray()
                    .OrderByDescending(i =>
                        i.TryGetProperty("width", out var w) ? w.GetInt32() : 0)
                    .FirstOrDefault();
                imageUrl = img.TryGetProperty("url", out var iu) ? iu.GetString() : null;
            }

            string? genre = null;
            if (el.TryGetProperty("classifications", out var classifications) &&
                classifications.EnumerateArray().Any())
            {
                var c = classifications.EnumerateArray().First();
                if (c.TryGetProperty("segment", out var segment))
                    genre = segment.GetProperty("name").GetString();
            }

            return new Event
            {
                Id = Guid.NewGuid(),
                TicketmasterEventId = id,
                Title = name,
                Artist = artist,
                Venue = venue,
                City = city,
                EventDate = eventDate,
                TicketmasterUrl = url,
                ImageUrl = imageUrl,
                Genre = genre
            };
        }
        catch
        {
            return null;
        }
    }
}
