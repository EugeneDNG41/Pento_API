using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;

namespace Pento.Application.FoodItems.Delete;

public sealed record DeleteFoodItemCommand(Guid FoodItemId) : ICommand;
internal sealed class DeleteFoodItemCommandValidator : AbstractValidator<DeleteFoodItemCommand>
{ 
    public DeleteFoodItemCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty().WithMessage("Food Item Id is required.");
    }
}
internal sealed class DeleteFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<FoodItemReservation> reservationRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<DeleteFoodItemCommand>
{
    public async Task<Result> Handle(DeleteFoodItemCommand command, CancellationToken cancellationToken)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.FoodItemId, cancellationToken);
        if (foodItem == null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        bool hasPendingReservations = await reservationRepository
            .AnyAsync(r => r.FoodItemId == foodItem.Id && r.Status == ReservationStatus.Pending, cancellationToken);
        if (hasPendingReservations)
        {
            return Result.Failure(FoodItemErrors.HasPendingReservation);
        }
        foodItem.Delete();
        foodItemRepository.Update(foodItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
