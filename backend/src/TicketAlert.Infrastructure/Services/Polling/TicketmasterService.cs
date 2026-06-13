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
        _apiKey = config["Ticketmaster:ApiKey"] ?? "";
    }

    public async Task<IReadOnlyList<Event>> SearchEventsAsync(
        string? keyword, string? artist, string? city,
        int page, int pageSize)
    {
        if (string.IsNullOrEmpty(_apiKey)) return Array.Empty<Event>();

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

    public async Task<(bool Available, int? TotalCount)> GetInventoryStatusAsync(
        string ticketmasterEventId,
        string? ticketmasterUrl = null)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return await CheckPublicTicketmasterPageAsync(ticketmasterUrl);

        try
        {
            var eventId = Uri.EscapeDataString(ticketmasterEventId);
            var response = await _http.GetAsync(
                $"https://app.ticketmaster.com/inventory-status/v1/availability?events={eventId}&apikey={_apiKey}");

            if (!response.IsSuccessStatusCode)
                return await CheckPublicTicketmasterPageAsync(ticketmasterUrl);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();

            return ParseInventoryStatus(body, ticketmasterEventId);
        }
        catch
        {
            return await CheckPublicTicketmasterPageAsync(ticketmasterUrl);
        }
    }

    private async Task<(bool Available, int? TotalCount)> CheckPublicTicketmasterPageAsync(string? ticketmasterUrl)
    {
        if (string.IsNullOrWhiteSpace(ticketmasterUrl)) return (false, null);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ticketmasterUrl);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 TicketAlert/1.0");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return (false, null);

            var html = await response.Content.ReadAsStringAsync();
            return (HasTicketAvailabilitySignal(html), null);
        }
        catch
        {
            return (false, null);
        }
    }

    private static bool HasTicketAvailabilitySignal(string html)
    {
        var verifiedResaleSignals = new[]
        {
            "Verified Resale Ticket",
            "Verified Resale Tickets",
            "verified resale",
            "resale tickets"
        };

        if (ContainsAny(html, verifiedResaleSignals)) return true;

        var unavailableSignals = new[]
        {
            "sold out",
            "utsolgt",
            "no tickets available",
            "currently no tickets",
            "tickets are not currently available",
            "ingen billetter tilgjengelig"
        };

        if (ContainsAny(html, unavailableSignals)) return false;

        var availableSignals = new[]
        {
            "find tickets",
            "buy tickets",
            "select tickets",
            "kjop billetter",
            "kjøp billetter",
            "finn billetter",
            "\"availability\":\"https://schema.org/InStock\"",
            "\"availability\": \"https://schema.org/InStock\""
        };

        return ContainsAny(html, availableSignals);
    }

    private static bool ContainsAny(string value, IEnumerable<string> signals) =>
        signals.Any(signal => value.Contains(signal, StringComparison.OrdinalIgnoreCase));

    private static (bool Available, int? TotalCount) ParseInventoryStatus(JsonElement body, string ticketmasterEventId)
    {
        if (body.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in body.EnumerateArray())
            {
                if (!item.TryGetProperty("eventId", out var id) ||
                    string.Equals(id.GetString(), ticketmasterEventId, StringComparison.OrdinalIgnoreCase))
                    return ParseInventoryStatusObject(item);
            }

            return (false, null);
        }

        return ParseInventoryStatusObject(body);
    }

    private static (bool Available, int? TotalCount) ParseInventoryStatusObject(JsonElement body)
    {
        if (body.TryGetProperty("available", out var available))
        {
            var totalCount = body.TryGetProperty("totalCount", out var count) ? (int?)count.GetInt32() : null;
            return (available.GetBoolean(), totalCount);
        }

        var primaryAvailable = body.TryGetProperty("status", out var status) && IsAvailableStatus(status.GetString());
        var resaleAvailable = body.TryGetProperty("resaleStatus", out var resaleStatus) &&
            IsAvailableStatus(resaleStatus.GetString());

        return (primaryAvailable || resaleAvailable, null);
    }

    private static bool IsAvailableStatus(string? status) =>
        string.Equals(status, "TICKETS_AVAILABLE", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(status, "FEW_TICKETS_LEFT", StringComparison.OrdinalIgnoreCase);

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
