using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Recipes;
using Pento.Domain.Recipes.Events;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers;

internal sealed class RecipeCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Recipe> compartmentRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<RecipeCreatedDomainEvent>
{
    public async override Task Handle(RecipeCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Recipe? recipe = await compartmentRepository.GetByIdAsync(domainEvent.RecipeId, cancellationToken);
        if (recipe == null)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), RecipeErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            null,
            ActivityCode.RECIPE_CREATE.ToString(),
            domainEvent.RecipeId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
