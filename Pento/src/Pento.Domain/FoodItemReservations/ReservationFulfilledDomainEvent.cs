using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodItemReservations;

public sealed class ReservationFulfilledDomainEvent(Guid reservationId, Guid userId) : DomainEvent
{
    public Guid ReservationId { get; } = reservationId;
    public Guid UserId { get; } = userId;
}



