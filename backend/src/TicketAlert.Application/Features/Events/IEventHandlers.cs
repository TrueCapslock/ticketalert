using TicketAlert.Application.Common.DTOs;

namespace TicketAlert.Application.Features.Events;

public interface ISearchEventsHandler
{
    Task<EventSearchResponse> HandleAsync(EventSearchRequest request);
}

public interface IGetEventHandler
{
    Task<EventDetailDto?> HandleAsync(Guid id);
    Task<EventDetailDto?> HandleByTicketmasterIdAsync(string ticketmasterEventId);
}
