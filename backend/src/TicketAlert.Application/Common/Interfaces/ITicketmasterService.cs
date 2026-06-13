using TicketAlert.Domain.Entities;

namespace TicketAlert.Application.Common.Interfaces;

public interface ITicketmasterService
{
    Task<IReadOnlyList<Event>> SearchEventsAsync(string? keyword, string? artist, string? city, int page, int pageSize);
    Task<Event?> GetEventDetailsAsync(string ticketmasterEventId);
    Task<bool> CheckTicketAvailabilityAsync(string ticketmasterEventId);
    Task<(bool Available, int? TotalCount)> GetInventoryStatusAsync(string ticketmasterEventId);
}
