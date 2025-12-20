using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;

namespace Pento.Application.EventHandlers.Compartments;

internal sealed class CompartmentDeletedEventHandler(
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<CompartmentDeletedDomainEvent>
{
    public async override Task Handle(CompartmentDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        IEnumerable<FoodItem> foodItems = await foodItemRepository.FindAsync(fi => fi.CompartmentId == domainEvent.CompartmentId, cancellationToken);
        await foodItemRepository.RemoveRangeAsync(foodItems, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
