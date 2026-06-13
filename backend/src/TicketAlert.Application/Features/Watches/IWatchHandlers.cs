using TicketAlert.Application.Common.DTOs;

namespace TicketAlert.Application.Features.Watches;

public interface ICreateWatchHandler
{
    Task<WatchDto> HandleAsync(Guid userId, CreateWatchRequest request);
}

public interface IGetUserWatchesHandler
{
    Task<IReadOnlyList<WatchDto>> HandleAsync(Guid userId, string? status);
}

public interface ICancelWatchHandler
{
    Task HandleAsync(Guid userId, Guid watchId);
}
